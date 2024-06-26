using MMORPG.Event;
using MMORPG.System;
using UnityEngine;
using QFramework;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : ViewController
    {
        private ActorController actorController;


        void Start()
        {
            actorController = this.GetSystem<IPlayerManagerSystem>().MineEntity.GetComponent<ActorController>();
        }

        void Update()
        {
            if (actorController != null)
            {
                var per = (float)actorController.Hp / actorController.MaxHp;
                per *= 100f;
                TextHpPercentage.text = $"{per:0}%";
            }
        }
	}
}
