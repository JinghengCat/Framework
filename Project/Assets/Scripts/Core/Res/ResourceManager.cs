using System.Collections.Generic;
using Core.Common;
using UnityEngine;

namespace Core.Res
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<string, AAssetHolder> _cacheAssetHolder = new Dictionary<string, AAssetHolder>();
        
        public void OnInit()
        {
        
        }

        public void OnRelease()
        {
        
        }

        
        public void TryDestroyAssetHolder(AAssetHolder holder)
        {
            if (holder.refCount != 0)
            {
                return;
            }

            string assetPath = holder.assetInfo.assetPath;
            if (_cacheAssetHolder.ContainsKey(assetPath))
            {
                _cacheAssetHolder.Remove(assetPath);
            }
        }

        public void Test_DestroyAll()
        {
            foreach (var holder in _cacheAssetHolder.Values)
            {
                holder.FreeRawAsset();   
            }
            
            _cacheAssetHolder.Clear();
        }

        public AAssetHolder GetOrCreateAssetHolder(AssetInfo assetInfo)
        {
            bool hasHolder = _cacheAssetHolder.TryGetValue(assetInfo.assetPath, out var assetHolder);
            if (!hasHolder)
            {
                assetHolder = AssetLoaderFactory.DynamicCreateAssetHolder(assetInfo);
                _cacheAssetHolder.Add(assetInfo.assetPath, assetHolder);
            }

            return assetHolder;
        }

        public AAssetHolder GetAssetHolder(string assetPath)
        {
            if (_cacheAssetHolder.TryGetValue(assetPath, out var assetHolder))
            {
                return assetHolder;
            }

            return null;
        }
    }
}
