using System.Collections.Generic;

namespace ProjectCfg.Asset
{
    public class AssetInfo
    {
        public readonly string assetPath;
        public readonly string bundlePath;
        public readonly int assetType;
        public readonly int assetSource;
        
        
        public AssetInfo(string assetPath, string bundlePath, int assetType, int assetSource)
        {
            this.assetPath = assetPath;
            this.bundlePath = bundlePath;
            this.assetType = assetType;
            this.assetSource = assetSource;
            
        }
    }
    
    public static class AssetInfoCfg
    {
        private static Dictionary<string, AssetInfo> _dictionary = new Dictionary<string, AssetInfo>()
        {
            {"AssetBundle/TestPrefab/Cube.prefab", new AssetInfo("AssetBundle/TestPrefab/Cube.prefab", "", 0, 0)},
            
        };
        
        public static AssetInfo Get(string key)
        {
            if (_dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}