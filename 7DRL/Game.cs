using _7DRL.Components;
using _7DRL.Managers;
using _7DRL.Utils;
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

        public List<string> CombatLog = new List<string>();

        private Tile[,] lastFrame;

        public Entities.drawable princess;

        public Entities.drawable player;
        public Components.cStats pcStats;
        public InventoryManager pcInv;

        public Entities.drawable stairsUp;
        public Entities.drawable stairsDown;

        public Entities.drawable[] enemy;
        public cEnemyAI[] enemyAI;
        private int enemyCount;

        public int floor;

        private int Pstr;
        private int Pdex;
        private int Pcon;
        private int Pintel;
        private int Pwis;
        private int Pcha;
        public string PName;

        private Dictionary<int, string> guiItem = new Dictionary<int, string>();

        public Game(int seed, int Pstr, int Pdex, int Pcon, int Pintel, int Pwis, int Pcha, string playerName)
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

            enemyCount = 35;
            floor = 1;

            worldSize = 100;
            screenX = 119;
            screenY = 28;

            gameX = 79;

            this.Pstr = Pstr;
            this.Pdex = Pdex;
            this.Pcon = Pcon;
            this.Pintel = Pintel;
            this.Pwis = Pwis;
            this.Pcha = Pcha;
            PName = playerName;

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
            InintializePrincess(reset);
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
                        floor++;
                    }
                    else
                    {
                        ResetWorld(seed - 1);
                        floor--;
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
                        if(Console.ForegroundColor != world[x + worldOffsetX, y + worldOffsetY].color)
                        {
                            Console.ForegroundColor = world[x + worldOffsetX, y + worldOffsetY].color;
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
                                for (var i = gameX + 1 + guiItem[j].Length; i < screenX + 1; i++)
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

        private void InintializePrincess(bool reset)
        {
            if(!reset)
            {
                princess = new Entities.drawable();
                princess.texture = 'P';
                princess.color = ConsoleColor.Gray;
                princess.tag = "Princess";
                onUpdate.Add(princess.update);
            }
            if(floor % 10 == 0 && floor != 0)
            {
                Utils.Point p = Utils.Point.getRandomPointInWorld();
                princess.pos.xPos = p.x;
                princess.pos.yPos = p.y;
            }
            else if(floor % 10 != 0)
            {
                princess.pos.xPos = -1;
                princess.pos.yPos = -1;
            }
            princess.active = true;
        }

        private void InitializePlayer(bool reset)
        {
            if (!reset)
            {
                player = new Entities.drawable();
                pcStats = new cStats(Pstr, Pdex, Pcon, Pintel, Pwis, Pcha);
                pcInv = new InventoryManager(5);
                player.texture = '@';
                player.color = ConsoleColor.Blue;
                player.tag = "Player";
                Utils.Point p = Utils.Point.getRandomPointInWorld();
                player.pos.xPos = p.x;
                player.pos.yPos = p.y;
                player.active = true;
                player.AddComponent(new Components.cKeyboardMoveAndCollide());
                player.AddComponent(new Components.cStory(10));
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
            enemyAI = new cEnemyAI[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                enemy[i] = new Entities.drawable();
                enemy[i].color = ConsoleColor.Red;
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
                        enemyAI[i] = new cEnemyAI(player, pcStats, "Rats",
                            40 + (pcStats.level * 2), 10 + (pcStats.level), 5, 6 + (pcStats.level / 10), 1.5, 0.10);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].update);
                    }
                    else
                    {
                        enemy[i].texture = 'E';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemyAI[i] = new cEnemyAI(player, pcStats, "Skeleton",
                            75 + (pcStats.level * 5), 20 + (pcStats.level * 2), 10, 4 + (pcStats.level / 10), 1.5, 0.25);
                        enemy[i].AddComponent(enemyAI[i]);
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
                        enemyAI[i] = new cEnemyAI(player, pcStats, "Hornet",
                            20 + (pcStats.level * 1), 4 + (pcStats.level), 1, 6 + (pcStats.level / 10), 3, 0.10);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].update);
                    }
                    else
                    {
                        enemy[i].texture = 'S';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemyAI[i] = new cEnemyAI(player, pcStats, "Archer",
                            40 + (pcStats.level * 5), 8 + (pcStats.level * 2), 1, 4 + (pcStats.level / 10), 4.5, 0.25);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].update);
                    }
                }
            }
        }

        private void InitializeStairs(bool reset)
        {
            if (!reset)
            {
                Point stairPos;
                stairsUp = new Entities.drawable();
                stairPos = Point.getRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.x;
                stairsUp.pos.yPos = stairPos.y;
                stairsUp.texture = '>';
                stairsUp.color = ConsoleColor.White;
                stairsUp.tag = "Stairs";
                stairsUp.active = true;
                stairsUp.AddComponent(new cStair(true));
                onUpdate.Add(stairsUp.update);

                stairsDown = new Entities.drawable();
                stairPos = Point.getRandomDoorPoint(new Point(stairsUp.pos.xPos, stairsUp.pos.yPos));
                stairsDown.pos.xPos = stairPos.x;
                stairsDown.pos.yPos = stairPos.y;
                stairsDown.texture = '<';
                stairsDown.color = ConsoleColor.White;
                stairsDown.tag = "Stairs";
                stairsDown.active = true;
                stairsDown.AddComponent(new cStair(false));
                onUpdate.Add(stairsDown.update);
            }
            else
            {
                Utils.Point stairPos = Point.getRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.x;
                stairsUp.pos.yPos = stairPos.y;

                stairPos = Point.getRandomDoorPoint(new Point(stairsUp.pos.xPos, stairsUp.pos.yPos));
                stairsDown.pos.xPos = stairPos.x;
                stairsDown.pos.yPos = stairPos.y;
            }
        }

        private void DrawInventory()
        {
            AddUIElement(0, PName + " Lvl: " + pcStats.level + " XP needed: " + (pcStats.NeededXP - pcStats.currentXP));
            AddUIElement(1, pcInv.playerInv.currentGoldAmount + "g Floor: " + floor);
            string str = "H: " + pcStats.currentHealth + "/" + pcStats.maxHealth + " M: " + pcStats.currentMana + "/" + pcStats.maxMana + " S: " + pcStats.currentStamina + "/" + pcStats.maxStamina;
            if (pcStats.outOfStam)
            {
                str += " tired";
            }
            AddUIElement(2, str);
            if (pcInv.playerInv.items.Count < 10)
            {
                AddUIElement(3, "-----------< Inventory " + pcInv.playerInv.items.Count + " >--------------");
            }
            else
            {
                AddUIElement(3, "-----------< Inventory " + pcInv.playerInv.items.Count + " >-------------");

            }
            AddUIElement(4, "1(" + (InvNum + 1) + ") " + pcInv.getItem(InvNum));
            AddUIElement(5, "2(" + (InvNum + 2) + ") " + pcInv.getItem(InvNum + 1));
            AddUIElement(6, "3(" + (InvNum + 3) + ") " + pcInv.getItem(InvNum + 2));
            AddUIElement(7, "4(" + (InvNum + 4) + ") " + pcInv.getItem(InvNum + 3));
            AddUIElement(8, "5(" + (InvNum + 5) + ") " + pcInv.getItem(InvNum + 4));
            if (input.isKeyRising(OpenTK.Input.Key.Period))
            {
                if (InvNum + 4 < pcInv.playerInv.items.Count)
                {
                    InvNum++;
                }
            }

            if (input.isKeyRising(OpenTK.Input.Key.Comma))
            {
                if (InvNum > 0)
                {
                    InvNum--;
                }
            }
            AddUIElement(9, "-------------< Equipment >--------------");
            if (pcInv.equipedWeapon != null)
            {
                AddUIElement(10, "W:" + pcInv.equipedWeapon.ToString());
            }
            else
            {
                AddUIElement(10, "W: ");
            }

            if (pcInv.equipedArmor != null)
            {
                AddUIElement(11, "A:" + pcInv.equipedArmor.ToString());
            }
            else
            {
                AddUIElement(11, "A: ");
            }

            if (pcInv.equipedRing != null)
            {
                AddUIElement(12, "R:" + pcInv.equipedRing.ToString());
            }
            else
            {
                AddUIElement(12, "R: ");
            }

            if (pcInv.equipedAmulet != null)
            {
                AddUIElement(13, "a:" + pcInv.equipedAmulet.ToString());
            }
            else
            {
                AddUIElement(13, "a: ");
            }

            if (pcInv.equipedTome != null)
            {
                AddUIElement(14, "T:" + pcInv.equipedTome.ToString());

                if (input.isKeyFalling(OpenTK.Input.Key.E))
                {
                    pcInv.UseTome();
                }
            }
            else
            {
                AddUIElement(14, "T: ");
            }

            AddUIElement(15, "----------------< Log >-----------------");
            for (var i = 0; i < 12; i++)
            {
                AddUIElement(16 + i, CombatLog.Count > i ? CombatLog[i] : string.Empty);
            }

            UseItem(OpenTK.Input.Key.Number1, InvNum);
            UseItem(OpenTK.Input.Key.Number2, InvNum + 1);
            UseItem(OpenTK.Input.Key.Number3, InvNum + 2);
            UseItem(OpenTK.Input.Key.Number4, InvNum + 3);
            UseItem(OpenTK.Input.Key.Number5, InvNum + 4);            
        }

        public void LogCombat(string combat)
        {
            if (CombatLog.Count >= 12)
            {
                CombatLog.RemoveAt(0);
            }

            CombatLog.Add(combat);
        }

        private void UseItem(OpenTK.Input.Key key, int num)
        {
            if (input.isKeyRising(key))
            {
                if(input.isKeyHeld(OpenTK.Input.Key.ControlLeft))
                {
                    if (pcInv.playerInv.items.Count > num)
                    {
                        pcInv.Sell(num);
                    }
                }
                else
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
                        else if (pcInv.playerInv.items[num] is Ring)
                        {
                            pcInv.EquipRing(num);
                        }
                        else if (pcInv.playerInv.items[num] is Amulet)
                        {
                            pcInv.EquipAmulet(num);
                        }
                        else if (pcInv.playerInv.items[num] is Potion)
                        {
                            pcInv.UsePotion(num);
                        }
                        else if (pcInv.playerInv.items[num] is Tome)
                        {
                            pcInv.EquipTome(num);
                        }
                    }
                }
            }
        }
    }
}