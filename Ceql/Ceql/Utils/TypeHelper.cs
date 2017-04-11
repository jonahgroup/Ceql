using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ceql.Utils
{
    public static class TypeHelper
    {

        public static IEnumerable<String> EnumerateTypeProperties(this Type type)
        {
            return type.GetProperties().Select(x => x.Name);
        }

        /// <summary>
        /// Searches inheritance chain for the attribute
        /// </summary>
        /// <typeparam name="T">Attribute type to look for</typeparam>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type sourceType) where T : System.Attribute
        {
            while (sourceType != null)
            {
                var attr = sourceType.GetTypeInfo().GetCustomAttribute<T>();
                if (attr != null)
                {
                    return attr;
                }
                sourceType = sourceType.GetTypeInfo().BaseType;
            }

            return default(T);
        }

        public static Type GetType<T>(Type sourceType) where T : System.Attribute
        {
            while (sourceType != null)
            {
                var attr = sourceType.GetTypeInfo().GetCustomAttribute<T>();
                if (attr != null)
                {
                    return sourceType;
                }
                sourceType = sourceType.GetTypeInfo().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Gets properties of a type that have attributes of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesForAttribute<T>(Type sourceType) where T : System.Attribute
        {
            var properties = sourceType.GetTypeInfo().GetProperties();
            return properties.Where(p => p.GetCustomAttribute<T>() != null).ToList();
        }


#if NET_CORE
        public static T LoadType<T>(string typeName) where T : class
        {
            var type = Type.GetType(typeName);
            var instance = Activator.CreateInstance(type) as T;
            return instance;
        }
#endif


#if !NET_CORE
        public static T LoadType<T>(string typeName) where T : class
        {
            var type = Type.GetType(typeName);
            var instance = Activator.CreateInstance(type) as T;
            return instance;
        }
#endif

    }
}
