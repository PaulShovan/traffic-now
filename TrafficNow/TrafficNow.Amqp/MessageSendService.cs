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
    public class MessageSendService : IMessageSendService
    {
        public Task SendMessage<T>(AmqpTypes amqpType, T model, List<string> receipeients)
        {
            return Task.Run(() =>
            {
                try
                {
                    var channelName = ChannelMap.GetUrl(amqpType);
                    var content = SerializeContent(model, receipeients);
                    ConnectionHelper.Current.Send(channelName, content);
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
        }
        private static string SerializeContent<T>(T model, List<string> receipeients)
        {
            var messageSendModel = new AmqpMessageModel<T>
            {
                Payload = model,
                Receipients = receipeients
            };
            var content = JsonConvert.SerializeObject(messageSendModel);
            return content;
        }
    }
}
