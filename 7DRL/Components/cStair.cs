﻿namespace _7DRL.Components
{
    using Entities;
    using Utils;

    public class cStair : iComponent
    {
        private bool up;

        public cStair(bool up)
        {
            this.up = up;
        }

        public void Run(Drawable d)
        {
            if (Game.input.IsKeyHeld(OpenTK.Input.Key.Space))
            {
                if (Point.SquareDist(d.pos, Game.g.player.pos) < 4)
                {
                    Game.g.resetWorld = true;
                    Game.g.resetWorldUp = up;
                }
            }
        }
    }
}
