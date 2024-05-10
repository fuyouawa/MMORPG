using QFramework;

namespace MMORPG.Model
{
    /// <summary>
    /// 当前登录用户的账号信息
    /// </summary>
    public interface IUserModel : IModel
    {
        public int UserId { get; }
        public string Username { get; }
        public void SetUsername(string username);
        public void SetUserId(int userId);
    }

    public class UserModel : AbstractModel, IUserModel
    {
        private int _userId;
        public int UserId => _userId;

        private string _username;
        public string Username => _username;

        public void SetUserId(int userId)
        {
            _userId = userId;
        }

        public void SetUsername(string username)
        {
            _username = username;
        }

        protected override void OnInit()
        {
        }
    }
}
