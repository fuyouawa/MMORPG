using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Db
{
    public class SqlDb
    {
        private static readonly string ConnectionString =
            $"Data Source={DbConfig.Host};Port={DbConfig.Port};User Id={DbConfig.User};Password={DbConfig.Password};" +
            $"Initial Catalog={DbConfig.DbName};Charset=utf8;SslMode=none;Max pool size=10";

        private static IFreeSql _connection;
        public static IFreeSql Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new FreeSql.FreeSqlBuilder()
                        .UseConnectionString(FreeSql.DataType.MySql, ConnectionString)
                        .UseAutoSyncStructure(true)
                        .Build();
                }
                return _connection;
            }
        }
    }
}
