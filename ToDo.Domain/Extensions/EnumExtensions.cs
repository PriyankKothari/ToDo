using System;
using System.ComponentModel;
using ToDo.Domain.Helpers;

namespace ToDo.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static T GetStringValue<T>(this string description) where T : struct
        {
            // Get the current enum type
            var type = typeof(T);

            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var fieldInfo in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute
                    descriptionAttribute)
                {
                    if (descriptionAttribute.Description == description) return (T)fieldInfo.GetValue(null);
                }
                else
                {
                    if (fieldInfo.Name == description) return (T)fieldInfo.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(description));
        }

        /// <summary>
        /// Convert string to an enumeration
        /// </summary>
        /// <typeparam name="T">Enumeration type to convert to</typeparam>
        /// <param name="value">String value to convert from</param>
        /// <param name="acceptInvalid">True to return default enumeration if string value is invalid</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns></returns>
        public static T ToEnumeration<T>(this string value, bool acceptInvalid = true, bool ignoreCase = false) where T : struct, IConvertible
        {
            // if the value is null then return the default
            if (value == null) return default(T);

            // convert the string to the enumeration type
            var enumValue = EnumHelper.Parse(typeof(T), value.Trim(), ignoreCase);
            // if conversion is null then the string is invalid for this enumeration
            // if specified we will return the default enum in this case
            if (acceptInvalid && enumValue == null) return default(T);
            // return the enumeration value

            return (T)enumValue;
        }

        /// <summary>
        /// Gets the string value of an enum
        /// </summary>
        public static string ToStringValue(this Enum value)
        {
            return EnumHelper.GetStringValue(value);
        }

        /// <summary>
        /// Convert integer to an enumeration
        /// </summary>
        /// <typeparam name="T">Enumeration type to convert to</typeparam>
        /// <param name="value">Integer value to convert from</param>
        /// <param name="acceptInvalid">True to return default enumeration if string value is invalid</param>
        /// <returns></returns>
        public static T ToEnumeration<T>(this int? value, bool acceptInvalid = true) where T : struct, IConvertible
        {
            // if the value is null then return the default
            if (value == null) return default(T);

            // default return to null - this will cause exception if not set
            object enumValue = null;
            // if integer value is valid then convert to enumeration value
            if (Enum.IsDefined(typeof(T), value)) enumValue = Enum.ToObject(typeof(T), value);
            // if we accept invalid integer values then return the default
            else if (acceptInvalid) enumValue = default(T);
            // return the enumeration value
            return (T)enumValue;
        }


    }
}