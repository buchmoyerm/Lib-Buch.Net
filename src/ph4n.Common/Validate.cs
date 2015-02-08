using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public static void ArgumentNotNullOrEmpty([CanBeNull] string value,
            [NotNull, InvokerParameterName] string parameterName)
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
        public static void ArgumentTypeIsEnum([CanBeNull] Type enumType,
            [NotNull, InvokerParameterName] string parameterName)
        {
            ArgumentNotNull(enumType, "enumType");

            if (!enumType.IsEnum)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Type {0} is not an Enum.", enumType), parameterName);
        }

        /// <summary>
        /// Validates argument is not null and is an enum
        /// </summary>
        /// <param name="value">argument to check</param>
        /// <param name="parameterName">name of arguement</param>
        /// <remarks>Throws ArgumentNullException</remarks>
        [ContractAnnotation("value: null => halt")]
        public static void ArgumentNotNull([CanBeNull] object value,
            [NotNull, InvokerParameterName] string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Validates that IEnumerable contains an elements that satisfies validFunc
        /// </summary>
        /// <typeparam name="T">type held by enumerable</typeparam>
        /// <param name="enumerable">enumerable object</param>
        /// <param name="testFunc">test function that is being searched for</param>
        /// <remarks>Throws ArgumentException, ArgumentNullException</remarks>
        public static void ArgumentContains<T>([NotNull] IEnumerable<T> enumerable, [NotNull] Func<T, bool> validFunc, [NotNull, InvokerParameterName] string parameterName)
        {
            Validate.ArgumentNotNull(enumerable, "enumerable");
            Validate.ArgumentNotNull(validFunc, "testFunc");
            Validate.ArgumentNotNull(parameterName, "parameterName");

            if (! enumerable.Any(validFunc))
            {
                throw new ArgumentException("Argument does not contain a valid element", parameterName);
            }
        }

        /// <summary>
        /// Validates that IEnumerable contains only elements that satisfy validFunc
        /// </summary>
        /// <typeparam name="T">type held by enumerable</typeparam>
        /// <param name="enumerable">enumerable object</param>
        /// <param name="testFunc">test function that is being searched for</param>
        /// <remarks>Throws ArgumentException, ArgumentNullException</remarks>
        public static void ArgumentContainsOnly<T>([NotNull] IEnumerable<T> enumerable, [NotNull] Func<T, bool> validFunc, [NotNull, InvokerParameterName] string parameterName)
        {
            Validate.ArgumentNotNull(enumerable, "enumerable");
            Validate.ArgumentNotNull(validFunc, "validFunc");
            Validate.ArgumentNotNull(parameterName, "parameterName");

            if (enumerable.Any(t => !validFunc(t)))
            {
                throw new ArgumentException("Argument does not contain a valid element", parameterName);
            }
        }

        /// <summary>
        /// Validates arguemnt is greater than a specific number
        /// </summary>
        /// <param name="argument">argument to compare</param>
        /// <param name="greaterThan">value argument must be greater than</param>
        /// <param name="parameterName">name of the argument</param>
        /// <remarks>throws ArgumentException, ArgumentNullException</remarks>
        public static void ArgumementGreaterThan(IComparable argument, IComparable greaterThan, [NotNull, InvokerParameterName] string parameterName)
        {
            Validate.ArgumentNotNullOrEmpty(parameterName, "parameterName");

            if (argument.CompareTo(greaterThan) <= 0 )
            {
                throw new ArgumentException(string.Format("Agrument is not greater than {0}", greaterThan), parameterName);
            }
        }
    }
}
