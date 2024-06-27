using MMORPG.System;
using QFramework;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : ViewController
    {
        private ActorController actorController;


        async void Start()
        {
            var mine = await this.GetSystem<IPlayerManagerSystem>().GetMineEntityTask();
            actorController = mine.GetComponent<ActorController>();
        }

        void Update()
        {
            if (actorController != null)
            {
                var per = (float)actorController.Hp / actorController.MaxHp;
                ProgressHp.fillAmount = per;
                per *= 100f;
                TextHpPercentage.text = $"{per:0}%";
            }
        }
	}
}
