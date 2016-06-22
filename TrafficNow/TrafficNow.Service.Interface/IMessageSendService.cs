using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Enums;

namespace TrafficNow.Service.Interface
{
    public interface IMessageSendService
    {
        Task SendMessage<T>(AmqpTypes amqpType, T model, List<string> receipeients);
    }
}
