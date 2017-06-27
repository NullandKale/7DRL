namespace _7DRL.Utils
{
    using System;
    using System.Collections.Generic;

    public class Point
    {
        public int X;
        public int Y;

        public Point()
        {
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static double Dist(Point p1, Point p2)
        {
            double x = Math.Pow(Math.Abs(p1.X - p2.X), 2);
            double y = Math.Pow(Math.Abs(p1.Y - p2.Y), 2);

            return Math.Sqrt(x + y);
        }

        public static double SquareDist(Point p1, Point p2)
        {
            double x = Math.Pow(Math.Abs(p1.X - p2.X), 2);
            double y = Math.Pow(Math.Abs(p1.Y - p2.Y), 2);

            return x + y;
        }

        public static double Dist(Entities.Transform t1, Entities.Transform t2)
        {
            double x = Math.Pow(Math.Abs(t1.xPos - t2.xPos), 2);
            double y = Math.Pow(Math.Abs(t1.yPos - t2.yPos), 2);

            return Math.Sqrt(x + y);
        }

        public static double SquareDist(Entities.Transform t1, Entities.Transform t2)
        {
            double x = Math.Pow(Math.Abs(t1.xPos - t2.xPos), 2);
            double y = Math.Pow(Math.Abs(t1.yPos - t2.yPos), 2);

            return x + y;
        }

        public static Point GetRandomPoint(List<Point> points)
        {
            return points[Game.g.rng.Next(0, points.Count)];
        }

        public static Point GetRandomPoint(int max)
        {
            return new Point(Game.g.rng.Next(0, max), Game.g.rng.Next(0, max));
        }

        public static Point GetRandomPointInWorld()
        {
            Point p = GetRandomPoint(Game.g.worldSize);
            if (!Managers.CollisionManager.CheckCollision(p.X, p.Y))
            {
                return GetRandomPointInWorld();
            }
            else
            {
                return p;
            }
        }

        public static Point GetRandomPointNearbyInWorld(Point loc)
        {
            Point p = GetRandomPoint(Game.g.worldSize);
            if (!Managers.CollisionManager.CheckCollision(p.X, p.Y) && Dist(p, loc) > 5)
            {
                return GetRandomPointInWorld();
            }
            else
            {
                return p;
            }
        }

        public static Point GetRandomDoorPoint(Point pos)
        {
            List<Point> validPoints = Util.FloodFill(Game.g.ground, pos, Game.g.worldSize);
            Point p = GetRandomPoint(validPoints);
            if (!Managers.CollisionManager.CheckCollision(p.X, p.Y))
            {
                return GetRandomPoint(validPoints);
            }
            else
            {
                return p;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Point p = (Point)obj;

            if(p.X != X)
            {
                return false;
            }
            else if (p.Y != Y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
