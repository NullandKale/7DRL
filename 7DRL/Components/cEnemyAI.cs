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

        private int health;
        private int damage;
        private int range;
        private int xpAmount;

        public cEnemyAI(drawable pc, cStats pcStats, int health, int attack, int range)
        {
            player = pc;
            playerStats = pcStats;
            this.health = health;
            this.range = range;
            damage = attack;
            xpAmount = health;
        }

        public void Run(drawable d)
        {
            if (Point.dist(player.pos, d.pos) > 4)
            {
                if(targetPos == null || getNewTargetPos)
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
                MoveTowards(new Point(player.pos.xPos, player.pos.yPos), d);
                Attack(d);
            }
        }

        private void Attack(drawable d)
        {
            if (Point.dist(d.pos, player.pos) < 1.5)
            {
                int attack = damage + Game.g.rng.Next(-range, range);

                int whoseFirst = Game.g.rng.Next(0, 21);

                if(whoseFirst < playerStats.dex)
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

        private void Damage(int amount, drawable d, cStats playerStats)
        {
            if(health - amount <= 0)
            {
                d.setPos(-1, -1);
                d.active = false;
                playerStats.GainXP(xpAmount);
            }
            else
            {
                health -= amount;
            }
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