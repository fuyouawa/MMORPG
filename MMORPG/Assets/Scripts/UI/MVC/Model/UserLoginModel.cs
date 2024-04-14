using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLoginModel : AbstractModel
{
    public string LoginUsername;
    public string LoginPassword;
    public string RegisterUsername;
    public string RegisterPassword;
    public string RegisterVerifyPassword;

    protected override void OnInit()
    {
    }
}
