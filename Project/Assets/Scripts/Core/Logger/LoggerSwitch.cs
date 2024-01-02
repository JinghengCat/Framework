using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerSwitch : MonoBehaviour
{
    public bool enableInfoLog = true;

    private void Awake()
    {
        OnInitLogger();
    }

    private void OnInitLogger()
    {
        Logger.OnInitLogger(enableInfoLog);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        OnInitLogger();
    }
#endif

}
