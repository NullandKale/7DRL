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

        public int healRate;
        public int manaRate;
        public int stamRate;
        public bool inCombat;
        public bool outOfStam;

        public float carryWeight;
        public bool isEncumbered;
        public int weaponDamage;
        public int damageReduction;

        private bool statsChanged;

        private bool debug;

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

            isEncumbered = false;
            weaponDamage = 0;
            RegenStats();

            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
        }

        public void Run(drawable d)
        {
            if(currentXP > NeededXP)
            {
                LevelUP();
            }

            if(statsChanged)
            {
                RegenStats();
            }

            if(Game.doTick)
            {
                PassiveHeal();
            }

            if (Game.input.isKeyRising(OpenTK.Input.Key.Tilde))
            {
                if (debug)
                {
                    debug = false;
                }
                else
                {
                    debug = true;
                }
            }

        }

        public void RegenStats()
        {
            maxHealth = con * 20 + 5 * level;
            maxMana = wis * 20 + 5 * level;
            maxStamina = dex * 20 + 5 * level;

            healRate = con / 6;
            manaRate = intel / 4;
            stamRate = dex / 2;

            NeededXP = 75 * level + 125;

            carryWeight = str * 2 + 20;

            statsChanged = false;
        }

        private void PassiveHeal()
        {
            if(debug)
            {
                Heal(100);
                RegenMana(100);
                RegenStam(100);
            }

            RegenMana(manaRate);

            if (!inCombat)
            {
                if (((float)currentHealth / (float)maxHealth) < .40f)
                {
                    Heal(healRate);
                }

                RegenStam(stamRate);
                
                if (outOfStam)
                {
                    if ((float)currentStamina > (float)maxStamina * 0.25f)
                    {
                        outOfStam = false;
                    }
                }
                else if (currentStamina <= 0)
                {
                    outOfStam = true;
                }                
            }
            else
            {
                inCombat = false;
            }
        }

        public void GainXP(int amount)
        {
            currentXP += amount;
        }

        private void LevelUP()
        {
            level++;
            statsChanged = true;
        }

        public void Heal(int amount)
        {
            if(currentHealth + amount > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += amount;
            }
        }

        public void RegenStam(int amount)
        {
            if (currentStamina + amount > maxStamina)
            {
                currentStamina = maxStamina;
            }
            else
            {
                currentStamina += amount;
            }
        }

        public void RegenMana(int amount)
        {
            if (currentMana + amount > maxMana)
            {
                currentMana = maxMana;
            }
            else
            {
                currentMana += amount;
            }
        }

        public int Damage(int attackAmount)
        {
            attackAmount = attackAmount - damageReduction;
            if(currentHealth - attackAmount < 0)
            {
                currentHealth -= attackAmount;
                Console.WriteLine(" GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER");
                Game.g.stop = true;
            }
            else if(attackAmount < 0)
            {
                //Do nothing
            }
            else
            {
                currentHealth -= attackAmount;
            }

            return attackAmount;
        }

        public int getAttack()
        {
            int crit = Game.g.rng.Next(0, 21);

            if(crit < dex)
            {
                return (str + weaponDamage) * 2;
            }
            else
            {
                return str + weaponDamage;
            }
        }
    }
}
