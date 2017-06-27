namespace _7DRL.Components
{
    using _7DRL.Entities;

    public class cCameraFollow : iComponent
    {
        private Game world;

        public cCameraFollow(Game g)
        {
            world = g;
        }

        public void Run(Drawable d)
        {
            if (d.pos.xPos > (world.gameX / 2) + world.worldOffsetX)
            {
                if (world.gameX + world.worldOffsetX < world.worldSize)
                {
                    world.worldOffsetX++;
                }
            }
            else if (d.pos.xPos < (world.gameX / 2) + world.worldOffsetX)
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
