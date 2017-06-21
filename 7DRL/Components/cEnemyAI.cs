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

        public cEnemyAI(drawable pc)
        {
            player = pc;
        }

        public void Run(drawable d)
        {
            if (Point.dist(player.pos, d.pos) > 4)
            {
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

            bool canMoveX = Managers.CollisionManager.CheckCollision(moveX, 0, d);
            bool canMoveY = Managers.CollisionManager.CheckCollision(0, moveY, d);

            if (canMoveX)
            {
                d.setPosRelative(moveX, 0);
            }
            if (canMoveY)
            {
                d.setPosRelative(0, moveY);
            }
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
