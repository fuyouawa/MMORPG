using System;
using System.Collections.Generic;
using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// 相机厚度检测组件，负责检测阻挡物体的厚度
    /// </summary>
    public class CameraThicknessDetector
    {
        /// <summary>厚度检测完成事件</summary>
        public event Action<float> OnThicknessDetected;

        /// <summary>最大物体厚度</summary>
        public float MaxObjectThickness { get; set; }

        /// <summary>最大厚度迭代次数</summary>
        public int MaxThicknessIterations { get; set; }

        /// <summary>相机半径</summary>
        public float CameraRadius { get; set; }

        /// <summary>目标距离</summary>
        public float TargetDistance { get; set; }

        /// <summary>碰撞检测图层</summary>
        public LayerMask CollisionLayers { get; set; }

        private float _currentThickness;
        private int _currentIterations;
        private Dictionary<string, RaycastHit> _thicknessStartHits;
        private Dictionary<string, RaycastHit> _thicknessEndHits;
        private Vector3 _thicknessStartPoint;
        private Vector3 _thicknessEndPoint;
        private RaycastHit _thicknessHit;
        private Vector3 _directionToTarget;

        public CameraThicknessDetector()
        {
            _currentIterations = 0;
            _thicknessStartHits = new Dictionary<string, RaycastHit>();
            _thicknessEndHits = new Dictionary<string, RaycastHit>();
            _currentThickness = float.MaxValue;
        }

        /// <summary>
        /// 检测物体厚度
        /// </summary>
        /// <param name="cameraPosition">相机位置</param>
        /// <param name="targetPositionWithOffset">目标带偏移的位置</param>
        /// <param name="directionToTarget">指向目标的向量</param>
        /// <returns>检测到的厚度</returns>
        public float DetectThickness(Vector3 cameraPosition, Vector3 targetPositionWithOffset, Vector3 directionToTarget)
        {
            _currentIterations = 0;
            _thicknessStartHits.Clear();
            _thicknessEndHits.Clear();

            _directionToTarget = directionToTarget;
            Vector3 dirToHit = (cameraPosition - targetPositionWithOffset).normalized;

            Vector3 hitVector = (targetPositionWithOffset - _thicknessHit.point);
            Vector3 targetVector = targetPositionWithOffset - cameraPosition;

            float dotProd = Vector3.Dot(hitVector, targetVector) / targetVector.magnitude;
            Vector3 unknownPoint = _thicknessHit.point + targetVector.normalized * dotProd;

            _thicknessStartPoint = unknownPoint + dirToHit * (TargetDistance + CameraRadius);
            _thicknessEndPoint = unknownPoint;

            float length = TargetDistance + CameraRadius;

            while (Physics.SphereCast(_thicknessEndPoint, CameraRadius, dirToHit, out _thicknessHit, length, CollisionLayers) && _currentIterations < MaxThicknessIterations)
            {
                length -= (_thicknessEndPoint - _thicknessHit.point).magnitude - 0.00001f;
                _thicknessEndPoint = _thicknessHit.point + _directionToTarget * 0.00001f;
                if (!_thicknessEndHits.ContainsKey(_thicknessHit.collider.name))
                    _thicknessEndHits.Add(_thicknessHit.collider.name, _thicknessHit);

                _currentIterations++;
            }

            _currentIterations = 0;
            length = TargetDistance;

            while (Physics.SphereCast(_thicknessStartPoint, CameraRadius, -dirToHit, out _thicknessHit, length, CollisionLayers) && _currentIterations < MaxThicknessIterations)
            {
                length -= (_thicknessStartPoint - _thicknessHit.point).magnitude - 0.00001f;
                _thicknessStartPoint = _thicknessHit.point - _directionToTarget * 0.00001f;

                if (!_thicknessStartHits.ContainsKey(_thicknessHit.collider.name))
                    _thicknessStartHits.Add(_thicknessHit.collider.name, _thicknessHit);

                _currentIterations++;
            }

            float tmpThickness = float.MaxValue;

            if (_thicknessEndHits.Count > 0 && _thicknessStartHits.Count > 0 && _thicknessEndHits.Count == _thicknessStartHits.Count)
            {
                bool thicknessFound = false;
                string currentColliderName = "";

                var enumerator = _thicknessEndHits.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    currentColliderName = enumerator.Current.Value.collider.name;

                    if (_thicknessStartHits.ContainsKey(currentColliderName))
                    {
                        if (!thicknessFound)
                        {
                            tmpThickness = 0;
                            thicknessFound = true;
                        }
                        tmpThickness += (_thicknessStartHits[currentColliderName].point - _thicknessEndHits[currentColliderName].point).magnitude;
                    }
                }
            }

            _currentThickness = tmpThickness;
            OnThicknessDetected?.Invoke(_currentThickness);
            return _currentThickness;
        }

        /// <summary>
        /// 重置厚度为最大值
        /// </summary>
        public void ResetThickness()
        {
            _currentThickness = float.MaxValue;
            OnThicknessDetected?.Invoke(_currentThickness);
        }

        /// <summary>当前厚度</summary>
        public float CurrentThickness => _currentThickness;
    }
}
