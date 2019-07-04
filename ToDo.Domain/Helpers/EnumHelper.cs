using System;
using System.Collections;
using ToDo.Domain.Attributes;

namespace ToDo.Domain.Helpers
{
    public static class EnumHelper
    {
        private static readonly Hashtable StringValuesHashtable = new Hashtable();

        /// <summary>
        /// Gets a string value for a particular enum value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>String Value associated via a <see cref="StringValueAttribute"/> attribute, or null if not found.</returns>
        public static string GetStringValue(Enum value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            string output = null;
            Type type = value.GetType();

            // check if we have already stored the converted string value
            // if we have then just return it
            if (StringValuesHashtable.ContainsKey(value))
                output = (StringValuesHashtable[value] as StringValueAttribute).Value;
            else
            {
                // Look for our 'StringValueAttribute' in the field's custom attributes
                var fieldInfo = type.GetField(value.ToString());
                if (fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[]
                        stringValueAttributes && stringValueAttributes.Length > 0)
                {
                    // lock the values array to prevent a 2nd thread calling 
                    // ContainsKey before the 1st has added the new value
                    // _stringValues is a static variable shared amongst multiple threads
                    lock (StringValuesHashtable)
                    {
                        if (!StringValuesHashtable.ContainsKey(value))
                            StringValuesHashtable.Add(value, stringValueAttributes[0]);
                    }
                    output = stringValueAttributes[0].Value;
                }
            }
            return output;
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="stringValue">String value.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue, bool ignoreCase)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            object output = null;
            string enumStringValue = null;

            if (!type.IsEnum)
                throw new ArgumentException($"Supplied type must be an Enum.  Type was {type}");

            //Look for our string value associated with fields in this enum
            foreach (var fieldInfo in type.GetFields())
            {
                //Check for our custom attribute
                if (fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[]
                        stringValueAttributes &&
                    stringValueAttributes.Length > 0) enumStringValue = stringValueAttributes[0].Value;

                //Check for equality then select actual enum value.
                if (string.Compare(enumStringValue, stringValue, ignoreCase) == 0)
                {
                    output = Enum.Parse(type, fieldInfo.Name);
                    break;
                }
            }

            return output;
        }
    }
}