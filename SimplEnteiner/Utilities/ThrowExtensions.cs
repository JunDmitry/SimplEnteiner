using System;

namespace SimplEnteiner.Utilities
{
    internal static class ThrowExtensions
    {
        public static T ThrowIfArgumentNull<T>(this T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj;
        }

        public static T ThrowInvalidIfNull<T>(this T obj, string message)
        {
            if (obj == null)
                throw new InvalidOperationException(message);

            return obj;
        }

        public static T ThrowIfInvalidOperation<T>(this T obj, bool isValid, string message)
        {
            if (isValid == false)
                throw new InvalidOperationException(message);

            return obj;
        }
    }
}
