using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class SendNotification
    {
        /// <summary>
        /// Usernames dos usuários ou códigos identificadores.
        /// </summary>
        public string[] UserIds;
        /// <summary>
        /// Código da empresa.
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// Código da aplicação.
        /// </summary>
        public byte ApplicationId { get; set; }
        /// <summary>
        /// Título da notificação.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Mensagem da notificação.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Payload de dados
        /// </summary>
        public object Data { get; set; }
    }
}
