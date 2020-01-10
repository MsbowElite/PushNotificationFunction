using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotificationFunction
{
    public class Config
    {
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
