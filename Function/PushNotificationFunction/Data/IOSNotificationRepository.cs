using Dapper;
using PushNotificationFunction.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationFunction.Data
{
    public class IOSNotificationRepository
    {
        public async Task<IEnumerable<IOSNotification>> GetNotifications()
        {

            var query = "SELECT * FROM [dbo].[IOSNotification]";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<IOSNotification>(query.ToString());
                return result;
            }
        }

        public async Task<int> CreateNotification(IOSNotification iOSNotification)
        {
            var query = @"INSERT Into [dbo].[IOSNotification] ([TokenId], [NotificationId], [Success], [CreatedAt])
                Values(@TokenId, @NotificationId, @Success, @CreatedAt)";
            iOSNotification.CreatedAt = DateTime.UtcNow;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(), iOSNotification);
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(Config.GetEnvironmentVariable("DefaultConnectionString"));
            }
        }
    }
}
