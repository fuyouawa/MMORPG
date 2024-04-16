using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



public class GameApp : Architecture<GameApp>
{
    protected override void Init()
    {
        this.RegisterSystem(new SpaceSystem());
        this.RegisterSystem(new CharacterSystem());
    }
}
