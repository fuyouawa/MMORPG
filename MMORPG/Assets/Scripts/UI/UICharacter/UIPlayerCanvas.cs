using UnityEngine;
using QFramework;
using Sirenix.OdinInspector;

namespace MMORPG.Game
{
	public partial class UIPlayerCanvas : ViewController
	{
        [Required]
        public PlayerBrain Player;

        void Start()
        {
            TextName.text = Player.ActorController.Name;
            TextLevel.text = $"Lv.{Player.ActorController.Level}";
            
            gameObject.SetActive(!Player.IsMine);
        }
        
        void Update()
        {
            UIHpBar.UpdateValue(Player.ActorController.Hp, Player.ActorController.MaxHp);
        }
	}
}
