using System;
using OpenHardwareMonitor.Hardware;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        bool createdNew;
        using (Mutex mutex = new Mutex(true, "Global\\OHWM_FanControl", out createdNew))
        {
            if (!createdNew)
            {
                Console.WriteLine("已经有一个实例在运行。");
                // 如果需要，可以在这里实现向已运行实例发送消息的逻辑
                return;
            }

            Computer computer = new Computer()
            {
                MainboardEnabled = true,
                CPUEnabled = true,
                RAMEnabled = true,
                GPUEnabled = true,
                FanControllerEnabled = true,
                HDDEnabled = true
            };

            computer.Open();
            Console.WriteLine("列出所有可用的控制项：");

            TraverseHardware(computer.Hardware);

            Console.WriteLine("程序将保持运行以维持风扇设置。按任意键退出...");
            Console.ReadKey();

            computer.Close();
        }
    }

    static void TraverseHardware(IHardware[] hardwareItems, string indent = "")
    {
        foreach (var hardware in hardwareItems)
        {
            if (hardware.HardwareType == HardwareType.Mainboard)
            {
                Console.WriteLine($"{indent}硬件名称: {hardware.Name}, 类型: {hardware.HardwareType}");
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
                                        //默认将所有的风扇控制值设置为50%
                                        float newValue = 30.0f;
                                        // 尝试设置一个新的控制值
                                        sensor.Control.SetSoftware(newValue);
                                        Console.WriteLine($"已将 {sensor.Name} 的控制值设置为 {newValue}%");
                                        subHardware.Update();
                                    }
                                }

                                Console.WriteLine(
                                    String.Format(
                                        "\n名称 {0} 类型 {1} \n          部件 {2} = 当前值 {3}\n index {4}",
                                        sensor.Name,
                                        sensor.SensorType,
                                        sensor.Hardware,
                                        sensor.Value.HasValue
                                            ? sensor.Value.Value.ToString()
                                            : "no value",
                                        sensor.Index
                                    )
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}
