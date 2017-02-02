using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Enums;

namespace TrafficNow.Service.Interface
{
    public interface IMessageReceiveService
    {
        Task<IEnumerable<T>> GetMessage<T>(AmqpTypes amqpType) where T : class;
    }
}
