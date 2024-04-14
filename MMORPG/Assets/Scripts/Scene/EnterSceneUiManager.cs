using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSceneUiManager : MonoBehaviour
{
    public void DoEnterGame()
    {
        SceneHelper.SwitchScene("GameScene");
    }
}
