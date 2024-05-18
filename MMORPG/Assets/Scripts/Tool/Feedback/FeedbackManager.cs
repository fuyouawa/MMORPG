using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
// using QFramework;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace MMORPG.Tool
{
    public abstract class BaseClass
    {
        public int BaseField;
    }

    public class FeedbackManager : SerializedMonoBehaviour
    {
        public bool AutoPlayOnStart;
        public bool AutoPlayOnEnable;

        [TypeFilter("GetFilteredFeedbackTypes")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        public List<Feedback> Feedbacks = new();

#if UNITY_EDITOR
        private static Type[] s_allFeedbackTypes;

        static FeedbackManager()
        {
            s_allFeedbackTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(Feedback).IsAssignableFrom(t) && t != typeof(Feedback)
                select t).ToArray();
        }

        public IEnumerable<Type> GetFilteredFeedbackTypes()
        {
            return s_allFeedbackTypes;
        }
#endif

        public GameObject Owner { get; private set; }

        public void Setup(GameObject owner)
        {
            Owner = owner;
        }

        private void Awake()
        {
            Feedbacks.ForEach(x =>
            {
                x.Setup(Owner, this);
                x.Awake();
            });
        }

        private void Start()
        {
            Feedbacks.ForEach(x => x.Start());
;       }

        private void Update()
        {
            Feedbacks.ForEach(x => x.Update());
        }

        public void PlayFeedbacks()
        {
            Feedbacks.ForEach(x => x.Play());
        }

        public void StopFeedbacks()
        {
            Feedbacks.ForEach(x => x.Stop());
        }

        protected virtual void OnDestroy()
        {
            Feedbacks.ForEach(x => x.OnDestroy());
        }
#if UNITY_EDITOR
        [HorizontalGroup(Title = "Test (Runtime only)")]
        [Button]
        private void TestPlay()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "只能在运行时测试!", "确定");
                return;
            }
            PlayFeedbacks();
        }

        [HorizontalGroup]
        [Button]
        private void TestStop()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorUtility.DisplayDialog("错误", "只能在运行时测试!", "确定");
                return;
            }
            StopFeedbacks();
        }
#endif
    }
}
