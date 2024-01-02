using UnityEngine;

namespace Core.Res
{
    public static class AssetLoaderFactory
    {
        public static AAssetLoader DynamicCreateAssetLoader(AssetInfo assetInfo)
        {
            AAssetLoader loader = null;
            var assetType = assetInfo.assetType;
            switch (assetType)
            {
                case EAssetType.AssetBundle:
                {
                    loader = new AssetBundleLoader(assetInfo);
                    break;
                }
                case EAssetType.Prefab:
                {
                    loader = new GameObjLoader(assetInfo);
                    break;
                }
            }

            return loader;
        }




        public static AAssetHolder DynamicCreateAssetHolder(AssetInfo assetInfo)
        {
            AAssetHolder holder = null;
            var assetType = assetInfo.assetType;
            switch (assetType)
            {
                case EAssetType.AssetBundle:
                {
                    holder = new AssetBundleHolder(assetInfo);
                    break;
                }
                case EAssetType.Prefab:
                {
                    holder = new RawAssetHolder(assetInfo);
                    break;
                }
            }

            return holder;
        }
    }
}
