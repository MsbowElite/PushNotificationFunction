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
    public class NotificationRepository
    {
        public async Task<Guid> CreateNotification(Notification notification)
        {
            var query = @"INSERT Into [dbo].[Notification] ([ApplicationId], [Title], [Message], [CreatedAt])
                OUTPUT INSERTED.[Id]
                Values(@ApplicationId, @Title, @Message, @CreatedAt)";
            notification.CreatedAt = DateTime.UtcNow;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.QuerySingleAsync<Guid>(query.ToString(), notification);
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
