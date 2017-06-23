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
        int frame = 0;
        bool debug;

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

                if(Game.input.isKeyRising(OpenTK.Input.Key.Tilde))
                {
                    if(debug)
                    {
                        Game.g.pcStats.weaponDamage = 0;
                        debug = false;
                        Console.SetCursorPosition(0, 29);
                        Console.Write("                                               ");
                    }
                    else
                    {
                        Game.g.pcStats.weaponDamage = 100;
                        debug = true;
                        Console.SetCursorPosition(0, 29);
                        Console.Write("                        GOD MODE");
                    }
                }

                if(Game.input.isKeyFalling(OpenTK.Input.Key.Space))
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if(!(x == 0 && y == 0))
                            {
                                int xPos = d.pos.xPos + x;
                                int yPos = d.pos.yPos + y;
                                if (Game.g.world[xPos, yPos].Visual == 'L')
                                {
                                    Game.g.world[xPos, yPos].Visual = ' ';
                                    //Game.g.pcInv.
                                }
                            }
                        }
                    }
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

                frame++;
                Console.SetCursorPosition(0, 29);
                Console.Write("[" + d.pos.xPos + "," + d.pos.yPos + "] " + frame);
            }
        }
    }
}
