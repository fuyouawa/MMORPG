using TMPro;
using UnityEngine.EventSystems;

namespace MMORPG.Global
{
    public static class InputManager
    {
        public static bool CanInput
        {
            get
            {
                return EventSystem.current?.currentSelectedGameObject?.GetComponent<TMP_InputField>() == null;
            }
        }
    }
}
