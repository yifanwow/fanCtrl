using System;
using OpenHardwareMonitor.Hardware;

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

        foreach (var hardware in computer.Hardware)
        {
            Console.WriteLine($"硬件名称: {hardware.Name}, 类型: {hardware.HardwareType}");
        }

        // 遍历所有硬件和子硬件来寻找并列出控制项
        //TraverseHardware(computer.Hardware);

        computer.Close();
    }

    static void TraverseHardware(IHardware[] hardwareItems)
    {
        foreach (var hardware in hardwareItems)
        {
            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.Control != null) // 检查是否存在控制项
                {
                    Console.WriteLine(
                        $"控制项: {hardware.Name} - {sensor.Name}, "
                            + $"当前值: {sensor.Control.SoftwareValue}"
                    );
                }
            }

            // 递归遍历子硬件的控制项
            if (hardware.SubHardware.Length > 0)
            {
                TraverseHardware(hardware.SubHardware);
            }
        }
    }
}
