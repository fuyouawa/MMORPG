using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoSingleton<StartManager>
{
    public async void ConnectServer()
    {
        await GameClient.Instance.ConnectAsync();
    }
}
