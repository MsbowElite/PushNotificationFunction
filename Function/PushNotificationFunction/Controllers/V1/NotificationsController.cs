using AzureFunctions.Extensions.Swashbuckle.Attribute;
using PushNotificationFunction.Data;
using PushNotificationFunction.Helpers;
using PushNotificationFunction.Models;
using PushNotificationFunction.Models.Validators;
using PushNotificationFunction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationFunction.Controllers.V1
{
    public class NotificationsController : ControllerBase
    {
        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AndroidToken[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [FunctionName("Notifications_Send_V1")]
        public async Task<IActionResult> Send(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/Notifications")]
            [RequestBodyType(typeof(SendNotification), "Enviar push notification")]HttpRequest request,
            ILogger logger, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            try
            {
                var form = await request.GetJsonBody<SendNotification, SendNotificationValidator>();
                if (!form.IsValid)
                {
                    logger.LogInformation($"Invalid form data.");
                    return form.ToBadRequest();
                }

                var input = form.Value;

                AndroidTokenRepository androidTokenRepository = new AndroidTokenRepository();
                IOSTokenRepository iOSTokenRepository = new IOSTokenRepository();
                IOSNotificationRepository iOSNotificationRepository = new IOSNotificationRepository();
                NotificationRepository notificationRepository = new NotificationRepository();

                AndroidNotificationService androidNotificationService = new AndroidNotificationService();

                var androidTokens = await androidTokenRepository.GetNotificationByUsers(input.UserIds, input.CompanyId, input.ApplicationId);
                var iOSTokens = await iOSTokenRepository.GetNotificationByUsers(input.UserIds, input.CompanyId);

                if (androidTokens.Count() < 1 && iOSTokens.Count() < 1)
                {
                    return new BadRequestObjectResult("Nenhum usuário cadastrado.");
                }

                Notification notification = new Notification
                {
                    ApplicationId = input.ApplicationId,
                    Title = input.Title,
                    Message = input.Message
                };

                notification.Id = await notificationRepository.CreateNotification(notification);
                var responseList = await androidNotificationService.Send(androidTokens, notification, input.Data, logger);

                //Adicionar código quando tiver algo do IOS
                //List<IOSNotification> responseListIOS = new List<IOSNotification>();

                //foreach (var iOSToken in iOSTokens)
                //{
                //    IOSNotification iOSNotification = new IOSNotification
                //    {
                //        TokenId = iOSToken.Id,
                //        NotificationId = notification.Id
                //    };
                //    try
                //    {
                //        await SendPushIOSAsync(iOSNotification, notification, iOSToken, input.Data, context);

                //    }
                //    finally
                //    {
                //        responseListIOS.Add(iOSNotification);
                //        try
                //        {
                //            await iOSNotificationRepository.CreateNotification(iOSNotification);
                //        }
                //        catch (Exception ex)
                //        {
                //            logger.LogError(ex.Message);
                //        }
                //    }
                //}

                return new OkObjectResult(responseList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
