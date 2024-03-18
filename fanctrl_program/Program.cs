using System;
using OpenHardwareMonitor.Hardware;
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
            Console.WriteLine($"{indent}硬件名称: {hardware.Name}, 类型: {hardware.HardwareType}");
            hardware.Update(); // 更新硬件信息以获取最新数据

            // 列出所有传感器和控制项
            foreach (var sensor in hardware.Sensors)
            {
                Console.WriteLine(
                    $"{indent}  传感器: {sensor.Name}, 类型: {sensor.SensorType}, 读数: {sensor.Value}"
                );
                if (sensor.Control != null) // 检查是否存在控制项
                {
                    Console.WriteLine(
                        $"{indent}    控制项: {sensor.Name}, 当前值: {sensor.Control.SoftwareValue}"
                    );
                }
            }

            // 处理子硬件
            if (hardware.HardwareType == HardwareType.Mainboard)
            {
                Console.WriteLine("主板: " + hardware.Name);
                if (hardware.SubHardware != null)
                {
                    Console.WriteLine("子硬件数量: " + hardware.SubHardware.Length);
                }
            }
        }
    }
}
