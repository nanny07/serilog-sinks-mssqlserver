using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Serilog.Sinks.MSSqlServer
{
    /// <summary>
    /// Helper for applying only those properties actually specified in external configuration.
    /// </summary>
    public static partial class SetProperty
    {
        // Usage:
        // SetProperty.IfValueNotNull<bool>(stringFromConfig, (boolOutputValue) => opts.BoolProperty = boolOutputValue);

        /// <summary>
        /// Simulates passing a property-setter to an "out" argument.
        /// </summary>
        public delegate void PropertySetter<T>(T value);

        /// <summary>
        /// This will only set a value (execute the PropertySetter delegate) if the value is non-null.
        /// It also converts the provided value to the requested type. This allows configuration to only
        /// apply property changes when external configuration has actually provided a value.
        /// </summary>
        public static void IfNotNull<T>(string value, PropertySetter<T> setter)
        {
            if (value == null || setter == null) return;
            try
            {
                var setting = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                setter(setting);
            }
            // don't change the property if the conversion fails
            catch (InvalidCastException) { }
            catch (OverflowException) { }
        }

        /// <summary>
        /// aaa
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="sectionKey"></param>
        /// <param name="setter"></param>
        public static void IfNotNull2<T>(IConfigurationSection section, string sectionKey, PropertySetter<T> setter)
        {
            if (section == null || sectionKey == null || setter == null) return;
            try
            {
                var setting1 = section.GetSection(sectionKey).Value;
                var setting = (T)Convert.ChangeType(setting1, typeof(T), CultureInfo.InvariantCulture);

                setter(setting);
            }
            // don't change the property if the conversion fails
            catch (InvalidCastException) { }
            catch (OverflowException) { }
        }

        /// <summary>
        /// This will only set a value (execute the PropertySetter delegate) if the value is non-null.
        /// It also converts the provided value to the requested enum type. This allows configuration to only
        /// apply property changes when external configuration has actually provided a value.
        /// </summary>
        public static void IfEnumNotNull<T>(string value, PropertySetter<T> setter) where T : System.Enum
        {
            if (value == null || setter == null) return;
            try
            {
                var setting = (T)Enum.Parse(typeof(T), value, ignoreCase: true);
                setter(setting);
            }
            // don't change the property if the conversion fails
            catch (InvalidCastException) { }
            catch (OverflowException) { }
        }

        /// <summary>
        /// This will only set a value (execute the PropertySetter delegate) if the value is non-null and
        /// isn't empty or whitespace. This override is used when {T} is a string value that can't be empty.
        /// It also converts the provided value to the requested type. This allows configuration to only
        /// apply property changes when external configuration has actually provided a value.
        /// </summary>
        public static void IfNotNullOrEmpty<T>(string value, PropertySetter<T> setter)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            IfNotNull(value, setter);
        }
    }
}
