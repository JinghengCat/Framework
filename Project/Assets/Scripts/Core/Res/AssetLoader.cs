using System;
using System.Collections;
using System.Collections.Generic;
using Core.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Res
{
    public abstract class AAssetLoader
    {
        public AssetInfo assetInfo { get; private set; }
        public ELoadState loadState
        {
            get => _assetHolder != null ? _assetHolder.loadState : ELoadState.None;
        }

        protected Action<UnityEngine.Object, int> _callback;
        protected AAssetHolder _assetHolder;
        protected Dictionary<string, AAssetLoader> _dependentAssetLoaders;

        public AAssetLoader(AssetInfo assetInfo)
        {
            Free();
            this.assetInfo = assetInfo;
            BuildDependencies();
        }
        
        public void Load(Action<UnityEngine.Object, int> callback)
        {
            if (_assetHolder != null)
            {
                Logger.Warn("this loader has already called load function");
                return;
            }

            this._callback = callback;
            _assetHolder = ResourceManager.Instance.GetOrCreateAssetHolder(this.assetInfo);
            
            if (_dependentAssetLoaders != null && _dependentAssetLoaders.Count > 0)
            {
                int currCount = 0;
                int maxCount = _dependentAssetLoaders.Count;

                Action<UnityEngine.Object, int> counterCallback = (o, i) =>
                {
                    currCount += 1;
                    if (currCount >= maxCount)
                    {
                        LoadImpl();
                    }
                };
                foreach (var loader in _dependentAssetLoaders.Values)
                {
                    loader.Load(counterCallback);
                }
            }
            else
            {
                LoadImpl();
            }
        }
        
        public void Free()
        {
            this._callback = null;
            FreeImpl();
            FreeDependentLoaders();
        }

        protected void InvokeCallback(UnityEngine.Object obj, int errCode)
        {
            var callback = this._callback;
            this._callback = null;
            callback?.Invoke(obj, errCode);
        }
        
        protected virtual void OnRawAssetLoaded(UnityEngine.Object obj, int errCode)
        {
            InvokeCallback(obj, errCode);
        }

        protected virtual void LoadImpl()
        {
            _assetHolder.refCount += 1;
            if (_assetHolder.asset != null)
            {
                OnRawAssetLoaded(_assetHolder.asset, _assetHolder.loadErrorCode);
                return;
            }
            
            _assetHolder.LoadRawAsset(OnRawAssetLoaded);
        }

        protected virtual void FreeImpl()
        {
            _assetHolder.refCount -= 1;
            _callback = null;
            _assetHolder = null;
        }
        
        protected virtual void BuildDependencies()
        {
// #if !UNITY_EDITOR
            if (_dependentAssetLoaders == null)
            {
                _dependentAssetLoaders = new Dictionary<string, AAssetLoader>(assetInfo.dependencies.Count);
            }
            else
            {
                FreeDependentLoaders();
            }
            
            // TODO 检查是否循环依赖
            
            foreach (var iterateAssetInfo in assetInfo.dependencies)
            {
                string assetPath = iterateAssetInfo.assetPath;
                if (!_dependentAssetLoaders.ContainsKey(assetPath))
                {
                    var assetLoader = AssetLoaderFactory.DynamicCreateAssetLoader(iterateAssetInfo);
                    if (assetLoader != null)
                    {
                        _dependentAssetLoaders.Add(assetPath, assetLoader);
                    }
                }
            }
// #endif
        }
        
        protected void FreeDependentLoaders()
        {
// #if !UNITY_EDITOR
            if (_dependentAssetLoaders == null)
            {
                return;
            }
            
            foreach (var assetLoader in _dependentAssetLoaders.Values)
            {
                assetLoader.Free();
            }
// #endif
        }
    }

    public class AssetBundleLoader : AAssetLoader
    {
        public AssetBundleLoader(AssetInfo assetInfo) : base(assetInfo)
        {
        }
    }

    public class GameObjLoader : AAssetLoader
    {
        private GameObject _instantiateGameObj;
        public GameObjLoader(AssetInfo assetInfo) : base(assetInfo)
        {
        }

        protected override void OnRawAssetLoaded(Object obj, int errCode)
        {
            GameObject go = obj as GameObject;
            if (go != null)
            {
                _instantiateGameObj = GameObject.Instantiate(go);
                GameObject.DontDestroyOnLoad(_instantiateGameObj);
            }
            
            InvokeCallback(_instantiateGameObj, errCode);
        }

        protected override void LoadImpl()
        {
            base.LoadImpl();
        }

        protected override void FreeImpl()
        {
            if (_instantiateGameObj != null)
            {
                GameObject.Destroy(_instantiateGameObj);
                _instantiateGameObj = null;
            }

            base.FreeImpl();
        }
    }
}
