using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration
{
    public interface IAdditionalConfigurationSource
    {
        void Load(ConfigurationBuilder builder);
    }
}
