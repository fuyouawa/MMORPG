using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.Db;
using GameServer.Model;

namespace GameServer.Manager
{
    public class DroppedItemManager
    {
        private Map _map;
        private Dictionary<int, DroppedItem> _itemDict = new();

        public DroppedItemManager(Map map)
        {
            _map = map;
        }

        public void Start()
        {
            NewItem(1001001, new(0, 5, 0), Vector3.Zero, 1);
            NewItem(1002001, new(5, 5, 0), Vector3.Zero, 1);
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
        public DroppedItem NewItem(int unitId, Vector3 pos, Vector3 dire, int amount)
        {
            var item = new DroppedItem(EntityManager.Instance.NewEntityId(), unitId, _map, amount)
            {
                Position = pos,
                Direction = dire,
            };
            EntityManager.Instance.AddEntity(item);

            lock (_itemDict)
            {
                _itemDict.Add(item.EntityId, item);
            }
            _map.EntityEnter(item);

            item.Start();
            return item;
        }
    }
}
