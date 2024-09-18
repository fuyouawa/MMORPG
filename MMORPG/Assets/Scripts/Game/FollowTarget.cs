using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform Target;

    public Vector3 Offset;

    void Update()
    {
        if (Target != null)
        {
            transform.position = Target.position + Offset;
        }
    }
}
