using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Player;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Serilog;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class LocalPlayerDeath : LocalPlayerAbility, IController
    {
        public FeedbacksManager DeathFeedbacks;

        private INetworkSystem _network;

        public override void OnStateInit()
        {
            _network = this.GetSystem<INetworkSystem>();
        }

        public override void OnStateEnter()
        {
            DeathFeedbacks?.Play();
            Revive();
        }

        private async void Revive()
        {
            _network.SendToServer(new ReviveRequest());

            var response = await _network.ReceiveAsync<ReviveResponse>();
            if (response.Error != NetError.Success)
            {
                Log.Error($"复活失败, 原因:{response.Error}");
                return;
            }
            Log.Information("复活成功");
            OwnerState.Brain.ChangeStateByName("Idle");
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
