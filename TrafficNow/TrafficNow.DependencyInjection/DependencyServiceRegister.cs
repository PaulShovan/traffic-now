using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Repository.Implementation.Map;
using TrafficNow.Repository.Implementation.Shout;
using TrafficNow.Repository.Implementation.User;
using TrafficNow.Repository.Interface.Map;
using TrafficNow.Repository.Interface.Shout;
using TrafficNow.Repository.Interface.User;
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
        }
    }
}
