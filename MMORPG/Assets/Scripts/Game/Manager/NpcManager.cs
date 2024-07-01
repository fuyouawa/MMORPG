using System;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Map;
using MMORPG.Event;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using Serilog;
using UnityEngine;
using MMORPG.Common.Proto.Npc;

namespace MMORPG.Game
{


    public class NpcManager : MonoBehaviour, IController, ICanSendEvent
    {
        private IEntityManagerSystem _entityManager;
        private IDataManagerSystem _dataManager;
        private INetworkSystem _network;

        private void Awake()
        {
            _entityManager = this.GetSystem<IEntityManagerSystem>();
            _dataManager = this.GetSystem<IDataManagerSystem>();
            _network = this.GetSystem<INetworkSystem>();

            _network.Receive<QueryDialogueIdResponse>(OnQueryDialogueIdReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnQueryDialogueIdReceived(QueryDialogueIdResponse response)
        {
            
        }


        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
