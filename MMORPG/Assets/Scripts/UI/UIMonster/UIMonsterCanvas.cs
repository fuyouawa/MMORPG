using MMORPG.Game;
using UnityEngine;
using QFramework;

namespace MMORPG.UI
{
	public partial class UIMonsterCanvas : ViewController
    {
        public ActorController ActorController;

		void Start()
		{
            if (ActorController == null)
            {
                ActorController = GetComponentInParent<ActorController>();
            }

            TextName.text = ActorController.Entity.UnitDefine.Name;
            TextLevel.text = $"Lv.{ActorController.Level}";
        }

        void Update()
        {
            UIHpBar.UpdateValue(ActorController.Hp, ActorController.MaxHp);
        }
	}
}
