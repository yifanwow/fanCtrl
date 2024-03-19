# fanCtrl
A C# program to control your computer fan speed (Hardware).  

### 实现原理：
原本是打算用wmi的方式来进行调整的，但是wmi的接口只提供了监听而不提供修改的功能。  
所以目前是使用OpenHardwareMonitorLib.dll作为接口，其提供了一系列对于设备底层的监控和调整的指令。  
由于原软件会导致多实例并存的问题，所以使用了 Mutex 类来确保只有一个实例的程序在运行，Mutex是协调多个线程对共享资源的访问的同步基元。  

程序在运行时会尝试创建一个名为 "Global\\OHWM_FanControl" 的全局互斥体。如果程序是第一个尝试创建这个互斥体的实例（即 createdNew 为 true），则它获得了互斥体的初始所有权，可以继续执行。  如果之前已经有一个实例在运行，则 createdNew 为 false，并且程序会输出一条消息并返回。  

再次运行，新的实例则会通知原有的实例对速度进行轮换的调整。  
在主实例界面输入任意值都可以关闭主实例。  

### 注意事项：
- 必须将文件部署到本地磁盘，**网络连接的磁盘无法启动管理员权限**。  
- 必须至少有一个主实例运行才能够保持风扇转速的调整。
- 由于硬件设备的性能涉及到管理员权限操作，所以请务必**开启管理员权限**或以管理员身份运行程序，否则会报错。
- 目前部署之后还需要一定的手动操作来完成自动化设置，后续如果有时间会在继续改进争取一键懒人部署。

### 步骤：
编译的部分需要使用VS的命令行来进行：  
`csc /out:fanSpeedFast.exe fanSpeedFast.cs`  
或者用`dotnet build`和`publish`的命令来生成可执行的fast.exe或者slow.exe
  
### 降低使用过程中的突兀感
- #### 使用Task Scheduler来绕过UAC的提示:
    - [How to Create an Elevated Program Shortcut without a UAC Prompt in Windows](https://www.sevenforums.com/tutorials/11949-elevated-program-shortcut-without-uac-prompt-create.html)
    - [How To Add Program To UAC Exception In Windows 10?](https://silicophilic.com/add-program-to-uac-exception/)
- #### 使用vbs脚本来执行任务指令。
> Q: 为什么不使用PowerShell或者Shortcut或者cmd?  
我个人尝试了这三种情况都会弹出窗口，一闪而过，但理论上来说应该也是可以避免的，不过vbs比较简单就用了。


### 友情链接：
感谢所有的开源开发者以及OpenHardwareMoniter的制作者和维护者们。
- [OpenHardwareMoniter](https://github.com/openhardwaremonitor/openhardwaremonitor)  
- [The Open Hardware Monitor WMI Provider](https://openhardwaremonitor.org/wordpress/wp-content/uploads/2011/04/OpenHardwareMonitor-WMI.pdf)