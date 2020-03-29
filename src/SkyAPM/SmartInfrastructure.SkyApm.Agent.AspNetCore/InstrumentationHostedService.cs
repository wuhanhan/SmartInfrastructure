using Microsoft.Extensions.Hosting;
using SkyApm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore
{
    internal class InstrumentationHostedService : IHostedService
    {
        private readonly IInstrumentStartup _startup;

        public InstrumentationHostedService(IInstrumentStartup startup)
        {
            _startup = startup;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _startup.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _startup.StopAsync(cancellationToken);
        }
    }
}
