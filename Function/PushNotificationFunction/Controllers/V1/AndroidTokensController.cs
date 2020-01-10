using AzureFunctions.Extensions.Swashbuckle.Attribute;
using PushNotificationFunction.Data;
using PushNotificationFunction.Helpers;
using PushNotificationFunction.Models;
using PushNotificationFunction.Models.Validators;
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
    public class AndroidTokensController : ControllerBase
    {
        /// <summary>
        /// Register android Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [FunctionName("AndroidTokens_Create_V1")]
        public async Task<IActionResult> Create(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/AndroidTokens")]
            [RequestBodyType(typeof(AndroidToken), "Criar um novo token")]HttpRequest request,
            ILogger logger)
        {
            try
            {
                var form = await request.GetJsonBody<AndroidToken, AndroidTokenValidator>();
                if (!form.IsValid)
                {
                    logger.LogInformation($"Invalid form data.");
                    return form.ToBadRequest();
                }

                var input = form.Value;
                AndroidTokenRepository androidTokenRepository = new AndroidTokenRepository();

                var notification = await androidTokenRepository.GetNotificationByKey(input.Id, input.CompanyId, input.UserId, input.ApplicationId);
                if (notification != null)
                {
                    if (notification.DeletedAt != null)
                    {
                        notification.DeletedAt = null;
                        await androidTokenRepository.UpdateNotification(notification);
                    }
                    return new OkResult();
                }
                else
                if (await androidTokenRepository.CreateNotification(input) == 1)
                    return new OkResult();

                return new BadRequestObjectResult("Token não foi gravado, favor enviar os dados corretamente.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
