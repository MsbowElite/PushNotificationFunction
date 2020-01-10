using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PushNotificationFunction.Models;

namespace PushNotificationFunction.Data
{
    public class AndroidTokenRepository
    {

        public async Task<IEnumerable<AndroidToken>> GetNotifications()
        {

            var query = "SELECT * FROM [dbo].[AndroidToken]";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<AndroidToken>(query.ToString());
                return result;
            }
        }

        public async Task<AndroidToken> GetNotificationByKey(string id, string companyId, string userId, byte applicationId)
        {

            var query = @"SELECT TOP 1 * FROM [dbo].[AndroidToken]
                WHERE Id = @Id  AND CompanyId = @CompanyId AND UserId = @UserId AND ApplicationId = @ApplicationId";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                var result = await conn.QueryAsync<AndroidToken>(query.ToString(),
                    new {id, companyId, userId, applicationId});
                return result.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<AndroidToken>> GetNotificationByUsers(string[] userIds, string companyId, byte applicationId)
        {

            var query = @"SELECT * FROM [dbo].[AndroidToken]
                        WHERE [CompanyId] = @CompanyId AND 
                        [DeletedAt] IS NULL AND
                        [ApplicationId] = @ApplicationId AND
                        [UserId] IN @UserIds";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.QueryAsync<AndroidToken>(query.ToString(), 
                    new { CompanyId = companyId,
                          ApplicationId = applicationId,
                          UserIds = userIds.Select(s => s) });
            }
        }

        public async Task<int> CreateNotification(AndroidToken notificationToken)
        {
            var query = @"INSERT Into [dbo].[AndroidToken] ([Id], [CreatedAt], [CompanyId], [UserId], [ApplicationId])
                Values(@Id, @CreatedAt, @CompanyId, @UserId, @ApplicationId)";
            notificationToken.CreatedAt = DateTime.UtcNow;
            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(),
                    new { notificationToken.Id, notificationToken.CreatedAt,
                        notificationToken.CompanyId, notificationToken.UserId,
                        notificationToken.ApplicationId
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

        public async Task<int> UpdateNotification(AndroidToken notificationToken)
        {
            notificationToken.UpdatedAt = DateTime.UtcNow;

            var query = @"UPDATE [dbo].[AndroidToken]
                        SET [UpdatedAt] = @UpdatedAt
                        ,[DeletedAt] = @DeletedAt
                        WHERE [Id] = @Id AND
                        [CompanyId] = @CompanyId AND
                        [UserId] = @UserId AND
                        [ApplicationId] = @ApplicationId";

            using (IDbConnection conn = Connection)
            {
                conn.Open();
                return await conn.ExecuteAsync(query.ToString(),
                    new
                    {
                        notificationToken.UpdatedAt,
                        notificationToken.DeletedAt,
                        notificationToken.Id,
                        notificationToken.CompanyId,
                        notificationToken.UserId,
                        notificationToken.ApplicationId
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
