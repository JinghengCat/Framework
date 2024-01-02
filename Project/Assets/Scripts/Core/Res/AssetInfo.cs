using System.Collections.Generic;

namespace Core.Res
{
    public class AssetInfo
    {
        public string assetPath;
        public int assetType;
        public string bundlePath;
        public List<AssetInfo> dependencies = new List<AssetInfo>();
    }
}