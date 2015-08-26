VMwareService
=============

A VMware Workstation virtual machine Windows service wrapper.

VMwareService wraps virtual machine as a windows service, which can be loaded
automaticly at the time Windows startup.  When the Windows shutdown, the
virtual machine will suspend automaticly.  And resume from the last time it
suspended while the Windows startup again.


REQUIREMENTS
------------
* .NET Framework 4.0 is required.
* Create `VMwareService.cfg` file manully is required.


HOW TO USE
----------
1. Unzip the `VMwareService.zip` you downloaded from [release zone] into a
   directory.

2. Create a plain text config file with your favorite editer (e.g. notepad)
   in the same diretory you unzipped `VMwareService.zip`.

3. Type the commands as follow in command line window (administrator is needed):

```batch
VMwareService install
NET START VMwareService
```


CONFIG FILE
-----------
The config file is a plain text file with name `VMwareService.cfg`, and stored
in the same directory as VMwareService.exe file is.  You can use any editor
you faviote (e.g. `Notepad` with Windows, or [Sublime Text] is mine) to
create/edit it.

It's filled within .vmx file (VMware virtual machine file) full path line by
line as follow:

```
C:\Users\somebody\Documents\Virtual Machines\Ubuntu-Trusty-VM\Ubuntu-Trusty-VM.vmx
C:\Users\somebody\Documents\Virtual Machines\Ubuntu-Vivid-VM\Ubuntu-Vivid-VM.vmx
```

The VMwareService read these .vmx file pull path from the config file, and
start them one by one.

**Note**

- VMwareService CAN NOT work without this config file.
- The released v1.0 CAN NOT support multi virtual machine.  It means, the
  config file can be parsed only the first line.


LOG FILE
--------
The VMwareService log file is a plain text file named as `VMwareService.log`,
and stored in the same directory of `VMwareService.exe`.  This file logs
start/stop, install/uninstall, and error informations of VMwareServcice while
it runs.  Usally, you don't need to care about this file and it's informations, but get problems in unsing VMwareService. e.g:

* The following logs declares that you're attempting to stop a virtual machine
  that is never started.  VmwareService tried times to accomplte it with
  interval 5 seconds.  And stopped the tries after 10 times.

```
[2015/8/26 17:06:27]	Trying to stop in 5 seconds. (try 1 of 10 times)
[2015/8/26 17:06:27]	Stopping VMware process ...
[2015/8/26 17:06:28]	Error: The virtual machine is not powered on: C:\Users\somebody\Documents\Virtual Machines\Ubuntu-Trusty-VM\Ubuntu-Trusty-VM.vmx
[2015/8/26 17:06:28]	ERROR: Failed to stop VMware process. (error code: -1)
[2015/8/26 17:06:33]	Trying to stop in 5 seconds. (try 2 of 10 times)
[2015/8/26 17:06:33]	Stopping VMware process ...
[2015/8/26 17:06:33]	Error: The virtual machine is not powered on: C:\Users\somebody\Documents\Virtual Machines\Ubuntu-Trusty-VM\Ubuntu-Trusty-VM.vmx
[2015/8/26 17:06:33]	ERROR: Failed to stop VMware process. (error code: -1)
...
[2015/8/26 17:07:16]	Trying to stop in 5 seconds. (try 10 of 10 times)
[2015/8/26 17:07:16]	Stopping VMware process ...
[2015/8/26 17:07:16]	Error: The virtual machine is not powered on: C:\Users\somebody\Documents\Virtual Machines\Ubuntu-Trusty-VM\Ubuntu-Trusty-VM.vmx
[2015/8/26 17:07:16]	ERROR: Failed to stop VMware process. (error code: -1)
[2015/8/26 17:07:16]	Reaches the max retry times. Stop trying.
```

* The following logs declares that the config file is missing.

```
[2015/8/26 17:31:49]	Trying to start in 5 seconds. (try 1 of 10 times)
[2015/8/26 17:31:49]	Starting VMware process ...
[2015/8/26 17:31:49]	Could not find config file. (Current directory: C:\Users\somebody\Documents\Visual Studio 2015\Projects\VirtualMachineServices\VMwareService\bin\Debug)
...
```


NEED TO BE KNOWN
----------------
* The `VMware Workstation` locks virtual machine while it run.  In this case,
  the virtual machine can not be accessed by another application, including
  VMwareService.  So, it's needed to close `VMware Workstation` application
  before start, stop, or restart the VMwareService.  And restart `VMware
  Workstation` after VMwareService is started, stopped, or restarted done.
  Otherwise, the VMwareService will not work correctly.

* If you want to edit the virtual machine's configuration in `VMware
  Workstation`, it's needed to stop VMwareService before, and start
  VMwareService after the editing is done.


LICENSE
-------
Copyright 2015 kimw

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.


[release zone]: https://github.com/kimw/VMwareService/releases
[Sublime Text]: https://www.sublimetext.com/
[GPL v3.0 license]: https://raw.githubusercontent.com/kimw/VMwareService/master/LICENSE
