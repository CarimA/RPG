using System.Globalization;

namespace PhotoVs.Engine.TiledMaps
{
    internal static class Helpers
    {
        private static readonly bool[] translate = { false, true };

        public static int? ParseInt32(this string str)
        {
            return str == null ? default(int?) : int.Parse(str, CultureInfo.InvariantCulture);
        }

        public static double? ParseDouble(this string str)
        {
            return str == null ? default(double?) : double.Parse(str, CultureInfo.InvariantCulture);
        }

        public static bool? ParseBool(this string str)
        {
            return str == null ? default(bool?) : translate[int.Parse(str, CultureInfo.InvariantCulture)];
        }
    }
}