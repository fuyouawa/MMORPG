using Cinemachine;
using MMORPG.Event;
using MMORPG.Game;
using QFramework;
using UnityEngine;

namespace MMORPG.Tool
{
    public class CinemachineCameraController : MonoBehaviour, IController
    {
        private CinemachineVirtualCamera _virtualCamera;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();

            this.RegisterEvent<PlayerJoinedMapEvent>(OnPlayerJoinedMap)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnPlayerJoinedMap(PlayerJoinedMapEvent e)
        {
            _virtualCamera.Follow = e.PlayerEntity.transform;
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }
    }
}
