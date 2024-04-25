using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEditor.UI
{
	public class NavigationTools {
	
		[MenuItem ("Tools/Navigation/Disable All Automatic")]
		static void DisableAutomaticNavigations()
		{
			Selectable[] selectables = Resources.FindObjectsOfTypeAll<Selectable>();
			
			int count = 0;
			foreach (Selectable s in selectables)
			{
				if (s.navigation.mode == Navigation.Mode.Automatic)
				{
					Navigation n = s.navigation;
					n.mode = Navigation.Mode.None;
					s.navigation = n;
					
					if (!s.gameObject.activeInHierarchy)
						EditorUtility.SetDirty(s);
					
					++count;
				}
			}
			
			Debug.Log("Affected objects: " + count.ToString());
		}
		
		[MenuItem ("Tools/Navigation/Disable All")]
		static void DisableAllNavigations()
		{
			Selectable[] selectables = Resources.FindObjectsOfTypeAll<Selectable>();
			
			int count = 0;
			foreach (Selectable s in selectables)
			{
				Navigation n = s.navigation;
				n.mode = Navigation.Mode.None;
				s.navigation = n;
				
				if (!s.gameObject.activeInHierarchy)
					EditorUtility.SetDirty(s);
				
				++count;
			}
			
			Debug.Log("Affected objects: " + count.ToString());
		}
	}
}