using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private T _instance;
    public T Instance { get { Debug.Assert(_instance != null); return _instance; } }

    protected virtual void Awake()
    {
        Debug.Assert(_instance == null);
        _instance = (T)this;
    }
}
