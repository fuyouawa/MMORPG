using MMORPG.Game;
using MMORPG.Tool;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace MMORPG.Editor.Game
{
    [CustomEditor(typeof(SkillDamageAreaDebugger))]
    public class SkillDamageAreaDebuggerEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            (target as SkillDamageAreaDebugger)?.OnSceneGUI();
        }
    }
}
