using System.Collections.Generic;

namespace SimplEnteiner.Utilities
{
    internal static class ListExtensions
    {
        public static int FindBinaryIndexMoreThan<T>(this List<(T, int Order)> values, (T, int Order) value)
        {
            if (values == null || values.Count == 0)
                return 0;

            int left = 0;
            int right = values.Count;
            int order = value.Order;

            while (left < right)
            {
                int mid = left + (right - left) / 2;

                if (values[mid].Order <= order)
                    left = mid + 1;
                else
                    right = mid;
            }

            return left;
        }
    }
}
