using DIMS.Core.Extensions;

namespace Construction.Core.DataAccess.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class EnumerationHelper
    {
        private static readonly IDictionary<Type, Func<int, object>> FromValueDelegates = new Dictionary<Type, Func<int, object>>();
        private static readonly IDictionary<Type, Func<string, object>> FromDisplayNameDelegates = new Dictionary<Type, Func<string, object>>();
        private static readonly IDictionary<Type, Func<IEnumerable<object>>> GetAllDelegates = new Dictionary<Type, Func<IEnumerable<object>>>();
        private static readonly IDictionary<Type, Func<IEnumerable<object>>> GetAllNonDeprecatedDelegates = new Dictionary<Type, Func<IEnumerable<object>>>();
        private static readonly IDictionary<Type, Func<object, int>> GetValueDelegates = new Dictionary<Type, Func<object, int>>();
        private static readonly IDictionary<Type, Func<object, string>> GetDisplayNameDelegates = new Dictionary<Type, Func<object, string>>();
        private static readonly IDictionary<Type, Func<object, int>> GetOrderDelegates = new Dictionary<Type, Func<object, int>>();

        public static object FromValue(Type enumerationType, int value)
        {
            Func<int, object> @delegate;

            lock (FromValueDelegates)
            {
                if (!FromValueDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var method = enumerationType.GetMethod("FromValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(int) }, null);

                    @delegate = method.GetLambda<int, object>().Compile();

                    FromValueDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate(value);
        }

        public static object FromDisplayName(Type enumerationType, string displayName)
        {
            Func<string, object> @delegate;

            lock (FromDisplayNameDelegates)
            {
                if (!FromDisplayNameDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var method = enumerationType.GetMethod("FromDisplayName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(string) }, null);

                    @delegate = method.GetLambda<string, object>().Compile();

                    FromDisplayNameDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate(displayName);
        }

        public static IEnumerable<object> GetAll(Type enumerationType)
        {
            Func<IEnumerable<object>> @delegate;

            lock (GetAllDelegates)
            {
                if (!GetAllDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var method = enumerationType.GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new Type[0], null);

                    @delegate = method.GetLambda<IEnumerable<object>>().Compile();

                    GetAllDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate();
        }

        public static IEnumerable<object> GetAllNonDeprecated(Type enumerationType)
        {
            Func<IEnumerable<object>> @delegate;

            lock (GetAllNonDeprecatedDelegates)
            {
                if (!GetAllNonDeprecatedDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var method = enumerationType.GetMethod("GetAllNonDeprecated", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new Type[0], null);

                    @delegate = method.GetLambda<IEnumerable<object>>().Compile();

                    GetAllNonDeprecatedDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate();
        }

        public static int GetValue(object enumeration)
        {
            var enumerationType = enumeration.GetType();

            Func<object, int> @delegate;

            lock (GetValueDelegates)
            {
                if (!GetValueDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var property = enumerationType.GetProperty("Value");

                    @delegate = property.GetLambda<object, int>().Compile();

                    GetValueDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate(enumeration);
        }

        public static string GetDisplayName(object enumeration)
        {
            var enumerationType = enumeration.GetType();

            Func<object, string> @delegate;

            lock (GetDisplayNameDelegates)
            {
                if (!GetDisplayNameDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var property = enumerationType.GetProperty("DisplayName");

                    @delegate = property.GetLambda<object, string>().Compile();

                    GetDisplayNameDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate(enumeration);
        }

        public static int GetOrder(object enumeration)
        {
            var enumerationType = enumeration.GetType();

            Func<object, int> @delegate;

            lock (GetOrderDelegates)
            {
                if (!GetOrderDelegates.TryGetValue(enumerationType, out @delegate))
                {
                    var property = enumerationType.GetProperty("Order");

                    @delegate = property.GetLambda<object, int>().Compile();

                    GetOrderDelegates.Add(enumerationType, @delegate);
                }
            }

            return @delegate(enumeration);
        }
    }
}