using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using OpenHardwareMonitor.Hardware;

class Program
{
    private static float[] fanSpeeds = { 30.0f, 50.0f, 70.0f, 90.0f, 100.0f };
    private static int currentIndex = 0;
    private static NamedPipeServerStream? pipeServer = null;
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
            Console.WriteLine("按任意键退出...\n");
            Console.ReadKey();
            computer.Close();
        }
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
        Console.WriteLine("按任意键退出...\n");
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
                                        Console.WriteLine(
                                            $"\n已把 {sensor.Name} 的控制值设置为 {newValue}%"
                                        );
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
