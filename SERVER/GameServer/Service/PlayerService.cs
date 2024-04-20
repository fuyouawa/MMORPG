using Common.Network;
using Common.Proto.Base;
using Common.Proto.Player;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Unit;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Channels;
using Common.Proto.Space;

namespace GameServer.Service
{
    // 可能有逻辑仍需要加锁
    public class PlayerService : ServiceBase<PlayerService>
    {
        private static readonly object _loginLock = new();
        private static readonly object _registerLock = new();
        private static readonly object _characterCreateLock = new();

        private bool NameVerify(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            var trimmedName = name.Trim();
            if (trimmedName.Length != name.Length)
            {
                return false;
            }

            if (name.Length < 4 || name.Length > 12)
            {
                return false;
            }
            return true;
        }

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            PlayerManager.Instance.RemovePlayer(sender.Player);
        }

        // TODO:校验用户名、密码的合法性(长度等)
        public void OnHandle(NetChannel sender, LoginRequest request)
        {
            Log.Information($"{sender.ChannelName}登录请求: Username={request.Username}, Password={request.Password}");

            lock (_loginLock)
            {
                if (sender.Player != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.UnknowError });
                    return;
                }

                if (PlayerManager.Instance.GetPlayerByName(request.Username) != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.LoginConflict });
                    return;
                }

                var dbPlayer = SqlDb.Connection.Select<DbPlayer>()
                    .Where(p => p.Username == request.Username)
                    .Where(p => p.Password == request.Password)
                    .First();
                if (dbPlayer == null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.IncorrectUsernameOrPassword });
                    return;
                }
                sender.Player = PlayerManager.Instance.NewPlayer(sender, dbPlayer.Username, dbPlayer.Id);
            }

            Log.Information($"{sender.ChannelName}登录成功");
            sender.Send(new LoginResponse() { Error = NetError.Success });
        }

        public void OnHandle(NetChannel sender, RegisterRequest request)
        {
            Log.Information($"{sender.ChannelName}注册请求: Username={request.Username}, Password={request.Password}");

            if (sender.Player != null)
            {
                sender.Send(new RegisterResponse() { Error = NetError.UnknowError });
                return;
            }

            if (!NameVerify(request.Username))
            {
                sender.Send(new RegisterResponse() { Error = NetError.IllegalUsername });
                return;
            }

            lock (_registerLock) {
                var dbPlayer = SqlDb.Connection.Select<DbPlayer>()
                    .Where(p => p.Username == request.Username)
                    .First();
                if (dbPlayer != null)
                {
                    sender.Send(new RegisterResponse() { Error = NetError.RepeatUsername });
                    return;
                }

                var newDbPlayer = new DbPlayer()
                {
                    Username = request.Username,
                    Password = request.Password,
                    Coin = 0,
                };
                var insertCount = SqlDb.Connection.Insert<DbPlayer>(newDbPlayer).ExecuteAffrows();
                if (insertCount <= 0)
                {
                    sender.Send(new RegisterResponse() { Error = NetError.UnknowError });
                    return;
                }
                Log.Information($"{sender.ChannelName}注册成功");
                sender.Send(new RegisterResponse() { Error = NetError.Success });
            }
        }

        public void OnHandle(NetChannel sender, EnterGameRequest request)
        {
            Log.Information($"{sender.ChannelName}进入游戏请求");

            if (sender.Player == null)
            {
                Log.Information($"{sender.ChannelName}进入游戏失败：未登录");
                return;
            }

            if (sender.Player.Character != null)
            {
                Log.Information($"{sender.ChannelName}进入游戏失败：重复进入");
                return;
            }

            var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                //.Where(t => t.PlayerId == sender.Player.PlayerId)
                .Where(t => t.Id == request.CharacterId)
                .First();
            if (dbCharacter == null)
            {
                sender.Send(new CharacterCreateResponse() { Error = NetError.InvalidCharacter });
                return;
            }
            var space = SpaceManager.Instance.GetSpaceById(dbCharacter.SpaceId);
            if (space == null)
            {
                sender.Send(new EnterGameResponse() { Error = NetError.InvalidMap });
                return;
            }

            var pos = new Vector3()
            {
                X = dbCharacter.X,
                Y = dbCharacter.Y,
                Z = dbCharacter.Z,
            };
            var character = space.CharacterManager.NewCharacter(sender.Player, pos, Vector3.Zero, dbCharacter.Name);
            sender.Player.SetCharacter(character);
            space.EntityEnter(character);
            var res = new EnterGameResponse()
            {
                Error = NetError.Success,
                //Character = character.ToNetCharacter(),
                EntityId = character.EntityId,
            };
            Log.Information($"{sender.ChannelName}进入游戏成功");
            sender.Send(res, null);

            var res2 = new EntityEnterResponse();
            res2.EntityList.Add(character.ToNetEntity());
            sender.Send(res2, null);
        }

        public void OnHandle(NetChannel sender, HeartBeatRequest request)
        {
            Log.Debug($"{sender.ChannelName}发送心跳请求");
            //sender.Send(new HeartBeatResponse() { }, null);
        }

        public void OnHandle(NetChannel sender, CharacterCreateRequest request)
        {
            if (sender.Player == null)
            {
                return;
            }

            var count = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.PlayerId.Equals(sender.Player.PlayerId))
                .Count();
            if (count >= 4)
            {
                sender.Send(new CharacterCreateResponse() { Error = NetError.CharacterCreationLimitReached });
                return;
            }

            if (!NameVerify(request.Name))
            {
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
                    return;
                }

                var newDbCharacter = new DbCharacter()
                {
                    Name = request.Name,
                    JobId = request.JobId,
                    Hp = 100,
                    Mp = 100,
                    Level = 1,
                    Exp = 0,
                    SpaceId = SpaceManager.Instance.InitSpaceId,
                    Gold = 0,
                    PlayerId = sender.Player.PlayerId
                };
                var insertCount = SqlDb.Connection.Insert(newDbCharacter).ExecuteAffrows();
                if (insertCount <= 0)
                {
                    sender.Send(new CharacterCreateResponse() { Error = NetError.UnknowError });
                    return;
                }
                sender.Send(new CharacterCreateResponse() { Error = NetError.Success });
            }
        }

        public void OnHandle(NetChannel sender, CharacterListRequest request)
        {
            if (sender.Player == null)
            {
                return;
            }

            var characterList = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.PlayerId.Equals(sender.Player.PlayerId))
                .ToList();

            var res = new CharacterListResponse();
            foreach (var character in characterList)
            {
                res.CharacterList.Add(new NetCharacter()
                {
                    CharacterId = character.Id,
                    Name = character.Name,
                    JobId = character.JobId,
                    Level = character.Level,
                    Exp = character.Exp,
                    SpaceId = character.SpaceId,
                    Gold = character.Gold,
                });
            }
            sender.Send(res, null);
        }

        public void OnHandle(NetChannel sender, CharacterDeleteRequest request)
        {
            if (sender.Player == null)
            {
                return;
            }

            var delete_count = SqlDb.Connection.Delete<DbCharacter>()
                .Where(t => t.PlayerId.Equals(sender.Player.PlayerId))
                .Where(t => t.Id == request.CharacterId)
                .ExecuteAffrows();
            sender.Send(new CharacterDeleteResponse() { Error = NetError.Success });
        }

        public void OnConnect(NetChannel sender)
        {
        }
    }
}
