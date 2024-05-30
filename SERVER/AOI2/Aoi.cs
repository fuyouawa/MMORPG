using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Aoi
{
    /// <summary>
    /// 基于九宫格的AOI系统
    /// 非线程安全
    /// </summary>
    public class AoiWord
    {
        public struct Vector2Int
        {
            public int X, Y;
        }

        public class AoiEntity
        {
            public int EntityId;
            public UInt64 ZoneKey;
            // 间距为2的待清理区域
            public HashSet<UInt64> PendingZoneKeySet;
            public HashSet<int> ExtraFollowingSet;     // 超出AOI区域的额外关注目标
        }

        public class AoiZone
        {
            // 当前区域内的所有实体
            public HashSet<int> EntitySet = new();

            // 在Pending中关注该区域的所有实体，即哪些实体的Pending中有当前区域
            public HashSet<int> PendingEntitySet = new();

            // 实体进入当前Zone
            public void Enter(AoiEntity aoiEntity)
            {
                EntitySet.Add(aoiEntity.EntityId);
            }

            // 实体离开当前Zone
            public void Leave(AoiEntity aoiEntity, List<int> leaveList)
            {
                // 在PendingZone中关注该区域的实体，选择向对方通知一下有实体离开，实际上当前实体可能观察不到对方实体
                foreach (var pendingEntity in PendingEntitySet)
                {
                    leaveList.Add(pendingEntity);
                }
                EntitySet.Remove(aoiEntity.EntityId);
            }
        }

        private Dictionary<UInt64, AoiZone> _zoneDict = new();
        public Dictionary<int, AoiEntity> _entittDict = new();
        private int _zoneSize;

        public AoiWord(int zoneSize)
        {
            _zoneSize = zoneSize;
        }

        public AoiEntity Enter(int entity, float x, float y, out List<int> viewList)
        {
            Debug.Assert(!_entittDict.ContainsKey(entity));

            PointToZonePoint(x, y, out var outX, out var outY);
            ZonePointToZoneKey(outX, outY, out var zoneKey);
            var aoiEntity = new AoiEntity()
            {
                ZoneKey = zoneKey,
                PendingZoneKeySet = new()
            };
            _entittDict.Add(entity, aoiEntity);

            var zone = CreateZone(zoneKey);
            zone.Enter(aoiEntity);

            viewList = new();
            var viewZoneArray = GetViewZoneArray(outX, outY);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(entity, curZoneKey, viewList);
            }
            return aoiEntity;
        }

        public void Leave(AoiEntity aoiEntity, out List<int> leaveList)
        {
            var zone = _zoneDict[aoiEntity.ZoneKey];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);

            leaveList = new();
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(aoiEntity.EntityId, curZoneKey, leaveList);
            }

            // 待清理区域中的实体
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
            {
                var pendingZone = _zoneDict[pendingZoneKey];
                AddZoneEntitysToList(aoiEntity.EntityId, pendingZone, leaveList);
                pendingZone.PendingEntitySet.Remove(aoiEntity.EntityId);
            }

            zone.Leave(aoiEntity, leaveList);
            _entittDict.Remove(aoiEntity.EntityId);
        }

        /// <summary>
        /// 返回是否跨越边界
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public bool Refresh(AoiEntity aoiEntity, float x, float y, out List<int>? enterList, out List<int>? leaveList)
        {
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
                oldZone.Leave(aoiEntity, leaveList);

                ZonePointToZoneKey(newZoneX, newZoneY, out var newZoneKey);

                var newZone = CreateZone(newZoneKey);
                newZone.Enter(aoiEntity);

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
                        AddZoneEntitysToList(aoiEntity.EntityId, newViewZoneKey, enterList);
                    }
                }
                // 首先跨边界移动可能出现PendingZone需要清除的情况，根据距离清除
                foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
                {
                    ZoneKeyToZonePoint(pendingZoneKey, out var pendingZoneX, out var pendingZoneY);
                    var distance = GetZoneDistance(newZoneX, newZoneY, pendingZoneX, pendingZoneY);
                    if (distance != 2)
                    {
                        var pendingZone = _zoneDict[pendingZoneKey];
                        // 移动后可能会将PendingZone划入九宫格的范围，划入九宫格的则不会离开当前视野
                        if (distance > 2)
                        {
                            AddZoneEntitysToList(aoiEntity.EntityId, pendingZone, leaveList);
                        }
                        aoiEntity.PendingZoneKeySet.Remove(pendingZoneKey);
                        pendingZone.PendingEntitySet.Remove(aoiEntity.EntityId);
                    }
                }

                // 添加原先九宫格的实体到LeaveList
                foreach (var oldViewZonePoint in oldViewZoneArray)
                {
                    ZonePointToZoneKey(oldViewZonePoint.X, oldViewZonePoint.Y, out var curZoneKey);
                    var distance = GetZoneDistance(newZoneX, newZoneY, oldViewZonePoint.X, oldViewZonePoint.Y);
                    // 将距离为2的Zone添加到Pending中
                    if (distance == 2)
                    {
                        aoiEntity.PendingZoneKeySet.Add(curZoneKey);
                        var curZone = CreateZone(curZoneKey);
                        curZone.PendingEntitySet.Add(aoiEntity.EntityId);
                    }
                    // 距离如果大于2就直接添加到LeaveList
                    else if (distance > 2)
                    {
                        AddZoneEntitysToList(aoiEntity.EntityId, curZoneKey, leaveList);
                    }
                }

                aoiEntity.ZoneKey = newZoneKey;
                return true;
            }

            leaveList = null;
            enterList = null;
            return false;
        }

        public List<int> GetFollowingEntityList(AoiEntity aoiEntity)
        {
            var viewList = new List<int>();
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(aoiEntity.EntityId, curZoneKey, viewList);
            }
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
            {
                AddZoneEntitysToList(aoiEntity.EntityId, pendingZoneKey, viewList);
            }
            foreach (var following in aoiEntity.ExtraFollowingSet)
            {
                viewList.Add(following);
            }
            return viewList;
        }

        public bool IsFollowing(AoiEntity aoiEntity, AoiEntity target)
        {
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);
            ZoneKeyToZonePoint(target.ZoneKey, out var targetX, out var targetY);
            if (Math.Abs(x - targetX) <= _zoneSize && Math.Abs(y - targetY) <= _zoneSize) return true;
            if (aoiEntity.PendingZoneKeySet.Contains(target.ZoneKey)) return true;
            return aoiEntity.ExtraFollowingSet.Contains(target.EntityId);
        }

        public void AddExtraFollowing(AoiEntity aoiEntity, int followingEntity)
        {
            aoiEntity.ExtraFollowingSet.Add(followingEntity);
        }

        public void RemoveExtraFollowing(AoiEntity aoiEntity, int followingEntity)
        {
            aoiEntity.ExtraFollowingSet.Remove(followingEntity);
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
