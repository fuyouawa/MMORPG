using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
using GameServer.Db;
using Google.Protobuf;
using Serilog;

namespace GameServer.Model
{
    public class Player : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        public User User;
        public int CharacterId;
        public int JobId;
        public int Exp;
        public int Gold;

        public Inventory.Inventory Knapsack;

        public Player(int entityId, int unitId, 
            Map map, string characterName, User user) 
            : base(EntityType.Player, entityId, unitId, map, characterName)
        {
            User = user;
            Knapsack = new(this);
        }

        public override void Start()
        {
            base.Start();

            var dbCharacter = Db.SqlDb.Connection.Select<DbCharacter>()
                .Where(p => p.Id == CharacterId)
                .First();
            if (dbCharacter == null)
            {
                Log.Error($"为不存在的角色读取背包");
                return;
            }
            Knapsack.LoadInventoryInfoData(dbCharacter.Knapsack);
        }

        public override void Update()
        {
            base.Update();
        }

        public DbCharacter ToDbCharacter()
        {
            return new DbCharacter(Name, User.UserId, UnitId, Map.MapId, Level, Exp, Gold, (int)Hp, (int)Mp, Knapsack.GetInventoryInfo().ToByteArray());
        }
    }
}
