using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Message.Common.Messages;
using StackExchange.Redis.Queue.Common;
using StackExchange.Redis.Queue.Common.Interfaces;
using StackExchange.Redis.Queue.Wrapper;

namespace Publisher
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Main(string[] args)
        {
            RegisterTypes();

            using (var queue = ServiceProvider.GetService<IQueue>())
            {
            
                var amount = 10;
                Console.WriteLine($"You are about to send {amount} messages, Press any key to start");
                Console.ReadKey();
                for (var i = 0; i < amount; i++)
                {
                    queue.Enqueue<FileWriteMessage>(new FileWriteMessage(@"D:\Temp\logFile.txt")
                    {
                        Lines = new List<string>() { "line 1","line 2"}
                    }, "FileQueue");

                    Console.WriteLine($"Sent {i} {nameof(FileWriteMessage)} messages");
                }

                for (var i = 0; i < amount; i++)
                {
                    queue.Enqueue<ConsoleWriteMessage>(new ConsoleWriteMessage($"Message number {i}"), "ConsoleQueue");

                    Console.WriteLine($"Sent {i}  {nameof(ConsoleWriteMessage)} messages");
                }

                Console.WriteLine($"Finished sending {amount} messages");
                Console.WriteLine("Press any key to finish");
                Console.ReadKey();
            }
        }

        private static void RegisterTypes()
        {
            var collection = new ServiceCollection().AddSingleton<IQueue>(new RedisQueueWrapper("localhost:6379"));
            ServiceProvider = collection.BuildServiceProvider();
        }

    }
}
