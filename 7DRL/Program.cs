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

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

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

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            Game game = new Game(6);
            game.onLoad();

            Timer updateTimer = new Timer(updateTime);

            updateTimer.Elapsed += game.update;
            updateTimer.Start();

            while (game.running)
            {
                if(game.stop)
                {
                    updateTimer.Stop();
                    //game.running = false;
                }
            }
        }
    }
}
