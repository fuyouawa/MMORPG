using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IMapManagerSystem : ISystem
{
    public int MapId { get; }

    public void JoinedMap(int mapId);
}

public class MapManagerSystem : AbstractSystem, IMapManagerSystem
{
    private int _mapId = -1;
    public int MapId => _mapId;

    public void JoinedMap(int mapId)
    {
        Debug.Assert(_mapId == -1);
        Logger.Info("Game", $"加入地图:{mapId}");
        _mapId = mapId;
        this.SendEvent(new JoinedMapEvent(mapId));
    }

    protected override void OnInit()
    {
    }
}