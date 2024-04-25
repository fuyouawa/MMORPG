using UnityEngine;
using System;

namespace DuloGames.UI
{
	public static class UIUtility {

        /// <summary>
        /// Brings the game object to the front.
        /// </summary>
        /// <param name="go">Game Object.</param>
        public static void BringToFront(GameObject go)
        {
            BringToFront(go, true);
        }

        /// <summary>
        /// Brings the game object to the front while specifing if re-parenting is allowed.
        /// </summary>
        /// <param name="go">The Game Object.</param>
        /// <param name="allowReparent">Should we allow the method to change the Game Object's parent.</param>
        public static void BringToFront(GameObject go, bool allowReparent)
        {
            Transform root = null;

            // Check if this game object is part of a UI Scene
            UIScene scene = UIUtility.FindInParents<UIScene>(go);

            // If the object has a parent ui scene
            if (scene != null && scene.content != null)
            {
                root = scene.content;
            }
            else
            {
                // Use canvas as root
                Canvas canvas = UIUtility.FindInParents<Canvas>(go);
                if (canvas != null) root = canvas.transform;
            }

            // If the object has a parent canvas
            if (allowReparent && root != null)
                go.transform.SetParent(root, true);

            // Set as last sibling
            go.transform.SetAsLastSibling();

            // Handle the always on top components
            if (root != null)
            {
                UIAlwaysOnTop[] alwaysOnTopComponenets = root.gameObject.GetComponentsInChildren<UIAlwaysOnTop>();

                if (alwaysOnTopComponenets.Length > 0)
                {
                    // Sort them by order
                    Array.Sort(alwaysOnTopComponenets);

                    foreach (UIAlwaysOnTop component in alwaysOnTopComponenets)
                    {
                        component.transform.SetAsLastSibling();
                    }
                }
            }
        }

        /// <summary>
        /// Finds the component in the game object's parents.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="go">Game Object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T FindInParents<T>(GameObject go) where T : Component
		{
			if (go == null)
				return null;
			
			var comp = go.GetComponent<T>();
			
			if (comp != null)
				return comp;
			
			Transform t = go.transform.parent;
			
			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}
			
			return comp;
		}
	}
}
