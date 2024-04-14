using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGameArch : Architecture<EnterGameArch>
{
    protected override void Init()
    {
    }
}

public class EnterGameController : MonoBehaviour, IController
{
    public void DoEnterGame()
    {
        this.SendCommand<EnterGameCommand>();
    }

    public IArchitecture GetArchitecture()
    {
        return EnterGameArch.Interface;
    }
}
