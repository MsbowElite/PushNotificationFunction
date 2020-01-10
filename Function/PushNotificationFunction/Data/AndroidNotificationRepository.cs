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
    public class AndroidNotificationRepository
    {
        public async Task<IEnumerable<AndroidNotification>> GetNotifications()
        {

            var query = "SELECT * FROM [dbo].[AndroidNotification]";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<AndroidNotification>(query.ToString());
                return result;
            }
        }

        public async Task<int> CreateNotification(AndroidNotification androidNotification)
        {
            var query = @"INSERT Into [dbo].[AndroidNotification] ([TokenId], [NotificationId], [Success], [Multicast_Id], [Message_Id], [CreatedAt])
                Values(@TokenId, @NotificationId, @Success, @Multicast_Id, @Message_Id, @CreatedAt)";
            androidNotification.CreatedAt = DateTime.UtcNow;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(), androidNotification);
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
