﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Managers
{
    public static class CollisionManager
    {
        public static bool CheckCollision(int xMove, int yMove, Entities.drawable d)
        {
            int futureX = d.pos.xPos + xMove;
            int futureY = d.pos.yPos + yMove;

            if (Game.isInWorld(futureX, futureY))
            {
                if (Game.g.ground[futureX, futureY].collideable == false && Game.g.world[futureX, futureY].collideable == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool CheckCollision(int x, int y)
        {
            if (Game.isInWorld(x, y))
            {
                if (Game.g.ground[x, y].collideable == false && Game.g.world[x, y].collideable == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
