using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public class CinemachineCameraShaker : MonoBehaviour
    {
        [ReadOnly]
        public float IdleAmplitude;
        [ReadOnly]
        public float IdleFrequency = 1f;
        public float LerpSpeed = 5f;

        [Title("ShakeTest")]
        public float TestDuration = 0.3f;
        public float TestAmplitude = 2f;
        public float TestFrequency = 20f;

        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _perlin;
        private float _targetAmplitude;
        private float _targetFrequency;
        private Coroutine _shakeCoroutine;

        private void Awake()
        {
            _virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
            _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        protected void Start()
        {
            if (_perlin != null)
            {
                IdleAmplitude = _perlin.m_AmplitudeGain;
                IdleFrequency = _perlin.m_FrequencyGain;
            }

            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        protected void Update()
        {
            if (_perlin != null)
            {
                _perlin.m_AmplitudeGain = _targetAmplitude;
                _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, _targetFrequency, Time.deltaTime * LerpSpeed);
            }
        }

        public void ShakeCamera(float duration, float amplitude, float frequency, bool infinite = false, bool useUnscaledTime = false)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }
            _shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite, useUnscaledTime));
        }

        protected IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime)
        {
            _targetAmplitude = amplitude;
            _targetFrequency = frequency;

            if (!infinite)
            {
                yield return new WaitForSeconds(duration);
                CameraReset();
            }
        }
        public void CameraReset()
        {
            _targetAmplitude = IdleAmplitude;
            _targetFrequency = IdleFrequency;
        }

        [Button]
        [DisableInEditorMode]
        private void TestShake()
        {
            ShakeCamera(TestDuration, TestAmplitude, TestFrequency);
        }
    }
}
