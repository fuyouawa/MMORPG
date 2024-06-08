using UnityEngine;

public class ItemController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = CalculateGroundPosition(transform.position, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 CalculateGroundPosition(Vector3 position, int layer)
    {
        int layerMask = 1 << layer;
        if (Physics.Raycast(position, Vector3.down, out var hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }
        return position;
    }
}
