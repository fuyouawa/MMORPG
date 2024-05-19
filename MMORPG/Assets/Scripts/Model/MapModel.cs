using QFramework;

 namespace MMORPG.Model
{
    public interface IMapModel : IModel
    {
        public BindableProperty<int> CurrentMapId { get; }
    }

    public class MapModel : AbstractModel, IMapModel
    {
        public BindableProperty<int> CurrentMapId { get; } = new();

        protected override void OnInit()
        {
            CurrentMapId.SetValueWithoutEvent(-1);
        }
    }
}
