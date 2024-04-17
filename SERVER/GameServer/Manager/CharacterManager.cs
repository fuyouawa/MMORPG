using Common.Tool;
using GameServer.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class CharacterManager : Singleton<EntityManager>
    {
        public Dictionary<int, Character> CharacterDict = new();
        private Space _space;

        public CharacterManager(Space space)
        {
            _space = space;
        }

        public Character NewCharacter(Player player, Vector3 pos, Vector3 dire, string name)
        {
            var character = new Character()
            {
                Player = player,
                EntityId = EntityManager.Instance.NewEntityId(),
                EntityType = EntityType.Character,
                Position = pos,
                Direction = dire,

                Name = name,
                Space = _space,
                Speed = 5,

            };
            EntityManager.Instance.AddEntity(character);

            lock (CharacterDict)
            {
                CharacterDict.Add(character.EntityId, character);
            }

            return character;
        }

        public void RemoveCharacter(Character character)
        {
            EntityManager.Instance.RemoveEntity(character);
            lock (CharacterDict)
            {
                CharacterDict.Remove(character.EntityId);
            }
            character.Space = null;
        }
    }
}
