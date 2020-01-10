using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using PushNotificationFunction.Data;
using PushNotificationFunction.Models;
using PushNotificationFunction.Models.Validators;
using PushNotificationFunction.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace PushNotificationFunction.Controllers
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
        [FunctionName("Notifications_Send")]
        public async Task<IActionResult> Send(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Notifications")]
            [RequestBodyType(typeof(SendNotification), "Enviar push notification")]HttpRequest request,
            ILogger logger, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<SendNotification>(requestBody);
                input.ApplicationId = 2;

                var validator = new SendNotificationValidator();
                var validationResult = validator.Validate(input);

                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(validationResult.Errors.Select(e => new {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }

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

        private async Task SendPushIOSAsync(IOSNotification iOSNotification, Notification notification, IOSToken iOSToken, object data, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            int port = 2195;
            string hostname = "gateway.sandbox.push.apple.com";

            var certificatePath = Path.GetFullPath(Path.Combine(context.FunctionDirectory, "..\\Files\\test.txt"));

            var lol = System.IO.File.ReadAllBytes(certificatePath);
            X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), "YOUR_PASSWORD");
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(AddressFamily.InterNetwork);
            await client.ConnectAsync(hostname, port);

            SslStream sslStream = new SslStream(
                client.GetStream(), false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null);

            try
            {
                await sslStream.AuthenticateAsClientAsync(hostname, certificatesCollection, SslProtocols.Tls, false);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                writer.Write(HexStringToByteArray(iOSToken.Id.ToUpper()));
                string payload = "{\"aps\":{\"alert\":\"" + notification.Message + "\",\"badge\":0,\"sound\":\"default\"}}";
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Dispose();
            }
            catch (AuthenticationException ex)
            {
                client.Dispose();
            }
            catch (Exception e)
            {
                client.Dispose();
            }
        }

        #region Helper methods
        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        #endregion

        /// <summary>
        /// Create Products
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AndroidToken))]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(Error))]
        //[ProducesResponseType((int)HttpStatusCode.PreconditionFailed, Type = typeof(Error))]
        //[ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(Error))]
        //[RequestHttpHeader("Idempotency-Key", isRequired: false)]
        //[RequestHttpHeader("Authorization", isRequired: true)]

        //[FunctionName("GetEnvironmentVariables")]
        //public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        //{
        //    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        //    log.LogInformation(GetEnvironmentVariable("AzureWebJobsStorage"));
        //    log.LogInformation(GetEnvironmentVariable("WEBSITE_SITE_NAME"));
        //    log.LogInformation(GetEnvironmentVariable("DefaultConnectionString"));
        //}
    }
}