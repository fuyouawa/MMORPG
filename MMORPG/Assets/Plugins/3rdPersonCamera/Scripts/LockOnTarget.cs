//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace ThirdPersonCamera
{
    public struct TargetableWithDistance
    { 
        public Targetable target {get;set;}
        public float distance { get; set; }
    }

    public class SortTargetables : IComparer<TargetableWithDistance>
    {
        public int Compare(TargetableWithDistance a, TargetableWithDistance b)
        {
            if (a.distance > b.distance) return 1;
            else if (a.distance < b.distance) return -1;
            else return 0;
        }
    }

    [RequireComponent(typeof(CameraController)), RequireComponent(typeof(FreeForm))]
    public class LockOnTarget : MonoBehaviour
    {
        public Targetable followTarget = null;
        public float rotationSpeed = 3.0f;
        public Vector3 tiltVector;
        public List<Targetable> targets;

        private CameraController cc;
        private FreeForm ff;
        private SortTargetables sortTargetsMethod;

        void Start()
        {
            cc = GetComponent<CameraController>();
            ff = GetComponent<FreeForm>();
            sortTargetsMethod = new SortTargetables(); // init the sort method
        }

        void Update()
        {
            if (Input.GetMouseButton(1)) // right mouse click starts lock on mode
            {
                if (followTarget == null)
                {
                    // find a viable target

                    targets = new List<Targetable>(FindObjectsOfType<Targetable>());

                    if (targets != null && targets.Count > 0)
                    {
                        // add target acquiring by distance 
                        // TODO get angle to find the best target
                        // TODO add target switching via input

                        List<TargetableWithDistance> items = new List<TargetableWithDistance>(targets.Count);
                        for (int i = 0; i < targets.Count; i++)
                        {
                            items.Add(new TargetableWithDistance() { target = targets[i], distance = (targets[i].transform.position - cc.target.transform.position).magnitude });
                        }

                        items.Sort(sortTargetsMethod);
                        
                        followTarget = items[0].target;
                    }
                }

                if (followTarget != null)
                {
                    ff.cameraEnabled = false; // disable freeform control
                    Vector3 dirToTarget = (followTarget.transform.position + followTarget.offset) - transform.position;
                    Quaternion toRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);

                    cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
                }
            }
            else if (followTarget != null)
            {
                followTarget = null;
                ff.cameraEnabled = true; // enable freeform control again
            }
        }
    }
}