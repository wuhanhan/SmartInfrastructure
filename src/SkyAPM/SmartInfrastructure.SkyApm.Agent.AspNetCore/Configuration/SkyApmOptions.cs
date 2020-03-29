using Microsoft.Extensions.Options;
using SkyApm.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class SkyApmOptions : IOptions<SkyApmOptions>
    {
        public string Namespace { get; set; } = "";
        public string ServiceName { set; get; } = "";
        public List<string> HeaderVersions { set; get; } = new List<string>() { "sw6" };
        public SkyApmSamplingOption Sampling { set; get; } = new SkyApmSamplingOption();
        public SkyApmTransportOption Transport { set; get; } = new SkyApmTransportOption();
        public SkyApmLoggingOption Logging { set; get; } = new SkyApmLoggingOption();
        public SkyApmGrpcOption Grpc { set; get; } = new SkyApmGrpcOption();

        public SkyApmOptions Value => this;
    }

    public class SkyApmSamplingOption
    {
        /// <summary>
        /// 3秒内最大采集数,小于0不限制
        /// </summary>
        public int SamplePer3Secs { set; get; } = -1;
        /// <summary>
        /// 随机采集百分比,小于或等于0不限制
        /// </summary>
        public double Percentage { set; get; } = -1.0;
    }

    /// <summary>
    /// 日志配置项
    /// </summary>
    public class SkyApmLoggingOption
    {
        public string Level { get; set; }

        public string FilePath { get; set; }
    }

    public class SkyApmTransportOption
    {
        /// <summary>
        /// 传输轮询时间,单位秒,默认3000
        /// </summary>
        public int Interval { set; get; } = 3000;
        /// <summary>
        /// 本地队列最大暂存值,默认30000
        /// </summary>
        public int QueueSize { set; get; } = 30000;
        /// <summary>
        /// 传输最大数量,默认3000
        /// </summary>
        public int BatchSize { set; get; } = 3000;

        /// <summary>
        /// 协议版本
        /// </summary>
        public string ProtocolVersion { get; set; } = ProtocolVersions.V6;

        /// <summary>
        /// Grpc配置
        /// </summary>
        public SkyApmGrpcOption gRPC { get; set; }
    }

    /// <summary>
    /// 传输Grpc配置
    /// </summary>
    public class SkyApmGrpcOption
    {
        public string Servers { get; set; } = "localhost:11800";
        public int ConnectTimeout { get; set; } = 10000;
        public int Timeout { get; set; } = 10000;
        public int ReportTimeout { get; set; } = 600000;
    }

    public static class HeaderVersions
    {
        public static string SW3 { get; } = "sw3";

        public static string SW6 { get; } = "sw6";
        public static string Bucket { get; } = "skyapm";
    }
}
