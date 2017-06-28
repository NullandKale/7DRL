namespace _7DRL.Components
{
    using System;
    using Entities;

    class cKeyboardMoveAndCollide : iComponent
    {
        private bool debug;

        public void Run(Drawable d)
        {
            Game.doTick = false;
            if (!Game.g.pcStats.isEncumbered)
            {
                int moveX = 0;
                int moveY = 0;

                if (Game.input.IsKeyHeld(OpenTK.Input.Key.ShiftLeft) && Game.g.pcStats.currentStamina > 0
                    && !Game.g.pcStats.outOfStam)
                {
                    if (Game.input.IsKeyHeld(OpenTK.Input.Key.A))
                    {
                        moveX--;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyHeld(OpenTK.Input.Key.D))
                    {
                        moveX++;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyHeld(OpenTK.Input.Key.W))
                    {
                        moveY--;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyHeld(OpenTK.Input.Key.S))
                    {
                        moveY++;
                        Game.doTick = true;
                    }

                    if (moveX != 0 || moveY != 0)
                    {
                        Game.g.pcStats.currentStamina -= 5;
                    }
                }
                else
                {
                    if (Game.input.IsKeyFalling(OpenTK.Input.Key.A))
                    {
                        moveX--;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyFalling(OpenTK.Input.Key.D))
                    {
                        moveX++;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyFalling(OpenTK.Input.Key.W))
                    {
                        moveY--;
                        Game.doTick = true;
                    }

                    if (Game.input.IsKeyFalling(OpenTK.Input.Key.S))
                    {
                        moveY++;
                        Game.doTick = true;
                    }
                }

                if (Game.input.IsKeyRising(OpenTK.Input.Key.Tilde))
                {
                    if (debug)
                    {
                        Game.g.pcStats.weaponDamage -= 100;
                        debug = false;
                    }
                    else
                    {
                        Game.g.pcStats.weaponDamage += 100;
                        debug = true;
                    }
                }

                if (Game.input.IsKeyFalling(OpenTK.Input.Key.Space))
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (!(x == 0 && y == 0))
                            {
                                int xPos = d.pos.xPos + x;
                                int yPos = d.pos.yPos + y;
                                if (Game.g.world[xPos, yPos].Visual == 'L')
                                {
                                    Game.g.pcInv.AddLootItem(Game.g.pcStats.level, xPos, yPos);
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
                    d.SetPosRelative(moveX, moveY);
                }
                else if (canMoveX && moveX != 0)
                {
                    d.SetPosRelative(moveX, 0);
                }
                else if (canMoveY && moveY != 0)
                {
                    d.SetPosRelative(0, moveY);
                }
            }
        }
    }
}
