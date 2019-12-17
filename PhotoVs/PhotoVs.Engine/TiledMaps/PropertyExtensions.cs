using System.Collections.Generic;

namespace PhotoVs.Engine.TiledMaps
{
    public static class PropertyExtensions
    {
        public static string GetValue(this Dictionary<string, string> properties, string key)
        {
            return properties?.ContainsKey(key) == true ? properties[key] : null;
        }
    }
}