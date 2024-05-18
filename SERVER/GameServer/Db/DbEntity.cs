using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Db
{

    [Table(Name = "user")]
    public class DbUser
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Coin { get; set; }

        public DbUser(string username, string password, int coin)
        {
            Username = username;
            Password = password;
            Coin = coin;
        }
    }

    [Table(Name = "character")]
    public class DbCharacter
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }
        public int UserId { get; set; }
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

        public DbCharacter(string name, int userId, int unitId, int mapId, int hp, int mp, int level, int exp, long gold)
        {
            Name = name;
            UserId = userId;
            UnitId = unitId;
            MapId = mapId;
            Hp = hp;
            Mp = mp;
            Level = level;
            Exp = exp;
            Gold = gold;
        }
    }
}
