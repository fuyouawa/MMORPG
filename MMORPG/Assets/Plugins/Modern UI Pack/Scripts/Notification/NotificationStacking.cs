using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.MUIP
{
    [AddComponentMenu("Modern UI Pack/Notification/Notification Stacking")]
    public class NotificationStacking : MonoBehaviour
    {
        public List<NotificationManager> notifications = new List<NotificationManager>();
        [HideInInspector] public bool enableUpdating = false;

        [Header("Settings")]
        public float delay = 1;
        int currentNotification = 0;

        void Update()
        {
            if (enableUpdating == true)
            {
                try
                {
                    notifications[currentNotification].gameObject.SetActive(true);

                    if (notifications[currentNotification].notificationAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
                    {
                        notifications[currentNotification].OpenNotification();
                        StartCoroutine("StartNotification");
                        enableUpdating = false;
                    }

                    if (currentNotification >= notifications.Count)
                    {
                        enableUpdating = false;
                        currentNotification = 0;
                    }
                }

                catch
                {
                    enableUpdating = false;
                    currentNotification = 0;
                    notifications.Clear();
                }
            }
        }

        IEnumerator StartNotification()
        {
            yield return new WaitForSeconds(notifications[currentNotification].timer + delay);
            Destroy(notifications[currentNotification].gameObject);
            // notifications.Remove(notifications[currentNotification]);
            enableUpdating = true;
            currentNotification += 1;
            StopCoroutine("StartNotification");
        }
    }
}