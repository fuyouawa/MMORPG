using Common.Inventory;
using MMORPG.Tool;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;

namespace MMORPG.UI
{
    public class UIKnapsackSlot : UISlotBase
    {

        public UIItem UIItem;


        /// <summary>
        /// 将Item赋值给此槽
        /// </summary>
        /// <param name="item"></param>
        public void Assign(Item item)
        {
            Debug.Assert(item.SlotId == SlotId);

            if (UIItem != null)
            {
                SetEmpty();
            }
            var resLoader = ResLoader.Allocate();
            var prefab = resLoader.LoadSync<UIItem>("UIItem");
            UIItem = Instantiate(prefab, transform);
            UIItem.Assign(item);

        }

        /// <summary>
        /// 将此槽置空
        /// </summary>
        public void SetEmpty()
        {
            Destroy(UIItem.transform.gameObject);
            UIItem = null;
        }
        
        //TODO 右键菜单
    }
}
