using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Queue.Common;
using StackExchange.Redis.Queue.Common.Interfaces;
using StackExchange.Redis.Queue.Wrapper;
using Unity;

namespace Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance(typeof(IQueue), RedisQueueWrapper.Instance);
                var queue = container.Resolve<QueueUsage>();

                queue.Connect("localhost:6379");

                var amount = 10000;
                Console.WriteLine($"You are about to send {amount} messages, Press any key to start");
                Console.ReadKey();
                for (var i = 0; i < amount; i++)
                {
                    queue.Publish<string>("Test Message", "StringQueue");
                    Console.WriteLine($"Sent {i} messages");
                }

                Console.WriteLine($"Finished sending {amount} messages");
                Console.WriteLine("Press any key to finish");
                Console.ReadKey();
            }
        }
    }
}
