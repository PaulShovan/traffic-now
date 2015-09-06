using MongoDB.Driver;
using System;

namespace TrafficAppAPI.DAL
{
    public class TrafficNowContext
    {
        public IMongoDatabase Database;

        public TrafficNowContext()
        {
            try
            {
                var client = new MongoClient(Properties.SettingsData.Default.ConnectionString);
                Database = client.GetDatabase(Properties.SettingsData.Default.Database);
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to db" + ex);
            }
        }
    }
}
