using System;
using System.Collections;
using System.Collections.Generic;
using Core.Common;
using Core.Res;
using UnityEngine;

public class MyTest : MonoBehaviour
{
    private void Awake()
    {

    }

    IEnumerator FreeTest(AAssetLoader loader)
    {
        yield return new WaitForSeconds(5);
        loader.Free();
    }
}
