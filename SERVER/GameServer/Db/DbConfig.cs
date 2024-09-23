using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Db
{
    public class DbConfig
    {
        /// <summary>
        /// 数据库Host
        /// </summary>
        public static readonly string Host = "127.0.0.1";
        /// <summary>
        /// 数据库端口
        /// </summary>
        public static readonly int Port = 3306;
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public static readonly string User = "root";
        /// <summary>
        /// 数据库密码
        /// </summary>
        public static readonly string Password = "root";
        /// <summary>
        /// 数据库名称
        /// </summary>
        public static readonly string DbName = "MMORPG";
    }
}
