using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL.Utils
{
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {

        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point p = (Point)obj;
                if (p.x == x && p.y == y)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static double dist(Point p1, Point p2)
        {
            double x = Math.Pow(Math.Abs(p1.x - p2.x), 2);
            double y = Math.Pow(Math.Abs(p1.y - p2.y), 2);

            return Math.Sqrt(x + y);
        }

        public static double dist(Entities.transform t1, Entities.transform t2)
        {
            double x = Math.Pow(Math.Abs(t1.xPos - t2.xPos), 2);
            double y = Math.Pow(Math.Abs(t1.yPos - t2.yPos), 2);

            return Math.Sqrt(x + y);
        }

        public static Point getRandomPoint(List<Point> points)
        {
            return points[Game.g.rng.Next(0, points.Count)];
        }

        public static Point getRandomPoint(int max)
        {
            return new Point(Game.g.rng.Next(0, max), Game.g.rng.Next(0, max));
        }

        public static Point getRandomPointInWorld()
        {
            Point p = getRandomPoint(Game.g.worldSize);
            if(!Managers.CollisionManager.CheckCollision(p.x, p.y))
            {
                return getRandomPointInWorld();
            }
            else
            {
                return p;
            }
        }

        public static Point getRandomDoorPoint(Point pos)
        {
            Point p = getRandomPoint(Util.FloodFill(Game.g.ground, pos, Game.g.worldSize));
            if (!Managers.CollisionManager.CheckCollision(p.x, p.y))
            {
                return getRandomPoint(Util.FloodFill(Game.g.ground, pos, Game.g.worldSize));
            }
            else
            {
                return p;
            }
        }
    }
}
