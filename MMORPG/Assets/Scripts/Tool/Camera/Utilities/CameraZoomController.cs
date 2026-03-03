using System;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 相机缩放控制组件，负责平滑调整相机距离
    /// </summary>
    public class CameraZoomController
    {
        /// <summary>缩放事件</summary>
        public event Action<float> OnZoomChanged;

        /// <summary>目标距离</summary>
        public float TargetDistance { get; set; }

        /// <summary>缩放步长</summary>
        public float ZoomStepSize { get; set; }

        /// <summary>每帧缩放速度</summary>
        public float ZoomSpeedPerFrame { get; set; }

        /// <summary>相机半径</summary>
        public float CameraRadius { get; set; }

        /// <summary>碰撞检测图层</summary>
        public LayerMask CollisionLayers { get; set; }

        private float _currentDistance;

        /// <summary>当前距离</summary>
        public float CurrentDistance => _currentDistance;

        /// <summary>
        /// 设置目标距离（带限制）
        /// </summary>
        /// <param name="distance">目标距离</param>
        /// <param name="minDistance">最小距离限制</param>
        /// <param name="maxDistance">最大距离限制</param>
        public void SetTargetDistance(float distance, float minDistance = 0.1f, float maxDistance = float.MaxValue)
        {
            TargetDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        /// <summary>
        /// 调整目标距离（相对调整）
        /// </summary>
        /// <param name="delta">距离变化量</param>
        /// <param name="minDistance">最小距离限制</param>
        /// <param name="maxDistance">最大距离限制</param>
        public void AdjustTargetDistance(float delta, float minDistance = 0.1f, float maxDistance = float.MaxValue)
        {
            TargetDistance = Mathf.Clamp(TargetDistance + delta, minDistance, maxDistance);
        }

        public CameraZoomController()
        {
            _currentDistance = 5.0f;
        }

        /// <summary>
        /// 设置初始距离
        /// </summary>
        public void SetInitialDistance(float distance)
        {
            _currentDistance = distance;
        }

        /// <summary>
        /// 更新缩放（正常模式）
        /// </summary>
        /// <param name="targetOffsetTransformed">变换后的目标偏移</param>
        /// <param name="cameraOffsetTransformed">变换后的相机偏移</param>
        /// <param name="targetPositionWithCameraOffset">带相机偏移的目标位置</param>
        /// <param name="cameraRotation">相机旋转</param>
        /// <param name="currentTargetPosition">当前目标位置</param>
        /// <param name="objectThickness">物体厚度</param>
        public void UpdateZoom(Vector3 targetOffsetTransformed, Vector3 cameraOffsetTransformed,
            Vector3 targetPositionWithCameraOffset, Quaternion cameraRotation, Vector3 currentTargetPosition, float objectThickness)
        {
            if (_currentDistance < TargetDistance)
            {
                Vector3 dirToTargetDistanced = ((cameraRotation * (new Vector3(0, 0, -_currentDistance - ZoomSpeedPerFrame) + cameraOffsetTransformed) + targetOffsetTransformed + currentTargetPosition) - targetPositionWithCameraOffset);

                RaycastHit zoomOutHit;
                if (objectThickness > 0.3f && Physics.SphereCast(targetPositionWithCameraOffset, CameraRadius, dirToTargetDistanced.normalized, out zoomOutHit, dirToTargetDistanced.magnitude, CollisionLayers))
                {
                    _currentDistance = (zoomOutHit.point - targetPositionWithCameraOffset).magnitude - CameraRadius;
                }
                else
                {
                    _currentDistance += ZoomSpeedPerFrame;
                }
            }

            if (_currentDistance > TargetDistance)
            {
                _currentDistance -= ZoomSpeedPerFrame;
            }

            OnZoomChanged?.Invoke(_currentDistance);
        }
    }
}
