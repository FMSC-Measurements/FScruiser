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
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e != null && e.Count() > 0;
        }
    }

    public static class IEnumerableStringExtentions
    {
        public static string ToString(this IEnumerable<char> @this)
        {
            return new string(@this.ToArray());
        }
    }
}