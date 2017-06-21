using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Entities;

namespace _7DRL.Components
{
    public class cStats : iComponent
    {
        public int str;
        public int dex;
        public int intel;
        public int wis;
        public int cha;

        public cStats(bool random, int maxStat)
        {
            if(random)
            {
                str = Game.g.rng.Next(1, maxStat);
                dex = Game.g.rng.Next(1, maxStat);
                intel = Game.g.rng.Next(1, maxStat);
                wis = Game.g.rng.Next(1, maxStat);
                cha = Game.g.rng.Next(1, maxStat);
            }
            else
            {
                str = 10;
                dex = 10;
                intel = 10;
                wis = 10;
                cha = 10;
            }
        }

        public void Run(drawable d)
        {

        }
    }
}
