using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class IOSToken
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
