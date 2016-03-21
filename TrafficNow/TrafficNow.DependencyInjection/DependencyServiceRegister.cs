using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Repository.Implementation.Device;
using TrafficNow.Repository.Implementation.Map;
using TrafficNow.Repository.Implementation.Notification;
using TrafficNow.Repository.Implementation.Point;
using TrafficNow.Repository.Implementation.Shout;
using TrafficNow.Repository.Implementation.User;
using TrafficNow.Repository.Implementation.UserConnections;
using TrafficNow.Repository.Interface.Device;
using TrafficNow.Repository.Interface.Map;
using TrafficNow.Repository.Interface.Notification;
using TrafficNow.Repository.Interface.Point;
using TrafficNow.Repository.Interface.Shout;
using TrafficNow.Repository.Interface.User;
using TrafficNow.Repository.Interface.UserConnections;
using TrafficNow.Service.Implementation;
using TrafficNow.Service.Interface;

namespace TrafficNow.DependencyInjection
{
    public class DependencyServiceRegister
    {
        public void Register(KernelBase kernel)
        {
            #region user
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IUserRepository>().To<UserRepository>();
            #endregion
            #region shout
            kernel.Bind<IShoutService>().To<ShoutService>();
            kernel.Bind<IShoutRepository>().To<ShoutRepository>();
            #endregion
            #region map
            kernel.Bind<IMapService>().To<MapService>();
            kernel.Bind<IMapRepository>().To<MapRepository>();
            #endregion
            #region point
            kernel.Bind<IPointService>().To<PointService>();
            kernel.Bind<IPointRepository>().To<PointRepository>();
            #endregion
            #region connection
            kernel.Bind<IFollowingRepository>().To<FollowingRepository>();
            kernel.Bind<IFollowerRepository>().To<FollowerRepository>();
            kernel.Bind<IFollowingService>().To<FollowingService>();
            kernel.Bind<IFollowerService>().To<FollowerService>();
            #endregion
            #region notification
            kernel.Bind<INotificationService>().To<NotificationService>();
            kernel.Bind<INotificationRepository>().To<NotificationRepository>();
            #endregion
            #region device
            kernel.Bind<IDeviceStatusRepository>().To<DeviceStatusRepository>();
            kernel.Bind<IDeviceService>().To<DeviceService>();
            #endregion
        }
    }
}
