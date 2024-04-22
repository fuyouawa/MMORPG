using MMORPG;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBehaviour : MonoBehaviour, IController
{
    protected INetworkSystem _network;

    private float _networkSyncDeltaTime;

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    protected virtual void Awake()
    {
        _network = this.GetSystem<INetworkSystem>();
        var config = this.GetModel<IGameConfigModel>();
        config.NetworkSyncDeltaTime.RegisterWithInitValue(time => _networkSyncDeltaTime = time)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    protected virtual void Start()
    {
        StartCoroutine(RunFixedUpdateNetwork());
    }

    private IEnumerator RunFixedUpdateNetwork()
    {
        while (true)
        {
            FixedUpdateNetwork(_networkSyncDeltaTime);
            yield return new WaitForSeconds(_networkSyncDeltaTime);
        }
    }

    protected virtual void FixedUpdateNetwork(float deltaTime)
    {

    }
}
