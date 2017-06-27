namespace _7DRL.Managers
{
    public static class CollisionManager
    {
        public static bool CheckCollision(int xMove, int yMove, Entities.Drawable d)
        {
            int futureX = d.pos.xPos + xMove;
            int futureY = d.pos.yPos + yMove;

            if (Game.isInWorld(futureX, futureY))
            {
                if (Game.g.ground[futureX, futureY].Collideable == false && Game.g.world[futureX, futureY].Collideable == false)
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
                if (Game.g.ground[x, y].Collideable == false && Game.g.world[x, y].Collideable == false)
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
