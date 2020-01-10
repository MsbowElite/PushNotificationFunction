using PushNotificationFunction.Data;
using PushNotificationFunction.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationFunction.Services
{
    public class AndroidNotificationService
    {
        public async Task<List<AndroidNotification>> Send(IEnumerable<AndroidToken> androidTokens, Notification notification, object data, ILogger logger)
        {
            List<AndroidNotification> responseList = new List<AndroidNotification>();
            AndroidTokenRepository androidTokenRepository = new AndroidTokenRepository();
            AndroidNotificationRepository androidNotificationRepository = new AndroidNotificationRepository();

            foreach (var androidToken in androidTokens)
            {
                AndroidNotification androidNotification = new AndroidNotification
                {
                    TokenId = androidToken.Id,
                    NotificationId = notification.Id
                };
                try
                {
                    await SendPushAndroidAsync(androidNotification, notification, androidToken, data, androidTokenRepository);
                }
                finally
                {
                    responseList.Add(androidNotification);
                    try
                    {
                        await androidNotificationRepository.CreateNotification(androidNotification);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.Message);
                    }
                }
            }
            return responseList;
        }

        private async Task SendPushAndroidAsync(AndroidNotification androidNotification, Notification notification, AndroidToken androidToken, object data, AndroidTokenRepository androidTokenRepository)
        {
            dynamic model = new ExpandoObject();
            if (data != null)
            {
                model = data;
                model.UserId = androidToken.UserId;
                model.CompanyId = androidToken.CompanyId;
            }
            //Analisar necessidade de setar a messagem em mais atributos
            //string alert;
            string body = notification.Message;
            string title = notification.Title;
            string to = androidNotification.TokenId;
            //int badge = 1;
            //string sound = "default";
            //string vibrate = "true";

            // Get the server key from FCM console
            var serverKey = string.Format("key={0}", Config.GetEnvironmentVariable("Firebase_ServerKey"));

            // Get the sender id from FCM console
            var senderId = string.Format("id={0}", Config.GetEnvironmentVariable("Firebase_SenderId"));

            object payLoad;

            if (data is null)
            {
                payLoad = new
                {
                    to,
                    notification = new
                    {
                        title,
                        body
                    }
                };
            }
            else
            {
                payLoad = new
                {
                    to,
                    notification = new
                    {
                        title,
                        body
                    },
                    data = new
                    {
                        model
                    }
                };
            }

            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
            var request = new RestSharp.RestRequest(Method.POST);
            IRestResponse response;
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", serverKey);
            request.AddHeader("Sender", senderId);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(payLoad);
            response = client.Execute(request);

            try
            {
                FcmResponse resposta = new FcmResponse();

                resposta = JsonConvert.DeserializeObject<FcmResponse>(response.Content);

                if (resposta.Results != null && resposta.Results.Count() > 0)
                {
                    androidNotification.Message_Id = resposta.Results.FirstOrDefault().Message_id;
                }
                androidNotification.Multicast_Id = resposta.Multicast_id;
                androidNotification.Success = resposta.Success;

                if (androidNotification.Success == 0)
                {
                    try
                    {
                        androidToken.DeletedAt = DateTime.UtcNow;
                        await androidTokenRepository.UpdateNotification(androidToken);
                    }
                    catch { }
                }
            }
            catch (Exception)
            {

                androidToken.DeletedAt = DateTime.UtcNow;
                await androidTokenRepository.UpdateNotification(androidToken);
            }

        }
    }
}
