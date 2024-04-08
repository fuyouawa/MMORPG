using Common.Network;
using Common.Proto.Base;
using Common.Proto.Player;
using GameServer.Db;
using GameServer.Mgr;
using GameServer.Model;
using GameServer.Network;
using GameServer.Tool;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Channels;

namespace GameServer.Service
{
    public class PlayerService : ServiceBase<PlayerService>
    {
        private Dictionary<string, Player> _playerSet = new();

        private static readonly object _register_lock = new();
        private static readonly object _character_create_lock = new();

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

            if (name.Length > 7)
            {
                return false;
            }
            return true;
        }

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            lock (_playerSet)
            {
                _playerSet.Remove(sender.Player.Character.Name);
            }
        }

        // TODO:校验用户名、密码的合法性(长度等)
        public void OnHandle(NetChannel sender, LoginRequest request)
        {
            Global.Logger.Info($"玩家登录请求: Username={request.Username}, Password={request.Password}");

            if (sender.Player != null)
            {
                sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "异常登录" }, null);
                return;
            }

            if (_playerSet.ContainsKey(request.Username))
            {
                sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "账号已在别处登录" }, null);
                return;
            }

            var dbPlayer = SqlDb.Connection.Select<DbPlayer>()
                .Where(p => p.Username == request.Username)
                .Where(p => p.Password == request.Password)
                .First();
            if (dbPlayer == null)
            {
                sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "用户名或密码不正确" }, null);
                return;
            }
            
            var player = new Player(sender);
            player.Character.Name = request.Username;
            lock (_playerSet)
            {
                _playerSet[request.Username] = player;
            }
            sender.Player = player;
            sender.SendAsync(new LoginResponse() { Status = Status.Ok, Message = "登录成功" }, null);
        }

        public void OnHandle(NetChannel sender, RegisterRequest request)
        {
            Global.Logger.Info($"玩家注册请求: Username={request.Username}, Password={request.Password}");

            if (sender.Player != null)
            {
                sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "异常注册" }, null);
                return;
            }

            if (!NameVerify(request.Username))
            {
                sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "用户名非法，请检查长度及首尾空格" }, null);
                return;
            }

            lock (_register_lock) {
                var dbPlayer = SqlDb.Connection.Select<DbPlayer>()
                    .Where(p => p.Username == request.Username)
                    .First();
                if (dbPlayer != null)
                {
                    sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "用户名已被注册" }, null);
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
                    sender.SendAsync(new LoginResponse() { Status = Status.Error, Message = "注册异常" }, null);
                    return;
                }
                sender.SendAsync(new LoginResponse() { Status = Status.Ok, Message = "注册成功" }, null);
            }
        }

        public void OnHandle(NetChannel sender, EnterGameRequest request)
        {
            if (sender.Player == null)
            {
                return;
            }

            Global.Logger.Info($"玩家进入游戏");

            var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                .Where(t => t.PlayerId == sender.Player.PlayerId)
                .Where(t => t.Id == request.CharacterId)
                .First();
            if (dbCharacter == null)
            {
                sender.SendAsync(new CharacterCreateResponse() { Status = Status.Error, Message = "选择的角色无效" }, null);
                return;
            }

            var playerCharacter = new Character()
            {
                EntityId = EntityMgr.Instance.NewEntityId(),
                SpeedId = dbCharacter.SpaceId,
                CharacterId = dbCharacter.Id,
                Name = dbCharacter.Name,
                Level = dbCharacter.Level,
                Exp = dbCharacter.Exp,
                Hp = dbCharacter.Hp,
                Mp = dbCharacter.Mp,
            };
            playerCharacter.Position = new() {
                X = dbCharacter.X,
                Y = dbCharacter.Y,
                Z = dbCharacter.Z,
            };
            playerCharacter.Direction = Vector3.Zero;

            EntityMgr.Instance.AddEntity(playerCharacter);

            var space = SpaceMgr.Instance.GetSpaceById(playerCharacter.SpeedId);
            if (space == null)
            {
                sender.SendAsync(new EnterGameResponse() { Status = Status.Error, Message = "无效地图" }, null);
                return;
            }
            sender.Player.Character = playerCharacter;
            space.PlayerEnter(sender.Player);

            var res = new EnterGameResponse()
            {
                Status = Status.Ok,
                Message = "加入游戏成功",
                Character = playerCharacter.ToNetCharacter(),
            };
            sender.SendAsync(res, null);
        }

        public void OnHandle(NetChannel sender, HeartBeatRequest request)
        {
            Global.Logger.Debug($"玩家发送心跳请求");
            sender.SendAsync(new HeartBeatResponse() { }, null);
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
                sender.SendAsync(new CharacterCreateResponse() { Status = Status.Error, Message = "角色创建已达上限" }, null);
                return;
            }

            if (!NameVerify(request.Name))
            {
                sender.SendAsync(new CharacterCreateResponse() { Status = Status.Error, Message = "角色名非法，请检查长度及首尾空格" }, null);
                return;
            }

            lock (_character_create_lock)
            {
                var dbCharacter = SqlDb.Connection.Select<DbCharacter>()
                    .Where(p => p.Name == request.Name)
                    .First();
                if (dbCharacter != null)
                {
                    sender.SendAsync(new CharacterCreateResponse() { Status = Status.Error, Message = "角色名已被注册" }, null);
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
                    SpaceId = SpaceMgr.Instance.InitSpaceId,
                    Gold = 0,
                    PlayerId = sender.Player.PlayerId
                };
                var insertCount = SqlDb.Connection.Insert(newDbCharacter).ExecuteAffrows();
                if (insertCount <= 0)
                {
                    sender.SendAsync(new CharacterCreateResponse() { Status = Status.Error, Message = "创建角色异常" }, null);
                    return;
                }
                sender.SendAsync(new CharacterCreateResponse() { Status = Status.Ok, Message = "创建角色成功" }, null);
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
            sender.SendAsync(res, null);
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
            sender.SendAsync(new CharacterDeleteResponse() { Status = Status.Ok, Message = "删除完成" }, null);
        }



        public void OnConnect(NetChannel sender)
        {
        }
    }
}
