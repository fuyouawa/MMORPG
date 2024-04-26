using UnityEngine;

namespace DuloGames.UI
{
	public class UIToggleWindowAndTab : MonoBehaviour {
		
		public UIWindow window;
		public UITab tab;
		
		public void Toggle()
		{
			if (this.window == null || this.tab == null)
				return;
			
			// Check if the window is open
			if (this.window.IsOpen)
			{
				// Check if the tab is active
				if (this.tab.isOn)
				{
					// Close the window since everything was already opened
					this.window.Hide();
					return;
				}
			}
			
			// If we have reached this part of the code, that means the we should open up things
			this.window.Show();
			this.tab.Activate();
		}
	}
}
