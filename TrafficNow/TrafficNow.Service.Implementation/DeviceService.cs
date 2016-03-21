using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Device;
using TrafficNow.Repository.Interface.Device;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class DeviceService : IDeviceService
    {
        private IDeviceStatusRepository _deviceStatusRepository;
        public DeviceService(IDeviceStatusRepository deviceStatusRepository)
        {
            _deviceStatusRepository = deviceStatusRepository;
        }

        public async Task<List<string>> GetDiviceIds(string userId)
        {
            try
            {
                List<string> deviceIds = new List<string>();
                var devices = await _deviceStatusRepository.GetDeviceStatus(userId);
                foreach (var device in devices)
                {
                    deviceIds.Add(device.deviceId);
                }
                return deviceIds;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> RegisterDevice(DeviceStatus status)
        {
            try
            {
                status.status = "active";
                return await _deviceStatusRepository.AddDeviceStatus(status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
