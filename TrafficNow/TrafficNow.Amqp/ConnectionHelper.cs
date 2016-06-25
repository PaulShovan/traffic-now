using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Configuration;
using System.Text;

namespace TrafficNow.Amqp
{
    public class ConnectionHelper
    {
        private static volatile ConnectionHelper _connectionHelper;
        private IConnection _connection;
        private IConnectionFactory _connectionFactory;
        private IModel _model;
        private Subscription _subscription;
        private ConnectionHelper()
        {
            InitConnectionFactory();
        }
        public static ConnectionHelper Current
        {
            get
            {
                if (_connectionHelper == null)
                {
                    lock (typeof(ConnectionHelper))
                    {
                        _connectionHelper = new ConnectionHelper();
                    }
                }
                return _connectionHelper;
            }
        }
        public void Send(string channelName, string content)
        {
            _model.ExchangeDeclare(channelName, ExchangeType.Fanout, true);
            var queueName = _model.QueueDeclare(channelName, false, false, false, null);
            _model.QueueBind(queueName, channelName, "", null);
            _model.BasicPublish(channelName, "", _model.CreateBasicProperties(), Encoding.UTF8.GetBytes(content));
        }

        public string Receive(string channelName)
        {
            CreateSubscription(channelName);
            var basicDeliveryEventArgs = _subscription.Next();
            var messageContent = String.Empty;
            if (basicDeliveryEventArgs.Body != null)
                messageContent = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body);
            _subscription.Ack(basicDeliveryEventArgs);
            return messageContent;
        }
        private void InitConnectionFactory()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "127.0.0.1",//ConfigurationManager.AppSettings["RabbitHostName"],
                UserName = "digbuzzi",//ConfigurationManager.AppSettings["RabbitUserName"],
                Password = "!23456",//ConfigurationManager.AppSettings["RabbitPassword"]
            };
            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
        }
        private void CreateSubscription(string channelName)
        {
            if (_subscription != null)
                return;
            _subscription = new Subscription(_model, channelName, false);
        }
    }
}
