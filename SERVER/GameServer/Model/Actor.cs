using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        private string _name;
        private int _level;
        private int speed;

        public Actor(int id, Vector3 position, Vector3 direction) : base(id, position, direction)
        {

        }
    }
}
