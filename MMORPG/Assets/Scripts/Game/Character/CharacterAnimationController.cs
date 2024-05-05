using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAnimationController : MonoBehaviour
{
    public GameObject Owner;

    private Animator _animator;
    private bool _animatorMove;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void EnableAnimatorMove()
    {
        _animatorMove = true;
    }

    public void DisableAnimatorMove()
    {
        _animatorMove = false;
    }

    private void OnAnimatorMove()
    {
        if (_animatorMove)
        {
            Owner.transform.position += _animator.deltaPosition;
            Owner.transform.rotation *= _animator.deltaRotation;
        }
    }
}
