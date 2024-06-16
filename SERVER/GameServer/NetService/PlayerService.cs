﻿using MMORPG.Common.Network;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using MMORPG.Common.Proto.Player;
using GameServer.Db;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Entity;
using GameServer.MapSystem;
using MMORPG.Common.Proto.Npc;

namespace GameServer.NetService
{
    public class PlayerService : ServiceBase<PlayerService>
    {
        public void OnConnect(NetChannel sender)
        {
        }

        public void OnChannelClosed(NetChannel sender)
        {
        }

        public void OnHandle(NetChannel sender, JoinMapRequest request)
        {
            Log.Debug($"{sender}进入游戏请求");
            if (sender.User == null)
            {
                Log.Debug($"{sender}进入游戏失败：用户未登录");
                return;
            }

            if (sender.User.Player != null)
            {
                Log.Debug($"{sender}进入游戏失败：重复进入");
                return;
            }
            var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.UserId == sender.User.UserId)
                .Where(t => t.Id == request.CharacterId)
                .First();
            if (dbCharacter == null)
            {
                sender.Send(new JoinMapResponse() { Error = NetError.InvalidCharacter });
                Log.Debug($"{sender}进入游戏失败：数据库中不存在指定的角色");
                return;
            }
            var map = MapManager.Instance.GetMapById(dbCharacter.MapId);
            if (map == null)
            {
                sender.Send(new JoinMapResponse() { Error = NetError.InvalidMap });
                Log.Debug($"{sender}进入游戏失败：指定的地图不存在");
                return;
            }
            var pos = new Vector3()
            {
                X = dbCharacter.X,
                Y = dbCharacter.Y,
                Z = dbCharacter.Z,
            };
            var player = map.PlayerManager.NewPlayer(sender.User, dbCharacter, pos, Vector3.Zero);
            sender.User.SetPlayer(player);
            var res = new JoinMapResponse()
            {
                Error = NetError.Success,
                EntityId = player.EntityId,
                MapId = dbCharacter.MapId,
                UnitId = dbCharacter.UnitId,
                Transform = ProtoHelper.ToNetTransform(player.Position, player.Direction),
            };
            sender.Send(res, null);
            Log.Debug($"{sender}进入游戏成功");
        }
    }
}