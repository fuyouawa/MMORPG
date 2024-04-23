using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IConfigModel : IModel
{
    public GameConfigObject GameConfig { get; }
}

public class ConfigModel : AbstractModel, IConfigModel
{
    private ResLoader _resLoader = ResLoader.Allocate();

    private GameConfigObject _config;

    public GameConfigObject GameConfig => _config;

    protected override void OnInit()
    {
        _config = _resLoader.LoadSync<GameConfigObject>("GameConfig");
    }
}
