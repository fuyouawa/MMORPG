using System;
using System.Collections;
using MMORPG.Tool;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThunderMaker : MonoBehaviour
{
    public float StartDelay;
    [Serializable]
    public struct Thunder
    {
        public AudioSource Audio;
        public float Duration;
        public float DelayBeforePlay;
        public float MaxIntensity;
    }

    public Thunder[] Thunders;
    public Light DirectionalLight;

    public AnimationCurve LightIntensityCurve;

    public Vector2 IntervalRange;
    public Vector2Int CountRange;

    private float _originLightIntensity;

    private void Start()
    {
        _originLightIntensity = DirectionalLight.intensity;
        StartCoroutine("ThunderCo");
    }

    private IEnumerator ThunderCo()
    {
        yield return new WaitForSeconds(StartDelay);
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(IntervalRange.x, IntervalRange.y));
            var count = Random.Range(CountRange.x, CountRange.y);

            for (int i = 0; i < count; i++)
            {
                var thunder = Thunders[Random.Range(0, Thunders.Length)];
                StartCoroutine(PlayAudioCo(thunder.Audio, thunder.DelayBeforePlay));
                var d = 0f;
                while (d <= thunder.Duration)
                {
                    DirectionalLight.intensity = _originLightIntensity +
                                                 MathHelper.Remap(
                                                     LightIntensityCurve.Evaluate(MathHelper.Remap(d, 0f, thunder.Duration, 0f, 1f))
                                                     , 0f, 1f, 0f, thunder.MaxIntensity);
                    yield return null;
                    d += Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator PlayAudioCo(AudioSource audio, float delay)
    {
        yield return new WaitForSeconds(delay);
        audio.Play();
    }
}
