using System;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        try
        {
            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            shell.Run("schtasks.exe /run /tn \"fanFast\"", 0, false);
        }
        catch (COMException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
