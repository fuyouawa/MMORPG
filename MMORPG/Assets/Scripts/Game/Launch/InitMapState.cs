using MMORPG;
using QFramework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InitMapState : AbstractState<LaunchStatus, LaunchController>
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
    }
}
