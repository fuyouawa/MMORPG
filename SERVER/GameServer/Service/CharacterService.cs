using Common.Network;
using Common.Proto.Base;
using Common.Proto.Character;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class CharacterService : ServiceBase<CharacterService>
    {
        private static readonly object _characterCreateLock = new();

        public void OnConnect(NetChannel sender)
        {
        }

        public void OnChannelClosed(NetChannel sender)
        {
        }


        public void OnHandle(NetChannel sender, CharacterCreateRequest request)
        {
            Log.Debug($"{sender.ChannelName}角色创建请求");
            if (sender.User == null)
            {
                Log.Debug($"{sender.ChannelName}角色创建失败：用户未登录");
                return;
            }
            var count = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.UserId.Equals(sender.User.UserId))
                .Count();
            if (count >= 4)
            {
                sender.Send(new CharacterCreateResponse() { Error = NetError.CharacterCreationLimitReached });
                Log.Debug($"{sender.ChannelName}角色创建失败：创建的角色已满");
                return;
            }
            if (!StringHelper.NameVerify(request.Name))
            {
                Log.Debug($"{sender.ChannelName}角色创建失败：角色名称非法");
                sender.Send(new CharacterCreateResponse() { Error = NetError.IllegalCharacterName });
                return;
            }
            lock (_characterCreateLock)
            {
                var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                    .Where(p => p.Name == request.Name)
                    .First();
                if (dbCharacter != null)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.RepeatCharacterName });
                    Log.Debug($"{sender.ChannelName}角色创建失败：角色名已存在");
                    return;
                }

                var newDbCharacter = new DbCharacter(request.Name, sender.User.UserId, request.UnitId, MapManager.Instance.InitMapId, 100, 100, 1,
                    0, 0);
                var insertCount = SqlDb.Connection.Insert(newDbCharacter).ExecuteAffrows();
                if (insertCount <= 0)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.UnknowError });
                    Log.Debug($"{sender.ChannelName}角色创建失败：数据库错误");
                    return;
                }
                sender.Send(new CharacterCreateResponse() { Error = NetError.Success });
                Log.Debug($"{sender.ChannelName}角色创建成功");
            }
        }

        public void OnHandle(NetChannel sender, CharacterListRequest request)
        {
            Log.Debug($"{sender.ChannelName}角色列表查询请求");
            if (sender.User == null)
            {
                Log.Debug($"{sender.ChannelName}角色列表查询失败：用户未登录");
                return;
            }
            var characterList = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.UserId.Equals(sender.User.UserId))
                .ToList();
            var res = new CharacterListResponse();
            foreach (var character in characterList)
            {
                res.CharacterList.Add(new NetCharacter()
                {
                    CharacterId = character.Id,
                    Name = character.Name,
                    UnitId = character.UnitId,
                    Level = character.Level,
                    Exp = character.Exp,
                    MapId = character.MapId,
                    Gold = character.Gold,
                });
            }
            sender.Send(res, null);
            Log.Debug($"{sender.ChannelName}角色列表查询成功");
        }

        public void OnHandle(NetChannel sender, CharacterDeleteRequest request)
        {
            Log.Debug($"{sender.ChannelName}角色删除请求");
            if (sender.User == null)
            {
                Log.Debug($"{sender.ChannelName}角色删除失败：用户未登录");
                return;
            }
            var deleteCount = SqlDb.Connection.Delete<DbCharacter>()
                .Where(t => t.UserId.Equals(sender.User.UserId))
                .Where(t => t.Id == request.CharacterId)
                .ExecuteAffrows();
            sender.Send(new CharacterDeleteResponse() { Error = NetError.Success });
            Log.Debug($"{sender.ChannelName}角色删除成功");
        }

    }
}
