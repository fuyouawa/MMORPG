using MMORPG.Game;
using UnityEngine;
using QFramework;

namespace MMORPG.UI
{
	public partial class UIMonsterCanvas : ViewController
    {
        public ActorController ActorController;
        private bool _reviving;
        private float _remainingReviveTime;

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
            if (_reviving)
            {
                _remainingReviveTime -= Time.deltaTime;
                TextReviveTime.SetText($"{_remainingReviveTime:0}");
            }
            else
            {
                UIHpBar.UpdateValue(ActorController.Hp, ActorController.MaxHp);
            }
        }

        public void BeginRevive(float time)
        {
            _reviving = true;
            _remainingReviveTime = time;
            GroupHub.gameObject.SetActive(false);
            TextReviveTime.gameObject.SetActive(true);
        }

        public void EndRevive()
        {
            _reviving = false;
            _remainingReviveTime = 0f;
            GroupHub.gameObject.SetActive(true);
            TextReviveTime.gameObject.SetActive(false);
        }
    }
}
