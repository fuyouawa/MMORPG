using UnityEngine;
using System.Collections.Generic;

namespace DuloGames.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, AddComponentMenu("UI/UI Scene/Manager")]
    public class UISceneManager : MonoBehaviour
    {
        private static UISceneManager m_Instance;

        /// <summary>
        /// Get the scene manager if any exists.
        /// </summary>
        public static UISceneManager instance
        {
            get { return m_Instance; }
        }

        private List<UIScene> m_Scenes;

        /// <summary>
        /// Get all the registered scenes.
        /// </summary>
        public UIScene[] scenes
        {
            get { return this.m_Scenes.ToArray(); }
        }

        protected void Awake()
        {
            // Check if we already have a scene manager
            if (m_Instance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }

                Debug.LogWarning("Multiple UISceneManagers are not allowed, destroying.");
                return;
            }

            // Save reference in the static variable
            m_Instance = this;

            // Prepare the list
            if (this.m_Scenes == null)
            {
                this.m_Scenes = new List<UIScene>();
            }

            // Prevent destruction on scene load
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        protected void OnDestroy()
        {
            // Remove reference
            m_Instance = null;
        }

        /// <summary>
        /// Register a scene.
        /// </summary>
        /// <param name="scene"></param>
        public void RegisterScene(UIScene scene)
        {
            // Make sure we have the list set
            if (this.m_Scenes == null)
            {
                this.m_Scenes = new List<UIScene>();
            }

            // Check if already registered
            if (this.m_Scenes.Contains(scene))
            {
                Debug.LogWarning("Trying to register a UIScene multiple times.");
                return;
            }

            // Store in the list
            this.m_Scenes.Add(scene);
        }

        /// <summary>
        /// Unregister a scene.
        /// </summary>
        /// <param name="scene"></param>
        public void UnregisterScene(UIScene scene)
        {
            if (this.m_Scenes != null)
            {
                this.m_Scenes.Remove(scene);
            }
        }

        /// <summary>
        /// Get all the active scenes.
        /// </summary>
        /// <returns></returns>
        public UIScene[] GetActiveScenes()
        {
            List<UIScene> activeScenes = this.m_Scenes.FindAll(x => x.isActivated == true);

            return activeScenes.ToArray();
        }

        /// <summary>
        /// Get the scene with the specified id. Returns null if not found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIScene GetScene(int id)
        {
            if (this.m_Scenes == null || this.m_Scenes.Count == 0)
            {
                return null;
            }

            return this.m_Scenes.Find(x => x.id == id);
        }

        /// <summary>
        /// Get the next available scene id;
        /// </summary>
        /// <returns></returns>
        public int GetAvailableSceneId()
        {
            if (this.m_Scenes.Count == 0)
            {
                return 0;
            }

            int id = 0;

            foreach (UIScene scene in this.m_Scenes)
            {
                if (scene.id > id)
                {
                    id = scene.id;
                }
            }

            return id + 1;
        }

        /// <summary>
        /// Transitions out of the active scene and in to the new one.
        /// </summary>
        /// <param name="scene"></param>
        public void TransitionToScene(UIScene scene)
        {
            UIScene.Transition transition = scene.transition;
            float transitionDuration = scene.transitionDuration;
            Tweens.TweenEasing transitionEasing = scene.transitionEasing;

            // Transition out of the current scenes
            UIScene[] activeScenes = this.GetActiveScenes();

            foreach (UIScene activeScene in activeScenes)
            {
                // Transition the scene out
                activeScene.TransitionOut(transition, transitionDuration, transitionEasing);
            }

            // Transition in the new scene
            scene.TransitionIn(transition, transitionDuration, transitionEasing);
        }
    }
}
