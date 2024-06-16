using UnityEngine;
using UnityEngine.SceneManagement;

namespace MMORPG.Tool
{
    public class LoadSceneOnPlay : MonoBehaviour
    {
        public string SceneName;

        private void Awake()
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
