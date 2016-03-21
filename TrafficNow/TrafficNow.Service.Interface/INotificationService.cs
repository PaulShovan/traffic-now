using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Service.Interface
{
    public interface INotificationService
    {
        void SendNotification(List<string> devices, string payload);
        Task<bool> AddNotification(UserBasicInformation from, UserBasicInformation to, string payload, string type);
    }
}
