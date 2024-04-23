using Google.Protobuf;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPlayerManagerSystem : ISystem
{
    public int MineId { get; }
    public Player MinePlayer { get; }

    public void SetMineId(int entityId);
}

public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
{
    private int _mineId = -1;
    private int _characterId = -1;
    private Player _minePlayer = null;
    private Dictionary<int, Entity> _playerDict = new();

    public Player MinePlayer => _minePlayer;

    public int CharacterId => _characterId;

    int IPlayerManagerSystem.MineId => _mineId;


    public void SetMineId(int entityId)
    {
        _mineId = entityId;
    }


    protected override void OnInit()
    {
        this.RegisterEvent<EntityEnterEvent>(OnEntityEnter);
    }

    private void OnEntityEnter(EntityEnterEvent e)
    {
        Debug.Assert(!_playerDict.ContainsKey(e.Entity.EntityId));
        _playerDict[e.Entity.EntityId] = e.Entity;
    }
}