using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Entity
    {
        private int _entityId;
        public int EntityId { get { return _entityId; } }

        private Vector3 _position;
        public Vector3 Position {  get { return _position; } }

        private Vector3 _direction;
        public Vector3 Direction {  get { return _direction; } }

        public Entity(int entityId, Vector3 position, Vector3 direction) {
            _entityId = entityId;
            _position = position;
            _direction = direction;
        }
    }
}
