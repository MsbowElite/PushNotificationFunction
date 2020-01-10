using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public byte ApplicationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
