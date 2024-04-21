using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerJoinedEvent
{
    public Player Player { get; }

    public PlayerJoinedEvent(Player player)
    {
        Player = player;
    }
}