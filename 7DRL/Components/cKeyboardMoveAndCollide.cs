using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Entities;

namespace _7DRL.Components
{
    class cKeyboardMoveAndCollide : iComponent
    {
        public void Run(drawable d)
        {
            int moveX = 0;
            int moveY = 0;

            if(Managers.InputManager.isKeyFalling(System.Windows.Input.Key.A))
            {
                moveX--;
            }

            if (Managers.InputManager.isKeyFalling(System.Windows.Input.Key.D))
            {
                moveX++;
            }

            if (Managers.InputManager.isKeyFalling(System.Windows.Input.Key.W))
            {
                moveY--;
            }

            if (Managers.InputManager.isKeyFalling(System.Windows.Input.Key.S))
            {
                moveY++;
            }

            bool canMoveX = Managers.CollisionManager.CheckCollision(moveX, 0, d);
            bool canMoveY = Managers.CollisionManager.CheckCollision(0, moveY, d);

            if(canMoveX)
            {
                d.setPosRelative(moveX, 0);
            }
            if(canMoveY)
            {
                d.setPosRelative(0, moveY);
            }
        }
    }
}
