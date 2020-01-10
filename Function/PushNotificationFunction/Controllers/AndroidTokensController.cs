using AzureFunctions.Extensions.Swashbuckle.Attribute;
using PushNotificationFunction.Data;
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

namespace PushNotificationFunction.Controllers
{
    public class AndroidTokensController : ControllerBase
    {
        /// <summary>
        /// Get android Tokens
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AndroidToken[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [FunctionName("AndroidTokens_GetAll")]
        public async Task<IActionResult> GetTokens(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "AndroidTokens")]HttpRequest request,
            ILogger logger)
        {
            try
            {
                AndroidTokenRepository androidTokenRepository = new AndroidTokenRepository();
                return new OkObjectResult(await androidTokenRepository.GetNotifications());
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        /// <summary>
        /// Register android Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [FunctionName("AndroidTokens_Create")]
        public async Task<IActionResult> Create(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "AndroidTokens")]
            [RequestBodyType(typeof(AndroidToken), "Criar um novo token")]HttpRequest request,
            ILogger logger)
        {
            try
            {
                AndroidTokenRepository androidTokenRepository = new AndroidTokenRepository();
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<AndroidToken>(requestBody);
                input.ApplicationId = 2;

                var validator = new AndroidTokenValidator();
                var validationResult = validator.Validate(input);

                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

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
