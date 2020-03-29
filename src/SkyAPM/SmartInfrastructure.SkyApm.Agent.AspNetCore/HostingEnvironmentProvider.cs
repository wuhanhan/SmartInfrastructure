using SkyApm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore
{
    internal class HostingEnvironmentProvider : IEnvironmentProvider
    {
        private const string ENVIRONMENT_KEY = "ASPNETCORE_ENVIRONMENT";

        public string EnvironmentName { get; }

        public HostingEnvironmentProvider()
        {
            EnvironmentName = Environment.GetEnvironmentVariable(ENVIRONMENT_KEY) ?? "Production";
        }
    }
}
