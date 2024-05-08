using Google.Protobuf;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public interface IPlayerManagerSystem : ISystem
{
    public int MineId { get; }
    public int CharacterId { get; }
    public void SetMineId(int entityId);
}

public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
{
    private readonly Dictionary<int, EntityView> _playerDict = new();

    public int CharacterId { get; private set; } = -1;
    public int MineId { get; private set; } = -1;


    public void SetMineId(int entityId)
    {
        MineId = entityId;
    }


    protected override void OnInit()
    {
        this.RegisterEvent<EntityEnterEvent>(OnEntityEnter);
        this.RegisterEvent<ApplicationQuitEvent>(OnApplicationQuit);
    }

    private void OnApplicationQuit(ApplicationQuitEvent e)
    {
        MineId = -1;
        CharacterId = -1;
        _playerDict.Clear();
    }

    private void OnEntityEnter(EntityEnterEvent e)
    {
        Debug.Assert(!_playerDict.ContainsKey(e.Entity.EntityId));
        _playerDict[e.Entity.EntityId] = e.Entity;
    }
}
