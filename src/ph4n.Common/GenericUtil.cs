using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ph4n.Common
{
    public static class GenericUtil
    {
        /// <summary>
        /// Swap two object references
        /// </summary>
        /// <typeparam name="T">type of objects being swapped</typeparam>
        /// <param name="lhs">will become rhs object</param>
        /// <param name="rhs">will become lhs object</param>
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            //rhs = Interlocked.Exchange(ref lhs, rhs);
            var temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// updates one object to the value of the other
        /// </summary>
        /// <typeparam name="T">type of object being updated</typeparam>
        /// <param name="oldval">object being updated</param>
        /// <param name="newval">value oldval is being updated to</param>
        /// <returns>true if oldval has changed</returns>
        public static bool UpdateVal<T>(ref T oldval, T newval)
        {
            if (oldval == null || !oldval.Equals(newval))
            {
                oldval = newval;
                return true;
            }
            return false;
        }

        /// <summary>
        /// converts an object to a double
        /// </summary>
        /// <param name="obj">object to be converted</param>
        /// <returns>double value</returns>
        public static double ToDouble([NotNull] object obj)
        {
            Validate.ArgumentNotNull(obj, "obj");
            return obj.ToDouble();
        }

        /// <summary>
        /// updates one double to the value of the other
        /// </summary>
        /// <param name="oldval">old value to be updated</param>
        /// <param name="newval">new value to be used</param>
        /// <returns>true if oldval changed</returns>
        public static bool UpdateVal(ref double oldval, [NotNull] object newval)
        {
            var d = (newval is double) ? (double)newval : (double)(long)newval;
            return UpdateVal(ref oldval, d);
        }

        /// <summary>
        /// Updates the value of one double to the other
        /// </summary>
        /// <param name="oldval">old value being updated</param>
        /// <param name="newval">new value to update to</param>
        /// <returns>true if oldval changed</returns>
        public static bool UpdateVal(ref double oldval, double newval)
        {
            if (Math.Abs(oldval - newval) > double.Epsilon)
            {
                oldval = newval;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a type with a generic type
        /// Example: CreateGeneric(typeof(List<>, typeof(string)); returns a new List<string>() object
        /// </summary>
        /// <param name="generic"></param>
        /// <param name="innerType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return Activator.CreateInstance(specificType, args);
        }

        /// <summary>
        /// Converts an object to a specific type
        /// </summary>
        /// <typeparam name="TOut">Type to convert to</typeparam>
        /// <param name="orig">original object</param>
        /// <returns>value of TOut type</returns>
        public static TOut ConvertTo<TOut>([NotNull] object orig)
        {
            Validate.ArgumentNotNull(orig, "orig");
            return orig.ConvertTo<TOut>();
        }

        /// <summary>
        /// Checks if an object is null or its value is zero
        /// </summary>
        /// <param name="obj">object to test</param>
        /// <returns>returns true of object is null or a zero</returns>
        public static bool IsZeroOrNull([NotNull] object obj)
        {
            Validate.ArgumentNotNull(obj, "obj");
            return obj.IsZeroOrNull();
        }
    }

    /// <summary>
    /// Class for Type extension methods
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// Checks to see a type is a Nullable type
        /// </summary>
        /// <param name="t">type to check</param>
        /// <returns>true if t is a NullableType</returns>
        public static bool IsNullableType([NotNull] this Type t)
        {
            Validate.ArgumentNotNull(t, "t");

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Determine whether a type is simple (String, Decimal, DateTime, etc)
        /// or complex (i.e. custom class with public properties and methods).
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
        /// <see cref="https://gist.github.com/jonathanconway/3330614"/>
        public static bool IsSimpleType([NotNull] this Type type)
        {
            Validate.ArgumentNotNull(type, "type");

            return
                type.IsValueType ||
                type.IsPrimitive ||
                new Type[]
                {
                    typeof (String),
                    typeof (Decimal),
                    typeof (DateTime),
                    typeof (DateTimeOffset),
                    typeof (TimeSpan),
                    typeof (Guid),
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }

        /// <summary>
        /// Determine whether a type is whole number type (int, long, short)
        /// </summary>
        public static bool IsWholeNumber([NotNull] this Type type)
        {
            Validate.ArgumentNotNull(type, "type");

            return
                new Type[]
                {
                    typeof (short),
                    typeof (int),
                    typeof (long),
                    typeof (Int16),
                    typeof (Int32),
                    typeof (Int64),
                }.Contains(type);
        }

        /// <summary>
        /// Determine whether a type is some time value type
        /// (Time values are DateTime, DateTimeOffset, TimeSpan)
        /// </summary>
        /// <param name="type">The type to examine</param>
        /// <returns>true if type is a time value type</returns>
        public static bool IsTimeType([NotNull] this Type type)
        {
            Validate.ArgumentNotNull(type, "type");

            return new Type[]
            {
                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan)
            }.Contains(type);
        }
    }

    /// <summary>
    /// Class extension for objects
    /// </summary>
    public static class ObjectEx
    {
        /// <summary>
        /// Checks if an object is null or its value is zero
        /// </summary>
        /// <param name="obj">object to test</param>
        /// <returns>returns true of object is null or a zero</returns>
        public static bool IsZeroOrNull(this object obj)
        {
            if (obj == null) return true;

            if (!obj.GetType().IsPrimitive)
                return false;

            if (obj is DateTime)
            {
                return ((DateTime)obj) == DateTime.MinValue; //this is the equivalent of zero for datetime
            }

            var vt = obj as ValueType;

            return vt != null && vt.Equals(Convert.ChangeType(0, obj.GetType()));
        }

        /// <summary>
        /// Converts an object to a specific type
        /// </summary>
        /// <typeparam name="TOut">Type to convert to</typeparam>
        /// <param name="orig">original object</param>
        /// <returns>value of TOut type</returns>
        [DebuggerStepThrough, DebuggerHidden]
        [CanBeNull]
        public static TOut ConvertTo<TOut>(this object orig)
        {
            return ConvertTo<object, TOut>(orig);
        }

        /// <summary>
        /// Converts from one type to a different type
        /// </summary>
        /// <typeparam name="TIn">Type of input value</typeparam>
        /// <typeparam name="TOut">Type to be converted to</typeparam>
        /// <param name="orig">value of TIn</param>
        /// <returns>value of TOut type</returns>
        [DebuggerStepThrough, DebuggerHidden]
        [CanBeNull]
        public static TOut ConvertTo<TIn, TOut>([CanBeNull] this TIn orig)
        {
            if (orig == null)
                return default(TOut);

            if (orig is TOut)
            {
                // HACK
                return (TOut)(object)orig;
            }
            else
            {
                try
                {
                    Type targetType = typeof(TOut);

                    if (targetType.IsNullableType())
                    {
                        targetType = Nullable.GetUnderlyingType(targetType);
                    }

                    //Force truncating when converting to a whole number
                    if (targetType.IsWholeNumber())
                    {
                        return (TOut)Convert.ChangeType(Math.Truncate(orig.ConvertTo<double>()), targetType,
                                    CultureInfo.InvariantCulture);
                    }

                    if (targetType.IsEnum && orig is string)
                    {
                        return (orig as string).ToEnum<TOut>();
                    }

                    return (TOut)Convert.ChangeType(orig, targetType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return default(TOut);
                }
            }
        }

        /// <summary>
        /// String extension method to convert a string to an enum value
        /// </summary>
        /// <typeparam name="TOut">Type of Enum</typeparam>
        /// <param name="str">string value to convert</param>
        /// <returns>enum value</returns>
        /// <remarks>Throws ArgumentNullException, ArgumentException</remarks>
        [NotNull]
        private static TOut ToEnum<TOut>([NotNull] this string str)
        {
            Validate.ArgumentTypeIsEnum(typeof(TOut), "TOut");
            Validate.ArgumentNotNullOrEmpty(str, "str");
            try
            {
                TOut res = (TOut)Enum.Parse(typeof(TOut), str, true);
                if (!Enum.IsDefined(typeof(TOut), res)) return default(TOut);
                return res;
            }
            catch
            {
                return default(TOut);
            }
        }

        /// <summary>
        /// converts an object to a double
        /// </summary>
        /// <param name="val">object to be converted</param>
        /// <returns>double value</returns>
        public static double ToDouble(this object val)
        {
            return (val is double) ? (double)val : val.ConvertTo<double>();
        }
    }

    public static class DateEx
    {
        private static readonly List<DayOfWeek> Weekend = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

        public static bool IsWeekend(this DateTime date)
        {
            return Weekend.Contains(date.DayOfWeek);
        }

        public static bool IsWeekday(this DateTime date)
        {
            return !date.IsWeekend();
        }
    }
}