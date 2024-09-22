using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;

namespace GameServer.Db
{
    public enum Authoritys
    {
        Player,
        Administrator
    }

    [Table(Name = "user")]
    public class DbUser
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Authoritys Authority { get; set; }

        public DbUser(string username, string password, Authoritys authority)
        {
            Username = username;
            Password = password;
            Authority = authority;
        }
    }
}
