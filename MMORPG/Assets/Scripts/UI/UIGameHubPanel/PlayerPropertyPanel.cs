using DuloGames.UI;
using MMORPG.System;
using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : ViewController
    {
        public float BgHpLerp = 3f;
        public UIProgressBar ProgressXpBar;

        private ActorController _actorController;

        async void Start()
        {
            var mine = await this.GetSystem<IPlayerManagerSystem>().GetMineEntityTask();
            _actorController = mine.GetComponent<ActorController>();
        }

        void Update()
        {
            if (_actorController != null)
            {
                var hpPer = (float)_actorController.Hp / _actorController.MaxHp;
                ImageHpFill.fillAmount = hpPer;
                if (ImageHpFill.fillAmount > ImageBgHpFill.fillAmount)
                {
                    ImageBgHpFill.fillAmount = ImageHpFill.fillAmount;
                }
                else
                {
                    ImageBgHpFill.fillAmount = Mathf.Lerp(ImageBgHpFill.fillAmount, hpPer, BgHpLerp * Time.deltaTime);
                }
                hpPer *= 100f;
                TextHpPercentage.text = $"{_actorController.Hp}/{_actorController.MaxHp}({hpPer:0}%)";

                var mpPer = (float)_actorController.Mp / _actorController.MaxMp;
                ImageMpFill.fillAmount = mpPer;
                mpPer *= 100f;
                TextMpPercentage.text = $"{_actorController.Mp}/{_actorController.MaxMp}({mpPer:0}%)";

                TextLevel.text = _actorController.Level.ToString();

                ProgressXpBar.fillAmount = (float)_actorController.Exp / _actorController.MaxExp;
            }
        }
	}
}
