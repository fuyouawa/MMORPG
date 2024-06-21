using System;
using MMORPG.Tool;

namespace MMORPG.Game
{
    public class LocalPlayerPickItem : LocalPlayerAbility
    {
        public override void OnStateInit()
        {
            //监听摘取按钮按下事件
            OwnerState.Brain.InputControls.Player.Pickup.started += context =>
            {
                // 如果可以摘取(脚下有物品), 切换到当前状态
                if (CanPick())
                {
                    OwnerState.Brain.ChangeStateByName("PickItem");
                }
            };
        }

        public override void OnStateEnter()
        {
            // 更新状态
            OwnerState.Brain.NetworkUploadTransform(OwnerStateId);

            //TODO 摘取行为

            //完事后回到Idle
            OwnerState.Brain.ChangeStateByName("Idle");
        }

        public bool CanPick()
        {
            throw new NotImplementedException();
        }

        public override void OnStateExit()
        {
        }
    }
}
