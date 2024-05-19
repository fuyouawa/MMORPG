using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;

namespace AOI2
{
    /// 优化的九宫格Aoi系统

    /// 频繁跨边界的优化方案：
    /// | |c| | |
    /// | | |b| | |
    ///   | | |a| |
    ///     | | | |
    /// 如此处从a移动到b，并不立刻移除a在下方和右侧的5个格子(保留依据为格子距离<=1)
    /// 此时如果b再移动到c，再将这5个格子距离>1的格子移除，这样最坏情况下是16个格子，但基本避免了频繁跨边界导致的广播进入、离开的问题
    
    public class AoiWord
    {
        public struct AoiEntity
        {
            public UInt64 ZoneKey;
            // 间距为2的待清理区域
            public List<UInt64> PendingZoneKeyList;
        }

        public struct Vector2Int
        {
            public int X, Y;
        }

        public class AoiZone
        {
            // 当前区域内的所有实体
            public HashSet<int> EntitySet = new();

            // 在Pending中关注该区域的所有实体，即哪些实体的Pending中有当前区域
            public HashSet<int> InterestEntitySet = new();
        }

        private Dictionary<UInt64, AoiZone> _zoneDict = new();
        public Dictionary<int, AoiEntity> _entittDict = new();
        private int _zoneSize;

        public AoiWord(int zoneSize)
        {
            _zoneSize = zoneSize;
        }

        public void Enter(int entity, Vector2 pos, out List<int> enterList)
        {
            Debug.Assert(!_entittDict.ContainsKey(entity));

            PointToZonePoint(pos, out var x, out var y);
            ZonePointToZoneKey(x, y, out var zoneKey);
            AoiZone zone;
            if (!_zoneDict.TryGetValue(zoneKey, out zone))
            {
                zone = new AoiZone();
                _zoneDict[zoneKey] = zone;
            }
            zone.EntitySet.Add(entity);
            _entittDict.Add(entity, new()
            {
                ZoneKey = zoneKey,
                PendingZoneKeyList = new()
            });

            enterList = new();
            var viewZone = GetViewZone(x, y);
            foreach (var curZonePoint in viewZone)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(curZoneKey, enterList);
            }
        }

        public void Leave(int entity, out List<int> leaveList)
        {
            Debug.Assert(!_entittDict.ContainsKey(entity));

            var aoiEntity = _entittDict[entity];
            var zone = _zoneDict[aoiEntity.ZoneKey];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var x, out var y);

            leaveList = new();
            var viewZone = GetViewZone(x, y);
            foreach (var curZonePoint in viewZone)
            {
                ZonePointToZoneKey(curZonePoint.X, curZonePoint.Y, out var curZoneKey);
                AddZoneEntitysToList(curZoneKey, leaveList);
            }
            foreach (var pendingZoneKey in aoiEntity.PendingZoneKeyList)
            {
                AddZoneEntitysToList(pendingZoneKey, leaveList);
            }

            // 关注该区域的实体，但不一定应该放到LeaveList里，或许需要另外放到一个变量中
            foreach (var interestEntity in zone.InterestEntitySet)
            {
                leaveList.Add(interestEntity);
            }

