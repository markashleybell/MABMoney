using System.Collections.Generic;
using System.ComponentModel;

namespace MABMoney.Web.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> AddProperty(this object obj, string name, object value)
        {
            var dictionary = obj.ToDictionary();
            dictionary.Add(name, value);
            return dictionary;
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }
            return result;
        }

        public static IDictionary<string, object> CombineWith(this object a, object b)
        {
            IDictionary<string, object> output = new Dictionary<string, object>();
            PropertyDescriptorCollection aproperties = TypeDescriptor.GetProperties(a);
            foreach (PropertyDescriptor property in aproperties)
            {
                var propertyName = property.Name.Replace('_', '-');

                output.Add(propertyName, property.GetValue(a));
            }

            PropertyDescriptorCollection bproperties = TypeDescriptor.GetProperties(b);
            foreach (PropertyDescriptor property in bproperties)
            {
                var propertyName = property.Name.Replace('_', '-');

                if (!output.ContainsKey(propertyName))
                    output.Add(propertyName, property.GetValue(b));
                else
                    output[propertyName] += (" " + property.GetValue(b));
            }

            return output;
        }
    }
}