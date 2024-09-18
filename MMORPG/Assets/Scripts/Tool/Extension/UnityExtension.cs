using System;
using System.Linq;
using UnityEngine;

namespace MMORPG.Tool
{
    public class TransformLocalStore
    {
        public Transform OriginParent;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;
    }
    
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
            
            return animator.parameters.FirstOrDefault(x => x.name == name && x.type == typeCheck) != null;
        }

        public static string ToHex(this Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255f);
            int g = Mathf.RoundToInt(color.g * 255f);
            int b = Mathf.RoundToInt(color.b * 255f);
            int a = Mathf.RoundToInt(color.a * 255f);
            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }

        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        public static void PlayWithChildren(this AudioSource audio)
        {
            foreach (var a in audio.GetComponentsInChildren<AudioSource>())
            {
                a.Play();
            }
        }

        public static TransformLocalStore GetLocalStore(this Transform transform)
        {
            return new TransformLocalStore()
            {
                OriginParent = transform.parent,
                LocalPosition = transform.localPosition,
                LocalRotation = transform.localRotation,
                LocalScale = transform.localScale,
            };
        }

        public static void FromLocalStore(this Transform transform, TransformLocalStore localStore)
        {
            transform.SetParent(localStore.OriginParent);
            transform.localPosition = localStore.LocalPosition;
            transform.localRotation = localStore.LocalRotation;
            transform.localScale = localStore.LocalScale;
        }
    }
}
