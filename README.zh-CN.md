# VMwareService

一个可以将 VMware 虚拟机包装成 Windows 服务程序的软件。

用 VMwareService 包装好的虚拟机可以跟随 Windows 操作系统开机时自动加载；Windows 操作系统关机时自动挂起在虚拟机中的当前操作，并在下一次 Windows 操作系统开机时恢复到上次关机的状态。


怎么开始
--------
开始使用 VMwareService 需要完成以下三个简单的步骤：

1. 将在发布区下载的最新版软件压缩文件解压缩到一个目录。

2. 用你熟悉的编辑器（例如 Windows 操作系统自带的“记事本”）在该目录中创建一个名为 VMwareService.cfg 的文本文件。该文件是 VMwareService 的配置文件，配置格式在“[配置文件]”一节中有说明。

3. 以管理员权限打开“命令行提示符”，输入以下两行命令：

```bat
VMwareService install
NET START VMwareService
```


配置文件
--------
VMwareService 配置文件是一个纯文本文件。你可以使用任何你喜欢的编辑器（例如 Windows 操作系统自带的“记事本”，或者也可以像我一样使用 [Sublime Text] 来完成编辑）创建或者修改它。

配置文件中要求每一行一个 VMware 的 .vmx 文件的完整路径。就像下面这个例子一样：

```
C:\Users\kimw\Documents\Virtual Machines\Ubuntu-Trusty-VM\Ubuntu-Trusty-VM.vmx
C:\Users\kimw\Documents\Virtual Machines\Ubuntu-Vivid-VM\Ubuntu-Vivid-VM.vmx
```

VMwareService 在启动时逐行读取这些 .vmx 文件的完整路径，然后一个一个的启动这些虚拟机。

**注意：**

- 没有这个配置文件，VMwareService 将不能正常工作。
- 已经发布的 v1.0 版本不支持启动多个虚拟机。即，配置文件只能识别第一行，其他行将被忽略。


运行要求
--------
* 需要 .NET Frameworks 4.0
* 必须手工创建 VMwareService 配置文件


注意事项
--------
因为`VMware Workstation 应用程序`在运行期间会锁定虚拟机，造成虚拟机无法访问，所以在`启动`、`停止`或者`重新启动`服务的时候，需要先关闭`VMware Workstation 应用程序`，待`启动`、`停止`或者`重新启动`完成后再打开`VMware Workstation 应用程序`。否则`VMwareService`不能启动成功。

如果需要`编辑虚拟机设置`，需要先`停止`服务。待完成`编辑虚拟机设置`后，再`启动`服务。


许可证
------
本软件遵循 [GPL v3.0 许可证]。


[Sublime Text]: https://www.sublimetext.com/
[GPL v3.0 许可证]: https://raw.githubusercontent.com/kimw/VMwareService/master/LICENSE
