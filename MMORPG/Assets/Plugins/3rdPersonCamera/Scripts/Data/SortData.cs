//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2019 Thomas Enzenebner
//            Version 1.0.5.3
//         t.enzenebner@gmail.com
//----------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPersonCamera
{
    public struct RayCastWithMags
    {
        public RaycastHit hit;
        public float distanceFromCamera;
        public float distanceFromTarget;
    }

    public class TransformWithTime
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public float time;
    }

    public class SortRayCastsTarget : IComparer<RayCastWithMags>
    {
        public int Compare(RayCastWithMags a, RayCastWithMags b)
        {
            if (a.distanceFromTarget > b.distanceFromTarget) return 1;
            else if (a.distanceFromTarget < b.distanceFromTarget) return -1;
            else return 0;
        }
    }

    public class SortRayCastsCamera : IComparer<RayCastWithMags>
    {
        public int Compare(RayCastWithMags a, RayCastWithMags b)
        {
            if (a.distanceFromCamera > b.distanceFromCamera) return 1;
            else if (a.distanceFromCamera < b.distanceFromCamera) return -1;
            else return 0;
        }
    }
}
