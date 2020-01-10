using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class AndroidNotification
    {
        public string TokenId { get; set; }
        public Guid NotificationId { get; set; }
        public byte Success { get; set; }
        public long Multicast_Id { get; set; }
        public string Message_Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
