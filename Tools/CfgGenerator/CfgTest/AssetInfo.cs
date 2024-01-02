using System.Collections.Generic;

namespace Asset
{
    public class AssetInfoInstance
    {
        public readonly string assetPath;
        public readonly string bundlePath;
        public readonly int assetType;
        
        
        public AssetInfoInstance(string assetPath, string bundlePath, int assetType)
        {
            this.assetPath = assetPath;
            this.bundlePath = bundlePath;
            this.assetType = assetType;
            
        }
    }
    
    public static class AssetInfo
    {
        private static Dictionary<string, AssetInfoInstance> _dictionary = new Dictionary<string, AssetInfoInstance>()
        {
            {"AssetBundle/TestPrefab/Cube.prefab", new AssetInfoInstance("AssetBundle/TestPrefab/Cube.prefab", "", 0)},
            
        };
        
        public static AssetInfoInstance Get(string key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}