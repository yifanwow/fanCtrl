using System;
using System.Text;
using OpenHardwareMonitor.Hardware;

namespace fanCtrl
{
    public class HardwareInfo
    {
        // 提供一个公共方法供外部调用，返回硬件信息汇总字符串
        public static string GetSummary()
        {
            StringBuilder sb = new StringBuilder();

            // 初始化硬件读取对象
            Computer computer = new Computer()
            {
                MainboardEnabled = true,
                CPUEnabled = true,
                RAMEnabled = true,
                GPUEnabled = true,
                HDDEnabled = true,
                FanControllerEnabled = true,
            };

            computer.Open(); // 开启硬件读取

            foreach (var hardware in computer.Hardware)
            {
                hardware.Update(); // 刷新硬件状态
                sb.AppendLine($"硬件名称: {hardware.Name}, 类型: {hardware.HardwareType}");

                // 处理主硬件上的传感器
                foreach (var sensor in hardware.Sensors)
                {
                    sb.AppendLine($"  传感器: {sensor.Name}, 类型: {sensor.SensorType}, 当前值: {sensor.Value}");
                }

                // 处理子硬件（例如主板的子模块）
                foreach (var subHardware in hardware.SubHardware)
                {
                    subHardware.Update();
                    foreach (var sensor in subHardware.Sensors)
                    {
                        sb.AppendLine($"  子硬件传感器: {sensor.Name}, 类型: {sensor.SensorType}, 当前值: {sensor.Value}");
                    }
                }
            }

            computer.Close(); // 释放资源
            return sb.ToString();
        }
    }
}
