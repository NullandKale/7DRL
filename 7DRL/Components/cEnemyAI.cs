namespace _7DRL.Components
{
    using Entities;
    using Utils;

    public class cEnemyAI : iComponent
    {
        public Point StartingPosition;
        public string enemyName;
        public int maxHealth;
        public int health;
        public int damage;
        public int range;
        public int detectRange;
        public double attackRange;

        private Point targetPos;
        private Drawable player;
        private cStats playerStats;
        private bool getNewTargetPos;
        private int xpAmount;
        private double lootChance;

        public cEnemyAI()
        {
        }

        public cEnemyAI(Drawable pc, cStats pcStats, Point startingPosition, string enemyName, int health, int attack, int range, int detectRange, double attackRange, double lootChance)
        {
            player = pc;
            playerStats = pcStats;
            this.StartingPosition = startingPosition;
            this.enemyName = enemyName;
            this.health = health;
            this.maxHealth = health;
            this.range = range;
            this.detectRange = detectRange;
            this.attackRange = attackRange;
            damage = attack;
            this.lootChance = lootChance;
            xpAmount = (health + attack + ((int)attackRange * 3)) / 5;
        }

        public void Run(Drawable d)
        {
            if (Point.Dist(player.pos, d.pos) > detectRange)
            {
                if (targetPos == null || getNewTargetPos || EnemiesInRange(d))
                {
                    targetPos = GenerateTarget(targetPos, d);
                }

                if (d.pos.xPos == targetPos.X && d.pos.yPos == targetPos.Y)
                {
                    targetPos = GenerateTarget(targetPos, d);
                }
                else
                {
                    MoveTowards(targetPos, d);
                }
            }
            else
            {
                if (Point.Dist(player.pos, d.pos) > attackRange)
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

        public void GetHurt(Drawable enemy)
        {
            Game.g.LogCombat("You did " + Damage(playerStats.GetAttack(), enemy, playerStats) + " damage to " + enemyName);
        }

        private void Attack(Drawable d)
        {
            if (Point.Dist(d.pos, player.pos) < attackRange)
            {
                playerStats.inCombat = true;

                int attack = damage + Game.g.rng.Next(-range, range);

                if (Point.Dist(player.pos, d.pos) > 1.5)
                {
                    Game.g.LogCombat("You got " + playerStats.Damage(attack) + " damage from " + enemyName);
                }
                else
                {
                    int whoseFirst = Game.g.rng.Next(0, 21);

                    if (whoseFirst < playerStats.dex)
                    {
                        Game.g.LogCombat("You did " + Damage(playerStats.GetAttack(), d, playerStats) + " damage to " + enemyName);
                        Game.g.LogCombat("You got " + playerStats.Damage(attack) + " damage from " + enemyName);
                    }
                    else
                    {
                        Game.g.LogCombat("You got " + playerStats.Damage(attack) + " damage from " + enemyName);
                        Game.g.LogCombat("You did " + Damage(playerStats.GetAttack(), d, playerStats) + " damage to " + enemyName);
                    }
                }
            }
        }
        
        private int Damage(int amount, Drawable d, cStats playerStats)
        {
            if (health - amount <= 0)
            {
                if (Game.g.rng.NextDouble() < lootChance)
                {
                    Game.g.pcInv.SpawnLoot(d.pos.xPos, d.pos.yPos);
                }

                d.SetPos(-1, -1);
                d.active = false;
                playerStats.GainXP(xpAmount);
            }
            else
            {
                health -= amount;
            }

            return amount;
        }

        private bool EnemiesInRange(Drawable d)
        {
            for (int i = -4; i <= 4; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    if (Game.isInWorld(d.pos.xPos + i, d.pos.yPos + j))
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

        private void MoveTowards(Point target, Drawable d)
        {
            int moveX = 0;
            int moveY = 0;

            if (target.X > d.pos.xPos)
            {
                moveX++;
            }

            if (target.X < d.pos.xPos)
            {
                moveX--;
            }

            if (target.Y > d.pos.yPos)
            {
                moveY++;
            }

            if (target.Y < d.pos.yPos)
            {
                moveY--;
            }

            bool canMoveBoth = Managers.CollisionManager.CheckCollision(moveX, moveY, d);
            bool canMoveX = Managers.CollisionManager.CheckCollision(moveX, 0, d);
            bool canMoveY = Managers.CollisionManager.CheckCollision(0, moveY, d);

            if (!(canMoveBoth || canMoveX || canMoveY))
            {
                // if the enemy is colliding pick a new targetPos
                getNewTargetPos = true;
            }

            if (canMoveBoth && moveX != 0 && moveY != 0)
            {
                d.SetPosRelative(moveX, moveY);
            }
            else if (canMoveX && moveX != 0)
            {
                d.SetPosRelative(moveX, 0);
            }
            else if (canMoveY && moveY != 0)
            {
                d.SetPosRelative(0, moveY);
            }
        }

        private Point GenerateTarget(Point p, Drawable d)
        {
            var temp = Point.GetRandomPointNearbyInWorld(StartingPosition);
            return temp;
        }
    }
}