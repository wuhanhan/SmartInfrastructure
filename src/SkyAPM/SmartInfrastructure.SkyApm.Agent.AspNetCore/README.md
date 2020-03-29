# SmartInfrastructure.SkyApm.Agent.AspNetCore
SkyApm NetCore Agent Hosting扩展，解决官方SkyApm.Agent.AspNetCore全家桶（内置所有模块都进行追踪）追踪模式的弊端，可自定义按需添加追踪模块（如 http, grpc, cap, mysql等等），同时兼容官方配置方式

# Abstract

   ** SkyWalking** is an open source APM system, including monitoring, tracing, diagnosing capabilities for distributed system
in Cloud Native architecture.
The core features are following.

- Service, service instance, endpoint metrics analysis
- Root cause analysis
- Service topology map analysis
- Service, service instance and endpoint dependency analysis
- Slow services and endpoints detected
- Performance optimization
- Distributed tracing and context propagation
- Database access metrics.Detect slow database access statements(including SQL statements).
- Alarm


<img src="http://skywalking.apache.org/assets/frame.jpeg?u=20190518"/>
	 
## Nuget Packages		
					
| Package Name |  NuGet | MyGet | Downloads 
|--------------|  ------- |  ------- |  ---- 
| SmartInfrastructure.SkyApm.Agent.AspNetCore | [![nuget](https://img.shields.io/nuget/v/SmartInfrastructure.SkyApm.Agent.AspNetCore.svg?style=flat-square)](https://www.nuget.org/packages/SmartInfrastructure.SkyApm.Agent.AspNetCore) |  | [![stats](https://img.shields.io/nuget/dt/SmartInfrastructure.SkyApm.Agent.AspNetCore.svg?style=flat-square)](https://www.nuget.org/stats/packages/SmartInfrastructure.SkyApm.Agent.AspNetCore?groupby=Version)

# Getting Started

## Deploy SkyWalking Collector

#### Requirements
- SkyWalking Collector 5.0.0-beta or higher.See SkyWalking backend deploy[docs](https://github.com/apache/incubator-skywalking/blob/5.x/docs/en/Deploy-backend-in-standalone-mode.md).
- SkyWalking 6 backend is compatible too.The deployment doc is [here](https://github.com/apache/incubator-skywalking/blob/master/docs/en/setup/backend/backend-ui-setup.md). If you are new user, recommand you to read the 
[whole official documents](https://github.com/apache/incubator-skywalking/blob/master/docs/README.md)

## Install SkyWalking .NET Core Agent

You can run the following command to install the SkyWalking .NET Core Agent in your project.

```
dotnet add package SmartInfrastructure.SkyApm.Agent.AspNetCore
```

## How to use

在StartUp文件ConfigureServices方法中添加Skywalking服务

```

 services.AddSkyApmCore((option) =>
            {
    option.ServiceName = "CodingTools";
    option.Transport = new SkyApmTransportOption()
    {
        gRPC = new SkyApmGrpcOption()
        {
            Servers = "192.168.1.240:11800"
        }
    };
});

```

如果需要追踪Cap等其他模块，需进行如下操作

- 添加Cap追踪模块的Nuget包

```
dotnet add package SkyAPM.Diagnostics.CAP
```

- 在项目中添加对Cap的追踪

```

 services.AddSkyApmCore((option) =>
            {
    option.ServiceName = "CodingTools";
    option.Transport = new SkyApmTransportOption()
    {
        gRPC = new SkyApmGrpcOption()
        {
            Servers = "192.168.1.240:11800"
        }
    };
}).AddCap();


```
对Http请求追踪

```
dotnet add package SkyAPM.Diagnostics.HttpClient
```

```
 services.AddSkyApmCore((option) =>
            {
    option.ServiceName = "CodingTools";
    option.Transport = new SkyApmTransportOption()
    {
        gRPC = new SkyApmGrpcOption()
        {
            Servers = "192.168.1.240:11800"
        }
    };
}).AddCap().AddHttpClient();

```

对EF方式下mysql追踪

```
dotnet add package SkyAPM.Diagnostics.EntityFrameworkCore.Pomelo.MySql
```

```
 services.AddSkyApmCore((option) =>
            {
    option.ServiceName = "CodingTools";
    option.Transport = new SkyApmTransportOption()
    {
        gRPC = new SkyApmGrpcOption()
        {
            Servers = "192.168.1.240:11800"
        }
    };
}).AddCap().AddHttpClient().AddEntityFrameworkCore(c => c.AddPomeloMysql());

```

## 说明

- 为什么要有这个包？

官方提供的追踪是代码无侵入型的，即按照官方的方式配置，在项目启动时自动注入服务，很便利。但便利的同时带来一些问题，比如 系统提供的全家桶模式中没有添加对Cap的追踪，无法按需加载需要追踪的模块。这个包是在官方提供的sdk的基础上进行扩展，拓展了官方sdk无法按需加载的问题。

AddSkyApmCore（） 方法返回 SkyApmExtensions ，通过链式.AddCap().AddHttpClient().AddEntityFrameworkCore(c => c.AddPomeloMysql())  方式按需加载需要追踪模块

- 什么情况下使用这个包？

如果您需要自定义追踪模块或者需要对Cap的追踪，可以使用。如果想使用全家桶无侵入模式，可直接添加 SkyApm.Agent.AspNetCore 官方包即可