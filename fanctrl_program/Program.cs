using System;
using System.Management;

class Program
{
    static void Main(string[] args)
    {
        // 创建WMI查询，用于获取传感器信息
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(
            "root\\OpenHardwareMonitor",
            "SELECT * FROM Sensor"
        );

        // 执行查询
        ManagementObjectCollection collection = searcher.Get();

        // 遍历传感器
        foreach (ManagementObject obj in collection)
        {
            // 检查传感器是否为风扇并且有转速信息
            if (
                obj["SensorType"].ToString() == "Fan"
                && obj["Value"] != null
                && obj["Value"].ToString() != "0"
            )
            {
                string sensorName = obj["Name"]?.ToString() ?? "Unknown"; //在转换之前添加 null 检查或使用 null 合并运算符（?.）来安全地访问这些值
                string fanSpeed = obj["Value"]?.ToString() ?? "Unknown";

                Console.WriteLine("风扇名称: " + sensorName + ", 当前转速: " + fanSpeed + " RPM");
            }
        }

        Console.WriteLine("输出完毕。按任意键退出。");
        Console.ReadKey();
    }
}
