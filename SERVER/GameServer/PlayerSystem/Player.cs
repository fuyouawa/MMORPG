using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Entity;
using GameServer.Db;
using Google.Protobuf;
using Serilog;
using GameServer.MapSystem;
using GameServer.UserSystem;
using GameServer.EntitySystem;
using GameServer.NpcSystem;

namespace GameServer.PlayerSystem
{
    public class Player : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        public User User;
        public long CharacterId;
        public int Exp;
        public long Gold;
        public InventorySystem.Inventory Knapsack;

        public Npc? InteractingNpc;
        public int CurrentDialogueId;
        public NpcSystem.DialogueManager DialogueManager;

        private DbCharacter _dbCharacter;

        public Player(int entityId, DbCharacter dbCharacter,
            Map map, Vector3 pos, Vector3 dire, User user)
            : base(EntityType.Player, entityId, dbCharacter.UnitId, map, pos, dire, dbCharacter.Name)
        {
            User = user;
            CharacterId = dbCharacter.Id;
            Knapsack = new(this);
            DialogueManager = new(this);

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
            Knapsack.LoadInventoryInfo(_dbCharacter.Knapsack);
            DialogueManager.LoadDialogueInfo(_dbCharacter.DialogueInfo);
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
                DialogueInfo = DialogueManager.GetDialogueInfo().ToByteArray(),
            };
        }
    }
}
