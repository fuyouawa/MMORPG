using MMORPG.Common.Network;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using GameServer.Db;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.MapSystem;
using GameServer.Manager;

namespace GameServer.NetService
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
            UpdateManager.Instance.AddTask(() =>
            {
                Log.Debug($"{sender}角色创建请求");
                if (sender.User == null)
                {
                    Log.Debug($"{sender}角色创建失败：用户未登录");
                    return;
                }
                var count = SqlDb.Connection.Select<DbCharacter>()
                    .Where(t => t.UserId.Equals(sender.User.UserId))
                    .Count();
                if (count >= 4)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.CharacterCreationLimitReached });
                    Log.Debug($"{sender}角色创建失败：创建的角色已满");
                    return;
                }
                if (!StringHelper.NameVerify(request.Name))
                {
                    Log.Debug($"{sender}角色创建失败：角色名称非法");
                    sender.Send(new CharacterCreateResponse() { Error = NetError.IllegalCharacterName });
                    return;
                }
                var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                    .Where(p => p.Name == request.Name)
                    .First();
                if (dbCharacter != null)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.RepeatCharacterName });
                    Log.Debug($"{sender}角色创建失败：角色名已存在");
                    return;
                }

                var newDbCharacter = new DbCharacter()
                {
                    Id = 0,
                    Name = request.Name,
                    UserId = sender.User.UserId,
                    UnitId = request.UnitId,
                    MapId = MapManager.Instance.InitMapId,
                    Level = 0,
                    Exp = 0,
                    Gold = 0,
                    Hp = 0,
                    Mp = 0,
                    X = 0,
                    Y = 0,
                    Z = 0,
                    Knapsack = null,
                };
                long characterId = SqlDb.Connection.Insert(newDbCharacter).ExecuteIdentity();
                if (characterId == 0)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.UnknowError });
                    Log.Debug($"{sender}角色创建失败：数据库错误");
                    return;
                }

                newDbCharacter.Id = characterId;
                sender.Send(new CharacterCreateResponse()
                {
                    Character = newDbCharacter.ToNetCharacter(),
                    Error = NetError.Success
                });
                Log.Debug($"{sender}角色创建成功");
            
            });
        }

        public void OnHandle(NetChannel sender, CharacterListRequest request)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                Log.Debug($"{sender}角色列表查询请求");
                if (sender.User == null)
                {
                    Log.Debug($"{sender}角色列表查询失败：用户未登录");
                    return;
                }

                var characterList = SqlDb.Connection.Select<DbCharacter>()
                    .Where(t => t.UserId.Equals(sender.User.UserId))
                    .ToList();
                var res = new CharacterListResponse();
                foreach (var character in characterList)
                {
                    res.CharacterList.Add(character.ToNetCharacter());
                }

                sender.Send(res, null);
                Log.Debug($"{sender}角色列表查询成功");
            });
        }

        public void OnHandle(NetChannel sender, CharacterDeleteRequest request)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                Log.Debug($"{sender}角色删除请求");
                if (sender.User == null)
                {
                    Log.Debug($"{sender}角色删除失败：用户未登录");
                    return;
                }

                var deleteCount = SqlDb.Connection.Delete<DbCharacter>()
                    .Where(t => t.UserId.Equals(sender.User.UserId))
                    .Where(t => t.Id == request.CharacterId)
                    .ExecuteAffrows();
                sender.Send(new CharacterDeleteResponse() { Error = NetError.Success });
                Log.Debug($"{sender}角色删除成功");
            });
        }

    }
}
