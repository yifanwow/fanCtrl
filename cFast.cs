using System.Diagnostics;

class Program
{
    static void Main()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "C:\\Windows\\System32\\schtasks.exe",
            Arguments = "/run /tn \"fanSlow\"", // 确保任务名称正确，如果有空格，用双引号包围
            UseShellExecute = false,
            CreateNoWindow = true, // 不显示新窗口
        };

        Process process = new Process() { StartInfo = startInfo };

        process.Start();
    }
}
