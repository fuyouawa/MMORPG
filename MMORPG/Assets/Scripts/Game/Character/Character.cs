using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterType
{
    Player,
    Enemy
}

public class Character : MonoBehaviour
{
    public Entity Entity;
    public CharacterController CharacterController;
    public Animator Animator;
    public CharacterType CharacterType;
    [EnumCondition("CharacterType", (int)CharacterType.Player)]
    public Player Player;

#if UNITY_EDITOR
    [ContextMenu("Build Player")]
    public void BuildPlayer()
    {
        Player = gameObject.AddComponent<Player>();
        Player.Character = this;
        CharacterType = CharacterType.Player;
    }
#endif


    [StateCondition]
    public bool CanMovement()
    {
        return true;
    }
    [StateCondition]
    public bool CanMovement2()
    {
        return true;
    }
    [StateCondition]
    public bool CanMovement3()
    {
        return true;
    }
}
