using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackExchange.Redis.Message.Common.Interface
{
    public interface IMessage
    {
        void Execute();
    }
}
