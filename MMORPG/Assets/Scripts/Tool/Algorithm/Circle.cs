using UnityEngine;

namespace MMORPG.Tool
{
    public struct Circle
    {
        public Vector2 Center;
        public float Radius;

        public float Diameter => Radius * 2;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public Circle(float radius)
            : this(Vector2.zero, radius)
        {
        }

        public bool Contains(Vector2 point)
        {
            var distance = Vector2.Distance(point, Center);
            return distance < Radius;
        }
    }

}
