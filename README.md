VMwareService
=============

A VMware Workstation virtual machine Windows service wrapper.

The virtual machine wrapped with VMwareService can be loaded automaticly
at the time Windows startup.  When the Windows shutdown, the virtual machine
will suspend automaticly.  And resume from the last time it suspended while
the Windows startup again.


REQUIREMENTS
------------
* .NET Framework 4.0 is required.
* Create VMwareService.cfg file manully is required.


HOW TO USE
----------
1. Unzip the zip file you downloaded from [release zone] into a directory.

2. Create a plain text config file with your favorite editer (e.g. notepad).

3. Type the command as follow in command line window (administrator is needed):

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
T.B.C


NEED TO BE KNOWN
----------------
T.B.C


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
