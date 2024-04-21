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
    public int CharacterId { get; }
    public Player MinePlayer { get; }

    public void SetMineId(int entityId);
    public void SetCharacterId(int characterId);

    public void RegisterPlayer(Player player, int entityId);
    public void UnregisterPlayer(Player player);

    public Player GetPlayerById(int entityId);
    public bool TryGetPlayerById(int entityId, out Player player);
}

public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
{
    private int _mineId = -1;
    private int _characterId = -1;
    private Player _minePlayer = null;
    private Dictionary<int, Player> _playerDict = new();

    public Player MinePlayer => _minePlayer.AssertNotNull();

    public int CharacterId => _characterId.AssertNotEqual(-1);

    int IPlayerManagerSystem.MineId => _mineId.AssertNotEqual(-1);

    public Player GetPlayerById(int entityId)
    {
        return _playerDict[entityId];
    }

    public void RegisterPlayer(Player player, int entityId)
    {
        Debug.Assert(!_playerDict.ContainsKey(entityId));
        player.Entity.EntityId = entityId;
        _playerDict.Add(entityId, player);
        Logger.Info($"[Game]玩家生成: Id:{entityId}");
        this.SendEvent(new PlayerJoinedEvent(player));
    }

    public void SetCharacterId(int characterId)
    {
        _characterId = characterId;
    }

    public void SetMineId(int entityId)
    {
        _mineId = entityId;
    }

    public bool TryGetPlayerById(int entityId, out Player player)
    {
        return _playerDict.TryGetValue(entityId, out player);
    }

    public void UnregisterPlayer(Player player)
    {
        _playerDict.Remove(player.Entity.EntityId);
    }

    protected override void OnInit()
    {
    }
}