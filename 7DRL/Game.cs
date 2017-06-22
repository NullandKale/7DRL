﻿using _7DRL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL
{
    public class Game
    {
        public static Game g;
        public static nullEngine.Managers.InputManager input;
        public Random rng;
        public List<Action> onUpdate;
        public bool running;
        public bool stop;
        public bool lastFrameDone;

        public Tile[,] ground;
        public Tile[,] world;

        public int worldSize;
        public int worldOffsetX;
        public int worldOffsetY;

        public int screenX;
        public int screenY;

        public int gameX;
        
        private Tile[,] lastFrame;

        private Entities.drawable player;
        private Entities.drawable enemy;

        public Game(int seed)
        {
            if(g == null)
            {
                g = this;
            }
            else
            {
                throw new Exception("Too many Game objects");
            }

            onUpdate = new List<Action>();
            rng = new Random(seed);

            input = new nullEngine.Managers.InputManager();

            worldSize = 400;
            screenX = 119;
            screenY = 28;

            gameX = 79;

            ground = new Tile[worldSize, worldSize];
            world = new Tile[worldSize, worldSize];
            lastFrame = new Tile[screenX, screenY];

            for (var i = 0; i < worldSize; i++)
            {
                for (var j = 0; j < worldSize; j++)
                {
                    ground[i, j] = new Tile();
                }
            }

            world = ground;

            for (var i = 0; i < screenX; i++)
            {
                for (var j = 0; j < screenY; j++)
                {
                    lastFrame[i, j] = new Tile();
                }
            }
            
            worldOffsetX = 0;
            worldOffsetY = 0;
        }

        public void onLoad()
        {
            Console.CursorVisible = false;

            running = true;
            stop = false;
            ground = WorldManager.GenerateWorld(ground, worldSize);
            ClearFrameBuffer();

            player = new Entities.drawable();
            player.pos.xPos = 10;
            player.pos.yPos = 10;
            player.texture = '@';
            player.tag = "Player";
            player.AddComponent(new Components.cKeyboardMoveAndCollide());
            player.AddComponent(new Components.cCameraFollow(this));
            onUpdate.Add(player.update);

            lastFrameDone = true;
            
        }

        public void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            if(lastFrameDone)
            {
                lastFrameDone = false;
                Draw();
                for (int i = 0; i < onUpdate.Count; i++)
                {
                    onUpdate[i].Invoke();
                }
                lastFrameDone = true;
            }
        }

        private void Draw()
        {
            for (int x = 0; x < gameX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if (lastFrame[x, y] != ground[x + worldOffsetX, y + worldOffsetY])
                    {
                        if (ground[x + worldOffsetX, y + worldOffsetY].Visual == '#')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }

                        Console.SetCursorPosition(x, y);
                        Console.Write(ground[x + worldOffsetX, y + worldOffsetY].Visual);
                        lastFrame[x, y] = ground[x + worldOffsetX, y + worldOffsetY];
                    }

                    if (world[x + worldOffsetX, y + worldOffsetY].Visual != ' ')
                    {
                        if (world[x + worldOffsetX, y + worldOffsetY].Visual == '@')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }

                        Console.SetCursorPosition(x, y);
                        Console.Write(world[x + worldOffsetX, y + worldOffsetY].Visual);
                        lastFrame[x, y] = world[x + worldOffsetX, y + worldOffsetY];
                        world[x + worldOffsetX, y + worldOffsetY].Visual = ' ';
                    }
                }
            }
            for (int x = gameX; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if (x == gameX)
                    {
                        if (lastFrame[x, y].Visual != '-'
                             || lastFrame[x, y].Visual != '|')
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x, y);
                            if (x == gameX)
                            {
                                Console.Write('|');
                            }

                            lastFrame[x, y] = new Tile()
                            {
                                Visual = '/'
                            };
                        }
                    }
                    else
                    {
                        if (y == 0 && x == gameX + 1)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x, y);
                            Random ran = new Random();
                            Console.Write("Health: " + (ran.Next() % 100 + 1).ToString());
                        }
                        if (y == 1 && x == gameX + 1)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition(x, y);
                            Random ran = new Random();
                            Console.Write("Stam: " + (ran.Next() % 100 + 1).ToString());
                        }
                    }
                }
            }

            for (int x = 0; x < screenX; x++)
            {
                Console.SetCursorPosition(x, 28);
                Console.Write('-');
            }
        }

        public static bool isInRange(int max, int x, int y)
        {
            return !(x < 0 || x >= max || y < 0 || y >= max);
        }

        public static bool isInWorld(int x, int y)
        {
            return !(x < 0 || x >= g.worldSize || y < 0 || y >= g.worldSize);
        }

        private void ClearFrameBuffer()
        {
            for (int x = 0; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    lastFrame[x, y].Visual = ' ';
                }
            }
        }
    }
}
