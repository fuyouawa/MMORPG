using Common.Proto.Event;
using Common.Proto.Event.Map;
using MMORPG;
using QFramework;
using System.Collections;
using Tool;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


public sealed class Entity : MonoBehaviour, IController
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

    private void Awake()
    {
        _network = this.GetSystem<INetworkSystem>();
        _allCallbacks = GetComponents<INetworkEntityCallbacks>();
    }

    private void Start()
    {
        StartCoroutine(NetworkFixedUpdate());
    }

    private void Update()
    {
        _allCallbacks.ForEach(cb => cb.NetworkMineUpdate());
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
        _network.SendToServer(new EntityTransformSyncRequest()
        {
            EntityId = EntityId,
            Transform = new()
            {
                Position = transform.position.ToNetVector3(),
                Direction = transform.rotation.eulerAngles.ToNetVector3()
            }
        });
    }

    private IEnumerator NetworkFixedUpdate()
    {
        var deltaTime = Config.GameConfig.NetworkSyncDeltaTime;
        while (true)
        {
            _allCallbacks.ForEach(cb =>
            {
                if (IsMine)
                    cb.NetworkMineFixedUpdate();
            });
            if (AutoUpdate)
                NetworkUpdatePositionAndRotation();
            yield return new WaitForSeconds(deltaTime);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
