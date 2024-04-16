using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 角色系统
/// 负责管理所有玩家的角色
/// 记录角色信息，包括位置、属性等
/// CharacterSystem通过发送CharacterPositionChangeEvent事件表示角色位置更新完毕
/// </summary>
public class CharacterSystem : AbstractSystem
{
    private Dictionary<int, NetPlayer> _playerSet;

    protected override void OnInit()
    {
        _playerSet = new();

        this.RegisterEvent<CharacterEnterEvent>(e =>
        {
           AddPlayer(e.Player);
        });
    }

    public void PositionChange(NetPlayer player, Vector3 position, Quaternion rotation)
    {
        player.Position = position;
        player.Rotation = rotation;
        // this.SendEvent(new CharacterPositionChangeEvent() { Player = player });
    }

    public NetPlayer GetPlayer(int entityId)
    {
        lock (_playerSet)
        {
            return _playerSet[entityId];
        }
    }

    public void AddPlayer(NetPlayer player)
    {
        lock (_playerSet)
        {
            _playerSet[player.EntityId] = player;
        }
    }
}
