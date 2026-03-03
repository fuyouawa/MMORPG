using System;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 相机碰撞处理组件，处理相机与环境的碰撞
    /// </summary>
    public class CameraCollisionHandler
    {
        /// <summary>地面检测事件</summary>
        public event Action<bool> OnGroundDetected;

        /// <summary>偏移裁剪事件</summary>
        public event Action<Vector3> OnOffsetClipped;

        /// <summary>相机半径</summary>
        public float CameraRadius { get; set; }

        /// <summary>碰撞检测图层</summary>
        public LayerMask CollisionLayers { get; set; }

        private RaycastHit _offsetTestHit;
        private bool _hasGroundHit;

        /// <summary>是否有地面碰撞</summary>
        public bool HasGroundHit => _hasGroundHit;

        /// <summary>
        /// 处理偏移裁剪
        /// </summary>
        /// <param name="targetPositionWithOffset">带偏移的目标位置</param>
        /// <param name="targetPositionWithCameraOffset">带相机偏移的目标位置</param>
        /// <returns>处理后的位置</returns>
        public Vector3 HandleOffsetClipping(Vector3 targetPositionWithOffset, Vector3 targetPositionWithCameraOffset)
        {
            Vector3 testDir = targetPositionWithCameraOffset - targetPositionWithOffset;

            if (Physics.SphereCast(targetPositionWithOffset, CameraRadius, testDir, out _offsetTestHit, testDir.magnitude, CollisionLayers))
            {
                Vector3 clippedPosition = _offsetTestHit.point + _offsetTestHit.normal * CameraRadius * 2;
                OnOffsetClipped?.Invoke(clippedPosition);
                return clippedPosition;
            }

            return targetPositionWithCameraOffset;
        }

        /// <summary>
        /// 检测地面碰撞
        /// </summary>
        /// <param name="previousPosition">上一帧位置</param>
        /// <param name="hasOcclusionHit">是否有遮挡命中</param>
        /// <param name="isInNormalMode">是否在正常模式</param>
        public void CheckGroundHit(Vector3 previousPosition, bool hasOcclusionHit, bool isInNormalMode)
        {
            if (hasOcclusionHit || _hasGroundHit)
            {
                if (Physics.Raycast(previousPosition, Vector3.down, out _, CameraRadius + 0.1f))
                {
                    _hasGroundHit = true;
                }
                else
                    _hasGroundHit = false;
            }
            else if (!hasOcclusionHit && isInNormalMode)
            {
                _hasGroundHit = false;
            }

            OnGroundDetected?.Invoke(_hasGroundHit);
        }

        /// <summary>
        /// 重置地面碰撞状态
        /// </summary>
        public void ResetGroundHit()
        {
            _hasGroundHit = false;
            OnGroundDetected?.Invoke(_hasGroundHit);
        }
    }
}
