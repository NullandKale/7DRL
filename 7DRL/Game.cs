using _7DRL.Managers;
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
        public List<Action> onUpdate;
        public bool running;
        public bool stop;
        public bool lastFrameDone;

        public char[,] ground;
        public char[,] world;

        public int worldSize;
        public int worldOffsetX;
        public int worldOffsetY;

        public int screenX;
        public int screenY;

        private char[,] lastFrame;

        private Entities.drawable player;

        public Game()
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

            input = new nullEngine.Managers.InputManager();

            worldSize = 1000;
            ground = new char[worldSize, worldSize];
            world = new char[worldSize, worldSize];
            screenX = 119;
            screenY = 30;
            lastFrame = new char[screenX, screenY];

            worldOffsetX = 0;
            worldOffsetY = 0;
        }

        public void onLoad()
        {
            Console.CursorVisible = false;

            running = true;
            stop = false;
            ground = WorldManager.GenerateWorld(worldSize);
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
            for(int x = 0; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if (ground[x + worldOffsetX, y + worldOffsetY] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else if (ground[x + worldOffsetX, y + worldOffsetY] == '.')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    
                    if (lastFrame[x, y] != ground[x + worldOffsetX, y + worldOffsetY])
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(ground[x + worldOffsetX, y + worldOffsetY]);
                        lastFrame[x, y] = ground[x + worldOffsetX, y + worldOffsetY];
                    }

                    if (world[x + worldOffsetX, y + worldOffsetY] == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }

                    if (world[x + worldOffsetX, y + worldOffsetY] != ' ')
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(world[x + worldOffsetX, y + worldOffsetY]);
                        lastFrame[x, y] = world[x + worldOffsetX, y + worldOffsetY];
                        world[x + worldOffsetX, y + worldOffsetY] = ' ';
                    }                    
                }
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
                    lastFrame[x, y] = ' ';
                }
            }
        }
    }
}
