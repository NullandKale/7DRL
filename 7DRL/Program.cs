using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace _7DRL
{
    class Program
    {
        static void Main(string[] args)
        {
            int fps = 30;
            int updateTime = 1000 / fps;

            Game game = new Game();
            game.onLoad();

            Timer updateTimer = new Timer(updateTime);

            updateTimer.Elapsed += game.update;
            updateTimer.Start();

            while (game.running)
            {
                if(game.stop)
                {
                    updateTimer.Stop();
                }
            }
        }
    }
}
