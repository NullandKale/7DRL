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
        private bool up;

        public cStair(bool _up)
        {
            up = _up;
        }

        public void Run(drawable d)
        {
            if(Game.input.isKeyHeld(OpenTK.Input.Key.Space))
            {
                if (Point.dist(d.pos, Game.g.player.pos) < 2)
                {
                    Game.g.resetWorld = true;
                    Game.g.resetWorldUp = up;
                }
            }
        }
    }
}
