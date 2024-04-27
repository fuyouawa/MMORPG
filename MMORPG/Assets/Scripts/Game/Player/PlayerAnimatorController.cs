using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
    [AnimatorParam]
    [ReadOnly]
    public bool Walking;
    [AnimatorParam]
    [ReadOnly]
    public float HoriSpeedNormalized;
    [AnimatorParam]
    [ReadOnly]
    public float VertSpeedNormalized;

    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    private bool _moveByAnim;

    private AnimatorMachine _animatorMachine;
    private Transform _owner;

    public void Setup(Transform owner, bool moveByAnim)
    {
        _owner = owner;
        _moveByAnim = moveByAnim;
    }

    void Awake()
    {
        _animatorMachine = new(this, GetComponent<Animator>());
    }

    private void Update()
    {
        _animatorMachine.UpdateAnimator();
    }

    public void SetMovement(Vector2 move)
    {
        var acc = Acceleration * Time.deltaTime;
        HoriSpeedNormalized = Mathf.MoveTowards(HoriSpeedNormalized, move.x, acc);
        VertSpeedNormalized = Mathf.MoveTowards(VertSpeedNormalized, move.y, acc);

        Walking = move.sqrMagnitude > 0.5f ||
            new Vector2(HoriSpeedNormalized, VertSpeedNormalized).sqrMagnitude > 0.5f;

        if (!(move.sqrMagnitude > 0.5f)) return;
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
    }

    private void OnAnimatorMove()
    {
        if (_moveByAnim)
        {
            if (_owner != null)
            {
                _owner.position += _animatorMachine.Animator.deltaPosition;
                _owner.rotation *= _animatorMachine.Animator.deltaRotation;
            }
        }
    }
}
