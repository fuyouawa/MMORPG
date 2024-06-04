using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace MMORPG.Tool
{
    public class FeedbackItemDrawer : OdinValueDrawer<FeedbackItem>
    {
        protected override void Initialize()
        {

        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();

            var headerRect = EditorGUILayout.GetControlRect(false);

            var label2 = GUIHelper.TempContent("      " + GetLabel());

            var buttonRect = new Rect(headerRect)
            {
                x = headerRect.x + 17,
                width = EditorGUIUtility.singleLineHeight,
                height = EditorGUIUtility.singleLineHeight
            };

            value.Enable = EditorGUI.Toggle(buttonRect, value.Enable);

            base.Property.State.Expanded = SirenixEditorGUIHelper.Foldout(
                headerRect,
                Property.State.Expanded,
                label2,
                out _);

            var progressBarBgRect = new Rect(headerRect)
            {
                height = 5,
                width = headerRect.width + 10,
                x = headerRect.x - 5,
                y = headerRect.yMax + 3
            };
            float progressBarWidthRadio = 0;
            float delayBeforePlayProgressBarWidthRadio = 0;

            if (value.Feedback != null && value.Feedback.IsPlaying)
            {
                var playTime = Time.time - value.Feedback.TimeSincePlay;
                if (value.Feedback.TotalDuartion != 0)
                {
                    progressBarWidthRadio = Mathf.Clamp01(playTime / value.Feedback.TotalDuartion);
                    if (playTime < value.Feedback.DelayBeforePlay)
                    {
                        delayBeforePlayProgressBarWidthRadio = Mathf.Clamp01(playTime / value.Feedback.TotalDuartion);
                    }
                    else
                    {
                        delayBeforePlayProgressBarWidthRadio = Mathf.Clamp01(value.Feedback.DelayBeforePlay / value.Feedback.TotalDuartion);
                    }
                }
            }

            var progressBarRect = new Rect(progressBarBgRect)
            {
                height = 3.5f
            };
            var delayBeforePlayProgressBarRect = new Rect(progressBarRect);

            progressBarRect.width *= progressBarWidthRadio;
            delayBeforePlayProgressBarRect.width *= delayBeforePlayProgressBarWidthRadio;

            SirenixEditorGUI.DrawSolidRect(progressBarBgRect, new Color(0f, 0f, 0f, 0.3f), false);
            SirenixEditorGUI.DrawSolidRect(progressBarRect, Color.yellow, false);
            SirenixEditorGUI.DrawSolidRect(delayBeforePlayProgressBarRect, Color.red, false);

            SirenixEditorGUI.EndBoxHeader();

            GUILayout.Space(5);

            if (value.Feedback != null)
            {
                using (new EditorGUI.DisabledScope(!value.Enable))
                {
                    if (SirenixEditorGUI.BeginFadeGroup(this, base.Property.State.Expanded))
                    {
                        foreach (var child in ValueEntry.Property.Children)
                        {
                            child.Draw();
                        }
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
            }
            else
            {
                ValueEntry.Property.Children["FeedbackName"].Draw();
            }

            SirenixEditorGUI.EndBox();

            if (value.Feedback != null && value.Feedback.IsPlaying)
            {
                GUIHelper.RequestRepaint();
            }

            ValueEntry.SmartValue = value;
        }



        private string GetLabel()
        {
            var value = ValueEntry.SmartValue;
            if (value.Feedback == null || string.IsNullOrEmpty(value.Label))
                return "TODO";
            var duration = value.Feedback.GetDuration();

            var timeDisplay = $"{value.Feedback.DelayBeforePlay:0.00}s + {duration:0.00}s";

            string label;
            if (value.Feedback.LoopPlay)
            {
                var loopCountDisplay = value.Feedback.LimitLoopAmount ? value.Feedback.AmountOfLoop.ToString() : "\u221e";
                if (value.Feedback.DelayBetweenLoop > float.Epsilon)
                {
                    timeDisplay += $" + {value.Feedback.DelayBetweenLoop:0.00}s";
                }
                label = $"{value.Label} ({timeDisplay}) x {loopCountDisplay}";
            }
            else
            {
                label = $"{value.Label} ({timeDisplay})";
            }

            if (value.ActiveEnablePredicate)
            {
                label += $" <EnableIf: {value.EnableIf.Getter}>";
            }

            return label;
        }
    }
}
