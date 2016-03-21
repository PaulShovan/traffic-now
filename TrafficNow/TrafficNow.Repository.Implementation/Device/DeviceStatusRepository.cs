using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Device;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Device;

namespace TrafficNow.Repository.Implementation.Device
{
    public class DeviceStatusRepository : Repository<DeviceStatus>, IDeviceStatusRepository
    {
        public async Task<bool> AddDeviceStatus(DeviceStatus status)
        {
            try
            {
                var filter1 = Builders<DeviceStatus>.Filter.Eq(d => d.userId, status.userId);
                var filter2 = Builders<DeviceStatus>.Filter.Eq(d => d.appId, status.appId);
                var filter = Builders<DeviceStatus>.Filter.And(filter1, filter2);
                var update = Builders<DeviceStatus>.Update.Set(u => u.status, status.status)
                    .Set(u => u.deviceId, status.deviceId)
                    .SetOnInsert(u=>u.appId, status.appId)
                    .SetOnInsert(u => u.platform, status.platform);
                var options = new FindOneAndUpdateOptions<DeviceStatus, DeviceStatus>();
                options.IsUpsert = true;
                options.ReturnDocument = ReturnDocument.After;
                var result = await Collection.FindOneAndUpdateAsync(filter, update, options);
                return !string.IsNullOrWhiteSpace(result.appId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<DeviceStatus>> GetDeviceStatus(string userId)
        {
            try
            {
                var statuses = await Collection.Find(status => status.userId == userId).ToListAsync();
                return statuses;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> RemoveDeviceStatus(DeviceStatus status)
        {
            try
            {
                var filter1 = Builders<DeviceStatus>.Filter.Eq(d => d.userId, status.userId);
                var filter2 = Builders<DeviceStatus>.Filter.Eq(d => d.appId, status.appId);
                var filter3 = Builders<DeviceStatus>.Filter.Eq(d => d.platform, status.platform);
                var filter = Builders<DeviceStatus>.Filter.And(filter1, filter2, filter3);
                var result = await Collection.DeleteOneAsync(filter);
                return result.IsAcknowledged;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
