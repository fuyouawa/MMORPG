using System;
using System.Collections.Generic;
using System.Linq;
using MMORPG.Common.Tool;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using MMORPG.Tool;
#if UNITY_EDITOR
using System.Collections;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
#endif
using UnityEngine;

namespace MMORPG.Game
{
#if UNITY_EDITOR
    [Serializable]
    public class SkillDamageAreaConfig
    {
        public int SkillId;
        public string SkillName;
        public bool EditArea;
        [Required]
        public Transform Target;
        [InlineButton("AreaOffsetToClipboard", "C")]
        public Vector3 AreaOffset;
        [InlineButton("AreaToClipboard", "C")]
        public float Area;
        [InlineButton("DurationToClipboard", "C")]
        public float Duration;
        [InlineButton("HitDelaysToClipboard", "C")]
        public float[] HitDelays = Array.Empty<float>();

        public bool Enable { get; set; }

        private static Dictionary<int, T> Load<T>(string jsonPath)
        {
            var jsonText = Resources.Load(jsonPath) as TextAsset;
            Debug.Assert(jsonText != null);
            var obj = JsonConvert.DeserializeObject<Dictionary<int, T>>(jsonText.text);
            Debug.Assert(obj != null);
            return obj;
        }

        [Button]
        public void AssignFromDataFile()
        {
            var skillDict = Load<SkillDefine>("Json/SkillDefine");
            if (skillDict.TryGetValue(SkillId, out var define))
            {
                var tmp = DataHelper.ParseVector3(define.AreaOffset);
                SkillName = define.Name;
                AreaOffset = new Vector3(tmp.X, tmp.Y, tmp.Z);
                HitDelays = DataHelper.ParseFloats(define.HitDelay);
                Duration = define.Duration;
            }
            else
            {
                EditorUtility.DisplayDialog("错误", $"没有SkillId:{SkillId}", "确认");
            }
        }

        public void AreaOffsetToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"({F2S(AreaOffset.x)},{F2S(AreaOffset.y)},{F2S(AreaOffset.z)})";
            Debug.Log($"Copy {GUIUtility.systemCopyBuffer} to clipboard!");
        }

        public void AreaToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"{F2S(Area)}";
            Debug.Log($"Copy {GUIUtility.systemCopyBuffer} to clipboard!");
        }

        public void DurationToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"{F2S(Duration)}";
            Debug.Log($"Copy {GUIUtility.systemCopyBuffer} to clipboard!");
        }

        public void HitDelaysToClipboard()
        {
            GUIUtility.systemCopyBuffer = $"[{string.Join(',', HitDelays.Select(F2S))}]";
            Debug.Log($"Copy {GUIUtility.systemCopyBuffer} to clipboard!");
        }

        public static string F2S(float f)
        {
            if (Mathf.Approximately(f, 0))
            {
                return "0";
            }

            if (Mathf.Approximately(f - Mathf.Floor(f), 0))
            {
                return $"{f:0}";
            }

            // 如果小数点后第2位很小, 直接舍弃
            if (Mathf.Abs(f - Mathf.Round(f * 10f) / 10f) < 0.05f)
            {
                return $"{f:0.0}";
            }
            return $"{f:0.00}";
        }


        SphereBoundsHandle _sphereBoundsHandle;

        public void OnSceneGUI()
        {
            if (EditArea && Target != null)
            {
                _sphereBoundsHandle ??= new();
                _sphereBoundsHandle.SetColor(Color.yellow);

                _sphereBoundsHandle.center = Target.position + AreaOffset;
                _sphereBoundsHandle.radius = Area;

                EditorGUI.BeginChangeCheck();

                _sphereBoundsHandle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    Area = _sphereBoundsHandle.radius;
                    AreaOffset = _sphereBoundsHandle.center - Target.position;
                }
            }
        }

        private static Color s_gizmosColor = new(1f, 0f, 0f, 0.2f);

        public void OnDrawGizmos()
        {
            Gizmos.color = s_gizmosColor;

            if (Enable)
                Gizmos.DrawSphere(Target.position + AreaOffset, Area);
        }

        public IEnumerator TestCo()
        {
            var hitDelays = HitDelays;
            if (HitDelays.Length == 0)
                hitDelays = new[] { 0f };
            foreach (var delay in hitDelays)
            {
                yield return new WaitForSeconds(delay);

                Enable = true;
                yield return new WaitForSeconds(Duration);
                Enable = false;
            }
        }
    }
#endif

    public class SkillDamageAreaDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "SkillName")]
        public List<SkillDamageAreaConfig> SkillDamageAreaConfigs = new();

        public void OnSceneGUI()
        {
            foreach (var config in SkillDamageAreaConfigs)
            {
                config.OnSceneGUI();
            }
        }

        public void OnDrawGizmos()
        {
            foreach (var config in SkillDamageAreaConfigs)
            {
                config.OnDrawGizmos();
            }
        }

        public void TestSkill(int skillId)
        {
            var config = SkillDamageAreaConfigs.Find(x => x.SkillId == skillId);
            if (config != null)
            {
                StartCoroutine(config.TestCo());
            }
        }
#endif
     }
}
