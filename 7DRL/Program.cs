namespace _7DRL
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Timers;

    public class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_SIZE = 0xF000;
        private const int ENABLE_QUICK_EDIT = 0x0040;
        private const int ENABLE_MOUSE_INPUT = 0x0010;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        private static void Main(string[] args)
        {
            int fps = 80;
            int updateTime = 1000 / fps;

            // Set size and Title.
            Console.SetWindowSize(120, 30);
            Console.Title = "Apocatastasis";

            // Disable resizing.
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            int mode = 0;

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            // Disable Mouse Clicking
            handle = GetStdHandle(-10);

            if (!GetConsoleMode(handle, out mode))
            {
                // error getting the console mode. Exit.
                Console.WriteLine(Marshal.GetLastWin32Error());
                return;
            }

            mode &= ~ENABLE_QUICK_EDIT;
            mode &= ~ENABLE_MOUSE_INPUT;

            if (!SetConsoleMode(handle, mode))
            {                
                // error setting console mode.
                Console.WriteLine(Marshal.GetLastWin32Error());
            }

            Console.WriteLine("    Apocatastasis Welcomes You.");
            Console.WriteLine("(0) Create Character and Game");
            Console.WriteLine("(1) Start Game with default Character");
            Console.WriteLine("(2) Quit");

            Timer updateTimer = new Timer(updateTime);
            Game game;

            bool inputValid = false;

            while (!inputValid)
            {
                string c = Console.ReadLine();

                int input = -1;

                try
                {
                    input = int.Parse(c);
                }
                catch
                {
                }

                if (input == -1)
                {
                    Console.WriteLine("Invalid input, input just a number.");
                }
                else if (input == 0)
                {
                    // STAT ROLL MENU
                    bool getRandom = true;
                    bool RandomSeed = false;
                    int seed = 0;
                    while (getRandom)
                    {
                        Console.Write("Press enter to use a random seed, or enter a seed");
                        string seedString = Console.ReadLine();

                        if (seedString == string.Empty)
                        {
                            RandomSeed = true;
                        }
                        else
                        {
                            RandomSeed = false;
                            byte[] toBytes = System.Text.Encoding.ASCII.GetBytes(seedString);
                            seed = BitConverter.ToInt32(toBytes, 0);
                            getRandom = false;
                        }
                    }

                    bool getName = true;
                    string playerName = string.Empty;

                    while (getName)
                    {
                        Console.Write("Enter Your Name!: ");
                        playerName = Console.ReadLine();

                        if (playerName == string.Empty || playerName.Count() > 21)
                        {
                            Console.WriteLine("Your name must be 1 - 20 characters long");
                        }
                        else
                        {
                            getName = false;
                        }
                    }

                    Console.Write("Press Enter to select stats or type \"R\" to roll stats ");

                    int str = 1;
                    int dex = 1;
                    int con = 1;
                    int intel = 1;
                    int wis = 1;
                    int cha = 1;

                    if (Console.ReadLine().ToUpper() == "R")
                    {
                        bool cont = true;
                        Random rng = new Random();

                        while (cont)
                        {
                            Console.SetCursorPosition(0, 7);
                            Console.WriteLine("Rolling Stats for " + playerName);

                            int Total = 0;

                            str = rng.Next(3, 10);
                            Total += str;

                            dex = rng.Next(1, 10);
                            Total += dex;

                            con = rng.Next(1, 10);
                            Total += con;

                            intel = rng.Next(1, 10);
                            Total += intel;

                            wis = rng.Next(1, 10);
                            Total += wis;

                            cha = rng.Next(1, 10);
                            Total += cha;

                            Console.WriteLine("Strength: " + str);
                            Console.WriteLine("Dexterity: " + dex);
                            Console.WriteLine("Constitution: " + con);
                            Console.WriteLine("Inteligence: " + intel);
                            Console.WriteLine("Wisdom: " + wis);
                            Console.WriteLine("Charisma: " + cha);
                            Console.WriteLine("Total: " + Total);
                            Console.WriteLine("Enter (Y) to accept and ANYTHING ELSE to ReRoll");

                            if (Console.ReadLine().ToUpper() == "Y")
                            {
                                cont = false;
                            }
                        }
                    }
                    else
                    {
                        bool cont = true;
                        Random rng = new Random();

                        int Total = 29;

                        while (cont)
                        {
                            Console.WriteLine("(S) Strength: " + str);
                            Console.WriteLine("(D) Dexterity: " + dex);
                            Console.WriteLine("(C) Constitution: " + con);
                            Console.WriteLine("(I) Inteligence: " + intel);
                            Console.WriteLine("(W) Wisdom: " + wis);
                            Console.WriteLine("(H) Charisma: " + cha);
                            Console.WriteLine("    Left: " + Total);

                            if (Total == 0)
                            {
                                Console.WriteLine("If you are satisfied enter (Y), otherwise enter ANYTHING ELSE");
                                if (Console.ReadLine().ToUpper() == "Y")
                                {
                                    cont = false;
                                }
                            }

                            if (cont)
                            {
                                Console.Write("Enter a letter to select stat: ");

                                string selection = Console.ReadLine().ToUpper();

                                if (selection == "S")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || str + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        str += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "D")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || dex + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        dex += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "C")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || con + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        con += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "I")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || intel + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        intel += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "W")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || wis + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        wis += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "H")
                                {
                                    Console.Write("Amount?: ");

                                    string read = Console.ReadLine();

                                    int amount = 0;

                                    try
                                    {
                                        amount = int.Parse(read);
                                    }
                                    catch
                                    {
                                    }

                                    if (amount > 10 || cha + amount > 10)
                                    {
                                        Console.WriteLine("Amount too big");
                                    }
                                    else
                                    {
                                        if (Total - amount < 0)
                                        {
                                            amount = Total;
                                        }

                                        cha += amount;
                                        Total -= amount;
                                    }
                                }
                                else if (selection == "GOD")
                                {
                                    str = 10;
                                    dex = 10;
                                    con = 10;
                                    intel = 10;
                                    wis = 10;
                                    cha = 10;

                                    cont = false;
                                }
                            }
                        }
                    }

                    game = new Game(seed, str, dex, con, intel, wis, cha, playerName, RandomSeed);
                    game.onLoad(false, Managers.GenerationType.Rooms);

                    updateTimer.Elapsed += game.update;

                    inputValid = true;

                    updateTimer.Start();

                    while (!game.stop)
                    {

                    }
                }
                else if (input == 1)
                {
                    string playerName = "Maaarrr";

                    int str = 8;
                    int dex = 7;
                    int con = 9;
                    int intel = 5;
                    int wis = 5;
                    int cha = 1;

                    game = new Game(100, str, dex, con, intel, wis, cha, playerName, true);
                    game.onLoad(false, Managers.GenerationType.Rooms);

                    updateTimer.Elapsed += game.update;

                    inputValid = true;

                    updateTimer.Start();

                    while (!game.stop)
                    {

                    }
                }
                else
                {
                    Console.WriteLine("Thanks for playing!");
                    inputValid = true;
                }
            }
        }
    }
}
