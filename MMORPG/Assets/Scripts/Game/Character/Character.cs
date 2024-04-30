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
    public CharacterBrain Brain;
    public CharacterType CharacterType;
    [EnumCondition("CharacterType", (int)CharacterType.Player)]
    public Player Player;

#if UNITY_EDITOR
    public void BuildBrain()
    {
        Brain = gameObject.AddComponent<CharacterBrain>();
        Brain.Character = this;
    }

    [ContextMenu("Build Player")]
    public void BuildPlayer()
    {
        BuildBrain();
        Player = gameObject.AddComponent<Player>();
        Player.Character = this;
        CharacterType = CharacterType.Player;
        gameObject.AddComponent<PlayerInput>();
        //TODO PlayerInput
    }
#endif
}
