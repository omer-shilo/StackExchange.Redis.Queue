using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Queue.Common;
using StackExchange.Redis.Queue.Common.Interfaces;
using StackExchange.Redis.Queue.Wrapper;

namespace Listener
{
    public class Program
    {
        private static int counter = 0;

        public static IServiceProvider ServiceProvider { get; private set; }
        private static Timer PollingTimer { get; set; }

        public static void Main(string[] args)
        {
            RegisterTypes();

            using (var queue = ServiceProvider.GetService<IQueue>())
            { 
                Console.WriteLine("Starting listening to the queue");
                PollingTimer = new Timer(async state =>
                {
                    var messages = await queue.DequeueAsync<string>("StringQueue", 50);

                    if (messages.Any())
                    {
                        foreach (var msg in messages)
                        {
                            Console.WriteLine($"message content : {msg}");
                            counter++;
                        }
                    }
                },null, 0, 100 );

                Console.ReadKey();
                PollingTimer.Dispose();
                Console.WriteLine($"Listener processed {counter} messages in total.");
            }
        }

        private static void RegisterTypes()
        {
            var collection = new ServiceCollection().AddSingleton<IQueue>(new RedisQueueWrapper("localhost:6379"));
            ServiceProvider = collection.BuildServiceProvider();
        }
    }
}
