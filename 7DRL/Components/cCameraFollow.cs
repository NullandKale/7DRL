using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Entities;
using nullEngine;

namespace _7DRL.Components
{
    class cCameraFollow : iComponent
    {
        private Game world;

        public cCameraFollow(Game g)
        {
            world = g;
        }

        public void Run(drawable d)
        {
            if (d.pos.xPos > (world.screenX / 2) + world.worldOffsetX)
            {
                if (world.screenX + world.worldOffsetX < world.worldSize)
                {
                    world.worldOffsetX++;
                }
            }
            else if (d.pos.xPos < (world.screenX / 2) + world.worldOffsetX)
            {
                if (world.worldOffsetX > 0)
                {
                    world.worldOffsetX--;
                }
            }

            if (d.pos.yPos > (world.screenY / 2) + world.worldOffsetY)
            {
                if (world.screenY + world.worldOffsetY < world.worldSize)
                {
                    world.worldOffsetY++;
                }
            }
            else if (d.pos.yPos < (world.screenY / 2) + world.worldOffsetY)
            {
                if (world.worldOffsetY > 0)
                {
                    world.worldOffsetY--;
                }
            }
        }
    }
}
