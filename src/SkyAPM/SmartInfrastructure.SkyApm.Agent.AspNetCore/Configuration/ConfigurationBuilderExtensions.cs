using Microsoft.Extensions.Configuration;
using SkyApm.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddSkyWalkingDefaultConfig(this IConfigurationBuilder builder)
        {
            var defaultLogFile = Path.Combine("logs", "skyapm-{Date}.log");
            var defaultConfig = new Dictionary<string, string>
            {
                {"SkyWalking:Namespace", string.Empty},
                {"SkyWalking:ServiceName", "My_Service"},
                {"SkyWalking:HeaderVersions:0", HeaderVersions.SW6},
                {"SkyWalking:Sampling:SamplePer3Secs", "-1"},
                {"SkyWalking:Sampling:Percentage", "-1"},
                {"SkyWalking:Logging:Level", "Information"},
                {"SkyWalking:Logging:FilePath", defaultLogFile},
                {"SkyWalking:Transport:Interval", "3000"},
                {"SkyWalking:Transport:ProtocolVersion", ProtocolVersions.V6},
                {"SkyWalking:Transport:QueueSize", "30000"},
                {"SkyWalking:Transport:BatchSize", "3000"},
                {"SkyWalking:Transport:gRPC:Servers", "localhost:11800"},
                {"SkyWalking:Transport:gRPC:Timeout", "10000"},
                {"SkyWalking:Transport:gRPC:ReportTimeout", "600000"},
                {"SkyWalking:Transport:gRPC:ConnectTimeout", "10000"}
            };
            return builder.AddInMemoryCollection(defaultConfig);
        }
    }
}
