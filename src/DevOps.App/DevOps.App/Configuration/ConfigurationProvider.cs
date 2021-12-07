using System;
using Microsoft.Extensions.Configuration;

namespace DevOps.App.Configuration
{
    public static class ConfigurationProvider
    {
        public static T Get<T>(string configurationValue)
        {
            if (string.IsNullOrWhiteSpace(configurationValue))
            {
                throw new ArgumentException("No configuration value was provided.");
            }

            var result = Program.Configuration.GetValue<T>(configurationValue);
            if (result == null)
            {
                throw new InvalidOperationException($"Setting '{configurationValue}' was not found in the configuration.");
            }

            return result;
        }
    }
}