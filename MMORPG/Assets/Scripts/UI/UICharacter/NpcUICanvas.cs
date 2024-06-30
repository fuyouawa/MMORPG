using UnityEngine;
using QFramework;
using MMORPG.Game;

namespace MMORPG.UI
{
	public partial class NpcUICanvas : ViewController
    {
        public ActorController ActorController;

        void Start()
        {
            if (ActorController == null)
            {
                ActorController = GetComponentInParent<ActorController>();
            }

            TextName.text = ActorController.Entity.UnitDefine.Name;
        }
    }
}
