using Google.Protobuf;
using QFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPlayerManagerSystem : ISystem
{
    public int MineId { get; }

    public void SetMineId(int entityId);
}

public class PlayerManagerSystem : AbstractSystem, IPlayerManagerSystem
{
    private int _mineId = -1;

    int IPlayerManagerSystem.MineId
    {
        get
        {
            Debug.Assert(_mineId > 0);
            return _mineId;
        }
    }

    public void SetMineId(int entityId)
    {
        _mineId = entityId;
    }

    protected override void OnInit()
    {
        this.RegisterEvent<EntityEnterEvent>(OnEntityEnter);
    }

    private void OnEntityEnter(EntityEnterEvent e)
    {
        this.SendEvent(new PlayerEnterEvent(e.EntityId, e.Position, e.Rotation));
    }
}