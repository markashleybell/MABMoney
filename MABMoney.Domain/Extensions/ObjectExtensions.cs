using mab.lib.SimpleMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MABMoney.Domain.Extensions
{
    public static class ObjectExtensions
    {
        public static void SetPrivatePropertyValue(this object obj, string propertyName, object value)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);

            if (property != null && property.SetMethod != null)
            {
                property.SetValue(obj, value);
            }
            else
            {
                // Try the base type
                property = type.BaseType.GetProperty(propertyName);

                if (property != null)
                {
                    property.SetValue(obj, value);
                }
                else
                {
                    throw new Exception(string.Format("Type has no property '{0}'", propertyName));
                }
            }
        }

        public static void SetPrivateFieldValue(this object obj, string fieldName, object value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
        }

        public static void PopulatePrivateList<TSource, TDestination>(this object obj, string fieldName, ICollection<TSource> source)
            where TSource : class
            where TDestination : class
        {
            if (source != null)
            {
                var items = source.ToList().MapToList<TDestination>();
                SetPrivateFieldValue(obj, fieldName, items);
            }
        }
    }
}
