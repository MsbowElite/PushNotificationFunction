using AzureFunctions.Extensions.Swashbuckle.Attribute;
using PushNotificationFunction.Data;
using PushNotificationFunction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationFunction.Controllers
{
    public class IOSTokensController : ControllerBase
    {
        /// <summary>
        /// Get iOS Tokens
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IOSToken[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [FunctionName("IOSTokens_GetAll")]
        public async Task<IActionResult> GetTokens(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "IOSTokens")]HttpRequest request,
            ILogger logger)
        {
            try
            {
                IOSTokenRepository iOSTokenRepository = new IOSTokenRepository();
                return new OkObjectResult(await iOSTokenRepository.GetNotifications());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// Register iOS Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [FunctionName("IOSTokens_Create")]
        public async Task<IActionResult> Create(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "IOSTokens")]
            [RequestBodyType(typeof(IOSToken), "Criar um novo token")]HttpRequest request,
            ILogger logger)
        {
            try
            {
                IOSTokenRepository iOSTokenRepository = new IOSTokenRepository();
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<IOSToken>(requestBody);

                if (!string.IsNullOrEmpty(input.CompanyId) && !string.IsNullOrEmpty(input.UserId))
                {
                    var notification = await iOSTokenRepository.GetNotificationByKey(input.Id, input.CompanyId, input.UserId);
                    if (notification != null)
                    {
                        if (notification.DeletedAt != null)
                        {
                            notification.DeletedAt = null;
                            await iOSTokenRepository.UpdateNotification(notification);
                        }
                        return new OkResult();
                    }
                    else
                    if (await iOSTokenRepository.CreateNotification(input) == 1)
                        return new OkResult();
                }

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
