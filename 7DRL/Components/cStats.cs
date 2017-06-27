namespace _7DRL.Components
{
    using System;
    using Entities;

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

        public cStats(int pStr, int pDex, int pCon, int pIntel, int pWis, int pCha)
        {
            level = 1;
            currentXP = 0;

            str = pStr;
            dex = pDex;
            con = pCon;
            intel = pIntel;
            wis = pWis;
            cha = pCha;

            isEncumbered = false;
            weaponDamage = 0;
            RegenStats();

            currentHealth = maxHealth;
            currentMana = maxMana;
            currentStamina = maxStamina;
        }

        public void Run(Drawable d)
        {
            if (currentXP > NeededXP)
            {
                LevelUP();
            }

            if (statsChanged)
            {
                RegenStats();
            }

            RegenStats();

            if (Game.doTick)
            {
                PassiveHeal();
            }

            if (Game.input.IsKeyRising(OpenTK.Input.Key.Tilde))
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

            Console.SetCursorPosition(0, 29);

            Console.Write("HP:");
            Console.ForegroundColor = ConsoleColor.Red;

            for (int i = 0; i < 10; i++)
            {
                if ((float)currentHealth / (float)maxHealth > (float)i / 10f)
                {
                    Console.Write((char)0x2588);
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Mana:");
            Console.ForegroundColor = ConsoleColor.Blue;

            for (int i = 0; i < 10; i++)
            {
                if ((float)currentMana / (float)maxMana > (float)i / 10f)
                {
                    Console.Write((char)0x2588);
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" Stam:");
            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < 10; i++)
            {
                if ((float)currentStamina / (float)maxStamina > (float)i / 10f)
                {
                    Console.Write((char)0x2588);
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Game.g.screenX - Game.g.startingSeed.Length - 1, 29);
            Console.Write(Game.g.startingSeed);
        }

        public void RegenStats()
        {
            maxHealth = (con * 20) + (5 * level);
            maxMana = (wis * 20) + (5 * level);
            maxStamina = (dex * 20) + (5 * level);

            healRate = con / 6;
            manaRate = intel / 4;
            stamRate = dex / 2;

            NeededXP = (150 * level) + 125;

            carryWeight = (str * 2) + 20;

            statsChanged = false;
        }
        
        public void GainXP(int amount)
        {
            currentXP += amount;
        }

        public void Heal(int amount)
        {
            if (currentHealth + amount > maxHealth)
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
            if (attackAmount < 0)
            {
                return 0;
            }
            else if (currentHealth - attackAmount < 0)
            {
                currentHealth -= attackAmount;
                Game.g.LogCombat("You DIED");
                Console.SetCursorPosition(0, 29);
                Console.WriteLine(" GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER GAME OVER");
                Game.g.stop = true;
            }
            else
            {
                currentHealth -= attackAmount;
            }

            return attackAmount;
        }

        public int GetAttack()
        {
            int crit = Game.g.rng.Next(0, 21);

            if (crit < dex)
            {
                return (str + weaponDamage) * 2;
            }
            else
            {
                return str + weaponDamage;
            }
        }


        private void PassiveHeal()
        {
            if (debug)
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

        private void LevelUP()
        {
            level++;
            statsChanged = true;
        }

    }
}
