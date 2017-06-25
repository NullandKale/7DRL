using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DRL.Entities;
using _7DRL.Utils;

namespace _7DRL.Components
{
    class cEnemyAI : iComponent
    {
        private Point targetPos;
        private drawable player;
        private cStats playerStats;

        private bool getNewTargetPos;

        public int maxHealth;
        public int health;
        public int damage;
        public int range;
        public int detectRange;
        public double attackRange;
        private int xpAmount;
        private double lootChance;
        
        public cEnemyAI()
        {   
        }

        public cEnemyAI(drawable pc, cStats pcStats, int health, int attack, int range, int detectRange, double attackRange, double lootChance)
        {
            player = pc;
            playerStats = pcStats;
            this.health = health;
            this.maxHealth = health;
            this.range = range;
            this.detectRange = detectRange;
            this.attackRange = attackRange;
            damage = attack;
            this.lootChance = lootChance;
            xpAmount = (health + attack + ((int)attackRange * 3)) / 5;
        }

        public void Run(drawable d)
        {
            if (Point.dist(player.pos, d.pos) > detectRange)
            {
                if(targetPos == null || getNewTargetPos || EnemiesInRange(d))
                {
                    targetPos = GenerateTarget();
                }
                if (d.pos.xPos == targetPos.x && d.pos.yPos == targetPos.y)
                {
                    targetPos = GenerateTarget();
                }
                else
                {
                    MoveTowards(targetPos, d);
                }
            }
            else
            {
                if (Point.dist(player.pos, d.pos) > attackRange)
                {
                    MoveTowards(new Point(player.pos.xPos, player.pos.yPos), d);
                }

                Attack(d);
            }
        }

        public void Reset()
        {
            getNewTargetPos = true;
            health = maxHealth;
        }

        private void Attack(drawable d)
        {
            if (Point.dist(d.pos, player.pos) < attackRange)
            {
                playerStats.inCombat = true;

                int attack = damage + Game.g.rng.Next(-range, range);

                if (Point.dist(player.pos, d.pos) > 1.5)
                {
                    playerStats.Damage(attack);
                }
                else
                { 
                    int whoseFirst = Game.g.rng.Next(0, 21);

                    if (whoseFirst < playerStats.dex)
                    {
                        Damage(playerStats.getAttack(), d, playerStats);
                        playerStats.Damage(attack);
                    }
                    else
                    {
                        playerStats.Damage(attack);
                        Damage(playerStats.getAttack(), d, playerStats);
                    }
                }
            }
        }

        private void Damage(int amount, drawable d, cStats playerStats)
        {
            if(health - amount <= 0)
            {
                if(Game.g.rng.NextDouble() < lootChance)
                {
                    Game.g.pcInv.SpawnLoot(d.pos.xPos, d.pos.yPos);
                }
                d.setPos(-1, -1);
                d.active = false;
                playerStats.GainXP(xpAmount);
            }
            else
            {
                health -= amount;
            }
        }

        private bool EnemiesInRange(drawable d)
        {
            for(int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    if(Game.isInWorld(d.pos.xPos + i, d.pos.yPos + j))
                    {
                        if (Game.g.world[d.pos.xPos + i, d.pos.yPos + j].Visual == 'e' ||
                            Game.g.world[d.pos.xPos + i, d.pos.yPos + j].Visual == 'E' ||
                            Game.g.world[d.pos.xPos + i, d.pos.yPos + j].Visual == 's' ||
                            Game.g.world[d.pos.xPos + i, d.pos.yPos + j].Visual == 'S')
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void MoveTowards(Point target, drawable d)
        {
            int moveX = 0;
            int moveY = 0;

            if (target.x > d.pos.xPos)
            {
                moveX++;
            }
            if (target.x < d.pos.xPos)
            {
                moveX--;
            }

            if (target.y > d.pos.yPos)
            {
                moveY++;
            }
            if (target.y < d.pos.yPos)
            {
                moveY--;
            }

            bool canMoveBoth = Managers.CollisionManager.CheckCollision(moveX, moveY, d);
            bool canMoveX = Managers.CollisionManager.CheckCollision(moveX, 0, d);
            bool canMoveY = Managers.CollisionManager.CheckCollision(0, moveY, d);

            if(!(canMoveBoth || canMoveX || canMoveY))
            {
                //if the enemy is colliding pick a new targetPos
                getNewTargetPos = true;
            }


            if (canMoveBoth && moveX != 0 && moveY != 0)
            {
                d.setPosRelative(moveX, moveY);
            }
            else if (canMoveX && moveX != 0)
            {
                d.setPosRelative(moveX, 0);
            }
            else if (canMoveY && moveY != 0)
            {
                d.setPosRelative(0, moveY);
            }

            //Console.SetCursorPosition(0, 29);
            //Console.Write(moveX + ", " + moveY + ", " + canMoveBoth + ", " + canMoveX + ", " + canMoveY);
        }

        private Point GenerateTarget()
        {
            Point p = Point.getRandomPoint(Game.g.worldSize);

            if (Managers.CollisionManager.CheckCollision(p.x, p.y))
            {
                return p;
            }
            else
            {
                return GenerateTarget();
            }
        }
    }
}