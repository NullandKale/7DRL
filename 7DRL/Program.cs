using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace _7DRL
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;
        public const int ENABLE_QUICK_EDIT = 0x0040;
        public const int ENABLE_MOUSE_INPUT = 0x0010;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        static void Main(string[] args)
        {
            int fps = 30;
            int updateTime = 1000 / fps;

            // Set size and Title.
            Console.SetWindowSize(120, 30);
            Console.Title = "7DRL";

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
                Console.WriteLine(Marshal.GetLastWin32Error());
                // error setting console mode.
            }

            Console.WriteLine("Welcome to ________");
            Console.WriteLine("(0) New Game");
            Console.WriteLine("(1) Load Game -- NOT IMPLEMENTED");
            Console.WriteLine("(2) Quit");

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

                if(input == -1)
                {
                    Console.WriteLine("Invalid Input");
                }
                else if(input == 0)
                {
                    //STAT ROLL MENU

                    Console.Write("Enter Your Name!: ");

                    string playerName = Console.ReadLine();

                    Console.Write("Press Enter to select stats or type \"R\" to roll stats ");

                    int str = 1;
                    int dex = 1;
                    int con = 1;
                    int intel = 1;
                    int wis = 1;
                    int cha = 1;

                    if (Console.ReadLine() == "R")
                    {
                        Console.SetCursorPosition(0, 6);
                        Console.WriteLine("Rolling Stats for " + playerName);

                        bool cont = true;
                        Random rng = new Random();

                        while (cont)
                        {
                            int Total = 0;

                            str = rng.Next(1, 10);
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

                            if (Console.ReadLine() == "Y")
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
                            }
                        }
                    }
                        
                    Game game = new Game(6, str, dex, con, intel, wis, cha);
                    game.onLoad(false, Managers.GenerationType.Rooms);

                    Timer updateTimer = new Timer(updateTime);

                    //updateTimer.Elapsed += game.update;
                    //updateTimer.Start();

                    while (game.running)
                    {
                        if (game.stop)
                        {
                            updateTimer.Stop();
                            //game.running = false;
                        }
                        else
                        {
                            game.update(null, null);
                        }
                    }
                }
                else if(input == 1)
                {
                    Console.WriteLine("We can't do that yet");
                }
                else
                {
                    Console.WriteLine("Thanks for playing!");
                }
            }
        }
    }
}
