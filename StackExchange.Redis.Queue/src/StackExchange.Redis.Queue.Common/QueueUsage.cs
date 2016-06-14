using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Queue.Common.Interfaces;

namespace StackExchange.Redis.Queue.Common
{
    public class QueueUsage : IQueue
    {
        #region Properties
        private IQueue _queue { get; set; }
        #endregion

        #region Methods

        public QueueUsage(IQueue queueService)
        {
            _queue = queueService;
        }

        public void Connect(string connectionString)
        {
            _queue.Connect(connectionString);
        }

        public void Enqueue<T>(T message, string queueName)
        {
            _queue.Enqueue<T>(message,queueName);
        }

        public Task<List<T>> DequeueAsync<T>(string queueName, int messageAmount = 1)
        {
            var res = _queue.DequeueAsync<T>(queueName, messageAmount);

            return res;
        }

        public List<T> Dequeue<T>(string queueName, int messageAmount = 1)
        {
            var res = _queue.Dequeue<T>(queueName, messageAmount);

            return res;
        }

        public void Publish<T>(T message, string queueName)
        {
            _queue.Publish(message,queueName);
        }

        public void SubscribeAsync<T>(string queueName, Action<List<T>> callback, int messageAmount = 1)
        {
            _queue.SubscribeAsync<T>(queueName, callback, messageAmount);
        }

        public void Subscribe<T>(string queueName, Action<List<T>> callback, int messageAmount = 1)
        {
            _queue.Subscribe<T>(queueName, callback, messageAmount);
        }
        #endregion
    }
}
