using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Amqp;
using TrafficNow.Model.Enums;
using TrafficNow.Service.Interface;

namespace TrafficNow.Amqp
{
    public class MessageReceiveService : IMessageReceiveService
    {
        public Task<IEnumerable<T>> GetMessage<T>(AmqpTypes amqpType) where T : class
        {
            return Task.Run(() =>
            {
                var channelName = ChannelMap.GetUrl(amqpType);
                var content = ConnectionHelper.Current.Receive(channelName);
                return DeSerializeContent<T>(new List<string> { content });
            });
        }
        private IEnumerable<T> DeSerializeContent<T>(IEnumerable<string> contents) where T : class
        {
            var deSerializeContentes = new List<T>();
            foreach (var content in contents)
            {
                if (String.IsNullOrEmpty(content))
                    continue;
                var deSerializedContent = JsonConvert.DeserializeObject<T>(content);
                deSerializeContentes.Add(deSerializedContent);
            }
            return deSerializeContentes;
        }
    }
}
