using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        Debug.Assert(_instance == null);
        _instance = (T)this;
    }


    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
