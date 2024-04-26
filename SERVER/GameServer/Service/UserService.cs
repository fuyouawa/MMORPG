using Common.Network;
using Common.Proto.Base;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Network;
using GameServer.Tool;
using Serilog;
using Common.Proto.User;

namespace GameServer.Service
{
    // 可能有逻辑仍需要加锁
    public class UserService : ServiceBase<UserService>
    {
        private static readonly object _loginLock = new();
        private static readonly object _registerLock = new();

        public void OnChannelClosed(NetChannel sender)
        {
            if (sender.User == null)
                return;
            UserManager.Instance.RemoveUser(sender.User);
        }

        // TODO:校验用户名、密码的合法性(长度等)
        public void OnHandle(NetChannel sender, LoginRequest request)
        {
            Log.Debug($"{sender.ChannelName}登录请求: Username={request.Username}, Password={request.Password}");
            lock (_loginLock)
            {
                if (sender.User != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.UnknowError });
                    Log.Debug($"{sender.ChannelName}登录失败：用户已登录");
                    return;
                }
                if (UserManager.Instance.GetUserByName(request.Username) != null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.LoginConflict });
                    Log.Debug($"{sender.ChannelName}登录失败：账号已在别处登录");
                    return;
                }
                var dbUser = SqlDb.Connection.Select<DbUser>()
                    .Where(p => p.Username == request.Username)
                    .Where(p => p.Password == request.Password)
                    .First();
                if (dbUser == null)
                {
                    sender.Send(new LoginResponse() { Error = NetError.IncorrectUsernameOrPassword });
                    Log.Debug($"{sender.ChannelName}登录失败：账号或密码错误");
                    return;
                }
                sender.User = UserManager.Instance.NewUser(sender, dbUser.Username, dbUser.Id);
            }
            sender.Send(new LoginResponse() { Error = NetError.Success });
            Log.Debug($"{sender.ChannelName}登录成功");
        }

        public void OnHandle(NetChannel sender, RegisterRequest request)
        {
            Log.Debug($"{sender.ChannelName}注册请求: Username={request.Username}, Password={request.Password}");
            if (sender.User != null)
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
                var dbUser = SqlDb.Connection.Select<DbUser>()
                    .Where(p => p.Username == request.Username)
                    .First();
                if (dbUser != null)
                {
                    sender.Send(new RegisterResponse() { Error = NetError.RepeatUsername });
                    Log.Debug($"{sender.ChannelName}注册失败：用户名已被注册");
                    return;
                }
                var newDbUser = new DbUser(request.Username, request.Password, 0);
                var insertCount = SqlDb.Connection.Insert<DbUser>(newDbUser).ExecuteAffrows();
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
