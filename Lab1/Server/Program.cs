using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class PipeServer
{
    public struct an
    {
        public int x;
        public int y;
        public int z;
    }
    static void Main()
    {
        var an1 = new an {x=7, y=8, z=9};
        
        Console.WriteLine("Створаны аб'ект ПатокCервераНайменнагаКанала.");
        
        var ton = new NamedPipeServerStream("pashcha", PipeDirection.InOut);
        
        Console.WriteLine("Чаканне падлучэння клiента. ..");
        
        ton.WaitForConnection();
        
        Console.WriteLine("Клiент падлучаны.");
        
        byte[] bytes = new byte[Unsafe.SizeOf<an>()];
        MemoryMarshal.TryWrite<an>(bytes, an1);
        ton.Write(bytes);
        
        Console.WriteLine($"Вось Iкс: {an1.x}, вось Iгрэк: {an1.y}, вось Зэт: {an1.z}");

        byte[] bates = new byte[Unsafe.SizeOf<an>()];
        ton.Read(bates);
        MemoryMarshal.Read<an>(bates); 

    }
}
