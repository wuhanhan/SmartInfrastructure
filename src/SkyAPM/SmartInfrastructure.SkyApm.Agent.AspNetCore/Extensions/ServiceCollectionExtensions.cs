using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm;
using SkyApm.Config;
using SkyApm.Diagnostics;
using SkyApm.Logging;
using SkyApm.Sampling;
using SkyApm.Service;
using SkyApm.Tracing;
using SkyApm.Transport;
using SkyApm.Transport.Grpc;
using SkyApm.Transport.Grpc.V5;
using SkyApm.Transport.Grpc.V6;
using SkyApm.Utilities.DependencyInjection;
using SkyApm.Utilities.Logging;
using SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration;
using System;
using System.Linq;
using SmartInfrastructure.SkyApm.Diagnostics.AspNetCore;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加链路追踪,来自skyapm,内置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static SkyApmExtensions AddSkyApmCore(this IServiceCollection services, Action<SkyApmOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //加载用户自定义配置项
            var skyApmOption = GetSkyApmOptionsFromConfigration(services);
            options.Invoke(skyApmOption);

            ///将配置信息写入全局配置中
            services.AddOptions().Configure<SkyApmOptions>((option) =>
            {
                option.HeaderVersions = skyApmOption.HeaderVersions;
                option.Logging = skyApmOption.Logging;
                option.Namespace = skyApmOption.Namespace;
                option.Sampling = skyApmOption.Sampling;
                option.ServiceName = skyApmOption.ServiceName;
                option.Transport = skyApmOption.Transport;
            });

            services.AddSingleton<ISegmentDispatcher, AsyncQueueSegmentDispatcher>();
            services.AddSingleton<IExecutionService, RegisterService>();
            services.AddSingleton<IExecutionService, PingService>();
            services.AddSingleton<IExecutionService, ServiceDiscoveryV5Service>();
            services.AddSingleton<IExecutionService, SegmentReportService>();
            services.AddSingleton<IInstrumentStartup, InstrumentStartup>();
            services.AddSingleton<IRuntimeEnvironment>(RuntimeEnvironment.Instance);
            services.AddSingleton<TracingDiagnosticProcessorObserver>();
            services.AddSingleton<IConfigAccessor, ConfigAccessor>();
            services.AddSingleton<IHostedService, InstrumentationHostedService>();
            services.AddTracing().AddSampling().AddGrpcTransport().AddSkyApmLogging();
            var extensions = services.AddSkyApmExtensions().AddAspNetCoreHosting();
            //.AddHttpClient()
            //.AddGrpcClient()
            //.AddSqlClient()
            //.AddCap()
            //.AddGrpc()
            //.AddEntityFrameworkCore(c => c.AddPomeloMysql().AddNpgsql().AddSqlite());

            return extensions;
        }

        /// <summary>
        /// 添加追踪配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        private static IServiceCollection AddTracing(this IServiceCollection services)
        {
            services.AddSingleton<ITracingContext, TracingContext>();
            services.AddSingleton<ICarrierPropagator, CarrierPropagator>();
            services.AddSingleton<ICarrierFormatter, Sw3CarrierFormatter>();
            services.AddSingleton<ICarrierFormatter, Sw6CarrierFormatter>();
            services.AddSingleton<ISegmentContextFactory, SegmentContextFactory>();
            services.AddSingleton<IEntrySegmentContextAccessor, EntrySegmentContextAccessor>();
            services.AddSingleton<ILocalSegmentContextAccessor, LocalSegmentContextAccessor>();
            services.AddSingleton<IExitSegmentContextAccessor, ExitSegmentContextAccessor>();
            services.AddSingleton<ISamplerChainBuilder, SamplerChainBuilder>();
            services.AddSingleton<IUniqueIdGenerator, UniqueIdGenerator>();
            services.AddSingleton<IUniqueIdParser, UniqueIdParser>();
            services.AddSingleton<ISegmentContextMapper, SegmentContextMapper>();
            services.AddSingleton<IBase64Formatter, Base64Formatter>();
            return services;
        }

        /// <summary>
        /// 添加采样配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        private static IServiceCollection AddSampling(this IServiceCollection services)
        {
            services.AddSingleton<SimpleCountSamplingInterceptor>();
            services.AddSingleton<ISamplingInterceptor>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.AddSingleton<IExecutionService>(p => p.GetService<SimpleCountSamplingInterceptor>());
            services.AddSingleton<ISamplingInterceptor, RandomSamplingInterceptor>();
            return services;
        }

        /// <summary>
        /// 添加Grpc传输配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        private static IServiceCollection AddGrpcTransport(this IServiceCollection services)
        {
            services.AddSingleton<ISkyApmClientV5, SkyApmClientV5>();
            services.AddSingleton<ISegmentReporter, SegmentReporter>();
            services.AddSingleton<ConnectionManager>();
            services.AddSingleton<IPingCaller, PingCaller>();
            services.AddSingleton<IServiceRegister, ServiceRegister>();
            services.AddSingleton<IExecutionService, ConnectService>();
            return services;
        }

        /// <summary>
        /// 添加日志配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        private static IServiceCollection AddSkyApmLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerFactory, DefaultLoggerFactory>();
            return services;
        }

        /// <summary>
        /// 从配置文件获得配置项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns></returns>
        private static SkyApmOptions GetSkyApmOptionsFromConfigration(IServiceCollection services)
        {
            IEnvironmentProvider environmentProvider = new
               HostingEnvironmentProvider();
            services.AddSingleton<IEnvironmentProvider>(environmentProvider); //环境配置
            services.AddSingleton<IConfigurationFactory>(new ConfigurationFactory(environmentProvider));//自定义配置工厂
            
            var configService = services.First(x => x.ServiceType == typeof(IConfigurationFactory));
            var configuration = ((IConfigurationFactory)configService.ImplementationInstance).Create();

            var skyApmOption = new SkyApmOptions();
            configuration.GetSection("SkyWalking").Bind(skyApmOption);

            return skyApmOption;
        }
    }
}
