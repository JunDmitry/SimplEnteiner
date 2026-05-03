using System;
using System.Collections.Generic;

namespace SimplEnteiner.Utilities
{
    internal static class ListExtensions
    {
        public static int FindBinaryIndexMoreThan<T>(this List<T> values, T value, Func<T, int> prioritySelector)
        {
            values.ThrowIfArgumentNull();
            prioritySelector.ThrowIfArgumentNull();

            if (values == null || values.Count == 0)
                return 0;

            int left = 0;
            int right = values.Count;
            int order = prioritySelector(value);

            while (left < right)
            {
                int mid = left + (right - left) / 2;

                if (prioritySelector(values[mid]) <= order)
                    left = mid + 1;
                else
                    right = mid;
            }

            return left;
        }
    }
}
