using QFramework;
using UnityEngine;


/// <summary>
/// 地图控制器
/// 负责监听在当前地图中创建角色的事件并将角色加入到地图
/// </summary>
public class SpaceController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    void Start()
    {
        this.RegisterEvent<CharacterEnterEvent>(e =>
        {
            SceneHelperController.Instance.Invoke(() =>
            {
                var playerGo = Instantiate(Resources.Load<GameObject>("Prefabs/Character/Player/DogPBR"),
                e.Player.Position, e.Player.Rotation);
                playerGo.GetComponent<NetCharacterController>().NetPlayer = e.Player;
            });
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }
}
