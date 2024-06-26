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
using GameServer.TaskSystem;

namespace GameServer.PlayerSystem
{
    public class Player : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        public User User;
        public DbCharacter DbCharacter;
        public int Exp;
        public long Gold;
        public InventorySystem.Inventory Knapsack;

        public Npc? InteractingNpc;
        public int CurrentDialogueId;
        public DialogueManager DialogueManager;
        public TaskManager TaskManager;

        public Player(int entityId, DbCharacter dbCharacter, UnitDefine unitDefine,
            Map map, Vector3 pos, Vector3 dire, User user, int level)
            : base(EntityType.Player, entityId, unitDefine, map, pos, dire, dbCharacter.Name, level)
        {
            User = user;
            DbCharacter = dbCharacter;
            Knapsack = new(this);
            DialogueManager = new(this);
            TaskManager = new(this);
        }

        public override void Start()
        {
            base.Start();
            Hp = DbCharacter.Hp;
            Mp = DbCharacter.Mp;
            Level = DbCharacter.Level;
            Exp = DbCharacter.Exp;
            Gold = DbCharacter.Gold;
            Knapsack.LoadInventoryInfo(DbCharacter.Knapsack);
            DialogueManager.LoadDialogueInfo(DbCharacter.DialogueInfo);
            TaskManager.LoadTaskInfo(DbCharacter.TaskInfo);
        }

        public override void Update()
        {
            base.Update();
        }

        public DbCharacter ToDbCharacter()
        {
            return new DbCharacter()
            {
                Id = DbCharacter.Id,
                Name = Name,
                UserId = User.UserId,
                UnitId = UnitDefine.ID,
                MapId = Map.MapId,
                Level = Level,
                Exp = Exp,
                Gold = Gold,
                Hp = (int)Hp,
                Mp = (int)Mp,
                X = (int)Position.X,
                Z = (int)Position.Y,
                Knapsack = Knapsack.GetInventoryInfo().ToByteArray(),
                DialogueInfo = DialogueManager.GetDialogueInfo().ToByteArray(),
                TaskInfo = TaskManager.GetTaskInfo().ToByteArray(),
            };
        }

        public void ChangeExp(int amount)
        {
            Exp += amount;
            var maxExp = CalculateExp(Level);
            var newLevel = Level;
            while (Exp >= maxExp)
            {
                Exp -= maxExp;
                maxExp = CalculateExp(++newLevel);
            }
            EntityAttributeEntrySync(EntityAttributeEntryType.Exp, Exp);
            ChangeLevel(newLevel);
            
        }

        public void ChangeLevel(int newLevel)
        {
            if (Level == newLevel) return;
            EntityAttributeEntrySync(EntityAttributeEntryType.Level, newLevel);
            AttributeManager.Recalculate();
            ChangeHP(AttributeManager.Final.MaxHp);
            ChangeMp(AttributeManager.Final.MaxMp);
        }


        private static int CalculateExp(int level)
        {
            // 基础经验值
            const int baseXP = 100;
            // 级数调整因子
            const double levelFactor = 1.5;
            // 经验值公式
            return (int)(baseXP * Math.Pow(level, levelFactor));
        }
    }
}
