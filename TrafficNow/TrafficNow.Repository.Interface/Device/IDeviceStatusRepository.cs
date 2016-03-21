using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Device;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.Device
{
    public interface IDeviceStatusRepository : IRepository<DeviceStatus>
    {
        Task<bool> AddDeviceStatus(DeviceStatus status);
        Task<bool> RemoveDeviceStatus(DeviceStatus status);
        Task<List<DeviceStatus>> GetDeviceStatus(string userId);
    }
}
