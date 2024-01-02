using System;
using System.Collections;
using UnityEngine;

namespace Core.Common
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                
                return _instance;
            }
        }

        public Coroutine StartGlobalCoroutine(IEnumerator coroutineFunction)
        {
            return StartCoroutine(coroutineFunction);
        }

        public void StopGlobalCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
    }
}
