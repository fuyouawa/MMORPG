using QFramework;
using MMORPG.Model;
using MMORPG.System;
using Serilog;
using UnityEngine;

namespace MMORPG.Game
{
    public class GameApp : Architecture<GameApp>
    {
        protected override void Init()
        {
            Debug.Log("[Init Architecture] GameApp");
            this.RegisterSystem<IBoxSystem>(new BoxSystem());
            this.RegisterSystem<IDataManagerSystem>(new DataManagerSystem());
            this.RegisterSystem<IEntityManagerSystem>(new EntityManagerSystem());
            this.RegisterSystem<INetworkSystem>(new NetworkSystem());
            this.RegisterSystem<IPlayerManagerSystem>(new PlayerManagerSystem());
            this.RegisterModel<IUserModel>(new UserModel());
            this.RegisterModel<IMapModel>(new MapModel());
        }

        protected override void OnDeinit()
        {
            Debug.Log("[DeInit Architecture] GameApp");
        }
    }
}
