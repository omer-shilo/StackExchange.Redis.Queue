using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Message.Common.Interface;
using StackExchange.Redis.Message.Common.Messages;
using StackExchange.Redis.Queue.Common.Interfaces;
using StackExchange.Redis.Queue.Wrapper;

namespace StackExchange.Redis.Worker
{
    public class Program
    {
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
                    var messages = await queue.DequeueAsync<IMessage>("FileQueue", 50);

                    if (messages.Any())
                    {
                        foreach (var msg in messages)
                        {
                            msg.Execute();
                        }
                    }

                    var consoleMessages = await queue.DequeueAsync<IMessage>("ConsoleQueue", 50);

                    if (consoleMessages.Any())
                    {
                        foreach (var msg in consoleMessages)
                        {
                            msg.Execute();
                        }
                    }
                }, null, 0, 100);

                Console.ReadKey();
                PollingTimer.Dispose();
            }
        }

        private static void RegisterTypes()
        {
            var collection = new ServiceCollection().AddSingleton<IQueue>(new RedisQueueWrapper("localhost:6379"));
            ServiceProvider = collection.BuildServiceProvider();
        }
    }
}
