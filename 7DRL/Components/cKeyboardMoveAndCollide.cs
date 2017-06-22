using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Entities;
using nullEngine;

namespace _7DRL.Components
{
    class cKeyboardMoveAndCollide : iComponent
    {
        public void Run(drawable d)
        {
            int moveX = 0;
            int moveY = 0;

            if(Game.input.isKeyHeld(OpenTK.Input.Key.A))
            {
                moveX--;
                Game.doTick = true;
            }

            if (Game.input.isKeyHeld(OpenTK.Input.Key.D))
            {
                moveX++;
                Game.doTick = true;
            }

            if (Game.input.isKeyHeld(OpenTK.Input.Key.W))
            {
                moveY--;
                Game.doTick = true;
            }

            if (Game.input.isKeyHeld(OpenTK.Input.Key.S))
            {
                moveY++;
                Game.doTick = true;
            }

            bool canMove = Managers.CollisionManager.CheckCollision(moveX, moveY, d);

            if (canMove)
            {
                d.setPosRelative(moveX, moveY);
            }
        }
    }
}
