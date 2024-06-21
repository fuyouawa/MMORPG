using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using MMORPG.Common.Tool;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
using UnityEditor;
#endif

namespace MMORPG.Game
{
    public class MeleeWeapon : Weapon
    {
#if UNITY_EDITOR
        [FoldoutGroup("Damage Area Debugger")]
        public string SkillName;
        [FoldoutGroup("Damage Area Debugger")]
        public bool EditArea;
        [FoldoutGroup("Damage Area Debugger")]
        [InlineButton("AreaOffsetToClipboard", "C")]
        public Vector3 AreaOffset;
        [FoldoutGroup("Damage Area Debugger")]
        [InlineButton("AreaToClipboard", "C")]
        public float Area;
        [FoldoutGroup("Damage Area Debugger")]
        [InlineButton("DurationToClipboard", "C")]
        public float Duration;
        [FoldoutGroup("Damage Area Debugger")]
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

        [FoldoutGroup("Damage Area Debugger")]
        [Button]
        public void AssignFromDataFile()
        {
            var skillDict = Load<SkillDefine>("Json/SkillDefine");
            if (skillDict.TryGetValue(WeaponId, out var define))
            {
                var tmp = DataHelper.ParseVector3(define.AreaOffset);
                SkillName = define.Name;
                Area = define.Area;
                AreaOffset = new Vector3(tmp.X, tmp.Y, tmp.Z);
                HitDelays = DataHelper.ParseFloats(define.HitDelay);
                Duration = define.Duration;
            }
            else
            {
                EditorUtility.DisplayDialog("错误", $"没有SkillId:{WeaponId}", "确认");
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
            if (EditArea)
            {
                _sphereBoundsHandle ??= new();
                _sphereBoundsHandle.SetColor(Color.yellow);

                _sphereBoundsHandle.center = transform.position + AreaOffset;
                _sphereBoundsHandle.radius = Area;

                EditorGUI.BeginChangeCheck();

                _sphereBoundsHandle.DrawHandle();

                if (EditorGUI.EndChangeCheck())
                {
                    Area = _sphereBoundsHandle.radius;
                    AreaOffset = _sphereBoundsHandle.center - transform.position;
                }
            }
        }

        private static Color s_gizmosColor = new(1f, 0f, 0f, 0.2f);

        public void OnDrawGizmos()
        {
            Gizmos.color = s_gizmosColor;

            if (Enable)
                Gizmos.DrawSphere(transform.position + AreaOffset, Area);
        }

        private IEnumerator TestDamageCo()
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

        public override void TurnWeaponOn()
        {
            base.TurnWeaponOn();
            StartCoroutine(TestDamageCo());
        }
#endif
    }
}
