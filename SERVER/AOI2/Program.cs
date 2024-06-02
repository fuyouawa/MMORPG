using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;

namespace Aoi
{
    /// 优化的九宫格Aoi系统

    /// 频繁跨边界的优化方案：
    /// | |c| | |
    /// | | |b| | |
    ///   | | |a| |
    ///     | | | |
    /// 如此处从a移动到b，并不立刻移除a在下方和右侧的5个格子(保留依据为格子距离<=1)
    /// 此时如果b再移动到c，再将这5个格子距离>1的格子移除，这样最坏情况下是16个格子，但基本避免了频繁跨边界导致的广播进入、离开的问题
    


internal class Program
    {
        static void Main(string[] args)
        {

            var zone = new AoiWord(20);
            var area = new Vector2(1, 1);

            Console.WriteLine($"{(-1) % 20 + 20}");

            Dictionary<int, AoiWord.AoiEntity> dir = new();

            // 添加玩家
            int count = 3;
            for (var i = 1; i <= count; i++)
            {
                for (var j = 1; j <= count; j++)
                {
                    dir[(i - 1) * count + j] = zone.Enter((i - 1) * count + j, i, j);
                    Console.WriteLine($"---------------{(i - 1) * count + j}--------------");

                    zone.ScanFollowerList(dir[(i - 1) * count + j], aoiKey =>
                    {
                        //var findEntity = zone[aoiKey];
                        //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");

                        int x = aoiKey / count + 1;
                        int y = aoiKey % count;
                        if (y == 0)
                        {
                            y = count;
                            x--;
                        }

                        Console.WriteLine($"entity:{aoiKey} X:{x}, Y:{y}");
                    });
                }
            }

            for (var i = 1; i <= count; i++)
            {
                for (var j = 1; j <= count; j++)
                {
                    zone.ScanFollowingList(dir[(i - 1) * count + j], aoiKey =>
                    {
                        //var findEntity = zone[aoiKey];
                        //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");

                        int x = aoiKey / count + 1;
                        int y = aoiKey % count;
                        if (y == 0)
                        {
                            y = count;
                            x--;
                        }
                        Console.WriteLine($"entity:{aoiKey} X:{x}, Y:{y}");
                    });
                    Console.WriteLine($"---------------{(i - 1) * count + j}--------------");
                }
            }

            {
                //Console.WriteLine($"---------------离开--------------");
                //zone.Leave(dir[1]);
                //foreach (var aoiKey in leaveList)
                //{
                //    //var findEntity = zone[aoiKey];
                //    //Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");

                //    int x = aoiKey / count + 1;
                //    int y = aoiKey % count;
                //    if (y == 0)
                //    {
                //        y = count;
                //        x--;
                //    }
                //    Console.WriteLine($"entity:{aoiKey} X:{x}, Y:{y}");
                //}
            }
            // 测试移动。
            // while (true)
            // {
            //     Console.WriteLine("1");
            //     zone.Refresh(new Random().Next(0, 50000), new Random().Next(0, 50000), new Random().Next(0, 50000), area);
            //     Console.WriteLine("2");
            // }



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
