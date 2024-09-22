using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Serilog;

namespace GameServer.Db
{
    public class SqlDb
    {
        public static IFreeSql FreeSql { get; }

        static SqlDb()
        {
            FreeSql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(global::FreeSql.DataType.MySql, $"Data Source={DbConfig.Host};Port={DbConfig.Port};User Id={DbConfig.User};Password={DbConfig.Password};")
                .UseAutoSyncStructure(true)
            .Build();

            // 检查数据库是否存在
            var exists = FreeSql.Ado.QuerySingle<int>($"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = '{DbConfig.DbName}'") > 0;
            if (!exists)
            {
                FreeSql.Ado.ExecuteNonQuery($"CREATE DATABASE {DbConfig.DbName}");
                Log.Information($"数据库“{DbConfig.DbName}”不存在，已自动创建");
            }

            // 重新链接
            FreeSql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(global::FreeSql.DataType.MySql,  $"Data Source={DbConfig.Host};Port={DbConfig.Port};User Id={DbConfig.User};Password={DbConfig.Password};" +
                                                                      $"Initial Catalog={DbConfig.DbName};Charset=utf8;SslMode=none;Max pool size=10")
                .UseAutoSyncStructure(true)
                .Build();

            exists = FreeSql.DbFirst.GetTablesByDatabase("user").Exists(t => t.Name == "user");

            if (!exists)
            {
                // FreeSql.CodeFirst.SyncStructure<DbUser>();
                FreeSql.Insert(new DbUser("root", "1234567890", Authoritys.Administrator)).ExecuteAffrows();
                Log.Information($"数据库“{DbConfig.DbName}”中的“user”表不存在，已自动创建，并添加一个管理员账号（账号=root，密码=1234567890）");
            }
        }
    }
}
