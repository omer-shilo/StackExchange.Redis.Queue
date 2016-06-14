using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Queue.Common.Interfaces
{
    public interface IQueue
    {
        void Connect(string connectionString);

        void Enqueue<T>(T message, string queueName);

        Task<List<T>> DequeueAsync<T>(string queueName, int messageAmount = 1);

        List<T> Dequeue<T>(string queueName, int messageAmount = 1); 

        void Publish<T>(T message, string queueName);

        void SubscribeAsync<T>(string queueName, Action<List<T>> callback, int messageAmount = 1);

        void Subscribe<T>(string queueName, Action<List<T>> callback, int messageAmount = 1);
    }
}
