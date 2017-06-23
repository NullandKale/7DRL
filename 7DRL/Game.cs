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
        public static bool doTick;
        public Random rng;
        public List<Action> onUpdate;
        public bool running;
        public bool stop;
        public bool lastFrameDone;
        public bool resetWorld;
        public bool rendering;

        public Tile[,] ground;
        public Tile[,] world;
        public string toPrint;

        public int worldSize;
        public int worldOffsetX;
        public int worldOffsetY;

        public int screenX;
        public int screenY;

        public int gameX;

        private Tile[,] lastFrame;

        public Entities.drawable player;
        public Components.cStats pcStats;

        private Entities.drawable[] enemy;
        private int enemyCount = 50;

        private Dictionary<int, string> guiItem = new Dictionary<int, string>();

        public Game(int seed)
        {
            if (g == null)
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

            worldSize = 200;
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

        public void ResetWorld(int seed)
        {
            rng = new Random(seed);
            input = new nullEngine.Managers.InputManager();

            player.active = false;

            for (var i = 1; i < enemy.Length; i++)
            {
                enemy[i].active = false;
            }
            
            onUpdate.Clear();
            
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

            onLoad(true);
        }

        public void onLoad(bool reset)
        {
            Console.CursorVisible = false;

            running = true;
            stop = false;
            ground = WorldManager.GenerateWorld(ground, worldSize);

            ClearFrameBuffer();

            InitializeCollisionMap();
            InitializePlayer(reset);  
            InitializeEnemies();
            InitializeStairs();
            
            lastFrameDone = true;
            resetWorld = false;
        }

        public void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (lastFrameDone && !resetWorld)
            {
                ClearWorld();
                lastFrameDone = false;
                for (int i = 0; i < onUpdate.Count; i++)
                {
                    onUpdate[i].Invoke();
                }
                Draw();
                lastFrameDone = true;

                

                doTick = false;
            }

            if (resetWorld && !rendering)
            {
                ResetWorld(Game.g.rng.Next() % 100 + 1);
            }
        }

        private void Draw()
        {
            rendering = true;
            for (int y = 0; y < screenY; y++)
            {
                //Console.SetCursorPosition(0, y);
                for (int x = 0; x < gameX; x++)
                {
                    Console.SetCursorPosition(x, y);

                    if (world[x + worldOffsetX, y + worldOffsetY].Visual != ' ')
                    {
                        if (world[x + worldOffsetX, y + worldOffsetY].Visual == '@')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        else if (world[x + worldOffsetX, y + worldOffsetY].Visual == 'E')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (world[x + worldOffsetX, y + worldOffsetY].Visual == '<'
                            || world[x + worldOffsetX, y + worldOffsetY].Visual == '>')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        Console.Write(world[x + worldOffsetX, y + worldOffsetY].Visual);
                        world[x + worldOffsetX, y + worldOffsetY].Visual = ' ';
                        lastFrame[x, y] = world[x + worldOffsetX, y + worldOffsetY];
                    }
                    else if (lastFrame[x, y] != ground[x + worldOffsetX, y + worldOffsetY])
                    {
                        if (ground[x + worldOffsetX, y + worldOffsetY].Visual == (char)0x2588)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }

                        Console.Write(ground[x + worldOffsetX, y + worldOffsetY].Visual);
                        lastFrame[x, y] = ground[x + worldOffsetX, y + worldOffsetY];
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
                            for (var j = 0; j < guiItem.Count; j++)
                            {
                                Console.SetCursorPosition(x, j);
                                Console.Write(guiItem[j]);
                                for (var i = gameX + 1 + guiItem[j].Length; i < screenX; i++)
                                {
                                    Console.SetCursorPosition(i, j);
                                    Console.Write(' ');
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < screenX; x++)
            {
                Console.SetCursorPosition(x, 28);
                Console.Write('-');
            }

            ClearWorld();
            rendering = false;
        }

        public void AddUIElement(int index, string item)
        {
            if (guiItem.ContainsKey(index))
            {
                guiItem[index] = item;
            }
            else
            { 
                guiItem.Add(index, item);
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

        private void ClearWorld()
        {
            for (var i = 0; i < worldSize; i++)
            {
                for (var j = 0; j < worldSize; j++)
                {
                    world[i, j].Visual = ' ';
                }
            }
        }

        private void InitializeCollisionMap()
        {
            for (var i = 0; i < worldSize; i++)
            {
                for (var j = 0; j < worldSize; j++)
                {
                    if (ground[i, j].Visual != (char)0x2588)
                    {
                        world[i, j].collideable = false;
                    }
                    else
                    {
                        world[i, j].collideable = true;
                    }
                }
            }
        }

        private void InitializePlayer(bool reset)
        {
            player = new Entities.drawable();
            Utils.Point playerPos = Utils.Point.getRandomPointInWorld();
            player.pos.xPos = playerPos.x;
            player.pos.yPos = playerPos.y;
            player.texture = '@';
            player.tag = "Player";
            player.active = true;
            player.AddComponent(new Components.cKeyboardMoveAndCollide());
            player.AddComponent(new Components.cCameraFollow(this));

            if (!reset)
            {
                pcStats = new Components.cStats(false, 100);
            }

            player.AddComponent(pcStats);
            onUpdate.Add(player.update);
        }

        private void InitializeEnemies()
        {
            enemy = new Entities.drawable[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                enemy[i] = new Entities.drawable();
                Utils.Point enemyPos = Utils.Point.getRandomPointInWorld();
                enemy[i].pos.xPos = enemyPos.x;
                enemy[i].pos.yPos = enemyPos.y;
                enemy[i].texture = 'E';
                enemy[i].tag = "Enemy";
                enemy[i].active = true;
                enemy[i].AddComponent(new Components.cEnemyAI(player, pcStats, 75, 20, 10));
                onUpdate.Add(enemy[i].update);
            }
        }

        private void InitializeStairs()
        {
            var stairs = new Entities.drawable();
            Utils.Point stairPos = Utils.Point.getRandomPointInWorld();
            stairs.pos.xPos = stairPos.x;
            stairs.pos.yPos = stairPos.y;
            stairs.texture = '>';
            stairs.tag = "Stairs";
            stairs.active = true;
            stairs.AddComponent(new Components.cStair(player));
            onUpdate.Add(stairs.update);

            stairs = new Entities.drawable();
            stairPos = Utils.Point.getRandomPointInWorld();
            stairs.pos.xPos = stairPos.x;
            stairs.pos.yPos = stairPos.y;
            stairs.texture = '<';
            stairs.tag = "Stairs";
            stairs.active = true;
            stairs.AddComponent(new Components.cStair(player));
            onUpdate.Add(stairs.update);
        }
    }
}