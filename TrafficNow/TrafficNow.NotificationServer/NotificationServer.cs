using Newtonsoft.Json;
using Ninject;
using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using TrafficNow.Amqp;
using TrafficNow.DependencyInjection;
using TrafficNow.Model.Amqp;
using TrafficNow.Model.Enums;
using TrafficNow.Model.NotificationModel;
using TrafficNow.Service.Interface;

namespace TrafficNow.NotificationServer
{
    public partial class NotificationServer : ServiceBase
    {
        private Timer _timer;
        public NotificationServer()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                InitializeDependencyInjection();
                var messageReceiveService = new MessageReceiveService();
                _timer = new Timer(10000);
                _timer.Elapsed += (s, e) => PullNotification(messageReceiveService);
                _timer.Enabled = true;
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            }
            catch (Exception)
            {
                
            }
            
        }
        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
        private static void InitializeDependencyInjection()
        {
            try
            {
                var dependencyResolver = new DependencyResolver();
                dependencyResolver.Resolve();
            }
            catch (Exception)
            {
                
            }
        }
        private static void SendNotification(NotificationBaseModel data)
        {
            Task.Run(() =>
            {
                try
                {
                    var payloadToSend = JsonConvert.SerializeObject(data.Payload);
                    var notificationService = DependencyResolver.Kernel.Get<INotificationService>();
                    notificationService.SendNotification(data.Receipients, payloadToSend);
                }
                catch (Exception exception)
                {
                    //System.Diagnostics.Debugger.Launch(); 
                }
            });
        }
        private static async void PullNotification(IMessageReceiveService messageReceiveService)
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                var notifications = await messageReceiveService.GetMessage<NotificationBaseModel>(AmqpTypes.Notification);
                foreach (var notification in notifications)
                {
                    SendNotification(notification);
                }
            }
            catch (Exception exception)
            {
                //System.Diagnostics.Debugger.Launch();
            }
        }
    }
}
