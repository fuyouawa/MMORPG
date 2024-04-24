using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AOI2
{
    public class AoiZone
    {
        public class AoiNode
        {
            public int EntityId;
            public LinkedList<int> InterestList;
        }

        private List<LinkedList<AoiNode>> _xList;
        private List<LinkedList<AoiNode>> _yList;

        private int _range;

        public AoiZone(int minX, int maxX, int minY, int maxY, int range)
        {
            _xList = new(maxX - minX);
            _yList = new(maxY - minY);
            for (int i = 0; i < _xList.Capacity; ++i)
            {
                _xList[i] = new();
                
            }
            for (int i = 0; i < _yList.Capacity; ++i)
            {
                _yList[i] = new();
            }
        }

        public void Enter(int entityId, float x, float y)
        {
            int ix = (int)x;
            int iy = (int)y;

            _xList[ix].AddLast(new AoiNode(){ EntityId = entityId });
            _yList[iy].AddLast(new AoiNode(){ EntityId = entityId });

            // 加载感兴趣的实体列表
        }

        public void Refresh(int entityId, Vector2 area)
        {
            
        }
    }
}
