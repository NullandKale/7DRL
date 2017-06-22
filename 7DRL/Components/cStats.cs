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
        public int con;
        public int intel;
        public int wis;
        public int cha;

        public int maxHealth;
        public int maxMana;
        public int maxStamina;

        public int currentHealth;
        public int currentMana;
        public int currentStamina;

        public int level;
        public int NeededXP;
        public int currentXP;

        private bool statsChanged;

        public cStats(bool random, int maxStat)
        {
            level = 1;
            currentXP = 0;
            if(random)
            {
                str = Game.g.rng.Next(1, maxStat);
                dex = Game.g.rng.Next(1, maxStat);
                con = Game.g.rng.Next(1, maxStat);
                intel = Game.g.rng.Next(1, maxStat);
                wis = Game.g.rng.Next(1, maxStat);
                cha = Game.g.rng.Next(1, maxStat);
            }
            else
            {
                str = 10;
                dex = 10;
                con = 10;
                intel = 10;
                wis = 10;
                cha = 10;
            }

            RegenStats();
        }

        public void Run(drawable d)
        {
            if(statsChanged)
            {
                RegenStats();
            }
            Game.g.toPrint = ("H: " + currentHealth + "/" + maxHealth + " M: " + currentMana + "/" + maxMana + " S: " + currentStamina + "/" + maxStamina);
        }

        private void RegenStats()
        {
            maxHealth = con * 20 + 5 * level;
            maxMana = wis * 20 + 5 * level;
            maxStamina = dex * 20 + 5 * level;

            NeededXP = 75 * level + 125;
        }
    }
}
