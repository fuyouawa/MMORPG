using MMORPG.Game;
 using Sirenix.OdinInspector.Editor;
 using UnityEditor;

 namespace MMORPG.Editor.Game
{
    [CustomEditor(typeof(MeleeWeapon))]
    public class MeleeWeaponEditor : OdinEditor
    {
        private void OnSceneGUI()
        {
            (target as MeleeWeapon)?.OnSceneGUI();
        }
    }
}
