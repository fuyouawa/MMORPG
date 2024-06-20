using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Db
{
    [Table(Name = "character")]
    public class DbCharacter
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
        public long UserId { get; set; }
        public int UnitId { get; set; }
        public int MapId { get; set; }
        public string Name { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public long Gold { get; set; }

        [Column(DbType = "blob")]
        public byte[]? Knapsack { get; set; }

        [Column(DbType = "blob")]
        public byte[]? DialogueInfo { get; set; }

        [Column(DbType = "blob")]
        public byte[]? TaskInfo { get; set; }


        //public DbCharacter(long id, string name, long userId, int unitId, int mapId, int level, int exp, long gold, int hp, int mp, byte[]? knapsack)
        //{
        //    Id = id;
        //    Name = name;
        //    UserId = userId;
        //    UnitId = unitId;
        //    MapId = mapId;
        //    Hp = hp;
        //    Mp = mp;
        //    Level = level;
        //    Exp = exp;
        //    Gold = gold;
        //    Knapsack = knapsack;
        //}
    }
}
