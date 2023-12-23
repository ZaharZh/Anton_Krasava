﻿using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Client
{
    public class Program
    {
        public struct Ad
        {
            public int X;
            public bool Podtv;
        }

        public static void Main()
        {
            try
            {
                Console.WriteLine("Подключение к серверу...\n");
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "pashcha", PipeDirection.InOut))
                {
                    pipeClient.Connect();
                    Console.WriteLine("Подключен к серверу!\n");
                    Console.WriteLine("Ожидание данных...\n");

                    while (true)
                    {
                        byte[] buffer = new byte[Unsafe.SizeOf<Ad>()];
                        pipeClient.Read(buffer);
                        var response = MemoryMarshal.Read<Ad>(buffer);

                        Console.WriteLine($"Получены: {response.X}, {response.Podtv}\n");
                        response.Podtv = true;
                        Console.WriteLine($"Отправлены: {response.X}, {response.Podtv}...\n");

                        byte[] data = new byte[Unsafe.SizeOf<Ad>()];
                        MemoryMarshal.Write(data, ref response);
                        pipeClient.Write(data);
                    }
                }
            }
            catch
            {
                //это читает лучший препод на свете!
            }
        }
    }
}
