using MMORPG.System;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : ViewController
    {
        public float BgHpLerp = 3f;

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
                if (ProgressHp.fillAmount > ProgressBgHp.fillAmount)
                {
                    ProgressBgHp.fillAmount = ProgressHp.fillAmount;
                }
                else
                {
                    ProgressBgHp.fillAmount = Mathf.Lerp(ProgressBgHp.fillAmount, per, BgHpLerp * Time.deltaTime);
                }
                per *= 100f;
                TextHpPercentage.text = $"{per:0}%";
            }
        }
	}
}
