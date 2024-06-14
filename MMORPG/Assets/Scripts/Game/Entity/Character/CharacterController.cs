using System.Collections;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Game
{
    public class CharacterController : MonoBehaviour, IController
    {
        [Required]
        public EntityView Entity;
        public float RotationSmooth = 10f;
        public float MoveSmooth = 10f;
        [Title("Binding")]
        [Required]
        public Animator Animator;
        public SkillsEffectManager EffectManager;

        public CharacterSkillManager SkillManager { get; private set; }

        public bool IsPreventingMovement { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public CapsuleCollider Collider { get; private set; }


        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            SkillManager = new(this);
        }
        private void Update()
        {
            SkillManager.Update();
        }

        public void SmoothRotate(Quaternion targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSmooth * Time.deltaTime);
        }

        public void SmoothMove(Vector3 position)
        {
            if (IsPreventingMovement) return;
            transform.position = Vector3.Lerp(transform.position, position, MoveSmooth * Time.deltaTime);
        }

        public void MoveDirection(Vector3 direction)
        {
            if (IsPreventingMovement) return;
            transform.position += direction;
        }

        public void RelativeRotate(Quaternion rotation)
        {
            transform.rotation *= rotation;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        protected Coroutine PreventMovementCoroutine;

        public void PreventMovement(float delay = float.MaxValue)
        {
            if (PreventMovementCoroutine != null)
                StopCoroutine(PreventMovementCoroutine);
            PreventMovementCoroutine = StartCoroutine(PreventMovementCo(delay));
        }

        public void StopPreventMovement()
        {
            if (PreventMovementCoroutine != null)
                StopCoroutine(PreventMovementCoroutine);
            IsPreventingMovement = false;
        }

        protected virtual IEnumerator PreventMovementCo(float delay)
        {
            IsPreventingMovement = true;
            yield return new WaitForSeconds(delay);
            IsPreventingMovement = false;
        }

        //private void OnGUI()
        //{
        //    if (!IsInView(gameObject))
        //    {
        //        return;
        //    }

        //    float height = 2f;
        //    var camera = Camera.main;
        //    var pos = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        //    Vector2 uiPos = camera.WorldToScreenPoint(pos);
        //    uiPos = new(uiPos.x, Screen.height - uiPos.y);

        //    Vector2 nameSize = GUI.skin.label.CalcSize(new("帅比"));
        //    GUI.color = Color.yellow;

        //    var rect = new Rect(uiPos.x - (nameSize.x / 2), uiPos.y - nameSize.y, nameSize.x, nameSize.y);
        //    GUI.Label(rect, "帅比");

        //}

        //// 判断物体是否处于摄像机拍摄的角度中
        //private bool IsInView(GameObject go)
        //{
        //    var worldPos = go.transform.position;
        //    var cameraTransform = Camera.main.transform;
        //    Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        //    Vector2 dir = (worldPos - cameraTransform.position).normalized;
        //    float dot = Vector3.Dot(cameraTransform.forward, dir);
        //    return dot > 0 &&
        //           viewPos.x >= 0 &&
        //           viewPos.x <= 1 &&
        //           viewPos.y >= 0 &&
        //           viewPos.y <= 1;
        //}
    }

}
