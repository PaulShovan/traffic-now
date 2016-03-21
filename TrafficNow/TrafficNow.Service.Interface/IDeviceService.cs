using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Device;

namespace TrafficNow.Service.Interface
{
    public interface IDeviceService
    {
        Task<bool> RegisterDevice(DeviceStatus status);
        Task<List<string>> GetDiviceIds(string userId);
    }
}
