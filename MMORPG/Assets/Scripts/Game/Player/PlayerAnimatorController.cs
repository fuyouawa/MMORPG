using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour, IAnimatorAutoUpdateParams
{
    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriSpeedNormalized { get; set; }
    [AnimatorParam]
    public float VertSpeedNormalized { get; set; }

    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    private bool _moveByAnim;
    private Animator _animator;
    private Transform _owner;

    public void Setup(Transform owner, bool moveByAnim)
    {
        _owner = owner;
        _moveByAnim = moveByAnim;
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        this.StartAnimatorAutoUpdate(gameObject, _animator);
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
                _owner.position += _animator.deltaPosition;
                _owner.rotation *= _animator.deltaRotation;
            }
        }
    }
}
