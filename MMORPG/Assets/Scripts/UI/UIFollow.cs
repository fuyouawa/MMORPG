using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
	public class UIFollow : MonoBehaviour
    {
        public bool IsWorldUI;
        public Camera ObservationCamera;
        public Transform FollowTarget;
        public Vector3 Offset;

        void Start()
        {
            if (ObservationCamera == null)
            {
                ObservationCamera = Camera.main;
            }
        }

        void Update()
        {
            if (FollowTarget == null) return;

            if (!IsWorldUI)
            {
                transform.position = ObservationCamera.WorldToScreenPoint(FollowTarget.position + Offset);
            }
            else
            {
                transform.position = FollowTarget.position + Offset;
                transform.rotation = ObservationCamera.transform.rotation;
            }
        }
    }
}
