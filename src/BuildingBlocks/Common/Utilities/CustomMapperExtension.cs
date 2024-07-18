using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Utilities;

public static class CustomMapperExtension
{
    public static T Map<T>(this object mapObject,
                           params string[] ignoreProperties) where T : class
    {
        return mapObject.Map<T>(null, null, ignoreProperties);
    }

    public static T Map<T>(this object mapObject,
                            List<MapMember> members = null,
                            params string[] ignoreProperties) where T : class
    {
        return mapObject.Map<T>(null, members, ignoreProperties);
    }

    public static T Map<T>(this object mapObject,
                            T resultObject,
                            params string[] ignoreProperties) where T : class
    {
        return mapObject.Map(resultObject, null, ignoreProperties);
    }

    public static IEnumerable<T> Map<T>(this IEnumerable mapObjects,
                                        IEnumerable<T> resultObject = null,
                                        List<MapMember> members = null,
                                        params string[] ignoreProperties) where T : class
    {
        if (mapObjects == null || !mapObjects.IsCollection()) return null;

        var result = (List<T>)resultObject ?? new List<T>();
        foreach (var item in mapObjects) result.Add(item.Map<T>(null, members, ignoreProperties));

        return result;
    }

    public static T Map<T>(this object mapObject,
                            T resultObject = null,
                            List<MapMember> members = null,
                            params string[] ignoreProperties) where T : class
    {
        if (members == null)
            members = new List<MapMember>();


        if (mapObject == null)
            return null;

        var target = typeof(T);
        var instance = resultObject ?? Activator.CreateInstance(target, false);
        var targetPropertyList = target.GetProperties().ToList();
        foreach (var targetProperty in targetPropertyList.Where(x => x.GetSetMethod(true) != null))
        {
            if (ignoreProperties != null && ignoreProperties.Contains(targetProperty.Name))
                continue;
            PropertyInfo mapObjectProperty = null;

            if (members.Any(x => x.To.Equals(targetProperty.Name)))
            {
                var fromName = members.FirstOrDefault(k => k.To.Equals(targetProperty.Name)).From;
                mapObjectProperty = mapObject.GetType().GetProperties()
                    .FirstOrDefault(x => x.CanWrite && x.Name.Equals(fromName));
            }
            else
            {
                mapObjectProperty = mapObject.GetType().GetProperties()
                    .FirstOrDefault(x => x.CanWrite && x.Name.Equals(targetProperty.Name));
            }


            if (mapObjectProperty != null)
            {
                if (!mapObjectProperty.PropertyType.IsSimple()) continue;
                var mapObjectValue = mapObjectProperty?.GetValue(mapObject, null);
                if (mapObjectValue is string)
                    mapObjectValue = mapObjectValue.ToString().Trim();

                targetProperty.SetValue(instance, mapObjectValue, null);
            }
        }

        return (T)instance;
    }


    public static T MapProperty<T>(this object mapObject,
                                    string propertyName,
                                    T resultObject,
                                    string mapPropertyName = null) where T : class
    {
        var target = typeof(T);
        var instance = resultObject ?? Activator.CreateInstance(target, null);

        var resultProperty = target.GetProperty(propertyName);
        if (resultProperty != null)
        {
            var mapProperty = mapObject.GetType().GetProperty(mapPropertyName ?? propertyName);
            var mapValue = mapProperty?.GetValue(mapObject, null);

            if (resultProperty.PropertyType == mapProperty?.PropertyType)
            {
                resultProperty.SetValue(instance, mapValue, null);
            }
            else
            {
                if (!resultProperty.PropertyType.IsArray)
                {
                    var typeConverter = TypeDescriptor.GetConverter(resultProperty.PropertyType);
                    var value = typeConverter.ConvertFromString(mapValue?.ToString());
                    resultProperty.SetValue(instance, value, null);
                }
            }
        }

        return (T)instance;
    }

    #region MapFromNestedProperty

    public static IEnumerable<T> MapNested<T>(this IEnumerable mapObjects,
                                              List<MapMember> members = null,
                                              params string[] ignoreProperties) where T : class
    {
        if (mapObjects == null || !mapObjects.IsCollection()) return null;

        var result = new List<T>();
        foreach (var item in mapObjects) result.Add(item.MapNested<T>(members, ignoreProperties));

        return result;
    }

    public static T MapNested<T>(this object mapObject,
                                List<MapMember> members = null,
                                params string[] ignoreProperties)
                                where T : class
    {
        if (members == null)
            members = new List<MapMember>();

        if (mapObject == null)
            return null;

        var target = typeof(T);
        var instance = Activator.CreateInstance(target, false);
        var targetPropertyList = target.GetProperties().ToList();
        foreach (var targetProperty in targetPropertyList.Where(x => x.GetSetMethod(true) != null))
        {
            if (ignoreProperties != null && ignoreProperties.Contains(targetProperty.Name))
                continue;

            PropertyInfo mapObjectProperty = null;
            object mapObjectValue = null;
            if (members.Any(x => x.To.Equals(targetProperty.Name)))
            {
                var Fromname = members.FirstOrDefault(k => k.To.Equals(targetProperty.Name)).From;
                var test2 = mapObject.GetPropertyValue(Fromname.Split(".")[0]).GetPropertyValue(Fromname.Split(".")[1]);
                var i = 0;
                foreach (var item in Fromname.Split("."))
                {
                    if (i == 0)
                        mapObjectValue = mapObject.GetPropertyValue(item);

                    if (i > 0)
                        mapObjectValue = mapObjectValue.GetPropertyValue(item);

                    i++;
                }
            }
            else
            {
                mapObjectValue = mapObject.GetPropertyValue(targetProperty.Name);
            }


            if (mapObjectValue != null)
            {
                if (mapObjectValue is string)
                    mapObjectValue = mapObjectValue.ToString().Trim();

                targetProperty.SetValue(instance, mapObjectValue, null);
            }
        }

        return (T)instance;
    }

    #endregion

    #region Helper
    private static bool IsCollection(this object obj)
    {
        if (obj == null) return false;
        return obj is IList &&
               obj.GetType().IsGenericType &&
               obj.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
    }
    #endregion
}


public class MapMember
{
    public MapMember(string from, string to)
    {
        From = from;
        To = to;
    }

    public string From { get; set; }
    public string To { get; set; }
}