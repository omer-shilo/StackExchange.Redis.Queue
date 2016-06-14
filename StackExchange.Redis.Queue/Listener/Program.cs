using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Queue.Common;
using StackExchange.Redis.Queue.Common.Interfaces;
using StackExchange.Redis.Queue.Wrapper;
using Unity;

namespace Listener
{
    public class Program
    {
        private static int counter = 0;

        public static void Main(string[] args)
        {
            using (var container = new UnityContainer())
            {
                container.RegisterInstance(typeof(IQueue), RedisQueueWrapper.Instance);
                var queue = container.Resolve<QueueUsage>();

                queue.Connect("localhost:6379");

                var action = new Action<List<string>>((inputList) =>
                {
                    foreach (var item in inputList)
                    {
                        counter++;
                        Console.WriteLine($"The following message was extracted : {item}, counter : {counter}");
                    }
                });

                CleanQueue(queue,"StringQueue", action);
                Console.WriteLine("Starting listening to the queue");
                queue.SubscribeAsync<string>("StringQueue",action,50);
                Console.WriteLine("Press Any Key To Finish");
                Console.ReadKey();
            }
        }

        private static async void CleanQueue<T>(IQueue queue,string queueName, Action<List<T>> action )
        {
            var flag = true;

            while (flag)
            {
                var res = await queue.DequeueAsync<T>(queueName, 50);

                if (res.Count != 0)
                {
                    action(res);
                }
                else
                {
                    flag = false;
                }
            }
        }
    }
}
