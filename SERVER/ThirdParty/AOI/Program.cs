using System;
using System.Numerics;

namespace AOI
{
    class Program
    {
        static void Main(string[] args)
        {
            var zone = new AoiZone(.001f, .001f);
            var area = new Vector2(3, 3);

            // 添加500个玩家。

            for (var i = 1; i <= 6; i++)
            {
                for (var j = 1; j <= 6; j++)
                {
                    zone.Enter((i - 1) * 6 + j, i, j);
                }
            }

            // 测试移动。
            // while (true)
            // {
            //     Console.WriteLine("1");
            //     zone.Refresh(new Random().Next(0, 50000), new Random().Next(0, 50000), new Random().Next(0, 50000), area);
            //     Console.WriteLine("2");
            // }
    
            // 刷新key为3的信息。
    
            zone.Refresh(9, area, out var enters);

            Console.WriteLine("---------------加入玩家范围的玩家列表--------------");
    
            foreach (var aoiKey in enters)
            {
                var findEntity = zone[aoiKey];
                Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            }
    
            // 更新key为50的坐标。
    
            var entity = zone.Refresh(3, 4, 4, new Vector2(3, 3), out enters);


            Console.WriteLine("---------------离开玩家范围的玩家列表--------------");
    
            foreach (var aoiKey in entity.Leave)
            {
                var findEntity = zone[aoiKey];
                Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            }
    
            Console.WriteLine("---------------key为3移动后玩家范围内的新玩家列表--------------");
    
            foreach (var aoiKey in entity.ViewEntity)
            {
                var findEntity = zone[aoiKey];
                Console.WriteLine($"X:{findEntity.X.Value} Y:{findEntity.Y.Value}");
            }
    
            // 离开当前AOI
    
            zone.Exit(50);
        }
    }
}