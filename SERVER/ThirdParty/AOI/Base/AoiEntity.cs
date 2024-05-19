using System;
using System.Collections.Generic;
using System.Linq;

namespace AOI
{
    public sealed class AoiEntity : IDisposable
    {
        public readonly long Key;
        public AoiNode X;
        public AoiNode Y;
        public HashSet<long> ViewEntity;
        public HashSet<long> ViewEntityBak;
        public IEnumerable<long> Move => ViewEntity.Union(ViewEntityBak);
        public IEnumerable<long> Leave => ViewEntityBak.Except(ViewEntity);
        public IEnumerable<long> Enter
        {
            get
            {
                var enters = new List<long>();
                foreach (var entity in ViewEntity)
                {
                    if (!ViewEntityBak.Contains(entity))
                    {
                        enters.Add(entity);
                    }
                }
                return enters;
            }
        }


        public AoiEntity(long key)
        {
            Key = key;
            ViewEntity = Pool<HashSet<long>>.Rent();
            ViewEntityBak = Pool<HashSet<long>>.Rent();
        }

        public void Dispose()
        {
            ViewEntity.Clear();
            Pool<HashSet<long>>.Return(ViewEntity);
            ViewEntityBak.Clear();
            Pool<HashSet<long>>.Return(ViewEntityBak);
        }
    }
}