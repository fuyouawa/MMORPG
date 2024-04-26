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
using Common.Proto.Event.Map;
using Common.Proto.User;
using Common.Proto.Character;

namespace GameServer.Service
{
    // 可能有逻辑仍需要加锁
    public class UserService : ServiceBase<UserService>
    {
        private static readonly object _loginLock = new();
        private static readonly object _registerLock = new();

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.Player == null)
                return;
            UserManager.Instance.RemovePlayer(sender.Player);
        }

        // TODO:校验用户名、密码的合法性(长度等)
        public void OnHandle(NetChannel sender, LoginRequest request)
        {
            Log.Debug($"{sender.ChannelName}登录请求: Username={request.Username}, Password={request.Password}");
            lock (_loginLock)
            {
                if (sender.Player != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.UnknowError });
                    Log.Debug($"{sender.ChannelName}登录失败：用户已登录");
                    return;
                }
                if (UserManager.Instance.GetPlayerByName(request.Username) != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.LoginConflict });
                    Log.Debug($"{sender.ChannelName}登录失败：账号已在别处登录");
                    return;
                }
                var dbPlayer = SqlDb.Connection.Select<DbUser>()
                    .Where(p => p.Username == request.Username)
                    .Where(p => p.Password == request.Password)
                    .First();
                if (dbPlayer == null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.IncorrectUsernameOrPassword });
                    Log.Debug($"{sender.ChannelName}登录失败：账号或密码错误");
                    return;
                }
                sender.Player = UserManager.Instance.NewUser(sender, dbPlayer.Username, dbPlayer.Id);
            }
            sender.Send(new LoginResponse() { Error = NetError.Success });
            Log.Debug($"{sender.ChannelName}登录成功");
        }

        public void OnHandle(NetChannel sender, RegisterRequest request)
        {
            Log.Debug($"{sender.ChannelName}注册请求: Username={request.Username}, Password={request.Password}");
            if (sender.Player != null)
            {
                sender.Send(new RegisterResponse() { Error = NetError.UnknowError });
                Log.Debug($"{sender.ChannelName}注册失败：用户已登录");
                return;
            }
            if (!StringHelper.NameVerify(request.Username))
            {
                sender.Send(new RegisterResponse() { Error = NetError.IllegalUsername });
                Log.Debug($"{sender.ChannelName}注册失败：用户名非法");
                return;
            }
            lock (_registerLock) {
                var dbPlayer = SqlDb.Connection.Select<DbUser>()
                    .Where(p => p.Username == request.Username)
                    .First();
                if (dbPlayer != null)
                {
                    sender.Send(new RegisterResponse() { Error = NetError.RepeatUsername });
                    Log.Debug($"{sender.ChannelName}注册失败：用户名已被注册");
                    return;
                }
                var newDbPlayer = new DbUser(username: request.Username, password: request.Password, coin: 0);
                var insertCount = SqlDb.Connection.Insert<DbUser>(newDbPlayer).ExecuteAffrows();
                if (insertCount <= 0)
                {
                    sender.Send(new RegisterResponse() { Error = NetError.UnknowError });
                    Log.Debug($"{sender.ChannelName}注册失败：数据库错误");
                    return;
                }
                sender.Send(new RegisterResponse() { Error = NetError.Success });
                Log.Debug($"{sender.ChannelName}注册成功");
            }
        }

        public void OnHandle(NetChannel sender, HeartBeatRequest request)
        {
            Log.Debug($"{sender.ChannelName}发送心跳请求");
            //sender.Send(new HeartBeatResponse() { }, null);
        }


        public void OnConnect(NetChannel sender)
        {
        }
    }
}
