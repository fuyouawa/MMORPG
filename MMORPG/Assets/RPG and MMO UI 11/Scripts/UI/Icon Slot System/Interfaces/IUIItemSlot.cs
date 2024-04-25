using UnityEngine;
using Object = UnityEngine.Object;

namespace DuloGames.UI
{
    public interface IUIItemSlot
    {
        UIItemInfo GetItemInfo();
        bool Assign(UIItemInfo itemInfo, Object source);
        void Unassign();
    }
}
