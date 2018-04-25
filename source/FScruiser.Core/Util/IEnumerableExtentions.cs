﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Core.Util
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this ?? Enumerable.Empty<T>();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || e.Count() == 0;
        }

        public static int MaxOrDefault<T>(this IEnumerable<T> @this, Func<T,int> selector, int dVal = default(int))
        {
            if(@this.IsNullOrEmpty())
            { return dVal; }
            else
            { return @this.Max(selector); }
        }
    }
}