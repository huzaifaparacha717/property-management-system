using System;

namespace prjRentalManagement.Helpers
{
    /// <summary>Formats PKR amounts like listing sites (Thousands / Lacs / Crore).</summary>
    public static class PakRsFormat
    {
        public static string Rent(decimal? rupees)
        {
            if (!rupees.HasValue || rupees.Value <= 0)
                return "Ask for rent";

            var r = rupees.Value;

            if (r >= 10000000m)
            {
                var c = r / 10000000m;
                return IsWhole(c) ? $"Rs {(int)c} Crore" : $"Rs {c:0.##} Crore";
            }

            if (r >= 100000m)
            {
                var l = r / 100000m;
                if (IsWhole(l))
                {
                    var n = (int)l;
                    return n == 1 ? "Rs 1 Lac" : $"Rs {n} Lacs";
                }
                return $"Rs {l:0.##} Lacs";
            }

            if (r >= 1000m)
            {
                var k = r / 1000m;
                if (IsWhole(k))
                    return $"Rs {(int)k} Thousands";
                return $"Rs {k:0.##} Thousands";
            }

            return $"Rs {r:0}";
        }

        static bool IsWhole(decimal x) => x == Math.Truncate(x);
    }
}
