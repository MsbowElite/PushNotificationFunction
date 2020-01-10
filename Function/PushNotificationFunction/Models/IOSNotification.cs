using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class IOSNotification
    {
        public string TokenId { get; set; }
        public Guid NotificationId { get; set; }
        public bool Success { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
