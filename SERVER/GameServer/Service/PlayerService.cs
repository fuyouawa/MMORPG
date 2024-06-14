using MMORPG.Common.Network;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using MMORPG.Common.Proto.Player;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Common.Proto.Entity;

namespace GameServer.Service
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

        public void OnHandle(NetChannel sender, InteractRequest req)
        {
            if (sender.User == null || sender.User.Player == null) return;
            var player = sender.User.Player;
            // 查找距离最近的Npc

            var entity = player.Map.GetEntityFollowingNearest(player, entity => entity.EntityType == EntityType.Npc);

            var res = new InteractResponse()
            {
                Error = NetError.InvalidEntity,
            };
            do
            {
                if (entity == null) break;
                var npc = entity as Npc;
                if (npc == null) break;
                var distance = Vector2.Distance(player.Position.ToVector2(), npc.Position.ToVector2());
                if (distance > 1) break;
                res.Error = NetError.Success;
                res.EntityId = entity.EntityId;
                res.DialogueId = ;
            } while (false);
            sender.Send(res, null);
        }
    }
}
