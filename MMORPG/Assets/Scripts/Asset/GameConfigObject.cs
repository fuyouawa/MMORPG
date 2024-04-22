using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "MMORPG/Assets/GameConfig", order = 1)]
public class GameConfigObject : ScriptableObject
{
    [Range(0.02f, 1f)]
    public float NetworkSyncDeltaTime = 0.1f;
}
