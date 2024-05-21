using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Aoi
{
    public class AoiWord
    {
        public struct Vector2Int
        {
            public int X, Y;
        }

        public class AoiEntity
        {
            public UInt64 ZoneKey;
            // 间距为2的待清理区域
            public HashSet<UInt64> PendingZoneKeySet;
        }

        public class AoiZone
        {
            // 当前区域内的所有实体
            public HashSet<int> EntitySet = new();

            // 在Pending中关注该区域的所有实体，即哪些实体的Pending中有当前区域
            public HashSet<int> PendingEntitySet = new();

            // 实体进入当前Zone
            public void Enter(int entity)
            {
                EntitySet.Add(entity);
            }

            // 实体离开当前Zone
            public void Leave(int entity, List<int> leaveList)
            {
                // 在PendingZone中关注该区域的实体，选择向对方通知一下有实体离开，实际上当前实体可能观察不到对方实体
                foreach (var pendingEntity in PendingEntitySet)
                {
                    leaveList.Add(pendingEntity);
                }
                EntitySet.Remove(entity);
            }
        }

        private Dictionary<UInt64, AoiZone> _zoneDict = new();
        public Dictionary<int, AoiEntity> _entittDict = new();
        private int _zoneSize;

        public AoiWord(int zoneSize)
        {
            _zoneSize = zoneSize;
        }

        public void Enter(int entity, float x, float y, out List<int> viewList)
        {
            Debug.Assert(!_entittDict.ContainsKey(entity));

            PointToZonePoint(x, y, out var outX, out var outY);
            ZonePointToZoneKey(outX, outY, out var zoneKey);
            _entittDict.Add(entity, new()
            {
                ZoneKey = zoneKey,
                PendingZoneKeySet = new()
            });

            var zone = CreateZone(zoneKey);
            zone.Enter(entity);

            viewList = new();
            var viewZoneArray = GetViewZoneArray(outX, outY);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(entity, curZoneKey, viewList);
            }
        }

        public void Leave(int entity, out List<int> leaveList)
        {
            Debug.Assert(_entittDict.ContainsKey(entity));

            //if (!_entittDict.TryGetValue(entity, out var aoiEntity))
            //{
            //    leaveList = null;
            //    return;
            //}
            var aoiEntity = _entittDict[entity];
            var zone = _zoneDict[aoiEntity.ZoneKey];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);

            leaveList = new();
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(entity, curZoneKey, leaveList);
            }
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
            {
                var pendingZone = _zoneDict[pendingZoneKey];
                AddZoneEntitysToList(entity, pendingZone, leaveList);
                pendingZone.PendingEntitySet.Remove(entity);
            }

            zone.Leave(entity, leaveList);
            _entittDict.Remove(entity);
        }

        /// <summary>
        /// 返回是否跨越边界
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public bool Refresh(int entity, float x, float y, out List<int>? enterList, out List<int>? leaveList)
        {
            Debug.Assert(_entittDict.ContainsKey(entity));

            var aoiEntity = _entittDict[entity];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var minX, out var minY);
            int maxX = minX + _zoneSize;
            int maxY = minY + _zoneSize;

            if (x < minX || y < minY ||
                x >= maxX || y >= maxY)
            {
                leaveList = new();
                enterList = new();
                PointToZonePoint(x, y, out var newZoneX, out var newZoneY);

                //Console.WriteLine($"实体{entity}跨边界移动：[{minX}, {minY}] -> [{newZoneX}, {newZoneY}]");
                //Console.WriteLine($"OldPendingZone：");
                //foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
                //{
                //    ZoneKeyToZonePoint(pendingZoneKey, out var pendingZoneX, out var pendingZoneY);
                //    Console.Write($"[{pendingZoneX}, {pendingZoneY}]，");
                //}
                //Console.WriteLine();

                var oldZone = _zoneDict[aoiEntity.ZoneKey];
                oldZone.Leave(entity, leaveList);

                ZonePointToZoneKey(newZoneX, newZoneY, out var newZoneKey);

                var newZone = CreateZone(newZoneKey);
                newZone.Enter(entity);

                // 跨越边界，根据新的九宫格来确定离开视野范围以及新进入视野范围的实体
                var oldViewZoneArray = GetViewZoneArray(minX, minY);
                var newViewZoneArray = GetViewZoneArray(newZoneX, newZoneY);

                // 添加实体到EnterList
                // 如果在原先的zone中不存在，则是新加入到观察列表的区域
                foreach (var newViewZonePoint in newViewZoneArray)
                {
                    bool exist = false;
                    foreach (var oldViewZonePoint in oldViewZoneArray)
                    {
                        if (newViewZonePoint.X == oldViewZonePoint.X && newViewZonePoint.Y == oldViewZonePoint.Y)
                        {
                            exist = true;
                            break;
                        }
                    }
                    ZonePointToZoneKey(newViewZonePoint.X, newViewZonePoint.Y, out var newViewZoneKey);
                    if (!exist)
                    {
                        // 如果已经存在于PendingZone，也跳过
                        exist = aoiEntity.PendingZoneKeySet.Contains(newViewZoneKey);
                    }
                    if (exist == false)
                    {
                        AddZoneEntitysToList(entity, newViewZoneKey, enterList);
                    }
                }
                // 首先跨边界移动可能出现PendingZone需要清除的情况，根据距离清除
                foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
                {
                    ZoneKeyToZonePoint(pendingZoneKey, out var pendingZoneX, out var pendingZoneY);
                    var distance = GetZoneDistance(newZoneX, newZoneY, pendingZoneX, pendingZoneY);
                    if (distance > 2 || distance <= 1)
                    {
                        var pendingZone = _zoneDict[pendingZoneKey];
                        // 移动后可能会将PendingZone划入九宫格的范围，划入九宫格的则不会离开当前视野
                        if (distance > 2)
                        {
                            AddZoneEntitysToList(entity, pendingZone, leaveList);
                        }
                        aoiEntity.PendingZoneKeySet.Remove(pendingZoneKey);
                        pendingZone.PendingEntitySet.Remove(entity);
                    }
                }

                // 添加实体到LeaveList
                foreach (var oldViewZonePoint in oldViewZoneArray)
                {
                    ZonePointToZoneKey(oldViewZonePoint.X, oldViewZonePoint.Y, out var curZoneKey);
                    var distance = GetZoneDistance(newZoneX, newZoneY, oldViewZonePoint.X, oldViewZonePoint.Y);
                    // 将距离为2的Zone添加到Pending中
                    if (distance == 2)
                    {
                        aoiEntity.PendingZoneKeySet.Add(curZoneKey);
                        var curZone = CreateZone(curZoneKey);
                        curZone.PendingEntitySet.Add(entity);
                    }
                    // 距离如果大于2就直接添加到LeaveList
                    else if (distance > 2)
                    {
                        AddZoneEntitysToList(entity, curZoneKey, leaveList);
                    }
                }

                aoiEntity.ZoneKey = newZoneKey;
                return true;
            }

            leaveList = null;
            enterList = null;
            return false;
        }

        public List<int> GetViewEntityList(int entity)
        {
            var viewList = new List<int>();
            var aoiEntity = _entittDict[entity];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(entity, curZoneKey, viewList);
            }
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
            {
                AddZoneEntitysToList(entity, pendingZoneKey, viewList);
            }

            return viewList;
        }


        private AoiZone CreateZone(UInt64 zoneKey)
        {
            if (!_zoneDict.TryGetValue(zoneKey, out var newZone))
            {
                newZone = new();
                _zoneDict[zoneKey] = newZone;
            }
            return newZone;
        }

        /// <summary>
        /// 获取九宫格可视区域
        /// </summary>
        private Vector2Int[] GetViewZoneArray(int x, int y)
        {
            var arr = new Vector2Int[9];
            arr[0].X = x - _zoneSize;
            arr[0].Y = y + _zoneSize;

            arr[1].X = x;
            arr[1].Y = y + _zoneSize;

            arr[2].X = x + _zoneSize;
            arr[2].Y = y + _zoneSize;

            arr[3].X = x - _zoneSize;
            arr[3].Y = y;

            arr[4].X = x;
            arr[4].Y = y;

            arr[5].X = x + _zoneSize;
            arr[5].Y = y;

            arr[6].X = x - _zoneSize;
            arr[6].Y = y - _zoneSize;

            arr[7].X = x;
            arr[7].Y = y - _zoneSize;

            arr[8].X = x + _zoneSize;
            arr[8].Y = y - _zoneSize;
            return arr;
        }

        private void PointToZonePoint(float x, float y, out int outX, out int outY)
        {
            if (x >= 0)
            {
                outX = (int)x;
                outX -= outX % _zoneSize;
            }
            else
            {
                outX = (int)(x - 1.0f);
                outX -= outX % _zoneSize + _zoneSize;
            }
            

            if (y >= 0)
            {
                outY = (int)y;
                outY -= outY % _zoneSize;
            }
            else
            {
                outY = (int)(y - 1.0f);
                outY -= outY % _zoneSize + _zoneSize;
            }
        }

        private void ZoneKeyToZonePoint(UInt64 zoneKey, out int x, out int y)
        {
            x = (int)(zoneKey >> 32);
            y = (int)zoneKey;
        }

        private void ZonePointToZoneKey(int x, int y, out UInt64 zoneKey)
        {
            zoneKey = (((UInt64)(UInt32)x) << 32) | (UInt32)y;
        }

        private int GetZoneDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2)) / _zoneSize;
        }

        private void AddZoneEntitysToList(int excludeEntity, AoiZone zone, List<int> list)
        {
            foreach (var entity in zone.EntitySet)
            {
                if (excludeEntity == entity) continue;
                list.Add(entity);
            }
        }

        private void AddZoneEntitysToList(int excludeEntity, UInt64 zoneKey, List<int> list)
        {
            if (!_zoneDict.TryGetValue(zoneKey, out var zone)) return;
            AddZoneEntitysToList(excludeEntity, zone, list);
        }
    }
}
