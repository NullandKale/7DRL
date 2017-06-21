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
        public bool running;
        public bool stop;

        public char[,] world;
        public int worldSize;
        public int worldOffsetX;
        public int worldOffsetY;

        public int screenX;
        public int screenY;

        private char[,] lastFrame;

        private List<Action> onUpdate;

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

            worldSize = 1000;
            world = new char[worldSize, worldSize];
            screenX = 119;
            screenY = 29;
            lastFrame = new char[screenX, screenY];

            worldOffsetX = 0;
            worldOffsetY = 0;

            onUpdate = new List<Action>();
        }

        public void onLoad()
        {
            running = true;
            stop = false;
            GenerateWorld();
            ClearFrameBuffer();

            player = new Entities.drawable();
            player.pos.xPos = 10;
            player.pos.yPos = 10;
            player.texture = '@';
            player.tag = "Player";
            //player.AddComponent(new Components.cKeyboardMoveAndCollide());
            onUpdate.Add(player.update);
            
        }

        public void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            for(int i = 0; i < onUpdate.Count; i++)
            {
                onUpdate[i].Invoke();
            }

            Draw();
        }

        private void Draw()
        {
            for(int x = 0; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if (lastFrame[x, y] != world[x + worldOffsetX, y + worldOffsetY])
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(world[x + worldOffsetX, y + worldOffsetY]);
                        lastFrame[x, y] = world[x + worldOffsetX, y + worldOffsetY];
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

        private void GenerateWorld()
        {
            for(int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    if(x == 0 || y == 0)
                    {
                        world[x, y] = '#';
                    }
                    else
                    {
                        world[x, y] = '.';
                    }
                }
            }
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
