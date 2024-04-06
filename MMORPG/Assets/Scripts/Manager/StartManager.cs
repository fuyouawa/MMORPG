using Common.Proto;
using Common.Proto.User;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StartManager : MonoSingleton<StartManager>
{
    void Start()
    {
        ConnectServer();
    }

    public void ConnectServer()
    {
        Task.Run(GameClient.Instance.ConnectAsync);
    }
}