            zone.EntitySet.Remove(entity);
            _entittDict.Remove(entity);
        }


        /// <summary>
        /// 返回是否跨越边界
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Refresh(int entity, Vector2 pos, out List<int>? leaveList, out List<int>? enterList)
        {
            Debug.Assert(!_entittDict.ContainsKey(entity));

            var aoiEntity = _entittDict[entity];
            ZoneKeyToZonePoint(aoiEntity.ZoneKey, out var minX, out var minY);
            int maxX = minX + _zoneSize - 1;
            int maxY = minY + _zoneSize - 1;

            if (pos.X < minX || pos.Y < minY ||
                pos.X > maxX || pos.Y > maxY)
            {
                leaveList = new();
                enterList = new();
                PointToZonePoint(pos, out var newZoneX, out var newZoneY);
                
                // 跨越边界，根据新的九宫格来确定离开视野范围以及新进入视野范围的实体

                // 首先跨边界移动可能出现PendingZone需要清除的情况，根据距离清除
                foreach (var zoneKey in aoiEntity.PendingZoneKeyList)
                {
                    ZoneKeyToZonePoint(zoneKey, out var zoneX, out var zoneY);
                    var distance = GetZoneDistance(newZoneX, newZoneY, zoneX, zoneY);
                    if (distance > 2)
                    {
                        var zone = _zoneDict[zoneKey];
                        AddZoneEntitysToList(zone, leaveList);
                        zone.InterestEntitySet.Remove(entity);
                    }
                }
                aoiEntity.PendingZoneKeyList.Clear();

                var oldViewZone = GetViewZone(minX, minY);
                var newViewZone = GetViewZone(newZoneX, newZoneY);

                // 添加实体到LeaveList和EnterList中
                foreach (var zonePoint in oldViewZone)
                {
                    ZonePointToZoneKey(zonePoint.X, zonePoint.Y, out var zoneKey);
                    var distance = GetZoneDistance(newZoneX, newZoneY, zonePoint.X, zonePoint.Y);
                    // 将距离为2的Zone添加到Pending中
                    if (distance == 2)
                    {
                        aoiEntity.PendingZoneKeyList.Add(zoneKey);
                        var zone = _zoneDict[zoneKey];
                        zone.InterestEntitySet.Add(entity);
                    }
                    // 距离如果大于2就直接添加到LeaveList
                    else if (distance > 2)
                    {
                        AddZoneEntitysToList(zoneKey, leaveList);
                    }
                }
                // 如果在原先的zone中不存在，则是新加入到观察列表的区域
                foreach (var newZonePoint in newViewZone)
                {
                    bool exist = false;
                    foreach (var zonePoint in oldViewZone)
                    {
                        if (newZonePoint.X == zonePoint.X && newZonePoint.Y == zonePoint.Y)
                        {
                            exist = true;
                            break;
                        }
                    }
                    ZonePointToZoneKey(newZonePoint.X, newZonePoint.Y, out var zoneKey);
                    if (exist == false)
                    {
                        AddZoneEntitysToList(zoneKey, enterList);
                    }
                }
                return true;
            }

            leaveList = null;
            enterList = null;
            return false;
        }

        /// <summary>
        /// 获取九宫格可视区域
        /// </summary>
        private Vector2Int[] GetViewZone(int x, int y)
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

        private void PointToZonePoint(Vector2 pos, out int x, out int y)
        {
            x = (int)pos.X; 
            x -= x % _zoneSize;
            y = (int)pos.Y;
            y -= y % _zoneSize;
        }

        private void ZoneKeyToZonePoint(UInt64 zoneKey, out int x, out int y)
        {
            x = (int)(zoneKey >> 32);
            y = (int)zoneKey;
        }

        private void ZonePointToZoneKey(int x, int y, out UInt64 zoneKey)
        {
            zoneKey = ((UInt64)x << 32) | (UInt64)y;
        }

        private int GetZoneDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Max(Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        private void AddZoneEntitysToList(AoiZone zone, List<int> list)
        {
            foreach (var entity in zone.EntitySet)
            {
                list.Add(entity);
            }
        }

        private void AddZoneEntitysToList(UInt64 zoneKey, List<int> list)
        {
            if (!_zoneDict.TryGetValue(zoneKey, out var zone)) return;
            AddZoneEntitysToList(zone, list);
        }
    }


internal class Program
    {
        static void Main(string[] args)
        {
            var zone = new AoiWord(1);
            var area = new Vector2(1, 1);

            // 添加500个玩家。

            //for (var i = 1; i <= 6; i++)
            //{
            //    for (var j = 1; j <= 6; j++)
            //    {
            //        zone.Enter((i - 1) * 6 + j, new(i, j), out var enterList);
            //    }
            //}

            // 测试移动。
            // while (true)
            // {
            //     Console.WriteLine("1");
            //     zone.Refresh(new Random().Next(0, 50000), new Random().Next(0, 50000), new Random().Next(0, 50000), area);
            //     Console.WriteLine("2");
            // }


            zone.Enter(1, new(1, 1), out var enterList);
            zone.Enter(2, new(2, 2), out var enterList2);
            zone.Enter(3, new(3, 3), out var enterList3);

            Console.WriteLine("---------------1--------------");
            foreach (var aoiKey in enterList)
            {
                //var findEntity = zone[aoiKey];
                //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
                Console.WriteLine($"entity:{aoiKey}");
            }

            Console.WriteLine("---------------2--------------");
            foreach (var aoiKey in enterList2)
            {
                //var findEntity = zone[aoiKey];
                //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
                Console.WriteLine($"entity:{aoiKey}");
            }

            Console.WriteLine("---------------3--------------");
            foreach (var aoiKey in enterList3)
            {
                //var findEntity = zone[aoiKey];
                //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
                Console.WriteLine($"entity:{aoiKey}");
            }

            //// 刷新key为3的信息。

            //zone.Refresh(3, area, out var enters);

            //Console.WriteLine("---------------加入玩家范围的玩家列表--------------");

            //foreach (var aoiKey in enters)
            //{
            //    var findEntity = zone[aoiKey];
            //    Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            //}

            //// 更新key为3的坐标。

            //var entity = zone.Refresh(3, 1, 3, area, out enters);


            //Console.WriteLine("---------------离开玩家范围的玩家列表--------------");

            //foreach (var aoiKey in entity.Leave)
            //{
            //    var findEntity = zone[aoiKey];
            //    Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            //}

            //Console.WriteLine("---------------进入玩家范围的玩家列表--------------");

            //foreach (var aoiKey in entity.Enter)
            //{
            //    var findEntity = zone[aoiKey];
            //    Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            //}




            //Console.WriteLine("---------------key为3移动后玩家范围内的新玩家列表--------------");

            //foreach (var aoiKey in entity.ViewEntity)
            //{
            //    var findEntity = zone[aoiKey];
            //    Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            //}

            //// 离开当前AOI

            //zone.Exit(50);
        }
    }
}
