using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using Sirenix.OdinInspector;
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

        [ValueDropdown("GetFeedbacksDropdown")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "GetLabel")]
        public Feedback[] Feedbacks = Array.Empty<Feedback>();

#if UNITY_EDITOR
        private static Type[] s_allFeedbackTypes;

        static FeedbackManager()
        {
            s_allFeedbackTypes = (from t in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(Feedback).IsAssignableFrom(t) && t != typeof(Feedback) && t.HasAttribute<AddFeedbackMenuAttribute>()
                select t).ToArray();
        }

        public IEnumerable GetFeedbacksDropdown()
        {
            var total = new ValueDropdownList<Feedback>();
            foreach (var type in s_allFeedbackTypes)
            {
                var attr = type.GetAttribute<AddFeedbackMenuAttribute>();
                Debug.Assert(attr != null);
                total.Add(attr.Path, (Feedback)Activator.CreateInstance(type));
            }

            return total;
        }
#endif

        public GameObject Owner { get; private set; }

        public void Setup(GameObject owner)
        {
            Owner = owner;
        }

        private void Awake()
        {
            Feedbacks?.ForEach(x =>
            {
                x.Setup(this);
                x.Awake();
            });
        }

        private void Start()
        {
            Feedbacks?.ForEach(x => x.Start());
;       }

        private void Update()
        {
            Feedbacks?.ForEach(x => x.Update());
        }

        public void PlayFeedbacks()
        {
            Feedbacks?.ForEach(x => x.Play());
        }

        public void StopFeedbacks()
        {
            Feedbacks?.ForEach(x => x.Stop());
        }
        public virtual void ProxyDestroy(GameObject gameObjectToDestroy, float delay)
        {
            Destroy(gameObjectToDestroy, delay);
        }

        protected virtual void OnDestroy()
        {
            Feedbacks?.ForEach(x => x.OnDestroy());
        }
#if UNITY_EDITOR
        [HorizontalGroup(Title = "Test (Only in Playing)")]
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
