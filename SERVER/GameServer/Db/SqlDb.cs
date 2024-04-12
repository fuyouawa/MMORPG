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

        private static IFreeSql _connection = new FreeSql.FreeSqlBuilder()
                        .UseConnectionString(FreeSql.DataType.MySql, ConnectionString)
                        .UseAutoSyncStructure(true)
                        .Build();
        public static IFreeSql Connection
        {
            get
            {
                // 可以在初次访问连接的时候发送一个查询，验证数据库连接是否成功
                
                return _connection;
            }
        }
    }
}
