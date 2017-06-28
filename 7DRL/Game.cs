namespace _7DRL
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Entities;
    using Managers;
    using Utils;

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

        public int seed;
        public string startingSeed;

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

        public Drawable princess;
        public Drawable dragon;
               
        public Drawable player;
        public cStats pcStats;
        public InventoryManager pcInv;

        public Drawable stairsUp;
        public Drawable stairsDown;

        public Drawable[] enemy;
        public cEnemyAI[] enemyAI;
        public int floor;

        public string PName;

        private int enemyCount;
        private int Pstr;
        private int Pdex;
        private int Pcon;
        private int Pintel;
        private int Pwis;
        private int Pcha;

        private Dictionary<int, string> guiItem = new Dictionary<int, string>();

        public Game(int seed, int Pstr, int Pdex, int Pcon, int Pintel, int Pwis, int Pcha, string playerName, bool random)
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

            this.seed = seed;

            if (random)
            {
                rng = new Random();
                this.seed = rng.Next();
            }

            rng = new Random(this.seed);
            startingSeed = "SEED: " + this.seed.ToString();

            input = new nullEngine.Managers.InputManager();

            enemyCount = 15;
            floor = 1;

            worldSize = 80;
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

            drawBorders();
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
            InintializePrincessAndDragon(reset);

            onUpdate.Add(player.Update);

            lastFrameDone = true;
            resetWorld = false;
        }

        public void update(object source, System.Timers.ElapsedEventArgs e)
        {
            if (lastFrameDone && !stop)
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
                        floor++;
                        ResetWorld(seed + 1);
                    }
                    else
                    {
                        floor--;
                        ResetWorld(seed - 1);
                    }
                    resetWorld = false;
                }

                lastFrameDone = true;
            }
        }

        private void Draw()
        {
            rendering = true;
            bool setPos = true;
            for (int y = 0; y < screenY; y++)
            {
                for (int x = 0; x < gameX; x++)
                {
                    if (setPos)
                    {
                        Console.SetCursorPosition(x, y);
                    }

                    if (world[x + worldOffsetX, y + worldOffsetY].Visual != ' ')
                    {
                        if (Console.ForegroundColor != world[x + worldOffsetX, y + worldOffsetY].Color)
                        {
                            Console.ForegroundColor = world[x + worldOffsetX, y + worldOffsetY].Color;
                        }

                        Console.Write(world[x + worldOffsetX, y + worldOffsetY].Visual);
                        setPos = false;
                        world[x + worldOffsetX, y + worldOffsetY].Visual = ' ';
                        lastFrame[x, y] = world[x + worldOffsetX, y + worldOffsetY];
                    }
                    else if (lastFrame[x, y] != ground[x + worldOffsetX, y + worldOffsetY])
                    {
                        if (Console.ForegroundColor != ground[x + worldOffsetX, y + worldOffsetY].Color)
                        {
                            Console.ForegroundColor = ground[x + worldOffsetX, y + worldOffsetY].Color;
                        }

                        Console.Write(ground[x + worldOffsetX, y + worldOffsetY].Visual);
                        setPos = false;
                        lastFrame[x, y] = ground[x + worldOffsetX, y + worldOffsetY];
                    }
                    setPos = true;
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            for (int x = gameX; x < screenX; x++)
            {
                for (int y = 0; y < screenY; y++)
                {
                    if(x != gameX)
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
            rendering = false;
        }

        public void drawBorders()
        {
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
                    world[i, j].Collideable = false;
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
                    ground[i, j].Collideable = false;
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
                        ground[i, j].Collideable = false;
                    }
                    else
                    {
                        ground[i, j].Collideable = true;
                    }
                }
            }
        }

        private void InintializePrincessAndDragon(bool reset)
        {
            if (!reset)
            {
                princess = new Drawable();
                princess.texture = 'P';
                princess.color = ConsoleColor.Magenta;
                princess.tag = "Princess";
                onUpdate.Add(princess.Update);

                dragon = new Drawable();
                dragon.color = ConsoleColor.Red;
                dragon.texture = 'D';
                dragon.tag = "Enemy";
                dragon.active = true;
                var enemyAI = new cEnemyAI(player, pcStats, new Point(-1, -1), "Dragon",
                    215 + (pcStats.level * 5), 35 + (pcStats.level * 2), 10, 8 + (pcStats.level / 10), 1.5, 0.25);
                dragon.AddComponent(enemyAI);
                onUpdate.Add(dragon.Update);
            }
            int flr = Math.Abs(this.floor);
            if (flr % 10 == 0 && flr != 0)
            {
                Point p = Point.GetRandomPointInWorld();
                princess.pos.xPos = p.X;
                princess.pos.yPos = p.Y;

                p = Point.GetRandomPointInWorld();
                dragon.GetComponent<cEnemyAI>().StartingPosition = p;
                dragon.pos.xPos = p.X;
                dragon.pos.yPos = p.Y;
                dragon.active = true;
            }
            else
            {
                princess.pos.xPos = -1;
                princess.pos.yPos = -1;
                dragon.pos.xPos = -1;
                dragon.pos.yPos = -1;
                dragon.active = false;
            }

            princess.active = true;
        }

        private void InitializePlayer(bool reset)
        {
            if (!reset)
            {
                player = new Drawable();
                pcStats = new cStats(Pstr, Pdex, Pcon, Pintel, Pwis, Pcha);
                pcInv = new InventoryManager(5);
                player.texture = '@';
                player.color = ConsoleColor.Blue;
                player.tag = "Player";
                Point p = Point.GetRandomPointInWorld();
                player.pos.xPos = p.X;
                player.pos.yPos = p.Y;
                player.active = true;
                player.AddComponent(new cKeyboardMoveAndCollide());
                player.AddComponent(new cStory(10));
                player.AddComponent(new cCameraFollow(this));
                player.AddComponent(pcStats);
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
                    if (onUpdate.Contains(enemy[i].Update))
                    {
                        onUpdate.Remove(enemy[i].Update);
                    }
                }
            }
            enemy = new Drawable[enemyCount];
            enemyAI = new cEnemyAI[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                enemy[i] = new Drawable();
                enemy[i].color = ConsoleColor.Red;
                Utils.Point enemyPos = Utils.Point.GetRandomPointInWorldAwayFromPlayer();
                enemy[i].pos.xPos = enemyPos.X;
                enemy[i].pos.yPos = enemyPos.Y;

                var r = rng.NextDouble();
                if (r < 0.25)
                {
                    r = rng.NextDouble();

                    if (r < 0.6 - Math.Max(-0.6, pcStats.level * -0.01))
                    {
                        enemy[i].texture = 'e';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemyAI[i] = new cEnemyAI(player, pcStats, enemyPos, "Rats",
                            40 + (pcStats.level * 2), 10 + pcStats.level, 5, 6 + (pcStats.level / 10), 1.5, 0.10);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].Update);
                    }
                    else
                    {
                        enemy[i].texture = 'E';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemyAI[i] = new cEnemyAI(player, pcStats, enemyPos, "Skeleton",
                            75 + (pcStats.level * 5), 20 + (pcStats.level * 2), 10, 4 + (pcStats.level / 10), 1.5, 0.25);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].Update);
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
                        enemyAI[i] = new cEnemyAI(player, pcStats, enemyPos, "Hornet",
                            20 + (pcStats.level * 1), 4 + pcStats.level, 1, 6 + (pcStats.level / 10), 3, 0.10);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].Update);
                    }
                    else
                    {
                        enemy[i].texture = 'S';
                        enemy[i].tag = "Enemy";
                        enemy[i].active = true;
                        enemyAI[i] = new cEnemyAI(player, pcStats, enemyPos, "Archer",
                            40 + (pcStats.level * 5), 8 + (pcStats.level * 2), 1, 4 + (pcStats.level / 10), 4.5, 0.25);
                        enemy[i].AddComponent(enemyAI[i]);
                        onUpdate.Add(enemy[i].Update);
                    }
                }
            }
        }

        private void InitializeStairs(bool reset)
        {
            if (!reset)
            {
                Point stairPos;
                stairsUp = new Drawable();
                stairPos = Point.GetRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.X;
                stairsUp.pos.yPos = stairPos.Y;
                stairsUp.texture = '>';
                stairsUp.color = ConsoleColor.White;
                stairsUp.tag = "Stairs";
                stairsUp.active = true;
                stairsUp.AddComponent(new cStair(true));
                onUpdate.Add(stairsUp.Update);

                stairsDown = new Drawable();
                stairPos = Point.GetRandomDoorPoint(new Point(stairsUp.pos.xPos, stairsUp.pos.yPos));
                stairsDown.pos.xPos = stairPos.X;
                stairsDown.pos.yPos = stairPos.Y;
                stairsDown.texture = '<';
                stairsDown.color = ConsoleColor.White;
                stairsDown.tag = "Stairs";
                stairsDown.active = true;
                stairsDown.AddComponent(new cStair(false));
                onUpdate.Add(stairsDown.Update);
            }
            else
            {
                Point stairPos = Point.GetRandomPointInWorld();
                stairsUp.pos.xPos = stairPos.X;
                stairsUp.pos.yPos = stairPos.Y;

                stairPos = Point.GetRandomDoorPoint(new Point(stairsUp.pos.xPos, stairsUp.pos.yPos));
                stairsDown.pos.xPos = stairPos.X;
                stairsDown.pos.yPos = stairPos.Y;
            }
        }

        private void DrawInventory()
        {
            AddUIElement(0, PName + " Lvl: " + pcStats.level + " XP needed: " + (pcStats.NeededXP - pcStats.currentXP));
            AddUIElement(1, pcInv.playerInv.currentGoldAmount + "g lbs: " + pcInv.playerInv.currentWeight + "/" + pcStats.carryWeight + " Floor: " + floor);
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
            AddUIElement(4, "1(" + (InvNum + 1) + ") " + pcInv.GetItem(InvNum));
            AddUIElement(5, "2(" + (InvNum + 2) + ") " + pcInv.GetItem(InvNum + 1));
            AddUIElement(6, "3(" + (InvNum + 3) + ") " + pcInv.GetItem(InvNum + 2));
            AddUIElement(7, "4(" + (InvNum + 4) + ") " + pcInv.GetItem(InvNum + 3));
            AddUIElement(8, "5(" + (InvNum + 5) + ") " + pcInv.GetItem(InvNum + 4));
            if (input.IsKeyRising(OpenTK.Input.Key.Period))
            {
                if (InvNum + 4 < pcInv.playerInv.items.Count)
                {
                    InvNum++;
                }
            }

            if (input.IsKeyRising(OpenTK.Input.Key.Comma))
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

                if (input.IsKeyFalling(OpenTK.Input.Key.E))
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
            if (input.IsKeyRising(key))
            {
                if (input.IsKeyHeld(OpenTK.Input.Key.ControlLeft))
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