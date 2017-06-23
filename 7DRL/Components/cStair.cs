using _7DRL.Entities;
using _7DRL.Managers;
using _7DRL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Components
{
    public class cStair : iComponent
    {
        private drawable player;

        public cStair(drawable p)
        {
            player = p;
        }

        public void Run(drawable d)
        {
            if (Point.dist(d.pos, player.pos) < 1.5)
            {
                // TODO: Fix world resesting. Problem Solution: needs to stop update/draw first.
                Game.g.resetWorld = true;
            }
        }
    }
}
