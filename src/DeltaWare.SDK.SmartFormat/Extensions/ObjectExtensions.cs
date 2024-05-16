using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System
{
    internal static class ObjectExtensions
    {
        public static Dictionary<string, object> GetPublicPropertiesAsDictionary(this object value)
        {
            Type objectType = value.GetType();

            Dictionary<string, object> propertyValues = new Dictionary<string, object>();

            foreach (PropertyInfo property in objectType.GetPublicProperties())
            {
                object propertyValue = property.GetValue(value);

                propertyValues.Add(property.Name, propertyValue);
            }

            return propertyValues;
        }
    }
}
