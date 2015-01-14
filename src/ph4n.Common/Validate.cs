using System;
using System.Globalization;
using JetBrains.Annotations;

namespace ph4n.Common
{
    public static class Validate
    {
        /// <summary>
        /// Validates argument is not null
        /// </summary>
        /// <param name="value">argument to check</param>
        /// <param name="parameterName">name of arguement</param>
        /// <remarks>Throws ArgumentNullException, ArgumentException</remarks>
        [ContractAnnotation("value: null => halt")]
        public static void ArgumentNotNullOrEmpty([CanBeNull] string value, [NotNull, InvokerParameterName] string parameterName)
        {
            ArgumentNotNull(value, parameterName);

            if (value.Length == 0)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "'{0}' cannot be empty.", parameterName), parameterName);
        }

        /// <summary>
        /// Validates argument is not null and is an enum
        /// </summary>
        /// <param name="value">argument to check</param>
        /// <param name="parameterName">name of arguement</param>
        /// <remarks>Throws ArgumentNullException, ArgumentException</remarks>
        [ContractAnnotation("enumType: null => halt")]
        public static void ArgumentTypeIsEnum([CanBeNull] Type enumType, [NotNull, InvokerParameterName] string parameterName)
        {
            ArgumentNotNull(enumType, "enumType");

            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type {0} is not an Enum.", enumType), parameterName);
        }

        /// <summary>
        /// Validates argument is not null and is an enum
        /// </summary>
        /// <param name="value">argument to check</param>
        /// <param name="parameterName">name of arguement</param>
        /// <remarks>Throws ArgumentNullException</remarks>
        [ContractAnnotation("value: null => halt")]
        public static void ArgumentNotNull([CanBeNull] object value, [NotNull, InvokerParameterName] string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}
