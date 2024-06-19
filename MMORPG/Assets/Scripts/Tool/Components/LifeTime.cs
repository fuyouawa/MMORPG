

using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    public class LifeTime : MonoBehaviour
    {
        public bool EnableLifeTime = true;
        public float DestroyLifeTime = 1f;
        [ShowInInspector]
        public float RemainingTime { get; private set; }

        private void Start()
        {
            RemainingTime = DestroyLifeTime;
        }
        private void Update()
        {
            if (EnableLifeTime)
            {
                if (RemainingTime <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    RemainingTime -= Time.deltaTime;
                }
            }
        }
    }
}
