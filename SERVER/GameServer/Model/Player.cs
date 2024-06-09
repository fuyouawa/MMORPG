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
        public long CharacterId;
        public int Exp;
        public long Gold;
        public Inventory.Inventory Knapsack;

        private DbCharacter _dbCharacter;

        public Player(int entityId, DbCharacter dbCharacter, 
            Map map, User user) 
            : base(EntityType.Player, entityId, dbCharacter.UnitId, map, dbCharacter.Name)
        {
            User = user;
            CharacterId = dbCharacter.Id;
            Knapsack = new(this);

            _dbCharacter = dbCharacter;
        }

        public override void Start()
        {
            base.Start();
            Hp = _dbCharacter.Hp;
            Mp = _dbCharacter.Mp;
            Level = _dbCharacter.Level;
            Exp = _dbCharacter.Exp;
            Gold = _dbCharacter.Gold;
            Knapsack.LoadInventoryInfoData(_dbCharacter.Knapsack);
        }

        public override void Update()
        {
            base.Update();
        }

        public DbCharacter ToDbCharacter()
        {
            return new DbCharacter()
            {
                Id = CharacterId,
                Name = Name,
                UserId = User.UserId,
                UnitId = UnitId,
                MapId = Map.MapId,
                Level = Level,
                Exp = Exp,
                Gold = Gold,
                Hp = (int)Hp,
                Mp = (int)Mp,
                X = (int)Position.X,
                Y = (int)Position.Y,
                Z = (int)Position.Z,
                Knapsack = Knapsack.GetInventoryInfo().ToByteArray(),
            };
        }
    }
}
