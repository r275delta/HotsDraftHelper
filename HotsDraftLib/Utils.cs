using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace HotsDraftLib
{
    public static class Utils
    {
        public static bool FastParse(string s, out int val)
        {
            bool digits = false;
            val = 0;
            bool negate = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (i == 0 && c == '-')
                {
                    negate = true;
                    continue;
                }
                if (c < '0' || c > '9')
                    return false;
                val = val * 10 + (c - '0');
                digits = true;
            }
            if (negate)
                val *= -1;
            return digits;
        }

        public static bool FastParse(string s, out long val)
        {
            bool digits = false;
            val = 0;
            bool negate = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (i == 0 && c == '-')
                {
                    negate = true;
                    continue;
                }
                if (c < '0' || c > '9')
                    return false;
                val = val * 10 + (c - '0');
                digits = true;
            }
            if (negate)
                val *= -1;
            return digits;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            return IndexOf(source, value, EqualityComparer<T>.Default);
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T> cmp)
        {
            int i = 0;
            foreach (var item in source)
            {
                if (cmp.Equals(item, value))
                    return i;
                i++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> matcher)
        {
            int i = 0;
            foreach (var item in source)
            {
                if (matcher(item))
                    return i;
                i++;
            }
            return -1;
        }

        private static readonly Normal _standardNormal = new Normal(0, 1);

        public static double CalcAdjustment(double baseWinrate, double conditionalWinrate)
        {
            if (baseWinrate == conditionalWinrate || conditionalWinrate == 0)
                return 0;

            return _standardNormal.InverseCumulativeDistribution(conditionalWinrate) - _standardNormal.InverseCumulativeDistribution(baseWinrate);
        }

        public static double ApplyAdjustment(double baseWinrate, double adjustment)
        {
            if (adjustment == 0)
                return baseWinrate;

            return _standardNormal.CumulativeDistribution(_standardNormal.InverseCumulativeDistribution(baseWinrate) + adjustment);
        }
    }
}
