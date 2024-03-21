using System;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using fanCtrl;

class Program
{
    private static float[] fanSpeeds = { 30.0f, 50.0f, 70.0f, 90.0f, 100.0f };
    private static int currentIndex = 0;
    private static NamedPipeServerStream? pipeServer = null;
    static Form? form; // 声明窗体作为全局变量
    private static Computer computer = new Computer()
    {
        MainboardEnabled = true,
        CPUEnabled = true,
        RAMEnabled = true,
        GPUEnabled = true,
        FanControllerEnabled = true,
        HDDEnabled = true
    };

    static void Main(string[] args)
    {
        // 使用Mutex确保单实例运行
        bool createdNew;
        using (Mutex mutex = new Mutex(true, "Global\\OHWM_FanControl", out createdNew))
        {
            if (!createdNew)
            {
                Console.WriteLine("\n\n检测到另一个实例正在运行，已发送新的风扇速度设置命令。");
                SendCommandToRunningInstance();
                return;
            }

            Console.WriteLine("\n未检测到另一个实例正在运行，已启动新的实例。");

            ShowWindow();

            IconThread();

            computer.Open();
            Console.WriteLine("\n启动风扇控制...");

            // 初始风扇速度设置
            TraverseHardware(computer.Hardware, currentIndex);

            pipeServer = new NamedPipeServerStream(
                "OHWM_FanControlPipe", // 管道名称
                PipeDirection.InOut, //指定管道的方向，即可以用于输入和输出。
                1, //管道的最大服务器实例数。在这里，指定为1表示只能有一个客户端连接到服务器。
                PipeTransmissionMode.Byte, //指定了管道的传输模式，即每次传输一个字节。
                PipeOptions.Asynchronous // 指定了管道的选项，表示管道可以异步进行通信。
            );

            // 异步接收命令
            pipeServer.BeginWaitForConnection(HandleClientConnection, null);
            Console.WriteLine("\n等待通信...");

            Application.Run();
            computer.Close();
            Application.Exit();
        }
    }

    static void IconThread()
    {
        // 创建托盘图标
        NotifyIcon notifyIcon = new NotifyIcon();
        notifyIcon.Icon = new Icon("Re.ico"); // 设置图标
        notifyIcon.Visible = true;

        // 添加托盘图标的右键菜单
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += (sender, e) =>
        {
            computer.Close(); // 在退出程序时关闭computer对象
            Application.Exit(); // 点击退出菜单时退出程序
        }; // 点击退出菜单时退出程序
        contextMenu.Items.Add(exitMenuItem);
        notifyIcon.ContextMenuStrip = contextMenu;

        notifyIcon.DoubleClick += (sender, e) =>
        {
            if (form != null)
            {
                form.Visible = true;
                form.WindowState = FormWindowState.Normal;
            }
        };
    }

