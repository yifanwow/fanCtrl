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

        // 遍历控制器传感器
        foreach (ManagementObject obj in collection)
        {
            if (
                obj["SensorType"].ToString() == "Control"
                && obj["Value"] != null
                && obj["Value"].ToString() != "0"
            )
            {
                string sensorName = obj["Name"]?.ToString() ?? "Unknown";
                string powerValue = obj["Value"]?.ToString() ?? "Unknown";
                Console.WriteLine("端口名称: " + sensorName + ", 当前功率: " + powerValue + " %");
            }
        }

        Console.WriteLine("输出完毕。按任意键退出。");
        Console.ReadKey();
    }
}
