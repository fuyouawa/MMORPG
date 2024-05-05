using UnityEngine;

public class HeroKnightWalking : PlayerAbility, IAnimatorAutoUpdateParams
{
    public float IdleThreshold = 0.05f;

    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriMovementNormalized { get; set; }
    [AnimatorParam]
    public float VertMovementNormalized { get; set; }

    public override void OnStateInit()
    {
        this.StartAnimatorAutoUpdate(gameObject, Brain.Character.Animator);
    }

    public override void OnStateEnter()
    {
        Walking = true;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;
    }

    public override void OnStateUpdate()
    {
        SetMovement();
        ForwardCamera();
    }

    public override void OnStateExit()
    {
        Walking = false;
        HoriMovementNormalized = 0;
        VertMovementNormalized = 0;
    }

    [StateCondition]
    public bool ReachIdleThreshold()
    {
        return Brain.CurrentMovementDirection.magnitude > IdleThreshold;
    }

    [StateCondition]
    public bool FinishInertia()
    {
        return new Vector2(HoriMovementNormalized, VertMovementNormalized).magnitude < IdleThreshold;
    }

    public void SetMovement()
    {
        var moveDirection = Brain.CurrentMovementDirection;

        var acc = Acceleration * Time.deltaTime;
        HoriMovementNormalized = Mathf.MoveTowards(HoriMovementNormalized, moveDirection.x, acc);
        VertMovementNormalized = Mathf.MoveTowards(VertMovementNormalized, moveDirection.y, acc);
    }

    private void ForwardCamera()
    {
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Brain.Character.SmoothMoveRotation(targetRotation);
    }
}
