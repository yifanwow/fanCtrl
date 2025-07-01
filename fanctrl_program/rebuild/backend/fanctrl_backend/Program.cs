using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using fanCtrl; // 引入 HardwareInfo 类所在的命名空间

namespace fanCtrl
{
    class Program
    {
        // 命名管道对象，用于与前端通信
        private static NamedPipeServerStream? pipeServer = null;

        static void Main(string[] args)
        {
            Console.WriteLine("fanCtrl 后端服务已启动");

            // 初始化硬件信息，只执行一次
            string info = HardwareInfo.GetSummary();
            Console.WriteLine("初始硬件信息已获取：\n" + info);

            // 创建命名管道，等待前端连接
            pipeServer = new NamedPipeServerStream(
                "OHWM_FanControlPipe",     // 管道名，前端要用同名连接
                PipeDirection.InOut,       // 读写双向
                1,                          // 最多一个客户端连接
                PipeTransmissionMode.Byte, // 字节传输模式
                PipeOptions.Asynchronous    // 异步处理
            );

            // 开始等待前端连接，一旦连接就发送 info
            pipeServer.BeginWaitForConnection(ar =>
            {
                try
                {
                    pipeServer?.EndWaitForConnection(ar);

                    // 连接成功，发送初始化硬件信息
                    byte[] buffer = Encoding.UTF8.GetBytes(info);
                    pipeServer?.Write(buffer, 0, buffer.Length);
                    pipeServer?.Flush();
                    Console.WriteLine("✅ 初始化硬件信息已发送给前端");

                    // 后续保留连接，可用于接收更多控制类指令（暂未处理）
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" 与前端通信失败: " + ex.Message);
                }
            }, null);

            // 阻止主线程退出
            while (true)
            {
                Thread.Sleep(5000);
            }
        }
    }
}
