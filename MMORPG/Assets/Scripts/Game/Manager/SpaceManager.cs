using Common.Proto.Player;
using QFramework;
using Tool;
using UnityEngine;


namespace MMORPG
{
    /// <summary>
    /// 地图控制器
    /// 负责监听在当前地图中创建角色的事件并将角色加入到地图
    /// </summary>
    public class SpaceManager : MonoBehaviour, IController, ICanSendEvent
    {
        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        async void Start()
        {
            var box = this.GetSystem<IBoxSystem>();
            var net = this.GetSystem<INetworkSystem>();
            box.ShowSpinner("");
            net.SendToServer(new EnterGameRequest
            {
                CharacterId = 1,
            });
            var response = await net.ReceiveAsync<EnterGameResponse>();
            box.CloseSpinner();
            this.GetSystem<IPlayerManagerSystem>().SetMineId(response.Character.Entity.EntityId);
        }
    }
}