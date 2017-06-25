using _7DRL.Components;
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
        public static bool doTick;
        public Random rng;
        public List<Action> onUpdate;
        public bool running;
        public bool stop;
        public bool lastFrameDone;
        public bool resetWorld;
        public bool resetWorldUp;
        public bool rendering;

        private int seed;

        public Tile[,] ground;
        public Tile[,] world;
        public string toPrint;

        public int worldSize;
        public int worldOffsetX;
        public int worldOffsetY;

        public int screenX;
        public int screenY;

        public int gameX;

        public int InvNum;

        private Tile[,] lastFrame;

        public Entities.drawable player;
        public Components.cStats pcStats;
        public InventoryManager pcInv;

        public Entities.drawable stairsUp;
        public Entities.drawable stairsDown;

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

            worldSize = 100;
            screenX = 119;
            screenY = 28;

            gameX = 79;

            ground = new Tile[worldSize, worldSize];
            world = new Tile[worldSize, worldSize];
            lastFrame = new Tile[screenX, screenY + 1];

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
                for (var j = 0; j < screenY + 1; j++)
                {
                    lastFrame[i, j] = new Tile();
                }
            }

            worldOffsetX = 0;
            worldOffsetY = 0;
        }

        public void ResetWorld(int _seed)
        {
            seed = _seed;
            rng = new Random(seed);
            input = new nullEngine.Managers.InputManager();

            player.active = false;

            for (var i = 1; i < enemy.Length; i++)
            {
                enemy[i].active = false;
            }
                        
            ground = new Tile[worldSize, worldSize];
            world = new Tile[worldSize, worldSize];
            lastFrame = new Tile[screenX, screenY + 1];

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
                for (var j = 0; j < screenY + 1; j++)
                {
                    lastFrame[i, j] = new Tile();
                }
            }

            worldOffsetX = 0;
            worldOffsetY = 0;

            var type = GenerationType.Rooms;

            if (seed % 3 == 0)
            {
                type = GenerationType.Caves;
            }
            
            onLoad(true, type);
        }

        public void onLoad(bool reset, GenerationType type)
        {
            Console.CursorVisible = false;

            ClearWorld();
            ClearGround();

            running = true;
            stop = false;
            ground = WorldManager.GenerateWorld(ground, worldSize, type);

            ClearFrameBuffer();
            ClearWorld();

            InitializeCollisionMap();
            InitializeStairs(reset);
            InitializePlayer(reset);  
            InitializeEnemies(reset);
            
            lastFrameDone = true;
            resetWorld = false;
        }

        public void update(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (lastFrameDone)
            {
                ClearWorld();
                lastFrameDone = false;
                for (int i = 0; i < onUpdate.Count; i++)
                {
                    onUpdate[i].Invoke();
                }

                DrawInventory();

                Draw();

                if (resetWorld)
                {
                    if (resetWorldUp)
                    {
                        ResetWorld(seed + 1);
                    }
                    else
                    {
                        ResetWorld(seed - 1);
                    }
                    resetWorld = false;
                }

                lastFrameDone = true;
                doTick = false;
            }
        }

        private void Draw()
        {
            rendering = true;
            for (int y = 0; y < screenY; y++)
            {
                for (int x = 0; x < gameX; x++)
                {
                    Console.SetCursorPosition(x, y);

                    if (world[x + worldOffsetX, y + worldOffsetY].Visual != ' ')
                    {
                        if (world[x + worldOffsetX, y + worldOffsetY].Visual == '@')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                        }
                        else if (world[x + worldOffsetX, y + worldOffsetY].Visual == 'E'                            
                            || world[x + worldOffsetX, y + worldOffsetY].Visual == 'e'
                            || world[x + worldOffsetX, y + worldOffsetY].Visual == 'S'
                            || world[x + worldOffsetX, y + worldOffsetY].Visual == 's')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (world[x + worldOffsetX, y + worldOffsetY].Visual == '<'
                            || world[x + worldOffsetX, y + worldOffsetY].Visual == '>')
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if(world[x + worldOffsetX, y + worldOffsetY].Visual == 'L')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
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

            Console.ForegroundColor = ConsoleColor.White;
            for (int x = gameX; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if (x == gameX)
                    {
                        if (lastFrame[x, y].Visual != '-'
                             || lastFrame[x, y].Visual != '|')
                        {
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
                            for (var j = 0; j < guiItem.Count; j++)
                            {
                                Console.SetCursorPosition(x, j);
                                Console.Write(guiItem[j]);
                                for (var i = gameX + 1 + guiItem[j].Length; i < screenX; i++)
                                {
                                    Console.Write(' ');
                                }
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < screenX; x++)
            {
                if (lastFrame[x, 28].Visual != '-')
                { 
                    Console.SetCursorPosition(x, 28);
                    Console.Write('-');
                }
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
                    world[i, j] = new Tile();
                    world[i, j].Visual = ' ';
                    world[i, j].collideable = false;
                }
            }
        }

        private void ClearGround()
        {
            for (var i = 0; i < worldSize; i++)
            {
                for (var j = 0; j < worldSize; j++)
                {
                    ground[i, j] = new Tile();
                    ground[i, j].Visual = ' ';
                    ground[i, j].collideable = false;
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
                        ground[i, j].collideable = false;
                    }
                    else
                    {
                        ground[i, j].collideable = true;
                    }
                }
            }
        }

        private void InitializePlayer(bool reset)
        {
            if (!reset)
            {
                player = new Entities.drawable();
                pcStats = new cStats(false, 100);
                pcInv = new InventoryManager(5);
                player.texture = '@';
                player.tag = "Player";
                Utils.Point p = Utils.Point.getRandomPointInWorld();
                player.pos.xPos = p.x;
                player.pos.yPos = p.y;
                player.active = true;
                player.AddComponent(new Components.cKeyboardMoveAndCollide());
                player.AddComponent(new Components.cCameraFollow(this));
                player.AddComponent(pcStats);
                onUpdate.Add(player.update);
            }
            else
            {
                if (resetWorldUp)
                {
                    player.pos.xPos = stairsDown.pos.xPos;
                    player.pos.yPos = stairsDown.pos.yPos;
                }
                else
                {
                    player.pos.xPos = stairsUp.pos.xPos;
                    player.pos.yPos = stairsUp.pos.yPos;
                }
            }

            pcInv.RegenLoot();

            player.active = true;
        }

        private void InitializeEnemies(bool reset)
        {
            if (reset)
            {
                for (int i = 0; i < enemyCount - 1; i++)
                {
                    if (onUpdate.Contains(enemy[i].update))
                    {
                        onUpdate.Remove(enemy[i].update);
                    }
                }
            }
            enemy = new Entities.drawable[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                enemy[i] = new Entities.drawable();
                Utils.Point enemyPos = Utils.Point.getRandomPointInWorld();
                enemy[i].pos.xPos = enemyPos.x;
                enemy[i].pos.yPos = enemyPos.y;

                var r = rng.NextDouble();
                if (r < 0.25)
                {
                    r = rng.NextDouble();

                    if (r < 0.6 - Math.Max(-0.6, pcStats.level * -0.01))
                    {
                        enemy[i].texture = 'e';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemy[i].AddComponent(new Components.cEnemyAI(player, pcStats,
                            40 + (pcStats.level * 2), 10 + (pcStats.level), 5, 6 + (pcStats.level / 10), 1.5, 0.10));
                        onUpdate.Add(enemy[i].update);
                    }
                    else
                    {
                        enemy[i].texture = 'E';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemy[i].AddComponent(new Components.cEnemyAI(player, pcStats,
                            75 + (pcStats.level * 5), 20 + (pcStats.level * 2), 10, 4 + (pcStats.level / 10), 1.5, 0.25));
                        onUpdate.Add(enemy[i].update);
                    }
                }
                else
                {
                    r = rng.NextDouble();

                    if (r < 0.6 - Math.Max(-0.6, pcStats.level * -0.01))
                    {
                        enemy[i].texture = 's';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemy[i].AddComponent(new Components.cEnemyAI(player, pcStats,
                            20 + (pcStats.level * 1), 4 + (pcStats.level), 1, 6 + (pcStats.level / 10), 3, 0.10));
                        onUpdate.Add(enemy[i].update);
                    }
                    else
                    {
                        enemy[i].texture = 'S';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemy[i].AddComponent(new Components.cEnemyAI(player, pcStats,
                            40 + (pcStats.level * 5), 8 + (pcStats.level * 2), 1, 4 + (pcStats.level / 10), 4.5, 0.25));
                        onUpdate.Add(enemy[i].update);
                    }
                }
            }
        }

        private void InitializeStairs(bool reset)
        {
            if (!reset)
            {
                stairsUp = new Entities.drawable();
                Utils.Point stairPos = Utils.Point.getRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.x;
                stairsUp.pos.yPos = stairPos.y;
                stairsUp.texture = '>';
                stairsUp.tag = "Stairs";
                stairsUp.active = true;
                stairsUp.AddComponent(new Components.cStair(true));
                onUpdate.Add(stairsUp.update);

                stairsDown = new Entities.drawable();
                stairPos = Utils.Point.getRandomPointInWorld();
                stairsDown.pos.xPos = stairPos.x;
                stairsDown.pos.yPos = stairPos.y;
                stairsDown.texture = '<';
                stairsDown.tag = "Stairs";
                stairsDown.active = true;
                stairsDown.AddComponent(new Components.cStair(false));
                onUpdate.Add(stairsDown.update);
            }
            else
            {
                Utils.Point stairPos = Utils.Point.getRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.x;
                stairsUp.pos.yPos = stairPos.y;

                stairPos = Utils.Point.getRandomPointInWorld();
                stairsDown.pos.xPos = stairPos.x;
                stairsDown.pos.yPos = stairPos.y;
            }
        }

        private void DrawInventory()
        {
            AddUIElement(0, "Level: " + pcStats.level + " XP: " + pcStats.currentXP + "/" + pcStats.NeededXP + " Gold: " + pcInv.playerInv.currentGoldAmount);
            string str = "H: " + pcStats.currentHealth + "/" + pcStats.maxHealth + " M: " + pcStats.currentMana + "/" + pcStats.maxMana + " S: " + pcStats.currentStamina + "/" + pcStats.maxStamina;
            if (pcStats.outOfStam)
            {
                str += " tired";
            }
            AddUIElement(1, str);
            AddUIElement(2, "-------------< Inventory >--------------");
            AddUIElement(3, "1(" + (InvNum + 1) + ") " + pcInv.getItem(InvNum));
            AddUIElement(4, "2(" + (InvNum + 2) + ") " + pcInv.getItem(InvNum + 1));
            AddUIElement(5, "3(" + (InvNum + 3) + ") " + pcInv.getItem(InvNum + 2));
            AddUIElement(6, "4(" + (InvNum + 4) + ") " + pcInv.getItem(InvNum + 3));
            AddUIElement(7, "5(" + (InvNum + 5) + ") " + pcInv.getItem(InvNum + 4));
            if (input.isKeyRising(OpenTK.Input.Key.Period))
            {
                InvNum++;
            }

            if (input.isKeyRising(OpenTK.Input.Key.Comma))
            {
                if (InvNum > 0)
                {
                    InvNum--;
                }
            }
            AddUIElement(8, "-------------< Equipment >--------------");
            if (pcInv.equipedWeapon != null)
            {
                AddUIElement(9, "W:" + pcInv.equipedWeapon.ToString());
            }
            else
            {
                AddUIElement(9, "W: ");
            }

            if (pcInv.equipedArmor != null)
            {
                AddUIElement(10, "A:" + pcInv.equipedArmor.ToString());
            }
            else
            {
                AddUIElement(10, "A: ");
            }

            UseItem(OpenTK.Input.Key.Number1, InvNum);
            UseItem(OpenTK.Input.Key.Number2, InvNum + 1);
            UseItem(OpenTK.Input.Key.Number3, InvNum + 2);
            UseItem(OpenTK.Input.Key.Number4, InvNum + 3);
            UseItem(OpenTK.Input.Key.Number5, InvNum + 4);
        }

        private void UseItem(OpenTK.Input.Key key, int num)
        {
            if (input.isKeyRising(key))
            {
                if (pcInv.playerInv.items.Count > num)
                {
                    if (pcInv.playerInv.items[num] is Weapon)
                    {
                        pcInv.EquipWeapon(num);
                    }
                    else if (pcInv.playerInv.items[num] is Armor)
                    {
                        pcInv.EquipArmor(num);
                    }
                }
            }
        }
    }
}