    static void ShowWindow()
    {
        form = new Form();
        form.Text = "fanCtrl V.0.51"; // 设置窗体标题
        form.Size = new Size(500, 770); // 设置窗体大小

        // 创建一个用于显示文本的 RichTextBox 控件
        RichTextBox richTextBox = new RichTextBox();
        richTextBox.Multiline = true;
        richTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        richTextBox.ReadOnly = true;
        richTextBox.Dock = DockStyle.Fill;
        richTextBox.SelectionBullet = false;
        richTextBox.Font = new Font("Consolas", 10f); // 设置文本框字体
        richTextBox.Text = "Version 0.51\n\n\n"; // 设置初始文本内容
        form.Controls.Add(richTextBox);

        // 在窗体加载完成后设置控制台输出的重定向
        form.Load += (sender, e) =>
        {
            // 将原本要显示在控制台上的文本写入文本框
            Console.SetOut(new ControlWriter(richTextBox));
        };

        // 当窗口关闭时最小化到托盘
        form.FormClosing += (sender, e) =>
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // 取消关闭操作
                form.Hide(); // 隐藏窗口
            }
        };

        // 先显示窗体
        form.Show();
    }

    private static void SendCommandToRunningInstance()
    {
        // 通过命名管道发送新的风扇速度索引
        using (var client = new NamedPipeClientStream("OHWM_FanControlPipe"))
        {
            client.Connect();
            client.WriteByte(3); // 3 表示轮换调整风扇速度
            Console.WriteLine($"\n发送调整风扇的指令完毕。\n\n");
            client.Flush();
            client.Close();
        }
    }

    private static void HandleClientConnection(IAsyncResult ar)
    {
        // 结束等待客户端连接
        pipeServer?.EndWaitForConnection(ar);
        if (pipeServer != null && pipeServer.CanRead)
        {
            int signal = pipeServer.ReadByte();
            Console.WriteLine("\n\n\n-----------------收到新的风扇速度设置命令-----------------\n\n\n");
            if (signal == 0)
            { // Slow.cs 发送的信号
                currentIndex--;
                Console.WriteLine("---收到降低风扇速度的指令---");
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    Console.WriteLine("\n已是最低速度\n");
                }
            }
            else if (signal == 1)
            { // Fast.cs 发送的信号
                currentIndex++;
                Console.WriteLine("+++收到提高风扇速度的指令+++");
                if (currentIndex >= fanSpeeds.Length)
                {
                    currentIndex = fanSpeeds.Length - 1;
                    Console.WriteLine("\n已是最高速度\n");
                }
            }
            else if (signal == 3)
            { // SendCommandToRunningInstance 发送的信号
                currentIndex++;
                Console.WriteLine("-+-收到轮换风扇速度的指令-+-");
                currentIndex = currentIndex % fanSpeeds.Length;
            }

            // 应用新的风扇速度设置
            Console.WriteLine($"\n将把风扇速度设置为 {fanSpeeds[currentIndex]}%.\n");
            TraverseHardware(computer.Hardware, currentIndex);
        }
        // 断开连接并关闭管道
        pipeServer?.Disconnect();
        pipeServer?.Close();

        Thread.Sleep(1700); // 等待1.7秒

        // 重新等待新的客户端连接
        pipeServer = new NamedPipeServerStream(
            "OHWM_FanControlPipe",
            PipeDirection.InOut,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous
        );

        Console.WriteLine("\n等待通信...");
        pipeServer.BeginWaitForConnection(HandleClientConnection, null);
    }

    static void TraverseHardware(IHardware[] hardwareItems, int index)
    {
        foreach (var hardware in hardwareItems)
        {
            if (hardware.HardwareType == HardwareType.Mainboard)
            {
                Console.WriteLine($"硬件名称: {hardware.Name}, 类型: {hardware.HardwareType}");
                hardware.Update(); // 更新硬件信息以获取最新数据
                if (hardware.SubHardware.Length > 0)
                {
                    foreach (IHardware subHardware in hardware.SubHardware)
                    {
                        subHardware.Update();
                        foreach (var sensor in subHardware.Sensors)
                        {
                            // 仅处理控制类型的传感器
                            if (sensor.SensorType == SensorType.Control)
                            {
                                //仅处理风扇控制器
                                if (sensor.Index == 1 || sensor.Index == 2)
                                {
                                    if (sensor.Control != null)
                                    {
                                        // 尝试设置一个新的控制值
                                        float newValue = fanSpeeds[index];
                                        sensor.Control.SetSoftware(newValue);
                                        Console.WriteLine($"已把 {sensor.Name} 的控制值设置为 {newValue}%");
                                        subHardware.Update();
                                    }
                                }

                                //调整水泵速度
                                if (sensor.Index == 5)
                                {
                                    if (sensor.Control != null)
                                    {
                                        // 尝试设置一个新的控制值
                                        float newValue = fanSpeeds[index];
                                        if (newValue >= 70.0f)
                                        { //如果风扇速度大于70%，则水泵速度设置为35%
                                            newValue = 35.0f;
                                        }
                                        else
                                        {
                                            newValue = 25.0f;
                                        }

                                        sensor.Control.SetSoftware(newValue);
                                        Console.WriteLine($"已把 {sensor.Name} 的控制值设置为 {newValue}%");
                                        subHardware.Update();
                                    }
                                }

                                /*Console.WriteLine(
                                    String.Format(
                                        "\n名称 {0} 类型 {1} \n\t\t 部件 {2} = 当前值 {3}\n index {4}",
                                        sensor.Name,
                                        sensor.SensorType,
                                        sensor.Hardware,
                                        sensor.Value.HasValue
                                            ? sensor.Value.Value.ToString()
                                            : "no value",
                                        sensor.Index
                                    )
                                );*/
                            }
                        }
                    }
                }
            }
        }
    }
}
