using UnityEngine;

public class HeroKnightWalking : PlayerAction, IAnimatorAutoUpdateParams
{
    [Range(0.1f, 100f)]
    public float Acceleration = 3;

    [AnimatorParam]
    public bool Walking { get; set; }
    [AnimatorParam]
    public float HoriMoveSpeedNormalized { get; set; }
    [AnimatorParam]
    public float VertMoveSpeedNormalized { get; set; }

    private void Awake()
    {
        this.StartAnimatorAutoUpdate(gameObject, Brain.Character.Animator);
    }

    public override void OnStateUpdate()
    {
        SetMovement(Brain.InputControls.Player.Move.ReadValue<Vector2>());
    }

    public override void OnStateExit()
    {
        Walking = false;
        HoriMoveSpeedNormalized = 0;
        VertMoveSpeedNormalized = 0;
    }

    [StateCondition]
    public bool StopWalking()
    {
        return !Walking;
    }

    public void SetMovement(Vector2 moveDirection)
    {
        var acc = Acceleration * Time.deltaTime;
        HoriMoveSpeedNormalized = Mathf.MoveTowards(HoriMoveSpeedNormalized, moveDirection.x, acc);
        VertMoveSpeedNormalized = Mathf.MoveTowards(VertMoveSpeedNormalized, moveDirection.y, acc);
    
        Walking = moveDirection.sqrMagnitude > 0.5f ||
                  new Vector2(HoriMoveSpeedNormalized, VertMoveSpeedNormalized).sqrMagnitude > 0.5f;
    
        if (!(moveDirection.sqrMagnitude > 0.5f)) return;
        var cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        var targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
    }
}
