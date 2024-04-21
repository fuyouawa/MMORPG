using QFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMapManagerSystem : ISystem
{
    public int CurrentMapId { get; }

    public void JoinedMap(int mapId);
}

public class MapManagerSystem : AbstractSystem, IMapManagerSystem
{
    private int _currentMapId = -1;
    public int CurrentMapId => _currentMapId.AssertNotEqual(-1);

    public void JoinedMap(int mapId)
    {
        Logger.Info($"[Game]加入地图:{mapId}");
        _currentMapId.AssertEqual(-1);
        _currentMapId = mapId;
        this.SendEvent(new JoinedMapEvent(mapId));
    }

    protected override void OnInit()
    {
    }
}