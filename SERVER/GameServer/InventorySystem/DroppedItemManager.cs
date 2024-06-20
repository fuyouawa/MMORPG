using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Db;
using GameServer.EntitySystem;
using GameServer.Manager;
using GameServer.MapSystem;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace GameServer.InventorySystem
{
    public class DroppedItemManager
    {
        private Map _map;
        private Dictionary<int, DroppedItem> _itemDict = new();
        Random _random = new();

        public DroppedItemManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {
            NewDroppedItem(1001, new(0, 5, 0), Vector3.Zero, 1);
            NewDroppedItem(1007, new(5, 5, 0), Vector3.Zero, 1);
        }

        public void Update()
        {
            foreach (var item in _itemDict.Values)
            {
                item.Update();
            }
        }

        /// <summary>
        /// 创建掉落物
        /// </summary>
        /// <returns></returns>
        public DroppedItem NewDroppedItem(int itemId, Vector3 pos, Vector3 dire, int amount)
        {
            var item = new DroppedItem(EntityManager.Instance.NewEntityId(),
                DataManager.Instance.UnitDict[DataManager.Instance.ItemDict[itemId].UnitId], _map, pos, dire, itemId, amount);
            EntityManager.Instance.AddEntity(item);

            lock (_itemDict)
            {
                _itemDict.Add(item.EntityId, item);
            }
            _map.EntityEnter(item);

            item.Start();
            return item;
        }


        /// <summary>
        /// 删除掉落物
        /// </summary>
        /// <param name="player"></param>
        public void RemoveDroppedItem(DroppedItem item)
        {
            EntityManager.Instance.RemoveEntity(item);
            lock (_itemDict)
            {
                _itemDict.Remove(item.EntityId);
            }
        }

        public DroppedItem NewDroppedItemWithOffset(int itemId, Vector3 pos, Vector3 dire, int amount, float offset)
        {
            
            // 位置偏移掉落
            float offsetX = _random.NextSingle() * offset * 2 - offset;
            float offsetZ = _random.NextSingle() * offset * 2 - offset;

            pos.X += offsetX;
            pos.Z += offsetZ;

            return NewDroppedItem(itemId, pos, Vector3.Zero, amount);
        }
    }
}
