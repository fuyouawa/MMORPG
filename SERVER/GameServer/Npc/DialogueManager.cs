using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;

namespace GameServer.Npc
{
    public class DialogueManager
    {
        public Player PlayerOwner;
        private Dictionary<int, DialogueRecord> _recordDict = new();        // key:NpcId

        public DialogueManager(Player playerOwner)
        {
            PlayerOwner = playerOwner;
        }


    }
}
