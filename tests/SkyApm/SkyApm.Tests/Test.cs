using SkyApm.Config;
using SkyApm.Transport.Grpc;
using SmartInfrastructure.SkyApm.Agent.AspNetCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xunit;
using SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration;

namespace SkyApm.Tests
{
    public class Test
    {
        [Fact]
        public void MapFromInstancePropertyTest()
        {
            Person p = new Person();
            object o = p;
            Person p1 = (Person)o;

            SkyApmOptions SkyApmOption = new SkyApmOptions()
            {
                ServiceName = "SkyApm",
                Transport = new SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration.SkyApmTransportOption()
                {
                    gRPC = new SkyApmGrpcOption()
                    {
                        Servers = "http://localhost:8000"
                    }
                }
            };
            string[] s = new string[3];

            //ConfigAccessor configAccessor = new ConfigAccessor(null);
            //var result = skyApmConfig.PropertyMapTo<GrpcConfig>("Grpc");
            //Assert.NotNull(result);
            //Assert.Equal(skyApmConfig.Grpc.Servers, result.Servers);


            InstrumentConfig grpcConfig = SkyApmOption.PropertyMapTo<InstrumentConfig>("");
            Assert.NotNull(grpcConfig);
            //Assert.Equal(SkyApmOption.Transport.gRPC.Servers, grpcConfig.);
        }

        [Fact]
        public void TypeConvertTest()
        {
            var person = new
                Person()
            { Name = "张三" };

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Man));
            if (converter.CanConvertFrom(person.GetType()))
            {
                var r = converter.ConvertFrom(person);
            }
        }

        [Fact]
        public void ReflectTest()
        {
            Person[] personArray = new Person[1] { new Person() { Name = "zx" } };

            var li = MapExtensions.ArrayListMap(personArray, typeof(List<Man>));

            var type = personArray.GetType().GetArrayElementType();

            Type listType = typeof(List<>).MakeGenericType(type);
            //不知道T，如果知道T， 就 List<T> list=(List<T>)List;
            var list = Activator.CreateInstance(listType);
            listType.GetMethod("Add").Invoke(list, new object[] { new Person() });

            var o = listType.GetMethod("ToArray").Invoke(list, null);


            var res = typeof(Person).IsAssignableFrom(typeof(Man));

        }

    }

    public class Person
    {
        public string Name { get; set; }

        public Profession Profession { get; set; }
    }

    public class Profession
    {
        public Company Company { get; set; }

        public string Position { get; set; }
    }

    public class Company
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }
    }

    public class ProfessionCopy
    {
        public Company Company { get; set; }

        public string Position { get; set; }
    }

    public class Man : Person
    {

    }
}
