using GameServer.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameServer.BuffSystem
{
    public class StateBuff : Buff
    {

        public StateBuff(int buffId, BuffManager buffManager, Actor? caster, float duration, string StateModifier)
            : base(buffId, buffManager, caster, duration)
        {
            //_attributeModifier = JArray.Parse(StateModifier);
        }

        public override void Start()
        {
            base.Start();
            Change("Start");
        }

        public override void Exit()
        {
            base.Exit();
            Change("Exit");
        }

        private void Change(string stage)
        {
            
        }
    }
}