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
        private bool up;

        public cStair(drawable p, bool _up)
        {
            player = p;
            up = _up;
        }

        public void Run(drawable d)
        {
            if(Game.input.isKeyHeld(OpenTK.Input.Key.Space))
            {
                if (Point.dist(d.pos, player.pos) < 1.5)
                {
                    Game.g.resetWorld = true;
                    Game.g.resetWorldUp = up;
                }
            }
        }
    }
}
