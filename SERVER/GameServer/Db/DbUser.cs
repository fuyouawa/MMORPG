using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace GameServer.Db
{
    [Table(Name = "user")]
    public class DbUser
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
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
}
