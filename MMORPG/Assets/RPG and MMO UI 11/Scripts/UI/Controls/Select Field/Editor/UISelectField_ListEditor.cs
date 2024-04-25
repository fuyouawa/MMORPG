using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UISelectField_List), true)]
	public class UISelectField_ListEditor : Editor {

		public override void OnInspectorGUI()
		{
		}
	}
}
