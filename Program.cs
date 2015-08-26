using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace VMwareService
{
    class Program : ServiceBase
    {
        const string configFile = "VMwareService.cfg";
        const string defaultServiceName = "VMwareService";
        private static int retry_times = 10;
        private static int retry_sleep = 5;

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                Run(new Program());
                return;
            }

            if (args.Length == 0)
            {
                PrintUsages();
                return;
            }
            switch (args[0])
            {
                case "install":
                    // Usage: VMwareService install [service-name]
                    switch (args.Length)
                    {
                        case 1:
                            InstallService();
                            break;
                        case 2:
                            InstallService(args[1]);
                            break;
                        default:
                            PrintUsages();
                            break;
                    }
                    break;
                case "uninstall":
                case "remove":
                    // Usage: VMwareService uninstall | remove [service-name]
                    switch (args.Length)
                    {
                        case 1:
                            UninstallService();
                            break;
                        case 2:
                            UninstallService(args[1]);
                            break;
                        default:
                            PrintUsages();
                            break;
                    }
                    break;
                case "start":
                    // Usage: VMwareService start [--as-service | -s [service-name]]
                    if (args.Length == 1)
                        StartVMWareVitualMachine(retry_times, retry_sleep);
                    else if (args.Length == 2 && (args[1] == "--as-service" || args[1] == "-s"))
                    {
                        // Start as a service
                        ServiceController sc = new ServiceController();
                        if (args.Length >= 3)
                            sc.ServiceName = args[2];
                        else
                            sc.ServiceName = defaultServiceName;
                        try
                        {
                            sc.Start();
                        }
                        catch (InvalidOperationException e)
                        {
                            LogMsg("Can't start service {0}.\n{1}", sc.ServiceName, e.ToString());
                            return;
                        }
                    }
                    else
                        PrintUsages();
                    break;
                case "suspend":
                    // Usage: VMwareService suspend
                    SuspendVMwareVirtualMachine(retry_times, retry_sleep);
                    break;
                case "stop":
                    // Usage: VMwareService stop [--as-service | -s [service-name]]
                    if (args.Length == 1)
                        StopVMWareVirtualMachine(retry_times, retry_sleep);
                    else if (args.Length == 2 && (args[1] == "--as-service" || args[1] == "-s"))
                    {
                        // Start as a service
                        ServiceController sc = new ServiceController();
                        if (args.Length >= 3)
                            sc.ServiceName = args[2];
                        else
                            sc.ServiceName = defaultServiceName;
                        try
                        {
                            sc.Stop();
                        }
                        catch (InvalidOperationException e)
                        {
                            LogMsg("Can't stop service {0}.\n{1}", sc.ServiceName, e.ToString());
                            return;
                        }
                    }
                    else
                        PrintUsages();
                    break;
                case "restart":
                    // Usage: VMwareService restart [--as-service | -s [service-name]]
                    if (args.Length == 1)
                    {
                        // TODO: check the service is running before Stop()
                        StopVMWareVirtualMachine(retry_times, retry_sleep);
                        StartVMWareVitualMachine(retry_times, retry_sleep);
                    }
                    else if (args.Length == 2 && (args[1] == "--as-service" || args[1] == "-s"))
                    {
                        // Start as a service
                        ServiceController sc = new ServiceController();
                        if (args.Length >= 3)
                            sc.ServiceName = args[2];
                        else
                            sc.ServiceName = defaultServiceName;
                        try
                        {
                            // TODO: check the service is running before Stop()
                            sc.Stop();
                            sc.Start();
                        }
                        catch (InvalidOperationException e)
                        {
                            LogMsg("Can't restart service {0}.\n{1}", sc.ServiceName, e.ToString());
                            throw;
                        }
                    }
                    else
                        PrintUsages();
                    break;
                default:
                    PrintUsages();
                    break;
            }
        }

        #region The methods overrided from BaseService

        /// <summary>
        /// Executes when a Start command is sent to the service by the Service
        /// Control Manager(SCM) or when the operating system starts(for a service
        /// that starts automatically). Specifies actions to take when the service
        /// starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
                StartVMWareVitualMachine(retry_times, retry_sleep);
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Executes when a Stop command is sent to the service by the Service
        /// Control Manager(SCM). Specifies actions to take when a service stops
        /// running.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                StopVMWareVirtualMachine(retry_times, retry_sleep);
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Executes when the system is shutting down.
        /// Specifies what should occur immediately prior to the system shutting down.
        /// </summary>
        protected override void OnShutdown()
        {
            try
            {
                base.OnShutdown();
                SuspendVMwareVirtualMachine(retry_times, retry_sleep);
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Executes when a Pause command is sent to the service by the Service Control
        /// Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                LogMsg("TODO: not implemented while install the service in sc command mode.");
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Executes when a Continue command is sent to the service by the Service
        /// Control Manager (SCM). Specifies actions to take when a service resumes
        /// normal functioning after being paused.
        /// </summary>
        protected override void OnContinue()
        {
            try
            {
                base.OnContinue();
                LogMsg("TODO: not implemented while install the service in sc command mode.");
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
        }

        /// <summary>
        /// Executes when the computer's power status has changed. This applies to
        /// laptop computers when they go into suspended mode, which is not the same
        /// as a system shutdown.
        /// </summary>
        /// <param name="powerStatus">
        /// A System.ServiceProcess.PowerBroadcastStatus that indicates a notification
        /// from the system about its power status.
        /// </param>
        /// <returns>
        /// When implemented in a derived class, the needs of your application determine
        /// what value to return.
        /// For example, if a QuerySuspend broadcast status is passed, you could cause
        /// your application to reject the query by returning false.
        /// </returns>
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            try
            {
                LogMsg("OnPowerEvent(): {0}", powerStatus);
            }
            catch (Exception e)
            {
                LogMsg(e.ToString());
            }
            return base.OnPowerEvent(powerStatus);
        }

        #endregion

        #region Functions of VMware virtual machine control

        /// <summary>
        /// Start VMware virtual machine.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <returns>True on success, False on failure.</returns>
        private static bool StartVMWareVitualMachine()
        {
            // TODO: add feature: support multi .vmx file.
            LogMsg("Starting VMware process ...");

            string vmxFilePath;
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            if (File.Exists(configFilePath))
                vmxFilePath = File.ReadAllLines(configFilePath)[0];
            else
            {
                LogMsg("Could not find config file. (Current directory: {0})", Environment.CurrentDirectory);
                return false;
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = Path.Combine(GetVMwareInstalledPath(), "vmrun.exe");
                if (!File.Exists(p.StartInfo.FileName))
                {
                    LogMsg("ERROR: File {0} not exists.", p.StartInfo.FileName);
                    return false;
                }
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(vmxFilePath);
                p.StartInfo.Arguments = string.Format("start \"{0}\" nogui", vmxFilePath);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                if (p.Start())
                {
                    p.OutputDataReceived += OnProcessOutputDataReceived;
                    p.BeginOutputReadLine();
                    p.ErrorDataReceived += OnProcessErrorDataReceived;
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        LogMsg("ERROR: Failed to start VMware process. (error code: {0})", p.ExitCode);
                        return false;
                    }
                    else
                    {
                        LogMsg("Starting VMware process OK.");
                        return true;
                    }
                }
                else
                {
                    LogMsg("ERROR: Failed to start VMware process.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Try to start VMware virtual machine times.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <param name="retry">The times to try.</param>
        /// <param name="sleep">The interval of tries. Unit in second.</param>
        /// <returns></returns>
        private static bool StartVMWareVitualMachine(int retry, int sleep)
        {
            bool retVal = false;
            for (int i = 0; i < retry; i++)
            {
                LogMsg("Trying to start in {2} {3}. (try {0} of {1} {4})", i + 1, retry, sleep, sleep > 1 ? "seconds" : "second", retry > 1 ? "times" : "time");
                if (StartVMWareVitualMachine())
                {
                    retVal = true;
                    break;
                }
                else
                {
                    if (i + 1 < retry)
                        Thread.Sleep(sleep * 1000);
                    else
                        LogMsg("Reaches the max retry times. Stop trying.");
                }
            }
            return retVal;
        }

        /// <summary>
        /// Shutdown VMware virtual machine.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <returns>True on success, False on failure.</returns>
        private static bool StopVMWareVirtualMachine()
        {
            LogMsg("Stopping VMware process ...");

            string vmxFilePath;
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            if (File.Exists(configFilePath))
                vmxFilePath = File.ReadAllLines(configFilePath)[0];
            else
            {
                LogMsg("Could not find config file. (Current directory: {0})", Environment.CurrentDirectory);
                return false;
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = Path.Combine(GetVMwareInstalledPath(), "vmrun.exe");
                if (!File.Exists(p.StartInfo.FileName))
                {
                    LogMsg("ERROR: File {0} not exists.", p.StartInfo.FileName);
                    return false;
                }
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(vmxFilePath);
                p.StartInfo.Arguments = string.Format("stop \"{0}\"", vmxFilePath);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                if (p.Start())
                {
                    p.OutputDataReceived += OnProcessOutputDataReceived;
                    p.BeginOutputReadLine();
                    p.ErrorDataReceived += OnProcessErrorDataReceived;
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        LogMsg("ERROR: Failed to stop VMware process. (error code: {0})", p.ExitCode);
                        return false;
                    }
                    else
                    {
                        LogMsg("Stopping VMware process OK.");
                        return true;
                    }
                }
                else
                {
                    LogMsg("ERROR: Failed to stop VMware process.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Try to shutdown VMware virtual machine times.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <param name="retry">The times to try.</param>
        /// <param name="sleep">The interval of tries. Unit in second.</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool StopVMWareVirtualMachine(int retry, int sleep)
        {
            bool retVal = false;
            for (int i = 0; i < retry; i++)
            {
                LogMsg("Trying to stop in {2} {3}. (try {0} of {1} {4})", i + 1, retry, sleep, sleep > 1 ? "seconds" : "second", retry > 1 ? "times" : "time");
                if (StopVMWareVirtualMachine())
                {
                    retVal = true;
                    break;
                }
                else
                {
                    if (i + 1 < retry)
                        Thread.Sleep(sleep * 1000);
                    else
                        LogMsg("Reaches the max retry times. Stop trying.");
                }
            }
            return retVal;
        }

        /// <summary>
        /// Suspend the VMware virtual machine.
        /// The suspended virtual machine can be restore as the status last time
        /// it runs, seems never stopped. Usually, this function should be use when
        /// the host is going to shutdown.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <returns>True on success, False on failure.</returns>
        private static bool SuspendVMwareVirtualMachine()
        {
            LogMsg("Suspending VMware process ...");

            string vmxFilePath;
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            if (File.Exists(configFilePath))
                vmxFilePath = File.ReadAllLines(configFilePath)[0];
            else
            {
                LogMsg("Could not find config file. (Current directory: {0})", Environment.CurrentDirectory);
                return false;
            }

            using (var p = new Process())
            {
                p.StartInfo.FileName = Path.Combine(GetVMwareInstalledPath(), "vmrun.exe");
                if (!File.Exists(p.StartInfo.FileName))
                {
                    LogMsg("ERROR: File {0} not exists.", p.StartInfo.FileName);
                    return false;
                }
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(vmxFilePath);
                p.StartInfo.Arguments = string.Format("suspend \"{0}\"", vmxFilePath);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                if (p.Start())
                {
                    p.OutputDataReceived += OnProcessOutputDataReceived;
                    p.BeginOutputReadLine();
                    p.ErrorDataReceived += OnProcessErrorDataReceived;
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        LogMsg("ERROR: Failed to suspend VMware process. (error code: {0})", p.ExitCode);
                        return false;
                    }
                    else
                    {
                        LogMsg("Suspending VMware process OK.");
                        return true;
                    }
                }
                else
                {
                    LogMsg("ERROR: Failed to suspend VMware process.");
                    return false;
                }
            }
        }

        /// <summary>
        /// Try to suspend VMware virtual machine times.
        /// The suspended virtual machine can be restore as the status last time
        /// it runs, seems never stopped. Usually, this function should be use when
        /// the host is going to shutdown.
        /// If there's a VMwareService.cfg file exists in the directory that the
        /// application installed, it reads the .vmx file (VMware virtual machine
        /// file) line by line, and start them one by one.
        /// </summary>
        /// <param name="retry">The times to try.</param>
        /// <param name="sleep">The interval of tries. Unit in second.</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool SuspendVMwareVirtualMachine(int retry, int sleep)
        {
            bool retVal = false;
            for (int i = 0; i < retry; i++)
            {
                LogMsg("Trying to suspend in {2} {3}. (retry {0} of {1} {4})", i + 1, retry, sleep, sleep > 1 ? "seconds" : "second", retry > 1 ? "times" : "time");
                if (SuspendVMwareVirtualMachine())
                {
                    retVal = true;
                    break;
                }
                else
                {
                    if (i + 1 < retry)
                        Thread.Sleep(sleep * 1000);
                    else
                        LogMsg("Reaches the max retry times. Stop trying.");
                }
            }
            return retVal;
        }

        #endregion

        #region Functions of service install/uninstall

        /// <summary>
        /// Create windows service by `sc` command line tool.
        /// The command line looks like:
        /// `sc create VMwareService binpath= "C:\VMwareService.exe" start= auto`
        /// </summary>
        /// <param name="serviceName">The service name, default as "VMwareService".</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool InstallService(string serviceName = defaultServiceName)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = string.Format("create {0} binpath= \"{1}\" start= auto", serviceName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName));
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                if (p.Start())
                {
                    p.OutputDataReceived += OnProcessOutputDataReceived;
                    p.BeginOutputReadLine();
                    p.ErrorDataReceived += OnProcessErrorDataReceived;
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        LogMsg("ERROR: Failed to install service. (error code: {0})", p.ExitCode);
                        return false;
                    }
                    else
                    {
                        LogMsg("Install service OK.");
                    }
                }
                else
                {
                    LogMsg("ERROR: Failed to install service.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Remove windows service by `sc` command line tool.
        /// The command line looks like:
        /// `sc delete VMwareService`
        /// </summary>
        /// <param name="serviceName">The service name, default as "VMwareService".</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool UninstallService(string serviceName = defaultServiceName)
        {
            // Stop the service before remove it.
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = string.Format("stop {0}", serviceName);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                if (p.Start())
                    p.WaitForExit();
                else
                {
                    LogMsg("ERROR: Failed to remove service.");
                    return false;
                }
            }

            // Remove the service.
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "sc";
                p.StartInfo.Arguments = string.Format("delete {0}", serviceName);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                if (p.Start())
                {
                    p.OutputDataReceived += OnProcessOutputDataReceived;
                    p.BeginOutputReadLine();
                    p.ErrorDataReceived += OnProcessErrorDataReceived;
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        LogMsg("ERROR: Failed to remove service. (Error code: {0})", p.ExitCode);
                        return false;
                    }
                    else
                    {
                        LogMsg("Remove service OK.");
                    }
                }
                else
                {
                    LogMsg("ERROR: Failed to remove service.");
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Functions of logging

        static void LogMsg(string msg)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VMwareService.log"); ;
            File.AppendAllText(logFilePath, string.Format("[{0}]\t{1}\n", DateTime.Now, msg), Encoding.UTF8);
            Console.WriteLine(msg);
        }

        static void LogMsg(string format, object arg0)
        {
            LogMsg(string.Format(format, arg0));
        }

        static void LogMsg(string format, params object[] args)
        {
            LogMsg(string.Format(format, args));
        }

        #endregion

        static string GetVMwareInstalledPath()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\vmware.exe"))
            {
                // The `Path` should be like: C:\Program Files (x86)\VMware\VMware Workstation
                var value = key.GetValue("Path");
                if (value != null)
                {
                    return value.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        private static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                LogMsg("ERROR: {0}", e.Data);
            }
        }

        private static void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                LogMsg(e.Data);
            }
        }

        private static void PrintUsages()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    VMwareService install [service-name]");
            Console.WriteLine("        or");
            Console.WriteLine("    VMwareService uninstall|remove [service-name]");
            Console.WriteLine("        or");
            Console.WriteLine("    VMwareService start | stop | restart [--as-service | -s [service-name]]");
            Console.WriteLine("        or");
            Console.WriteLine("    VMwareService suspend");
            Console.WriteLine();
            Console.WriteLine("You can specify the service name or not. The default service name is VMwareService.");
            Console.WriteLine();
            Console.WriteLine("You can start, stop, or restart the vmware virtual machine as a service by option -s (or --as-service). Or without the option, it will operates in command line mode.");
        }
    }
}
