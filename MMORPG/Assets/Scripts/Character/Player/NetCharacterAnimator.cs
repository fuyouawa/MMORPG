using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCharacterAnimator : MonoBehaviour
{
    private Animator _animator;

    public enum AnimationStatus
    {
        Idle = 1,
        Run = 2,
        Attack = 3,
        GetHit = 4,
        Die = 5,
    }
    private AnimationStatus _status = AnimationStatus.Idle;
    public AnimationStatus Status {  get { return _status; } }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Reset();
        switch (_status)
        {
            case AnimationStatus.Idle:
            {
                _animator.SetBool("Idle", true);
                break;
            }
            case AnimationStatus.Run:
            {
                _animator.SetBool("Run", true);
                break;
            }
            case AnimationStatus.GetHit:
            {
                _animator.SetBool("GetHit", true);
                break;
            }
            case AnimationStatus.Attack:
            {
                _animator.SetBool("Attack01", true);
                break;
            }
            case AnimationStatus.Die:
            {
                _animator.SetBool("Die", true);
                break;
            }
        }
    }


    private void Reset()
    {
        _animator.SetBool("Idle", false);
        _animator.SetBool("Attack01", false);
        _animator.SetBool("GetHit", false);
        _animator.SetBool("Die", false);
        _animator.SetBool("Run", false);
    }

    public void PlayIdle()
    {
        if (_status == AnimationStatus.Attack)
        {
            return;
        }
        _status = AnimationStatus.Idle;
    }

    public void PlayAttack01()
    {
        _status = AnimationStatus.Attack;
    }

    public void PlayGetHit()
    {
        _status = AnimationStatus.GetHit;
    }

    public void PlayDie()
    {
        _status = AnimationStatus.Die;
    }

    public void PlayRun()
    {
        _status = AnimationStatus.Run;
    }

    public void StopAttack01()
    {
        _status = AnimationStatus.Idle;
    }

    public void StopGetHit()
    {
        _status = AnimationStatus.Idle;
    }
}
