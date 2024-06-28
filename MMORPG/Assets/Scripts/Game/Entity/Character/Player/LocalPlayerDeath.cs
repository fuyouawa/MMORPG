using System.Collections;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Player;
using MMORPG.System;
using MMORPG.Tool;
using QFramework;
using Serilog;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
    public class LocalPlayerDeath : LocalPlayerAbility, IController
    {
        public FeedbacksManager DeathFeedbacks;
        public FeedbacksManager ReviveFeedbacks;

        private INetworkSystem _network;

        public override void OnStateInit()
        {
            _network = this.GetSystem<INetworkSystem>();
        }

        public override void OnStateEnter()
        {
            DeathFeedbacks?.Play();
            OwnerState.Brain.ActorController.Animator.SetTrigger("Die");
            OwnerState.Brain.ActorController.Animator.SetBool("Death", true);
            StartCoroutine("ReviveCo");
        }

        public override void OnStateExit()
        {
            StopCoroutine("ReviveCo");
        }

        private IEnumerator ReviveCo()
        {
            yield return new WaitForSeconds(3);
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
            ReviveFeedbacks?.Play();
            OwnerState.Brain.ActorController.Animator.SetBool("Death", false);
            OwnerState.Brain.ChangeStateByName("Idle");
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
