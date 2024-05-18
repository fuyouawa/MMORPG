using QFramework;
using MMORPG.Model;
using MMORPG.System;

namespace MMORPG.Game
{
    public class GameApp : Architecture<GameApp>
    {
        protected override void Init()
        {
            this.RegisterSystem<IBoxSystem>(new BoxSystem());
            this.RegisterSystem<IDataManagerSystem>(new DataManagerSystem());
            this.RegisterSystem<IEntityManagerSystem>(new EntityManagerSystem());
            this.RegisterSystem<IMapManagerSystem>(new MapManagerSystem());
            this.RegisterSystem<INetworkSystem>(new NetworkSystem());
            this.RegisterSystem<IPlayerManagerSystem>(new PlayerManagerSystem());
            this.RegisterModel<IUserModel>(new UserModel());
        }
    }
}
