
using MMORPG.Common.Proto.Inventory;
using GameServer.Db;
using GameServer.Manager;
using Serilog;
using System.Diagnostics;
using MMORPG.Common.Inventory;
using GameServer.PlayerSystem;

namespace GameServer.InventorySystem
{
    public class Inventory
    {
        public const int InitCapacity = 10;

        public Player OwnerPlayer { get; }
        public int Capacity { get; private set; }

        private List<Item?> Items { get; } = new();

        private bool _hasChange = false;

        public Inventory(Player ownerPlayer)
        {
            OwnerPlayer = ownerPlayer;
        }

        public bool SetItem(int slotId, Item? item)
        {
            if (slotId >= Capacity) return false;
            _hasChange = true;
            if (item is null)
            {
                var origin = Items[slotId];
                if (origin != null) origin.SlotId = -1;
                Items[slotId] = null;
                return true;
            }
            Items[slotId] = item;
            item.SlotId = slotId;
            return true;
        }

        private InventoryInfo? _inventoryInfo;

        public InventoryInfo GetInventoryInfo()
        {
            if (_hasChange || _inventoryInfo == null)
            {
                _inventoryInfo = new();
                _inventoryInfo.Capacity = Capacity;
                _inventoryInfo.Items.AddRange(Items.Where(x => x != null).Select(x => x.GetItemInfo()));
                _hasChange = false;
            }
            return _inventoryInfo;
        }

        public void LoadInventoryInfo(byte[]? inventoryData)
        {
            InventoryInfo? info = null;
            if (inventoryData == null)
            {
                Capacity = InitCapacity;
            }
            else
            {
                info = InventoryInfo.Parser.ParseFrom(inventoryData);
                Capacity = info.Capacity;
            }

            for (int i = 0; i < Capacity; i++)
            {
                Items.Add(null);
            }

            if (info != null)
            {
                foreach (var item in info.Items)
                {
                    if (!DataManager.Instance.ItemDict.TryGetValue(item.ItemId, out var define))
                    {
                        Log.Error($"物品id不存在:{item.ItemId}");
                        continue;
                    }

                    Items[item.SlotId] = new Item(define, item.Amount, item.SlotId);
                }
            }
        }


        public int AddItem(int itemId, int amount = 1)
        {
            if (!DataManager.Instance.ItemDict.TryGetValue(itemId, out var define))
            {
                Log.Error($"物品id不存在:{itemId}");
                return 0;
            }

            int slotId = 0;
            int residue = amount;

            while (residue > 0)
            {
                var item = GetItem(itemId, slotId);
                if (item == null)
                {
                    // 如果找不到ItemId对应的Slot了, 那就尝试在空Slot上分配
                    return AddItemInEmptySlot(itemId, residue);
                }
                Debug.Assert(item != null);
                var processableAmount2 = Math.Min(amount, item.Capacity - item.Amount);
                item.Amount += processableAmount2;
                residue -= processableAmount2;
                slotId = item.SlotId + 1;
            }

            _hasChange = true;
            return 0;
        }

        public int AddItemInEmptySlot(int itemId, int amount = 1)
        {
            if (!DataManager.Instance.ItemDict.TryGetValue(itemId, out var define))
            {
                Log.Error($"物品id不存在:{itemId}");
                return 0;
            }

            int residue = amount;
            int slotId = 0;

            _hasChange = true;

            while (residue > 0)
            {
                slotId = FindEmptySlot(slotId);
                if (slotId != -1)
                {
                    var processableAmount = Math.Min(amount, define.Capacity);
                    SetItem(slotId, new Item(define, processableAmount, slotId));
                    residue -= processableAmount;
                }
                else
                {
                    Log.Debug($"{OwnerPlayer.User.Channel}物品槽满了, 还有{amount}个物品被丢失");
                    return amount;
                }
            }

            return 0;
        }

        public bool Exchange(int originSlotId, int targetSlotId)
        {
            if (originSlotId == targetSlotId) return false;
            if (originSlotId < 0 || targetSlotId < 0) return false;

            var originItem = Items[originSlotId];
            if (originItem == null)
            {
                return false;
            }
            _hasChange = true;
            var targetItem = Items[targetSlotId];
            if (targetItem == null)
            {
                SetItem(originSlotId, null);
                SetItem(targetSlotId, originItem);
                return true;
            }

            //如果物品类型相同
            if (originItem.Id == targetItem.Id)
            {
                int moveableAmount = Math.Min(targetItem.Capacity - targetItem.Amount, originItem.Amount);
                // 如果原始物品数量小于等于可移动数量，将原始物品全部移动到目标插槽
                if (originItem.Amount < moveableAmount)
                {
                    targetItem.Amount += moveableAmount;
                    SetItem(originSlotId, null);
                }
                else
                {
                    targetItem.Amount += moveableAmount;
                    originItem.Amount -= moveableAmount;
                }
            }
            //如果类型不同则交换位置
            else
            {
                SetItem(targetSlotId, originItem);
                SetItem(originSlotId, targetItem);
            }
            return true;
        }

        /// <summary>
        /// 根据ItemId移除指定数量的物品
        /// </summary>
        /// <returns>剩余待移除的数量</returns>
        public int RemoveItem(int itemId, int amount = 1)
        {
            _hasChange = true;

            int residue = amount;
            int slotId = 0;
            while (residue > 0)
            {
                var item = GetItem(itemId, slotId);
                if (item == null)
                    break;
                slotId = item.SlotId;

                int removeAmount = Math.Min(residue, item.Amount);
                item.Amount -= removeAmount;
                residue -= removeAmount;
                if (item.Amount == 0)
                {
                    SetItem(item.SlotId, null);
                }
            }
            return residue;
        }

        public int RemoveItemBySlot(int slotId, int amount = 1)
        {
            if (amount < 1) return 0;

            var item = GetItemBySlot(slotId);
            if (item == null) return amount;

            Debug.Assert(item != null);

            _hasChange = true;
            if (amount < item.Amount)
            {
                item.Amount -= amount;
                return 0;
            }
            SetItem(slotId, null);
            return amount - item.Amount;
        }


        public bool HasItem(int itemId, int amount = 1)
        {
            int residue = amount;
            int slotId = 0;
            while (residue > 0)
            {
                var item = GetItem(itemId, slotId);
                if (item == null)
                {
                    return false;
                }
                slotId = item.SlotId;
                int removeAmount = Math.Min(residue, item.Amount);
                residue -= removeAmount;
            }
            return true;
        }

        /// <summary>
        /// 丢弃指定Slot上指定数量的物品
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="amount"></param>
        /// <returns>还有多少数量没有成功丢弃</returns>

        public Item? GetItem(int itemId, int beginSlot = 0)
        {
            return Items.Skip(beginSlot).FirstOrDefault(x => x != null && x.Id == itemId);
        }

        public Item? GetItemBySlot(int slotId)
        {
            if (slotId >= Items.Count) return null;
            return Items[slotId];
        }

        private int FindEmptySlot(int beginSlot = 0)
        {
            for (int i = beginSlot; i < Items.Count; i++)
            {
                if (Items[i] == null)
                    return i;
            }
            return -1;
        }
    }
}