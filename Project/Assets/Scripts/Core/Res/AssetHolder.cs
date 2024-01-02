using System;
using System.Collections;
using Core.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Res
{
    public abstract class AAssetHolder
    {
        public UnityEngine.Object asset;
        public int refCount;
        public AssetInfo assetInfo;
        public ELoadState loadState = ELoadState.None;
        public int loadErrorCode = ELoadAssetErrorCode.SUCCESS;

        private event Action<UnityEngine.Object, int> _callback;

        public AAssetHolder(AssetInfo assetInfo)
        {
            this.assetInfo = assetInfo;
        }
        
        protected void RegisterCallback(Action<UnityEngine.Object, int> callback)
        {
            this._callback += callback;
        }

        protected void ClearCallback()
        {
            this._callback = null;
        }

        protected void InvokeCallback()
        {
            var callback = this._callback;
            this._callback = null;
            
            callback?.Invoke(asset, loadErrorCode);
        }

        public abstract void LoadRawAsset(Action<UnityEngine.Object, int> callback);
        public abstract void FreeRawAsset();
    }

    public class AssetBundleHolder : AAssetHolder
    {
        
        private Coroutine _loadCoroutine;
        private AssetBundleCreateRequest _createRequest;
        
        public AssetBundleHolder(AssetInfo assetInfo) : base(assetInfo)
        {
        }

        IEnumerator LoadAssetBundleAsync()
        {
            // TODO 替换成接口和配置
            string bundlePath = $"{Application.dataPath}/../Build/{this.assetInfo.bundlePath}";
            _createRequest = UnityEngine.AssetBundle.LoadFromFileAsync(bundlePath);
            yield return _createRequest;

            this.asset = _createRequest.assetBundle;
            _loadCoroutine = null;
            _createRequest = null;
            loadState = ELoadState.Loaded;
            InvokeCallback();
        }
        
        public override void LoadRawAsset(Action<UnityEngine.Object, int> callback)
        {
            RegisterCallback(callback);
            loadState = ELoadState.Loading;
            _loadCoroutine = CoroutineRunner.Instance.StartGlobalCoroutine(LoadAssetBundleAsync());
        }

        public override void FreeRawAsset()
        {
            ClearCallback();
            loadState = ELoadState.None;
            if (_loadCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_loadCoroutine);
                _loadCoroutine = null;
            }

            if (_createRequest != null)
            {
                _createRequest.assetBundle.Unload(true);
                _createRequest = null;
            }

            if (asset != null)
            {
                var ab = asset as AssetBundle;
                if (ab != null)
                    ab.Unload(true);
                
                asset = null;
            }
        }
    }

    public class RawAssetHolder : AAssetHolder
    {
        private Coroutine _loadCoroutine;
        
        public RawAssetHolder(AssetInfo assetInfo) : base(assetInfo)
        {
        }

        IEnumerator LoadAssetAsync(AssetBundle bundle, string name)
        {
            var request = bundle.LoadAssetAsync<UnityEngine.Object>(name);
            yield return request;

            _loadCoroutine = null;
            asset = request.asset;
            loadState = ELoadState.Loaded;
            InvokeCallback();
            yield break;
        }
        
        public override void LoadRawAsset(Action<Object, int> callback)
        {
            RegisterCallback(callback);
            var bundleHolder = ResourceManager.Instance.GetAssetHolder(assetInfo.bundlePath);
            var bundle = (bundleHolder?.asset as AssetBundle);
            if (bundle == null)
            {
                Logger.Error("Load Raw Asset Failed");
                InvokeCallback();
                return;
            }
            loadState = ELoadState.Loading;
            _loadCoroutine = CoroutineRunner.Instance.StartGlobalCoroutine(LoadAssetAsync(bundle, "Cube.prefab"));
        }

        public override void FreeRawAsset()
        {
            loadState = ELoadState.None;
            if (_loadCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_loadCoroutine);
                _loadCoroutine = null;
            }

            asset = null;
        }
    }
}
