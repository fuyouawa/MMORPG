using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    public GameObject Owner;

    public float MoveAcceleration = 3f;

    private Animator _animator;
    private bool _animatorMove = true;

    [AnimatorParam]
    public bool Movement { get; set; }

    [AnimatorParam]
    public float HoriMovementDirection { get; set; }
    [AnimatorParam]
    public float VertMovementDirection { get; set; }

    public Vector2 MovementDirection => new(HoriMovementDirection, VertMovementDirection);

    private Vector2 _targetMoveDirection;

    private AnimatorMachine _machine;

    public void SmoothMoveDirection(Vector2 dir)
    {
        _targetMoveDirection = dir;
    }



    void Awake()
    {
        _animator = GetComponent<Animator>();
        _machine = new(this, gameObject, _animator);
        _machine.Run();
    }

    void Update()
    {
        SmoothMoving();
    }

    public void EnableAnimatorMove()
    {
        _animatorMove = true;
    }

    public void DisableAnimatorMove()
    {
        _animatorMove = false;
    }

    private void SmoothMoving()
    {
        var acc = MoveAcceleration * Time.deltaTime;
        HoriMovementDirection = Mathf.MoveTowards(HoriMovementDirection, _targetMoveDirection.x, acc);
        VertMovementDirection = Mathf.MoveTowards(VertMovementDirection, _targetMoveDirection.y, acc);
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
