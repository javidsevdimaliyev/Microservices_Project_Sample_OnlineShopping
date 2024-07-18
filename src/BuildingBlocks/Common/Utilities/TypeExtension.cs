using System.Collections;
using System.Reflection;

namespace Utilities
{
    public static class TypeExtension
    {
        public static PropertyInfo GetProperty(this object obj, string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
                return obj?.GetType().GetProperty(propertyName);

            return null;
        }

        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetProperty(propertyName)?.GetValue(obj, null);
        }


        public static PropertyInfo GetPropertyWithoutAttribute<T>(this Type type, string propertyName) where T : Attribute
        {
            if (!string.IsNullOrEmpty(propertyName))
                return type.GetPropertiesWithoutAttribute<T>().FirstOrDefault(x => x.Name == propertyName);

            return null;
        }

        public static PropertyInfo GetPropertyWithoutAttribute<T>(this object obj, string propertyName) where T : Attribute
        {
            return obj.GetType().GetPropertyWithoutAttribute<T>(propertyName);
        }



        public static PropertyInfo[] GetPropertiesWithoutAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetProperties().Where(x => !Attribute.IsDefined(x, typeof(T))).ToArray();
        }

        public static PropertyInfo[] GetPropertiesWithoutAttribute<T>(this object obj) where T : Attribute
        {
            return obj.GetType().GetPropertiesWithoutAttribute<T>();
        }

        private static bool IsEnumerable(this object obj)
        {
            if (obj == null) return false;
            return obj is IList &&
                   obj.GetType().IsGenericType &&
                   obj.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
        }

        public static bool IsSimple(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                // nullable type, check if the nested type is simple.
                return typeInfo.GetGenericArguments()[0].IsSimple();
            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type.Equals(typeof(string))
                   || type.Equals(typeof(DateTime))
                   || type.Equals(typeof(object))
                   || type.Equals(typeof(decimal))
                   || type.Equals(typeof(TimeSpan));
        }
    }
}
