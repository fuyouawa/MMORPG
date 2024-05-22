using System;
using System.Linq;
using UnityEngine;

namespace MMORPG.Tool
{
    public static class UnityExtension
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


        public static bool Contain(this LayerMask layerMask, int layer)
        {
            return ((1 << layer) & layerMask) != 0;
        }

        public static bool HasParam(this Animator animator, string name)
        {
            return animator.parameters.FirstOrDefault(x => x.name == name) != null;
        }

        public static bool HasParam(this Animator animator, string name, AnimatorControllerParameterType typeCheck)
        {
            foreach (var parameter in animator.parameters)
            {
                
            }
            return animator.parameters.FirstOrDefault(x => x.name == name && x.type == typeCheck) != null;
        }
    }
}
