using System;
using OpenHardwareMonitor.Hardware;
using System.Text;
using OpenHardwareMonitor.Hardware.LPC;

class Program
{
    static void Main(string[] args)
    {
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

        // 遍历所有硬件来寻找并列出控制项
        TraverseHardware(computer.Hardware);

        computer.Close();
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
                            Console.WriteLine(
                                String.Format(
                                    "名称 {0} 部件 {1} = 当前值 {2}",
                                    sensor.Name,
                                    sensor.Hardware,
                                    sensor.Value.HasValue
                                        ? sensor.Value.Value.ToString()
                                        : "no value"
                                )
                            );
                        }
                    }
                }
            }
        }
    }
}
