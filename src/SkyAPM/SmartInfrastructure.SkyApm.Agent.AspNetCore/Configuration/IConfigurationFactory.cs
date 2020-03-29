using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration
{
    public interface IConfigurationFactory
    {
        IConfiguration Create();
    }
}
