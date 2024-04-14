using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TransformExtension
{
    public static Transform FindIncludeAllChildren(
        this Transform transform,
        string name,
        StringComparison stringComparison = StringComparison.Ordinal)
    {
        Transform FindChildRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                if (child.name.Equals(name, stringComparison))
                {
                    return child;
                }
                var result = FindChildRecursive(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        return FindChildRecursive(transform, name);
    }
}