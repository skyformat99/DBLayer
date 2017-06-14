using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLayer.Core
{
    public static class FuncExtensions
    {
        public static bool In<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool NotIn<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool InFunc<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool NotInFunc<T>(this T obj, T[] array)
        {
            return true;
        }
        public static bool Like(this string str, string likeStr)
        {
            return true;
        }
        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
    }
}
