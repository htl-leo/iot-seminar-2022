using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Helper
{
    public class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            return configuration;

        }


        public static string GetConfiguration(string key, string section = null)
        {
            var configuration = GetConfiguration();
            if (section != null)
            {
                var appSettingsSection = configuration.GetSection(section);
                if (!appSettingsSection.Exists())
                {
                    throw new ApplicationException($"GetConfiguration; Section: {section} doesn't exist");
                }
                var sectionValue = appSettingsSection[key];
                if (string.IsNullOrEmpty(sectionValue))
                {
                    throw new ApplicationException($"GetConfiguration; Section: {section}, Key: {key} doesn't exist");
                }
                return sectionValue;
                //var key = Encoding.ASCII.GetBytes();
            }
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new ApplicationException($"GetConfiguration; Section: {section}, Key: {key} doesn't exist");
            }
            return value;
        }
    }
}
