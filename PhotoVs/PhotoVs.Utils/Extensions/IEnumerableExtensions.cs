﻿using System;
using System.Collections.Generic;

namespace PhotoVs.Utils.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
                action(item);
        }
    }
}