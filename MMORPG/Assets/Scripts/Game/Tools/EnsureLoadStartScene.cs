using MMORPG.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMORPG.Tool
{
    public class EnsureLoadStartScene : MonoBehaviour
    {
        private void Awake()
        {
            if (LaunchController.Instance == null)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
