using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Message.Common.Interface;

namespace StackExchange.Redis.Message.Common.Messages
{
    public class ConsoleWriteMessage : IMessage, IKillable
    {
        public string Message { get; set; }

        public ConsoleWriteMessage(string message)
        {
            Message = message;
        }

        public void Execute()
        {
            Console.WriteLine($"Message content : {Message}");
        }

        public bool IsKillMessage { get; set; }
    }
}
