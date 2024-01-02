using System;
using UnityEngine;

namespace Core.Res
{
    public static class LoadAssetUtils
    {
        public static AssetBundleCreateRequest LoadAssetBundle(string path)
        {
            AssetBundleCreateRequest createRequest = UnityEngine.AssetBundle.LoadFromFileAsync(path);
            return createRequest;
        }
        
        
        public static UnityEngine.Object LoadAsset_Editor(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
#else
            return null;
#endif
        }


        
    }
}
