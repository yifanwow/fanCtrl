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
            client.WriteByte(0); // 0 表示向下调整风扇速度
            client.Flush();
            client.Close();
        }
    }
}
