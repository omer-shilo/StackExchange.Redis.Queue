using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Queue.Common;
using StackExchange.Redis.Queue.Common.Interfaces;

namespace StackExchange.Redis.Queue.Wrapper
{
    public class RedisQueueWrapper : IQueue
    {
        #region Fields
        private static readonly object _syncObj = new object();
        private static RedisQueueWrapper _Instance;
        #endregion

        #region Properties

        public static RedisQueueWrapper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_syncObj)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new RedisQueueWrapper();
                        }
                    }
                }

                return _Instance;
            }
        }

        private ConnectionMultiplexer _ConnectionMultiplexer { get; set; }
        #endregion
        private RedisQueueWrapper()
        {
        }
        
        public void Connect(string connectionString)
        {
            try
            {
                if (_ConnectionMultiplexer == null)
                {
                    _ConnectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
                    _ConnectionMultiplexer.PreserveAsyncOrder = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with connecting to the following connection string: {connectionString}", ex);
            }
        }

        public void Enqueue<T>(T message, string queueName)
        {
            try
            {
                var db = _ConnectionMultiplexer.GetDatabase();

                db.ListLeftPush(queueName, Message.SerializeObject(message), When.Always, CommandFlags.FireAndForget);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with performing enqueue to following queue: {queueName}",ex);
            }
        }

        public async Task<List<T>> DequeueAsync<T>(string queueName, int messageAmount = 1)
        {
            try
            {
                var db = _ConnectionMultiplexer.GetDatabase();
                var resultList = new List<T>();

                for (var i = 0; i < messageAmount; i++)
                {
                    var res = await db.ListRightPopAsync(queueName);

                    if (res.HasValue)
                    {
                        resultList.Add(Message.DeserializeObject<T>(res));
                    }
                    else
                    {
                        break;
                    }
                }

                return resultList;
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with performing {nameof(DequeueAsync)}, queue name: {queueName}", ex);
            }
        }

        public List<T> Dequeue<T>(string queueName, int messageAmount = 1)
        {
            try
            {
                var db = _ConnectionMultiplexer.GetDatabase();
                var resultList = new List<T>();

                for (var i = 0; i < messageAmount; i++)
                {
                    var res = db.ListRightPop(queueName);

                    if (res.HasValue)
                    {
                        resultList.Add(Message.DeserializeObject<T>(res));
                    }
                    else
                    {
                        break;
                    }
                }

                return resultList;
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with performing {nameof(Dequeue)}, queue name: {queueName}", ex);
            }
        }

        public void Publish<T>(T message, string queueName)
        {
            try
            {
                var pub = _ConnectionMultiplexer.GetSubscriber();

                Enqueue(message, queueName);
                pub.Publish(queueName, "", CommandFlags.FireAndForget);
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with {nameof(Publish)}, queue name: {queueName}");
            }
        }

        public void SubscribeAsync<T>(string queueName, Action<List<T>> callback, int messageAmount = 1)
        {
            try
            {
                var sub = _ConnectionMultiplexer.GetSubscriber();

                sub.SubscribeAsync(queueName, async delegate
                 {
                     var res = await DequeueAsync<T>(queueName, messageAmount);

                     callback(res);
                 });
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with performing {nameof(SubscribeAsync)}, queue name: {queueName}", ex);
            }
        }

        public void Subscribe<T>(string queueName, Action<List<T>> callback, int messageAmount = 1)
        {
            try
            {
                var sub = _ConnectionMultiplexer.GetSubscriber();

                sub.Subscribe(queueName,  delegate
                {
                    var res = Dequeue<T>(queueName, messageAmount);

                    callback(res);
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"There was a problem with performing {nameof(Subscribe)}, queue name: {queueName}", ex);
            }
        }
    }
}
