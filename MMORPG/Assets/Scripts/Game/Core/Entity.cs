using Common.Proto.Event.Space;
using MMORPG;
using QFramework;
using System.Collections;
using Tool;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public class Entity : MonoBehaviour, IController
{
    [SerializeField]
    [ReadOnly]
    private int _entityId;
    [SerializeField]
    [ReadOnly]
    private bool _isMine;
    public bool AutoUpdate = true;

    public INetworkEntityCallbacks[] _allCallbacks { get; private set; }

    public int EntityId => _entityId;

    public bool IsMine => _isMine;

    private INetworkSystem _network;

    protected void Awake()
    {
        _network = this.GetSystem<INetworkSystem>();
        _allCallbacks = GetComponents<INetworkEntityCallbacks>();
        StartCoroutine(NetworkFixedUpdate());
    }

    public void SetEntityId(int entityId)
    {
        _entityId = entityId;
    }

    public void SetIsMine(bool isMine)
    {
        _isMine = isMine;
    }

    public void NetworkUpdatePositionAndRotation()
    {
        Debug.Assert(IsMine);
        _network.SendToServer(new EntitySyncRequest()
        {
            EntitySync = new()
            {
                Entity = new()
                {
                    EntityId = EntityId,
                    Position = transform.position.ToNetVector3(),
                    Direction = transform.rotation.eulerAngles.ToNetVector3()
                }
            }
        });
    }

    private IEnumerator NetworkFixedUpdate()
    {
        var config = this.GetModel<IConfigModel>().GameConfig;
        var data = new NetworkControlData()
        {
            DeltaTime = config.NetworkSyncDeltaTime,
        };
        while (true)
        {
            _allCallbacks.ForEach(cb =>
            {
                if (IsMine)
                    cb.NetworkControlFixedUpdate(data);
            });
            if (AutoUpdate)
                NetworkUpdatePositionAndRotation();
            yield return new WaitForSeconds(data.DeltaTime);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
