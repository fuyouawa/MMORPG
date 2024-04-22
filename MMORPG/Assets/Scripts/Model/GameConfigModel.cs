using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameConfigModel : IModel
{
    public BindableProperty<float> NetworkSyncDeltaTime { get; }
}

public class GameConfigModel : AbstractModel, IGameConfigModel
{
    private ResLoader _resLoader = ResLoader.Allocate();

    public BindableProperty<float> NetworkSyncDeltaTime { get; } = new();

    protected override void OnInit()
    {
        var config = _resLoader.LoadSync<GameConfigObject>("GameConfig");
        NetworkSyncDeltaTime.SetValueWithoutEvent(config.NetworkSyncDeltaTime);
    }
}