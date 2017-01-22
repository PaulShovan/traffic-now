using PushSharp;
using PushSharp.Android;
using PushSharp.Core;
using System;
using TrafficNow.Service.Interface;
using System.Threading.Tasks;
using TrafficNow.Model.NotificationModel;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Core.Helpers;
using TrafficNow.Repository.Interface.Notification;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using TrafficNow.Model.Enums;

namespace TrafficNow.Service.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly PushBroker push;
        private Utility _utility;
        private INotificationRepository _notificationRepository;
        private IDeviceService _deviceService;
        private IMessageSendService _messageSendService;
        public NotificationService(INotificationRepository notificationRepository, IDeviceService deviceService, IMessageSendService messageSendService)
        {
            push = new PushBroker();
            _utility = new Utility();
            _notificationRepository = notificationRepository;
            _deviceService = deviceService;
            _messageSendService = messageSendService;
            Initialize();
        }
        private void Initialize()
        {
            push.OnNotificationSent += NotificationSent;
            push.OnChannelException += ChannelException;
            push.OnServiceException += ServiceException;
            push.OnNotificationFailed += NotificationFailed;
            push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
            push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
            push.OnChannelCreated += ChannelCreated;
            push.OnChannelDestroyed += ChannelDestroyed;
        }
        public void SendNotification(List<string> devices, string payload)
        {
            //devices.Add("eBmZB2WHebM:APA91bGVhFKs50c0cOQ-lmCH7tjwfrF_f8WkzOnYASne9SOn6P--jLTmwasUX2Y9LVdsGy9icDhVjrjCT1UuRg9E0po1LeBHhq1xvqCgIOJCXtsMgcegTz8xaK9jjbVYe4ICGLJhgJq8");
            push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyB9WCL0T_yJKerl-SDkzu8C2HlG4K_gY98"));
            //push.QueueNotification(new GcmNotification().ForDeviceRegistrationId("eBmZB2WHebM:APA91bGVhFKs50c0cOQ-lmCH7tjwfrF_f8WkzOnYASne9SOn6P--jLTmwasUX2Y9LVdsGy9icDhVjrjCT1UuRg9E0po1LeBHhq1xvqCgIOJCXtsMgcegTz8xaK9jjbVYe4ICGLJhgJq8")
            //                     .WithJson(payload));
            //test: "fM-5H8nQ__M:APA91bFBhkkgeoKrsj6BhOVEzFIss-V_VMriGOZnU4yFyhtCW86kjJVMiVsuofCWuYVQ6rIfhoOit08yunyBO6_o_6x0A-2RydwEnljSWo64KYBClffsL-Cz52weAgP9MWO3-oGHzU45"
            //push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyB9WCL0T_yJKerl-SDkzu8C2HlG4K_gY98"));
            //foreach (var device in devices)
            //{
            //    push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(device).WithJson(payload));
            //    Thread.Sleep(1000);
            //}
            push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(devices).WithJson(payload));
            push.StopAllServices();
        }
        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine("Sent: " + sender + " -> " + notification);
        }

        static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
        }

        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }
        private Model.NotificationModel.Notification PrepareNotification(UserBasicInformation from, UserBasicInformation to, string payload, string type, string shoutId = "")
        {
            try
            {
                var notification = new Model.NotificationModel.Notification();
                notification.type = type;
                notification.text = payload;
                notification.shoutId = shoutId;
                notification.time = _utility.GetTimeInMilliseconds();
                notification.participantUserId = from.userId;
                notification.participantUserName = from.userName;
                notification.participentAvatar = from.photo;
                notification.receipentUserId = to.userId;
                notification.receipentUserName = to.userName;
                notification.receipentAvatar = to.photo;
                return notification;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AddNotification(UserBasicInformation from, UserBasicInformation to, string payload, string type, string shoutId = "")
        {
            try
            {
                var notification = PrepareNotification(from, to, payload, type, shoutId);
                await _notificationRepository.AddNotification(notification);
                var devices = await _deviceService.GetDiviceIds(to.userId);
                if (devices.Count < 1)
                {
                    return true;
                }
                //var payloadToSend = JsonConvert.SerializeObject(notification);
                //SendNotification(devices, payloadToSend);
                await _messageSendService.SendMessage(AmqpTypes.Notification, notification, devices);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
