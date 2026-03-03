using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 射线检测结果，包含到目标的距离信息
    /// </summary>
    public readonly struct RaycastHitWithTargetDistance
    {
        public RaycastHit Hit { get; }
        public float TargetDistance { get; }

        public RaycastHitWithTargetDistance(RaycastHit hit, float targetDistance)
        {
            Hit = hit;
            TargetDistance = targetDistance;
        }
    }

    /// <summary>
    /// 相机遮挡检测组件，负责检测相机与目标之间的遮挡物
    /// </summary>
    public class CameraOcclusionDetector
    {
        /// <summary>遮挡检测事件</summary>
        public event Action<RaycastHit?> OnOcclusionDetected;

        /// <summary>相机半径</summary>
        public float CameraRadius { get; set; }

        /// <summary>碰撞检测图层</summary>
        public LayerMask CollisionLayers { get; set; }

        private RaycastHit? _occlusionHit = null;
        private RaycastHitWithTargetDistance[] _rayCastResultsArray;
        private readonly IComparer<RaycastHitWithTargetDistance> _rayCastResultComparer =
            Comparer<RaycastHitWithTargetDistance>.Create((a, b) => a.TargetDistance.CompareTo(b.TargetDistance));

        /// <summary>当前遮挡物命中</summary>
        public RaycastHit? OcclusionHit => _occlusionHit;

        /// <summary>
        /// 检测遮挡物
        /// </summary>
        /// <param name="origin">射线起点</param>
        /// <param name="direction">射线方向</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="targetPositionWithOffset">目标带偏移的位置</param>
        public void DetectOcclusion(Vector3 origin, Vector3 direction, float maxDistance, Vector3 targetPositionWithOffset)
        {
            RaycastHit[] hits = Physics.SphereCastAll(origin, CameraRadius, direction, maxDistance, CollisionLayers);
            FindOcclusionHit(hits, targetPositionWithOffset);
            OnOcclusionDetected?.Invoke(_occlusionHit);
        }

        private void FindOcclusionHit(RaycastHit[] hits, Vector3 targetPositionWithOffset)
        {
            if (hits.Length > 0)
            {
                // 重用或分配数组
                if (_rayCastResultsArray == null || _rayCastResultsArray.Length < hits.Length)
                {
                    _rayCastResultsArray = new RaycastHitWithTargetDistance[hits.Length];
                }

                int validCount = 0;
                for (int i = 0; i < hits.Length; i++)
                {
                    float targetDist = (targetPositionWithOffset - hits[i].point).magnitude;
                    _rayCastResultsArray[validCount] = new RaycastHitWithTargetDistance(hits[i], targetDist);
                    validCount++;
                }

                // 使用比较器排序
                Array.Sort(_rayCastResultsArray, 0, validCount, _rayCastResultComparer);

                float lowestDistance = _rayCastResultsArray[0].TargetDistance;
                float threshold = lowestDistance + CameraRadius * 2;

                // 就地过滤，保留阈值内的项
                int filteredCount = 0;
                for (int i = 0; i < validCount; i++)
                {
                    if (_rayCastResultsArray[i].TargetDistance <= threshold)
                    {
                        if (i != filteredCount)
                        {
                            _rayCastResultsArray[filteredCount] = _rayCastResultsArray[i];
                        }
                        filteredCount++;
                    }
                }

                if (filteredCount > 0 && _rayCastResultsArray[0].Hit.distance > 0)
                {
                    _occlusionHit = _rayCastResultsArray[0].Hit;
                }
                else
                {
                    _occlusionHit = null;
                }
            }
            else
            {
                _occlusionHit = null;
            }
        }
    }
}
