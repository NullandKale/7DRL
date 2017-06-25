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
            Console.WriteLine("(0) Play");
            Console.WriteLine("(1) Quit");
            string c = string.Empty;
            while (c != "0" && c != "1")
            {
                c = Console.ReadLine();

                if (c != "0" && c != "1")
                {
                    Console.WriteLine("Invalid");
                }
            }

            if (c == "0")
            {
                Game game = new Game(6);
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
            else
            {
                Console.WriteLine("Thanks for playing!");
            }
        }
    }
}
