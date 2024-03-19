using System;
using System.IO;
using System.IO.Pipes;

class fanSpeedsFast
{
    static void Main(string[] args)
    {
        using (var client = new NamedPipeClientStream("OHWM_FanControlPipe"))
        {
            client.Connect();
            Console.WriteLine("发送增加风扇速度的指令...");
            client.WriteByte(1); // 1 表示向上调整风扇速度
            client.Flush();
            client.Close();
        }
    }
}
