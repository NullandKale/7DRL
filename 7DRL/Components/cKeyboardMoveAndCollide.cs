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
            if(!Game.g.pcStats.isEncumbered)
            {
                int moveX = 0;
                int moveY = 0;

                if (Game.input.isKeyHeld(OpenTK.Input.Key.A))
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

                bool canMoveBoth = Managers.CollisionManager.CheckCollision(moveX, moveY, d);
                bool canMoveX = Managers.CollisionManager.CheckCollision(moveX, 0, d);
                bool canMoveY = Managers.CollisionManager.CheckCollision(0, moveY, d);

                if (canMoveBoth && moveX != 0 && moveY != 0)
                {
                    d.setPosRelative(moveX, moveY);
                }
                else if (canMoveX && moveX != 0)
                {
                    d.setPosRelative(moveX, 0);
                }
                else if (canMoveY && moveY != 0)
                {
                    d.setPosRelative(0, moveY);
                }

                //Console.SetCursorPosition(0, 29);
                //Console.Write(moveX + ", " + moveY + ", " + canMoveBoth + ", " + canMoveX + ", " + canMoveY);
            }
        }
    }
}
