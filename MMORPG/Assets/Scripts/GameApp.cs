using MMORPG;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameApp : Architecture<GameApp>
{
    protected override void Init()
    {
        this.RegisterSystem<IBoxSystem>(new BoxSystem());
        this.RegisterSystem<IEntityManagerSystem>(new EntityManagerSystem());
        this.RegisterSystem<IMapManagerSystem>(new MapManagerSystem());
        this.RegisterSystem<INetworkSystem>(new NetworkSystem());
        this.RegisterSystem<IPlayerManagerSystem>(new PlayerManagerSystem());
        this.RegisterModel<IUserModel>(new UserModel());
        this.RegisterModel<IConfigModel>(new ConfigModel());
    }
}
