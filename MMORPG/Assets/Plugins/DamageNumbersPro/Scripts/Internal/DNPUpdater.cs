using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

namespace DamageNumbersPro.Internal
{
    public class DNPUpdater : MonoBehaviour
    {
        //Dicitonary:
        static Dictionary<float, DNPUpdater> unscaledUpdaters;
        static Dictionary<float, DNPUpdater> scaledUpdaters;

        //Static:
        public static Vector3 upVector;
        public static Vector3 rightVector;
        public static bool vectorsNeedUpdate;
        public static Quaternion cameraRotation;

        //Settings:
        public bool isUnscaled = false;
        public float updateDelay = 0.0125f;
        public HashSet<DamageNumber> activePopups;
        public HashSet<DamageNumber> removedPopups;

        //Internal:
        float lastUpdateTime = 0;
        float delta = 0;
        float time = 0;

        void Start()
        {
            StartCoroutine(UpdatePopups());
        }

        IEnumerator UpdatePopups()
        {
            //Delay:
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(updateDelay);

            while(true)
            {
                //Vector Update:
                vectorsNeedUpdate = true;

                //Update:
                foreach (DamageNumber popup in activePopups)
                {
                    if(popup != null)
                    {
                        popup.UpdateDamageNumber(delta, time);
                    }
                    else
                    {
                        removedPopups.Add(popup);
                    }
                }

                //Clean Up:
                if(removedPopups.Count > 0)
                {
                    foreach (DamageNumber removed in removedPopups)
                    {
                        activePopups.Remove(removed);
                    }
                    removedPopups = new HashSet<DamageNumber>();
                }

                //Wait:
                if (isUnscaled)
                {
                    lastUpdateTime = Time.unscaledTime;
                    yield return delay;
                    time = Time.unscaledTime;
                    delta = time - lastUpdateTime;
                }
                else
                {
                    lastUpdateTime = Time.time;
                    yield return delay;
                    time = Time.time;
                    delta = time - lastUpdateTime;
                }
            }
        }

        public static void RegisterPopup(bool unscaledTime, float updateDelay, DamageNumber popup)
        {
            ref Dictionary<float, DNPUpdater> updaters = ref unscaledTime ? ref unscaledUpdaters : ref scaledUpdaters;

            if (updaters == null)
            {
                updaters = new Dictionary<float, DNPUpdater>();
            }

            bool containsKey = updaters.ContainsKey(updateDelay);
            if (containsKey && updaters[updateDelay] != null)
            {
                updaters[updateDelay].activePopups.Add(popup);
            }
            else
            {
                if(containsKey)
                {
                    updaters.Remove(updateDelay);
                }

                GameObject newUpdater = new GameObject("");
                newUpdater.hideFlags = HideFlags.HideInHierarchy;

                DNPUpdater dnpUpdater = newUpdater.AddComponent<DNPUpdater>();
                dnpUpdater.activePopups = new HashSet<DamageNumber>();
                dnpUpdater.removedPopups = new HashSet<DamageNumber>();
                dnpUpdater.isUnscaled = unscaledTime;
                dnpUpdater.updateDelay = updateDelay;
                DontDestroyOnLoad(newUpdater);

                updaters.Add(updateDelay, dnpUpdater);

                dnpUpdater.activePopups.Add(popup);
            }
        }

        public static void UnregisterPopup(bool unscaledTime, float updateDelay, DamageNumber popup)
        {
            Dictionary<float, DNPUpdater> updaters = unscaledTime ? unscaledUpdaters : scaledUpdaters;

            if (updaters != null && updaters.ContainsKey(updateDelay) && updaters[updateDelay].activePopups.Contains(popup))
            {
                updaters[updateDelay].removedPopups.Add(popup);
            }
        }

        public static void UpdateVectors(Transform popup)
        {
            vectorsNeedUpdate = false;
            upVector = popup.up;
            rightVector = popup.right;
        }
    }
}
