using QFramework;
using UnityEngine;

namespace MMORPG.Game
{
    public enum LaunchStatus
    {
        InitLog,
        InitPlugins,
        InitTool,
        InitNetwork,
        WaitForJoinMap,
        Playing,
        ApplicationQuit
    }

    public class LaunchController : MonoBehaviour, IController
    {
        public FSM<LaunchStatus> FSM = new();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            Tool.Log.Info("Launch", "开始生命周期");
            FSM.AddState(LaunchStatus.InitLog, new InitLogState(FSM, this));
            FSM.AddState(LaunchStatus.InitPlugins, new InitPluginsState(FSM, this));
            FSM.AddState(LaunchStatus.InitTool, new InitToolState(FSM, this));
            FSM.AddState(LaunchStatus.InitNetwork, new InitNetworkState(FSM, this));
            FSM.AddState(LaunchStatus.WaitForJoinMap, new WaitForJoinMapState(FSM, this));
            FSM.AddState(LaunchStatus.Playing, new PlayingState(FSM, this));
            FSM.AddState(LaunchStatus.ApplicationQuit, new ApplicationQuitState(FSM, this));

            FSM.StartState(LaunchStatus.InitLog);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        protected void OnApplicationQuit()
        {
            FSM.ChangeState(LaunchStatus.ApplicationQuit);
            FSM.Clear();
        }
    }
}
