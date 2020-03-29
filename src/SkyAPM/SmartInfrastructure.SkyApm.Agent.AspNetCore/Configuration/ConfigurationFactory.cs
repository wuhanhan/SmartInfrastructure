using Microsoft.Extensions.Configuration;
using SkyApm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        private const string CONFIG_FILE_PATH_COMPATIBLE = "SKYWALKING__CONFIG__PATH";
        private const string CONFIG_FILE_PATH = "SKYAPM__CONFIG__PATH";
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly IEnumerable<IAdditionalConfigurationSource> _additionalConfigurations;

        public ConfigurationFactory(IEnvironmentProvider environmentProvider)
        {
            _environmentProvider = environmentProvider;
        }

        public IConfiguration Create()
        {
            var builder = new ConfigurationBuilder();

            builder.AddSkyWalkingDefaultConfig();

            builder.AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{_environmentProvider.EnvironmentName}.json", true);

            builder.AddJsonFile("skywalking.json", true)
                .AddJsonFile($"skywalking.{_environmentProvider.EnvironmentName}.json", true);

            builder.AddJsonFile("skyapm.json", true)
                .AddJsonFile($"skyapm.{_environmentProvider.EnvironmentName}.json", true);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CONFIG_FILE_PATH_COMPATIBLE)))
            {
                builder.AddJsonFile(Environment.GetEnvironmentVariable(CONFIG_FILE_PATH_COMPATIBLE), false);
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CONFIG_FILE_PATH)))
            {
                builder.AddJsonFile(Environment.GetEnvironmentVariable(CONFIG_FILE_PATH), false);
            }

            builder.AddEnvironmentVariables();

            if (_additionalConfigurations != null)
            {
                foreach (var additionalConfiguration in _additionalConfigurations)
                {
                    additionalConfiguration.Load(builder);
                }
            }

            return builder.Build();
        }
    }
}
