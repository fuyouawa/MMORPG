using System;
using System.Collections;
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
            public HashSet<UInt64> PendingZoneKeySet = new();
            // 特别关注目标
            //public HashSet<int> SpecialFollowingSet = new();
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
            public void Leave(AoiEntity aoiEntity)
            {
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

        public AoiEntity Enter(int entityId, float x, float y)
        {
            Debug.Assert(!_entittDict.ContainsKey(entityId));

            PointToZonePoint(x, y, out var outX, out var outY);
            ZonePointToZoneKey(outX, outY, out var zoneKey);
            var aoiEntity = new AoiEntity()
            {
                EntityId = entityId,
                ZoneKey = zoneKey,
            };
            _entittDict.Add(entityId, aoiEntity);

            var zone = CreateZone(zoneKey);
            zone.Enter(aoiEntity);
            return aoiEntity;
        }

        public void Leave(AoiEntity aoiEntity)
        {
            var zone = _zoneDict[aoiEntity.ZoneKey];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);

            zone.Leave(aoiEntity);

            _entittDict.Remove(aoiEntity.EntityId);
        }

        /// <summary>
        /// 返回是否跨越边界
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newPos"></param>
        /// <param name="enterFollowingList">返回刷新后进入当前实体视距的实体列表</param>
        /// <param name="leaveFollowingList">返回刷新后离开当前实体视距的实体列表</param>
        /// <param name="enterFollowerList">返回刷新后当前实体会进入哪些实体视距的实体列表</param>
        /// <param name="leaveFollowerList">返回刷新后当前实体会离开哪些实体视距的实体列表</param>
        /// <returns></returns>
        public bool Refresh(AoiEntity aoiEntity, float x, float y, 
            Action<int> enterFollowingCallback, Action<int> leaveFollowingCallback, 
            Action<int> enterFollowerCallback, Action<int> leaveFollowerCallback)
        {
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var minX, out var minY);
            int maxX = minX + _zoneSize;
            int maxY = minY + _zoneSize;

            if (x < minX || y < minY ||
                x >= maxX || y >= maxY)
            {
                PointToZonePoint(x, y, out var newZoneX, out var newZoneY);

                //Console.WriteLine($"实体{aoiEntity.EntityId}PendingZone：");
                //foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
                //{
                //    ZoneKeyToZonePoint(pendingZoneKey, out var pendingZoneX, out var pendingZoneY);
                //    Console.Write($"[{pendingZoneX}, {pendingZoneY}]，");
                //}
                //Console.WriteLine();
                //Console.WriteLine();
                //Console.WriteLine($"实体{aoiEntity.EntityId}跨边界移动：[{minX}, {minY}] -> [{newZoneX}, {newZoneY}]");

                var oldZoneKey = aoiEntity.ZoneKey;
                ZonePointToZoneKey(newZoneX, newZoneY, out var newZoneKey);
                ZoneKeyToZonePoint(oldZoneKey, out var oldZoneX, out var oldZoneY);
                var oldZone = _zoneDict[oldZoneKey];
                var newZone = CreateZone(newZoneKey);

                // 我可能会进入一些实体的PendingZone，甚至是自己的PendingZone
                foreach (var pendingEntityId in newZone.PendingEntitySet)
                {
                    if (pendingEntityId == aoiEntity.EntityId) continue;
                    var pendingEntity = _entittDict[pendingEntityId];
                    // 但我也可能原本就被这个实体关注
                    if (!IsFollowing(pendingEntity, aoiEntity))
                    {
                        enterFollowerCallback(pendingEntityId);
                    }
                }

                oldZone.Leave(aoiEntity);
                newZone.Enter(aoiEntity);
                aoiEntity.ZoneKey = newZoneKey;

                // 我可能会离开一些通过PendingZone关注我的实体的视距
                foreach (var pendingEntityId in oldZone.PendingEntitySet)
                {
                    var pendingEntity = _entittDict[pendingEntityId];
                    // 新位置的我可能依旧在该实体的关注区域内
                    if (!IsFollowing(pendingEntity, aoiEntity))
                    {
                        leaveFollowerCallback(pendingEntityId);
                    }
                }

                // 跨越边界，根据新的九宫格来确定离开视野范围以及新进入视野范围的实体
                var oldViewZoneArray = GetViewZoneArray(minX, minY);
                var newViewZoneArray = GetViewZoneArray(newZoneX, newZoneY);

                // 我关注的实体
                // 两个九宫格之间不重叠的新区域
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
                    if (!exist)
                    {
                        // 该区域内的实体是刚进入视距的实体
                        ScanZoneEntitysAndExclude(aoiEntity.EntityId, newViewZoneKey, enterFollowingCallback);
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
                            // 该区域内的实体是刚离开视距的实体
                            ScanZoneEntitysAndExclude(aoiEntity.EntityId, pendingZone, leaveFollowingCallback);
                        }
                        aoiEntity.PendingZoneKeySet.Remove(pendingZoneKey);
                        pendingZone.PendingEntitySet.Remove(aoiEntity.EntityId);
                    }
                }

                // 添加原先九宫格的实体到leaveFollowingList
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
                    // 距离如果大于2就直接添加到leaveFollowingList
                    else if (distance > 2)
                    {
                        ScanZoneEntitysAndExclude(aoiEntity.EntityId, curZoneKey, leaveFollowingCallback);
                    }
                }

                // 特别关注者所在位置可能被纳入新的关注区域，但不需要处理
                //foreach (var following in aoiEntity.SpecialFollowingSet)
                //{
                //    var followingAoiEntity = _entittDict[following];
                //    if (IsViewZoneEntity(aoiEntity, followingAoiEntity))
                //    {
                //        RemoveSpecialFollowing(aoiEntity, followingAoiEntity);
                //    }
                //}

                // 关注我的实体
                // 两个九宫格之间不重叠的新区域
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
                        // 找到了不重叠的新区域，当前实体可能新加入了该区域内的实体的视距中
                        // 检查该区域的实体是不是已经通过PendingZone关注了旧位置的我
                        ScanZoneEntitys(newViewZoneKey, e =>
                        {
                            if (e == aoiEntity.EntityId || oldZone.PendingEntitySet.Contains(e))
                            {
                                return;
                            }
                            enterFollowerCallback(e);
                        });
                    }
                }

                // 我可能会离开通过原先九宫格关注我的实体的视距
                foreach (var oldViewZonePoint in oldViewZoneArray)
                {
                    bool exist = false;
                    foreach (var newViewZonePoint in newViewZoneArray)
                    {
                        if (newViewZonePoint.X == oldViewZonePoint.X && newViewZonePoint.Y == oldViewZonePoint.Y)
                        {
                            exist = true;
                            break;
                        }
                    }
                    ZonePointToZoneKey(oldViewZonePoint.X, oldViewZonePoint.Y, out var oldViewZoneKey);
                    if (!exist)
                    {
                        // 找到了不重叠的旧区域
                        // 检查该区域的实体是不是通过PendingZone关注了新位置的我
                        ScanZoneEntitys(oldViewZoneKey, e =>
                        {
                            if (e == aoiEntity.EntityId || newZone.PendingEntitySet.Contains(e))
                            {
                                return;
                            }
                            leaveFollowerCallback(e);
                        });
                    }
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取我关注的实体列表
        /// </summary>
        /// <param name="aoiEntity"></param>
        /// <returns></returns>
        public void ScanFollowingList(AoiEntity aoiEntity, Action<int> callback)
        {
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                ScanZoneEntitysAndExclude(aoiEntity.EntityId, curZoneKey, callback);
            }
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeySet)
            {
                ScanZoneEntitysAndExclude(aoiEntity.EntityId, pendingZoneKey, callback);
            }
            //foreach (var following in aoiEntity.SpecialFollowingSet)
            //{
            //    // 如果在关注区域内，说明已经添加过了，跳过
            //    var followingAoiEntity = _entittDict[following];
            //    if (!IsViewZoneEntity(aoiEntity, followingAoiEntity))
            //    {
            //        list.Add(following);
            //    }
            //}
        }

        /// <summary>
        /// 获取关注我的实体列表
        /// </summary>
        /// <param name="aoiEntity"></param>
        /// <returns></returns>
        public void ScanFollowerList(AoiEntity aoiEntity, Action<int> callback)
        {
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);
            var viewZoneArray = GetViewZoneArray(x, y);
            foreach (var curZonePoint in viewZoneArray)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                ScanZoneEntitysAndExclude(aoiEntity.EntityId, curZoneKey, callback);
            }
            // 在Pending区域中关注我的实体
            var zone = _zoneDict[aoiEntity.ZoneKey];
            foreach (var entityId in zone.PendingEntitySet)
            {
                callback(entityId);
            }
        }

        /// <summary>
        /// 判断src是否关注了dest
        /// </summary>
        /// <param name="aoiEntity"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsFollowing(AoiEntity src, AoiEntity dest)
        {
            return IsViewZoneEntity(src, dest);
            //|| aoiEntity.SpecialFollowingSet.Contains(dest.EntityId);
        }


        private bool IsViewZoneEntity(AoiEntity src, AoiEntity dest)
        {
            ZoneKeyToZonePoint(src.ZoneKey, out var x, out var y);
            ZoneKeyToZonePoint(dest.ZoneKey, out var targetX, out var targetY);
            if (Math.Abs(x - targetX) <= _zoneSize && Math.Abs(y - targetY) <= _zoneSize) return true;
            return src.PendingZoneKeySet.Contains(dest.ZoneKey);
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

        private void ScanZoneEntitys(AoiZone zone, Action<int> callback)
        {
            foreach (var entityId in zone.EntitySet)
            {
                callback(entityId);
            }
        }

        private void ScanZoneEntitys(UInt64 zoneKey, Action<int> callback)
        {
            if (!_zoneDict.TryGetValue(zoneKey, out var zone)) return;
            ScanZoneEntitys(zone, callback);
        }

        private void ScanZoneEntitysAndExclude(int entityId, AoiZone zone, Action<int> callback)
        {
            ScanZoneEntitys(zone, e =>
            {
                if (entityId == e)
                {
                    return;
                }
                callback(e);
            });
        }

        private void ScanZoneEntitysAndExclude(int entityId, UInt64 zoneKey, Action<int> callback)
        {
            if (!_zoneDict.TryGetValue(zoneKey, out var zone)) return;
            ScanZoneEntitysAndExclude(entityId, zone, callback);
        }

    }
}
