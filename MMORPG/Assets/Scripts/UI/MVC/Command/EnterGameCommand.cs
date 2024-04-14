using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnterGameCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        SceneHelper.SwitchScene("Space1Scene");
    }
}