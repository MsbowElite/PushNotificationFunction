using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction.Models
{
    public class FcmResponse
    {
        public long Multicast_id { get; set; }
        public byte Success { get; set; }
        public int Failure { get; set; }
        public int Canonical_ids { get; set; }
        public List<FcmResult> Results { get; set; }
    }
}
