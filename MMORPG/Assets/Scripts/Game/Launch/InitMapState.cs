using MMORPG;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitedMapCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new InitedMapEvent());
    }
}

public class InitedMapEvent { }

public class InitMapState : AbstractState<LaunchStatus, LaunchController>, IController
{
    public InitMapState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
    {
    }

    protected override void OnEnter()
    {
        Logger.Info("Launch", "初始化地图");
        var group = new GameObject("Manager(Auto Create)").transform;
        new GameObject(nameof(MapManager)).AddComponent<MapManager>().transform.SetParent(group);
        new GameObject(nameof(EntityManager)).AddComponent<EntityManager>().transform.SetParent(group);
        new GameObject(nameof(PlayerManager)).AddComponent<PlayerManager>().transform.SetParent(group);
        this.SendCommand(new InitedMapCommand());
    }

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }
}
