using MMORPG.Event;
using UnityEngine;
using QFramework;
using NotImplementedException = System.NotImplementedException;

namespace MMORPG.Game
{
	public partial class PlayerPropertyPanel : ViewController
    {
        private ActorController actorController;

        void Awake()
        {
            this.RegisterEvent<PlayerJoinedMapEvent>(OnPlayerJoined)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnPlayerJoined(PlayerJoinedMapEvent e)
        {
            actorController = e.PlayerEntity.GetComponent<ActorController>();
        }

        void Start()
		{
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
