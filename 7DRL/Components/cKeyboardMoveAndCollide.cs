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

            if(Game.input.isKeyFalling(OpenTK.Input.Key.A))
            {
                moveX--;
            }

            if (Game.input.isKeyFalling(OpenTK.Input.Key.D))
            {
                moveX++;
            }

            if (Game.input.isKeyFalling(OpenTK.Input.Key.W))
            {
                moveY--;
            }

            if (Game.input.isKeyFalling(OpenTK.Input.Key.S))
            {
                moveY++;
            }

            bool canMove = Managers.CollisionManager.CheckCollision(moveX, moveY, d);

            if (canMove)
            {
                d.setPosRelative(moveX, moveY);
            }
        }
    }
}
