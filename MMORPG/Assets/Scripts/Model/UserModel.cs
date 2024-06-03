using QFramework;

namespace MMORPG.Model
{
    /// <summary>
    /// 当前登录用户的账号信息
    /// </summary>
    public interface IUserModel : IModel
    {
        public BindableProperty<string> UserId { get; }
        public BindableProperty<string> Username { get; }
        public BindableProperty<long> CharacterId { get; }
        public BindableProperty<string> CharacterName { get; }
    }

    public class UserModel : AbstractModel, IUserModel
    {
        public BindableProperty<string> UserId { get; } = new();
        public BindableProperty<string> Username { get; } = new();
        public BindableProperty<long> CharacterId { get; } = new();
        public BindableProperty<string> CharacterName { get; } = new();

        protected override void OnInit()
        {
        }
    }
}
