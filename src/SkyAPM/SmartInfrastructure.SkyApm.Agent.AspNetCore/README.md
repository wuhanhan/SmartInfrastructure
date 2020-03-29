# SmartInfrastructure.SkyApm.Agent.AspNetCore
SkyApm NetCore Agent Hosting��չ������ٷ�SkyApm.Agent.AspNetCoreȫ��Ͱ����������ģ�鶼����׷�٣�׷��ģʽ�ı׶ˣ����Զ��尴�����׷��ģ�飨�� http, grpc, cap, mysql�ȵȣ���ͬʱ���ݹٷ����÷�ʽ

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

��StartUp�ļ�ConfigureServices���������Skywalking����

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

�����Ҫ׷��Cap������ģ�飬��������²���

- ���Cap׷��ģ���Nuget��

```
dotnet add package SkyAPM.Diagnostics.CAP
```

- ����Ŀ����Ӷ�Cap��׷��

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
��Http����׷��

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

��EF��ʽ��mysql׷��

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

## ˵��

- ΪʲôҪ���������

�ٷ��ṩ��׷���Ǵ����������͵ģ������չٷ��ķ�ʽ���ã�����Ŀ����ʱ�Զ�ע����񣬺ܱ�������������ͬʱ����һЩ���⣬���� ϵͳ�ṩ��ȫ��Ͱģʽ��û����Ӷ�Cap��׷�٣��޷����������Ҫ׷�ٵ�ģ�顣��������ڹٷ��ṩ��sdk�Ļ����Ͻ�����չ����չ�˹ٷ�sdk�޷�������ص����⡣

AddSkyApmCore���� �������� SkyApmExtensions ��ͨ����ʽ.AddCap().AddHttpClient().AddEntityFrameworkCore(c => c.AddPomeloMysql())  ��ʽ���������Ҫ׷��ģ��

- ʲô�����ʹ���������

�������Ҫ�Զ���׷��ģ�������Ҫ��Cap��׷�٣�����ʹ�á������ʹ��ȫ��Ͱ������ģʽ����ֱ����� SkyApm.Agent.AspNetCore �ٷ�������