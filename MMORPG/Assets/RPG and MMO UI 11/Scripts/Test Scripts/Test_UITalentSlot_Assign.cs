using UnityEngine;

namespace DuloGames.UI
{
	public class Test_UITalentSlot_Assign : MonoBehaviour {
		
		[SerializeField] private UITalentSlot slot;
		[SerializeField] private int assignTalent = 0;
		[SerializeField] private int addPoints = 0;
		
		void Start()
		{
			if (this.slot == null)
				this.slot = this.GetComponent<UITalentSlot>();
			
			if (this.slot == null || UITalentDatabase.Instance == null || UISpellDatabase.Instance == null)
			{
				this.Destruct();
				return;
			}
			
			UITalentInfo info = UITalentDatabase.Instance.GetByID(this.assignTalent);
			
			if (info != null)
			{
				this.slot.Assign(info, UISpellDatabase.Instance.GetByID(info.spellEntry));
				this.slot.AddPoints(this.addPoints);
			}
			
			this.Destruct();
		}
		
		private void Destruct()
		{
			DestroyImmediate(this);
		}
	}
}
