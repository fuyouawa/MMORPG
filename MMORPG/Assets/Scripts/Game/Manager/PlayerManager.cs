using MMORPG.Event;
using QFramework;
using MMORPG.System;
using UnityEngine;

namespace MMORPG.Game
{
    //TODO PlayerManager
    public class PlayerManager : MonoBehaviour, IController
    {
        private IEntityManagerSystem _entityManager;
        private ResLoader _resLoader = ResLoader.Allocate();

        private void Awake()
        {
            _entityManager = this.GetSystem<IEntityManagerSystem>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        void OnDestroy()
        {
            _resLoader.Recycle2Cache();
            _resLoader = null;
        }
    }
}
