using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSceneUiLogic : MonoBehaviour
{
    public void DoEnterGame()
    {
        SceneHelper.SwitchScene("GameScene");
    }
}
