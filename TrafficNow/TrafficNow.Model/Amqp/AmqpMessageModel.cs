using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Model.Amqp
{
    public class AmqpMessageModel<T>
    {
        public T Payload { get; set; }
        public List<string> Receipients { get; set; }
    }
}
