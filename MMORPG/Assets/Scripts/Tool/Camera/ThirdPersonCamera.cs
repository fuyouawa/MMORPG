using System;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 第三人称相机控制器
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] private Vector3 _targetOffset = new(0, 1.0f, 0);
        [SerializeField] private Vector3 _cameraOffset = new(0, 0.5f, 0);
        [SerializeField] private float _targetDistance = 5.0f;
        [SerializeField] private float _cameraRadius = 0.5f;
        [SerializeField] private float _maxObjectThickness = 0.3f;
        [SerializeField] private int _maxThicknessIterations = 5;
        [SerializeField] private float _zoomStepSize = 1.0f;
        [SerializeField] private float _zoomSpeedPerFrame = 0.1f;
        [SerializeField] private LayerMask _collisionLayers;

        private bool _hasGroundHit;

        private bool _isInitialized;
        private Vector3 _previousTargetPosition;
        private Vector3 _previousPosition;
        private Vector3 _currentTargetPosition;
        private float _currentThickness;

        // 组件引用
        private CameraThicknessDetector _thicknessDetector;
        private CameraOcclusionDetector _occlusionDetector;
        private CameraZoomController _zoomController;
        private CameraCollisionHandler _collisionHandler;

        private Vector3 _targetPositionWithOffset;
        private Vector3 _directionToTarget;


        public Transform Target => _target;

        public Vector3 TargetOffset => _targetOffset;

        public float ZoomStepSize => _zoomStepSize;

        public bool HasGroundHit => _hasGroundHit;

        void Awake()
        {
            _isInitialized = false;

            // 初始化组件
            InitializeComponents();

            InitializeFromTarget();
        }

        private void InitializeComponents()
        {
            _thicknessDetector = new CameraThicknessDetector
            {
                MaxObjectThickness = _maxObjectThickness,
                MaxThicknessIterations = _maxThicknessIterations,
                CameraRadius = _cameraRadius,
                TargetDistance = _targetDistance,
                CollisionLayers = _collisionLayers
            };

            _occlusionDetector = new CameraOcclusionDetector
            {
                CameraRadius = _cameraRadius,
                CollisionLayers = _collisionLayers
            };

            _zoomController = new CameraZoomController
            {
                TargetDistance = _targetDistance,
                ZoomStepSize = _zoomStepSize,
                ZoomSpeedPerFrame = _zoomSpeedPerFrame,
                CameraRadius = _cameraRadius,
                CollisionLayers = _collisionLayers
            };
            _zoomController.SetInitialDistance(_targetDistance);

            _collisionHandler = new CameraCollisionHandler
            {
                CameraRadius = _cameraRadius,
                CollisionLayers = _collisionLayers
            };

            // 订阅组件事件
            SubscribeToComponentEvents();
        }

        private void SubscribeToComponentEvents()
        {
            _thicknessDetector.OnThicknessDetected += thickness => _currentThickness = thickness;
            _zoomController.OnZoomChanged += distance =>
            {
                /* 距离已由组件管理 */
            };
            _collisionHandler.OnGroundDetected += hit => _hasGroundHit = hit;
            _collisionHandler.OnOffsetClipped += position =>
            {
                /* 偏移裁剪已由组件处理 */
            };
        }

        void LateUpdate()
        {
            if (_target == null)
                return;

            if (!_isInitialized)
                return;

            // 更新组件参数
            UpdateComponentParameters();

            // 1. 计算基础位置
            Vector3 targetOffsetTransformed = _target.rotation * _targetOffset;
            Vector3 cameraOffsetTransformed = transform.rotation * _cameraOffset;

            _currentTargetPosition = _target.position;
            _targetPositionWithOffset = _currentTargetPosition + targetOffsetTransformed;
            Vector3 targetPositionWithCameraOffset = _targetPositionWithOffset + cameraOffsetTransformed;

            // 2. 处理偏移裁剪
            targetPositionWithCameraOffset =
                _collisionHandler.HandleOffsetClipping(_targetPositionWithOffset, targetPositionWithCameraOffset);

            // 3. 计算方向和距离
            _directionToTarget =
                (transform.rotation * (new Vector3(0, 0, -_zoomController.CurrentDistance) + _cameraOffset) +
                 targetOffsetTransformed + _currentTargetPosition) - targetPositionWithCameraOffset;
            float cameraToPlayerDistance = Mathf.Min(_directionToTarget.magnitude, _targetDistance);
            _directionToTarget = _directionToTarget.normalized;

            // 4. 触发遮挡检测
            _occlusionDetector.DetectOcclusion(targetPositionWithCameraOffset, _directionToTarget, _targetDistance,
                _targetPositionWithOffset);

            // 5. 更新厚度
            if (_occlusionDetector.OcclusionHit != null)
            {
                _thicknessDetector.DetectThickness(transform.position, _targetPositionWithOffset, _directionToTarget);
            }
            else
            {
                _thicknessDetector.ResetThickness();
            }

            // 6. 正常模式下更新缩放
            _zoomController.UpdateZoom(targetOffsetTransformed, cameraOffsetTransformed,
                targetPositionWithCameraOffset, transform.rotation, _currentTargetPosition, _currentThickness);

            // 7. 检测地面碰撞
            _collisionHandler.CheckGroundHit(_previousPosition, _occlusionDetector.OcclusionHit != null,
                true);

            // 8. 应用相机位置
            ApplyCameraPosition(targetOffsetTransformed, cameraOffsetTransformed, targetPositionWithCameraOffset);

            // 9. 保存状态
            _previousTargetPosition = _currentTargetPosition;
            _previousPosition = transform.position;
        }

        private void UpdateComponentParameters()
        {
            _thicknessDetector.CameraRadius = _cameraRadius;
            _thicknessDetector.TargetDistance = _targetDistance;
            _thicknessDetector.CollisionLayers = _collisionLayers;

            _occlusionDetector.CameraRadius = _cameraRadius;
            _occlusionDetector.CollisionLayers = _collisionLayers;

            _zoomController.TargetDistance = _targetDistance;
            _zoomController.ZoomStepSize = _zoomStepSize;
            _zoomController.ZoomSpeedPerFrame = _zoomSpeedPerFrame;
            _zoomController.CameraRadius = _cameraRadius;
            _zoomController.CollisionLayers = _collisionLayers;

            _collisionHandler.CameraRadius = _cameraRadius;
            _collisionHandler.CollisionLayers = _collisionLayers;
        }

        /// <summary>
        /// 旋转相机到指定旋转
        /// </summary>
        public void RotateTo(Quaternion targetRotation, float timeModifier)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timeModifier);
        }

        /// <summary>
        /// 从目标初始化
        /// </summary>
        public void InitializeFromTarget()
        {
            InitializeFromTarget(_target);
        }

        /// <summary>
        /// 从目标初始化（用于切换角色）
        /// </summary>
        public void InitializeFromTarget(Transform newTarget)
        {
            _target = newTarget;

            if (_target != null)
            {
                _previousTargetPosition = _target.position;
                _previousPosition = transform.position;
                _currentTargetPosition = _target.position;

                _isInitialized = true;
            }
        }

        private void ApplyCameraPosition(Vector3 targetOffsetTransformed, Vector3 cameraOffsetTransformed,
            Vector3 targetPositionWithCameraOffset)
        {
            if (_occlusionDetector.OcclusionHit != null)
            {
                if (_currentThickness > _maxObjectThickness)
                {
                    transform.position = _occlusionDetector.OcclusionHit.Value.point +
                                         _occlusionDetector.OcclusionHit.Value.normal.normalized * _cameraRadius;
                }
                else
                {
                    transform.position =
                        (transform.rotation *
                         (new Vector3(0, 0, -_zoomController.CurrentDistance) + _cameraOffset)) +
                        targetOffsetTransformed + _currentTargetPosition;
                }
            }
            else
            {
                _currentThickness = float.MaxValue;
                transform.position =
                    transform.rotation * (new Vector3(0, 0, -_zoomController.CurrentDistance) + _cameraOffset) +
                    targetOffsetTransformed + _currentTargetPosition;
            }
        }

        #region Zoom API

        /// <summary>
        /// 设置目标缩放距离（带范围限制）
        /// </summary>
        /// <param name="distance">目标距离</param>
        /// <param name="minDistance">最小距离限制</param>
        /// <param name="maxDistance">最大距离限制</param>
        public void SetZoomDistance(float distance, float minDistance = 0.1f, float maxDistance = float.MaxValue)
        {
            if (_zoomController != null)
            {
                _zoomController.SetTargetDistance(distance, minDistance, maxDistance);
            }
        }

        /// <summary>
        /// 放大（减少相机距离）
        /// </summary>
        /// <param name="amount">缩放量，默认使用ZoomStepSize</param>
        /// <param name="minDistance">最小距离限制</param>
        public void ZoomIn(float amount = -1f, float minDistance = 0.1f)
        {
            if (_zoomController != null)
            {
                float delta = amount < 0 ? -_zoomStepSize : -amount;
                _zoomController.AdjustTargetDistance(delta, minDistance, float.MaxValue);
            }
        }

        /// <summary>
        /// 缩小（增加相机距离）
        /// </summary>
        /// <param name="amount">缩放量，默认使用ZoomStepSize</param>
        /// <param name="maxDistance">最大距离限制</param>
        public void ZoomOut(float amount = -1f, float maxDistance = float.MaxValue)
        {
            if (_zoomController != null)
            {
                float delta = amount < 0 ? _zoomStepSize : amount;
                _zoomController.AdjustTargetDistance(delta, 0.1f, maxDistance);
            }
        }

        /// <summary>
        /// 调整目标距离（相对调整）
        /// </summary>
        /// <param name="delta">距离变化量（正值增加距离，负值减少距离）</param>
        /// <param name="minDistance">最小距离限制</param>
        /// <param name="maxDistance">最大距离限制</param>
        public void AdjustZoom(float delta, float minDistance = 0.1f, float maxDistance = float.MaxValue)
        {
            if (_zoomController != null)
            {
                _zoomController.AdjustTargetDistance(delta, minDistance, maxDistance);
            }
        }

        /// <summary>
        /// 重置缩放到初始距离
        /// </summary>
        public void ResetZoom()
        {
            if (_zoomController != null)
            {
                _zoomController.SetTargetDistance(_targetDistance);
            }
        }

        #endregion
    }
}
