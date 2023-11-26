using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class PipeClient
{
    public struct an
    {
        public int x;
        public int y;
        public int z;
    }
    static void Main(string[] args)
    {
        var ton = new NamedPipeClientStream(".", "pashcha", PipeDirection.InOut);
        
        Console.WriteLine("Спроба падлучэння да трубы. ..");
        ton.Connect();
        
        Console.WriteLine("Падлучаны да трубы.");
        
        byte[] bates = new byte[Unsafe.SizeOf<an>()];
        ton.Read(bates);
        var m = MemoryMarshal.Read<an>(bates);

        m.x -= 1;
        m.y -= 2;
        m.z -= 3;

        Console.WriteLine($"Вось Iкс: {m.x}, вось Iгрэк: {m.y}, вось Зэт: {m.z}");
        
        byte[] bytes = new byte[Unsafe.SizeOf<an>()];
        MemoryMarshal.TryWrite<an>(bytes, m);
        ton.Write(bytes);
           
        Console.WriteLine("Нацiснiце Enter, каб працягнуць...");  
    }
}