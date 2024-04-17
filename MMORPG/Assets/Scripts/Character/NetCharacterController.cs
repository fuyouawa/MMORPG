using QFramework;
using ThirdPersonCamera;
using UnityEngine;


/// <summary>
/// 角色控制器，负责接收玩家对角色的输入，及角色的表现
/// </summary>
public class NetCharacterController : MonoBehaviour, IController
{
    // NetPlayer应该属于Model层，这里先直接用
    public NetPlayer NetPlayer;

    private NetCharacterAnimator _animator;

    private float _rotateLerp = 0.2f;
    private float _moveLerp = 0.2f;

    public IArchitecture GetArchitecture()
    {
        return GameApp.Interface;
    }

    private void Awake()
    {
        _animator = GetComponent<NetCharacterAnimator>();
    }

    private void Start()
    {
        if (NetPlayer.IsMine)
        {
            var camera = Camera.main.GetComponent<CameraController>();
            camera.InitFromTarget(transform);
        }
    }

    private void FixedUpdate()
    {
        if (NetPlayer.IsMine)
        {
            ControlCharacter();
        }
    }

    private void Update()
    {
        if (!NetPlayer.IsMine)
        {
            SyncCharacter();
        }
    }

    protected virtual void ControlCharacter()
    {
        if (Input.GetMouseButton(0))
        {
            _animator.PlayAttack01();
        }

        else if (_animator.Status != NetCharacterAnimator.AnimationStatus.Attack)
        {
            // 获取摄像机的前方向，即摄像机的观察方向
            var cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f; // 忽略y轴，保持在水平方向

            // 获取输入方向
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            Vector3 moveDirection = Vector3.zero;
            if (inputDirection != Vector3.zero)
            {
                moveDirection = Quaternion.LookRotation(cameraForward) * inputDirection;

                _animator.PlayRun();
            }
            else
            {
                _animator.PlayIdle();
            }

            CharacterController controller = GetComponent<CharacterController>();
            controller.SimpleMove(moveDirection * NetPlayer.MoveSpeed);

            // 如果有输入，旋转角色朝向移动方向
            if (inputDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotateLerp);
            }

            this.SendCommand(new CharacterPositionChangeCommand(NetPlayer, transform.position, transform.rotation));
        }
    }

    protected virtual void SyncCharacter()
    {
        // 是网络玩家，进行位置同步
        //transform.position = NetPlayer.Position;
        transform.position = Vector3.Lerp(transform.position, NetPlayer.Position, _moveLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, NetPlayer.Rotation, _rotateLerp);
    }
}
