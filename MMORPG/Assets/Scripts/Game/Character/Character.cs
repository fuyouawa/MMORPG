using System.Collections;
using System.Collections.Generic;
using QFramework;
using Sirenix.OdinInspector;
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
    public float RotationLerp = 0.2f;
    [Header("Binding")]
    public Animator Animator;
    public CharacterAnimationController AnimationController;
    [Header("Action")]
    [ChildGameObjectsOnly]
    public GameObject[] AdditionalAbilityNodes;

    public void SmoothMoveRotation(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationLerp);
    }
}
