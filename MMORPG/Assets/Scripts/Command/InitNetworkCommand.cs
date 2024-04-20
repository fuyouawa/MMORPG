using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InitNetworkCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent(new InitNetworkEvent());
    }
}