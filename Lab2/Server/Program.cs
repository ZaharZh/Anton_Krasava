﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public struct Ad
    {
        public int X;
        public bool Podtv;
        public override string ToString() => $"Данные = {X}, Ответ = {Podtv}";
    }

    internal class Program
    {
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static CancellationToken cancellationToken = cancellationTokenSource.Token;
        private static PriorityQueue<Ad, int> queue = new PriorityQueue<Ad, int>();
        private static Mutex mutex = new Mutex();

        private static Task clientTask(CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine("Введите значение -> ");
                    var value = Console.ReadLine();
                    if (string.IsNullOrEmpty(value))
                    {
                        Console.WriteLine("Нет данных, попробуйте заново\n");
                        continue;
                    }

                    Console.WriteLine("Введите приоритет -> ");
                    var priority = Console.ReadLine();
                    if (string.IsNullOrEmpty(priority))
                    {
                        Console.WriteLine("Нет данных, попробуйте заново\n");
                        continue;
                    }

                    var data = new Ad { X = Convert.ToInt32(value), Podtv = false };

                    mutex.WaitOne();
                    queue.Enqueue(data, Convert.ToInt32(priority));
                    mutex.ReleaseMutex();
                }
            });
        }

        private static Task serverTask(NamedPipeServerStream stream, CancellationToken token)
        {
            return Task.Run(() =>
            {
                List<Ad> dataList = new List<Ad>();
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (queue.Count >= 1)
                        {
                            mutex.WaitOne();
                            var data = queue.Dequeue();
                            mutex.ReleaseMutex();

                            byte[] buffer = new byte[Unsafe.SizeOf<Ad>()];
                            MemoryMarshal.Write(buffer, ref data);
                            stream.Write(buffer);

                            buffer = new byte[Unsafe.SizeOf<Ad>()];
                            stream.Read(buffer);
                            dataList.Add(MemoryMarshal.Read<Ad>(buffer));
                        }
                    }
                    catch (Exception)
                    {
                        //хочу пиццу(и пять за экзамен)
                    }
                }

                foreach (var item in dataList)
                {
                    Console.WriteLine(item);
                }
            });
        }

        private static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            Console.WriteLine("Жду клиента\n");
            using (var stream = new NamedPipeServerStream("pashcha", PipeDirection.InOut))
            {
                stream.WaitForConnection();
                Console.WriteLine("Клиент подключен!\n");

                Task task1 = serverTask(stream, cancellationToken);
                Task task2 = clientTask(cancellationToken);

                await Task.WhenAll(task1, task2);
            }
        }
    }
}
