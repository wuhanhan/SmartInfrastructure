using Microsoft.Extensions.Options;
using SkyApm.Config;
using SkyApm.Transport.Grpc;
using SkyApm.Utilities.Logging;
using SmartInfrastructure.SkyApm.Agent.AspNetCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SmartInfrastructure.SkyApm.Agent.AspNetCore
{
    /// <summary>
    /// 配置存取器
    /// </summary>
    public class ConfigAccessor : IConfigAccessor
    {
        private readonly Dictionary<Type, string> _skyApmConfigMap = new Dictionary<Type, string>();
        private readonly SkyApmOptions _skyApmOption;

        public ConfigAccessor(IOptions<SkyApmOptions> skyApmOptionOptions)
        {
            _skyApmOption = skyApmOptionOptions.Value;
            _skyApmConfigMap = new Dictionary<Type, string>()
            {
                {typeof(InstrumentConfig), "" },
                {typeof(SamplingConfig), "Sampling"},
                {typeof(TransportConfig), "Transport"},
                {typeof(GrpcConfig), "Transport:gRPC"},
                { typeof(LoggingConfig), "Logging"}
            };
        }

        public T Get<T>() where T : class, new()
        {
            var propertyName = _skyApmConfigMap.Where(x => x.Key == typeof(T)).Select(x => x.Value).FirstOrDefault();

            //类型转换也可以使用Json互转方式，这里自己写了一个映射关系
            return _skyApmOption.PropertyMapTo<T>(propertyName);
        }

        public T Value<T>(string key, params string[] sections)
        {
            return default(T);
        }

    }

    /// <summary>
    /// 类型映射
    /// </summary>
    public static class MapExtensions
    {
        /// <summary>
        /// 属性示例转成类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T PropertyMapTo<T>(this object instance, string propertyName)
                 where T : new()
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return (T)TypeMap(instance, typeof(T));
            }
            else
            {
                var currentInstance = instance;
                PropertyInfo propertyInfo = null;
                object propertyValue = null;

                // 考虑多级别情况
                var propertyNameArray = propertyName.TrimEnd(':').Split(':').ToList();
                foreach (var pName in propertyNameArray)
                {
                    propertyInfo = currentInstance.GetType().GetProperties().FirstOrDefault(p => p.Name == pName);
                    if (propertyInfo == null)
                    {
                        return default(T);
                    }
                    propertyValue = propertyInfo.GetValue(currentInstance);
                    currentInstance = propertyValue;
                }

                if (propertyInfo != null && propertyValue != null)
                {
                    var targetInstance = TypeMap(propertyValue, typeof(T));
                    if (targetInstance != null)
                    {
                        return (T)targetInstance;
                    }
                }
            }
            return default(T);
        }

        private static object ClassMap(object instance, Type type)
        {
            if (instance == null)
            {
                return null;
            }

            var targetInstance = Activator.CreateInstance(type, true);
            var fromInstance = instance.GetType();
            var fromProperties = fromInstance.GetProperties().ToList();

            var targetProperties = targetInstance.GetType().GetProperties();
            fromProperties.ForEach(property =>
            {
                var targetProperty = targetProperties.FirstOrDefault(x => x.Name == property.Name);
                if (targetProperty != null)//如果目标对象有这个属性
                {
                    targetProperty.SetValue(targetInstance, TypeMap(property.GetValue(instance), targetProperty.PropertyType));
                }

            });

            return targetInstance;
        }


        /// <summary>
        /// 泛型集合映射
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        private static object GenericListMap(object instance, Type targetType)
        {
            if (instance == null)
            {
                return null;
            }

            var innerType = targetType.IsArray ? GetArrayElementType(targetType) : targetType.GetGenericArguments()[0];

            Type listType = typeof(List<>).MakeGenericType(innerType);
            var list = Activator.CreateInstance(listType);

            int count = Convert.ToInt32(instance.GetType().GetProperty("Count").GetValue(instance, null));
            for (int i = 0; i < count; i++)
            {
                object item = instance.GetType().GetProperty("Item").GetValue(instance, new object[] { i });

                var targetInstance = TypeMap(item, innerType);
                if (targetInstance != null)
                {
                    listType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public).Invoke(list, new
                         object[] { targetInstance });
                }
            }

            if (targetType.IsArray)
            {
                return listType.GetMethod("ToArray").Invoke(list, null);
            }
            return listType;
        }

        /// <summary>
        /// 数组集合映射
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public static object ArrayListMap(object instance, Type targetType)
        {
            if (instance == null)
            {
                return null;
            }

            var innerType = targetType.IsArray ? GetArrayElementType(targetType) : targetType.GetGenericArguments()[0];

            Type listType = typeof(List<>).MakeGenericType(innerType);
            var list = Activator.CreateInstance(listType);

            int count = Convert.ToInt32(instance.GetType().GetProperty("Length").GetValue(instance, null));
            for (int i = 0; i < count; i++)
            {
                object item = instance.GetType().GetMethod("GetValue", new Type[] { typeof(int) }).Invoke(instance, new object[] { i });
                if (item == null)
                {
                    continue;
                }
                var targetInstance = TypeMap(item, innerType);
                if (targetInstance != null)
                {
                    listType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public).Invoke(list, new
                         object[] { targetInstance });
                }
            }

            if (targetType.IsArray)
            {
                return listType.GetMethod("ToArray").Invoke(list, null);
            }
            return listType;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="instance">源对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        private static object TypeMap(object instance, Type targetType)
        {
            if (instance == null)
            {
                return null;
            }

            object targetInstance = null;
            if (instance.GetType() == targetType) // 如果待转换对象的类型与目标类型兼容，则无需转换
            {
                targetInstance = instance;
            }
            else if (targetType.IsGenericType || targetType.IsArray)//List
            {
                if (instance.GetType().IsGenericType)
                {
                    targetInstance = GenericListMap(instance, targetType);
                }
                else if (instance.GetType().IsArray)
                {
                    targetInstance = ArrayListMap(instance, targetType);
                }
            }
            else if (((!targetType.IsPrimitive && targetType != typeof(string)) || targetType.IsInterface))
            {
                targetInstance = ClassMap(instance, targetType);
            }
            else if (targetType.IsEnum) // 如果待转换的对象的基类型为枚举
            {
                if (IsNullableType(targetType) && string.IsNullOrEmpty(instance.ToString())) // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                {
                    return null;
                }
                else
                {
                    return Enum.Parse(targetType, instance.ToString());
                }
            }
            else if (typeof(IConvertible).IsAssignableFrom(targetType)) // 如果目标类型的基类型实现了IConvertible，则直接转换
            {
                try
                {
                    return Convert.ChangeType(instance, targetType, null);
                }
                catch
                {
                    return Activator.CreateInstance(targetType);
                }
            }
            else
            {
                targetInstance = instance;
            }

            return targetInstance;
        }

        /// <summary>
        /// 获得数组元素类型
        /// </summary>
        /// <param name="t">数组类型</param>
        /// <returns></returns>
        public static Type GetArrayElementType(this Type t)
        {
            if (!t.IsArray) return null;

            string tName = new Regex(@"\[\]").Replace(t.FullName, string.Empty, 1);
            Type elType = t.Assembly.GetType(tName);
            return elType;
        }

        /// <summary>
        /// 是否是可空类型
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        private static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
            GetGenericTypeDefinition().Equals
            (typeof(Nullable<>)));
        }

        /// <summary>
        /// high performance
        /// </summary>
        private static class New<T> where T : new()
        {
            public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
            (
                Expression.New(typeof(T))
            ).Compile();
        }
    }
}