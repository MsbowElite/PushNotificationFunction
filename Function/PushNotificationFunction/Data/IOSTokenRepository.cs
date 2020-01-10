using Dapper;
using PushNotificationFunction.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PushNotificationFunction.Data
{
    public class IOSTokenRepository
    {
        public async Task<IEnumerable<IOSToken>> GetNotifications()
        {

            var query = "SELECT * FROM [dbo].[IOSToken]";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<IOSToken>(query.ToString());
                return result;
            }
        }

        public async Task<IOSToken> GetNotificationByKey(string id, string companyId, string userId)
        {

            var query = @"SELECT TOP 1 * FROM [dbo].[IOSToken]
                WHERE Id = @Id  AND CompanyId = @CompanyId AND UserId = @UserId";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<IOSToken>(query.ToString(),
                    new { id, companyId, userId });
                return result.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<IOSToken>> GetNotificationByUsers(string[] userIds, string companyId)
        {

            var query = @"SELECT * FROM [dbo].[IOSToken]
                        WHERE [CompanyId] = @CompanyId AND [DeletedAt] IS NULL AND
                        [UserId] IN @UserIds";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.QueryAsync<IOSToken>(query.ToString(),
                    new
                    {
                        CompanyId = companyId,
                        UserIds = userIds.Select(s => s)
                    });
            }
        }

        public async Task<int> CreateNotification(IOSToken iOSToken)
        {
            var query = @"INSERT Into [dbo].[IOSToken] ([Id], [CreatedAt], [CompanyId], [UserId])
                Values(@Id, @CreatedAt, @CompanyId, @UserId)";
            iOSToken.CreatedAt = DateTime.UtcNow;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(),
                    new
                    {
                        iOSToken.Id,
                        iOSToken.CreatedAt,
                        iOSToken.CompanyId,
                        iOSToken.UserId
                    });
            }
        }

        //public async Task<int> UpdateNotification(AndroidToken notificationToken)
        //{
        //    var query = @"UPDATE [dbo].[AndroidToken]
        //                SET[UpdatedAt] = @UpdatedAt
        //                ,[CompanyId] = @CompanyId
        //                ,[UserId] = @UserId
        //                WHERE [Id] = @Id";
        //    notificationToken.UpdatedAt = DateTime.UtcNow;
        //    using (IDbConnection conn = Connection)
        //    {
        //        conn.Open();
        //        return await conn.ExecuteAsync(query.ToString(),
        //            new
        //            {
        //                notificationToken.UpdatedAt,
        //                notificationToken.CompanyId,
        //                notificationToken.UserId,
        //                notificationToken.Id
        //            });
        //    }
        //}

        public async Task<int> UpdateNotification(IOSToken iOSToken)
        {
            iOSToken.UpdatedAt = DateTime.UtcNow;

            var query = @"UPDATE [dbo].[IOSToken]
                        SET [UpdatedAt] = @UpdatedAt
                        ,[DeletedAt] = @DeletedAt
                        WHERE [Id] = @Id AND
                        [CompanyId] = @CompanyId AND
                        [UserId] = @UserId";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(),
                    new
                    {
                        iOSToken.UpdatedAt,
                        iOSToken.DeletedAt,
                        iOSToken.Id,
                        iOSToken.CompanyId,
                        iOSToken.UserId,
                    });
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
