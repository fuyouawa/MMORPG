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
    public CharacterType CharacterType;
    [EnumCondition("CharacterType", (int)CharacterType.Player)]
    public PlayerBrain Player;
    [Header("Binding")]
    public Animator Animator;
    public CharacterAnimationController AnimationController;

#if UNITY_EDITOR
    //TODO
    public void BuildPlayer()
    {
        Player = gameObject.AddComponent<PlayerBrain>();
        Player.Character = this;
        CharacterType = CharacterType.Player;
    }
#endif
}
