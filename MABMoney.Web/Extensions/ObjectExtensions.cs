using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;

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
                output.Add(property.Name, property.GetValue(a));
            }

            PropertyDescriptorCollection bproperties = TypeDescriptor.GetProperties(b);
            foreach (PropertyDescriptor property in bproperties)
            {
                if (!output.ContainsKey(property.Name))
                    output.Add(property.Name, property.GetValue(b));
                else
                    output[property.Name] += (" " + property.GetValue(b));
            }

            return output;
        }
    }
}