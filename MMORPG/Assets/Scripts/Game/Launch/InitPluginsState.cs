using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using MessagePack;
using MessagePack.Resolvers;
using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.Game
{
    public class InitPluginsState : AbstractState<LaunchStatus, LaunchController>, IController
    {
        public InitPluginsState(FSM<LaunchStatus> fsm, LaunchController target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            Tool.Log.Info("Launch", "初始化插件");
            ResKit.Init();
            new GameObject(nameof(UnityMainThreadDispatcher)).AddComponent<UnityMainThreadDispatcher>();

            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard
                .WithCompression(MessagePackCompression.Lz4BlockArray)
                .WithResolver(CompositeResolver.Create(
                    new[] { new Vector2Formatter() },
                    new[] { StandardResolver.Instance })
                );

            mFSM.ChangeState(LaunchStatus.InitTool);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
