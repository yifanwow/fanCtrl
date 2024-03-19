using System;
using System.IO;
using System.IO.Pipes;

class fanSpeedSlow
{
    static void Main(string[] args)
    {
        using (var client = new NamedPipeClientStream("OHWM_FanControlPipe"))
        {
            client.Connect();
            Console.WriteLine("发送降低风扇速度的指令...");
            client.WriteByte(0); // 0 表示向下调整风扇速度
            client.Flush();
            client.Close();
        }
    }
}
