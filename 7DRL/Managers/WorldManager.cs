namespace _7DRL.Managers
{
    using _7DRL.Utils;
    using System;
    using System.Drawing;
    using System.Collections.Generic;

    public static class WorldManager
    {
        private static char wall = (char)0x2588;
        private static char air = ' ';

        private static Tile[,] initialiseMap(Tile[,] map, int worldSize, float chanceToStartAlive)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    if (Game.g.rng.NextDouble() < chanceToStartAlive)
                    {
                        map[x, y] = new Tile();
                        map[x, y].Visual = wall;
                        map[x, y].collideable = true;
                    }
                }
            }
            return map;
        }

        private static Tile[,] clearMap(Tile[,] map, int worldSize)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    map[x, y].Visual = air;
                    map[x, y].collideable = false;
                }
            }
            return map;
        }

        //Returns the number of cells in a ring around (x,y) that are alive.
        private static int countAliveNeighbours(Tile[,] map, int x, int y, int worldSize)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    //If we're looking at the middle point
                    if (i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= worldSize || neighbour_y >= worldSize)
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (map[neighbour_x, neighbour_y].Visual == wall)
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }

        private static Tile[,] doSimulationStep(Tile[,] oldMap, int worldSize)
        {
            Tile[,] newMap = new Tile[worldSize, worldSize];
            //Loop over each row and column of the map
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    newMap[x, y] = new Tile();
                    if (x == 0 || y == 0 || x == worldSize || y == worldSize)
                    {
                        newMap[x, y].Visual = wall;
                        newMap[x, y].collideable = true;
                        continue;
                    }


                    int nbs = countAliveNeighbours(oldMap, x, y, worldSize);
                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (oldMap[x, y].Visual == wall)
                    {
                        if (nbs < 4)
                        {
                            newMap[x, y].Visual = air;
                            newMap[x, y].collideable = false;
                        }
                        else
                        {
                            newMap[x, y].Visual = wall;
                            newMap[x, y].collideable = true;
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > 4)
                        {
                            newMap[x, y].Visual = wall;
                            newMap[x, y].collideable = true;
                        }
                        else
                        {
                            newMap[x, y].Visual = air;
                            newMap[x, y].collideable = false;
                        }
                    }
                }
            }
            return newMap;
        }

        private static Tile[,] generateBorders(Tile[,] map, int worldSize)
        {
            for (var x = 0; x < worldSize; x++)
            {
                for (var y = 0; y < worldSize; y++)
                {
                    if (x == 0 || y == 0 || x == worldSize - 1 || y == worldSize - 1)
                    {
                        map[x, y].Visual = wall;
                        map[x, y].collideable = true;
                    }
                }
            }

            return map;
        }

        public static Tile[,] GenerateWorld(Tile[,] cellmap, int worldSize, GenerationType type)
        {
            switch(type)
            {
                case GenerationType.Caves:
                    cellmap = GenerateCave(cellmap, worldSize);
                    break;
                case GenerationType.Rooms:
                    cellmap = GenerateRooms(cellmap, worldSize);
                    break;
                default:
                    cellmap = GenerateCave(cellmap, worldSize);
                    break;
            }

            return cellmap;
        }

        public static Tile[,] GenerateCave(Tile[,] cellmap, int worldSize)
        {
            float chanceToStartAlive = 0.46f;
            //Set up the map with random values
            cellmap = initialiseMap(cellmap, worldSize, chanceToStartAlive);
            //And now run the simulation for a set number of steps
            for (int i = 0; i < 10; i++)
            {
                cellmap = doSimulationStep(cellmap, worldSize);
            }

            cellmap = generateBorders(cellmap, worldSize);

            return cellmap;
        }

        public static Tile[,] GenerateRooms(Tile[,] cellmap, int worldSize)
        {
            int numberRooms = 20;
            int maxRoomSize = 20;
            int minRoomSize = 5;
            int hallSize = 2;

            cellmap = doSimulationStep(cellmap, worldSize);
            cellmap = clearMap(cellmap, worldSize);

            cellmap = initialiseMap(cellmap, worldSize, 1f);

            List<Room> rooms = new List<Room>();

            for(int i = 0; i < numberRooms; i++)
            {
                rooms.Add(GetRandomRect(worldSize, minRoomSize, maxRoomSize, rooms));
            }

            cellmap = drawRooms(rooms, cellmap);

            cellmap = connectRooms(rooms, cellmap, hallSize, worldSize);

            cellmap = generateBorders(cellmap, worldSize);

            return cellmap;
        }

        public static Room GetRandomRect(int worldSize,int minRoomSize, int maxRoomSize, List<Room> rooms)
        {
            int height = Game.g.rng.Next(minRoomSize, maxRoomSize);
            int width = Game.g.rng.Next(minRoomSize, maxRoomSize);
            int xCord = Game.g.rng.Next(0, worldSize - width + 1);
            int yCord = Game.g.rng.Next(0, worldSize - height + 1);
            Room temp = new Room();
            temp.roomRect = new Rectangle(xCord, yCord, width, height);

            for (int i = 0; i < rooms.Count; i++)
            {
                if(rooms[i].roomRect.IntersectsWith(temp.roomRect))
                {
                    return GetRandomRect(worldSize,minRoomSize, maxRoomSize, rooms);
                }
            }

            return temp;

        }

        public static Tile[,] drawRooms(List<Room> rooms, Tile[,] cellmap)
        {
            for(int i = 0; i < rooms.Count; i++)
            {
                for(int x = 0; x < rooms[i].roomRect.Width; x++)
                {
                    for (int y = 0; y < rooms[i].roomRect.Height; y++)
                    {
                        if((x == 0 || x == rooms[i].roomRect.Width - 1) || (y == 0 || y == rooms[i].roomRect.Height - 1))
                        {
                            int xPos = x + rooms[i].roomRect.X;
                            int yPos = y + rooms[i].roomRect.Y;
                            cellmap[xPos, yPos].Visual = wall;
                            cellmap[xPos, yPos].collideable = true;
                        }
                        else
                        {
                            int xPos = x + rooms[i].roomRect.X;
                            int yPos = y + rooms[i].roomRect.Y;
                            cellmap[xPos, yPos].Visual = air;
                            cellmap[xPos, yPos].collideable = false;
                        }
                    }
                }
            }

            return cellmap;
        }

        public static Tile[,] connectRooms(List<Room> rooms, Tile[,] cellmap, int hallWidth, int worldSize)
        {
            double distprecentage = 4.0;

            for (int i = 0; i < rooms.Count; i++)
            {
                for(int j = 0; j < rooms.Count; j++)
                {
                    double dist = Util.dist(rooms[i].roomRect.X, rooms[i].roomRect.Y, rooms[j].roomRect.X, rooms[j].roomRect.Y);

                    if (i != j && dist < worldSize / distprecentage)
                    {
                        rooms[i].connectedRoom0 = rooms[j];
                        break;
                    }
                    else if(j == rooms.Count - 1)
                    {
                        rooms[i].connectedRoom0 = rooms[j];
                    }
                }

                if(i % 4 == 0)
                {
                    rooms[i].twoConnectedRooms = true;
                    for (int j = 0; j < rooms.Count; j++)
                    {
                        double dist = Util.dist(rooms[i].roomRect.X, rooms[i].roomRect.Y, rooms[j].roomRect.X, rooms[j].roomRect.Y);

                        if (i != j && rooms[i].connectedRoom0 != rooms[j] && dist < worldSize / distprecentage)
                        {
                            rooms[i].connectedRoom1 = rooms[j];
                            break;
                        }
                        else if (j == rooms.Count - 1)
                        {
                            rooms[i].connectedRoom1 = rooms[j];
                        }
                    }
                }
            }

            List<Room> connectedRooms = new List<Room>();

            for(int i = 0; i < 100; i++)
            {
                Room temp = WalkRooms(i, rooms[1]);

                if (!connectedRooms.Contains(temp))
                {
                    connectedRooms.Add(temp);
                }
            }

            for(int i = 0; i < rooms.Count; i++)
            {
                if (!connectedRooms.Contains(rooms[i]))
                {
                    for(int j = 0; j < rooms.Count; j++)
                    {
                        if(!rooms[j].twoConnectedRooms && rooms[j] != rooms[i] && rooms[i] != rooms[j].connectedRoom0)
                        {
                            rooms[j].twoConnectedRooms = true;
                            rooms[j].connectedRoom1 = rooms[i];
                        }
                    }
                }
            }


            for (int i = 0; i < rooms.Count; i++)
            {
                int xDiff = rooms[i].roomRect.X - rooms[i].connectedRoom0.roomRect.X;
                int yDiff = rooms[i].roomRect.Y - rooms[i].connectedRoom0.roomRect.Y;

                Rectangle drill = new Rectangle(rooms[i].roomRect.X + (rooms[i].roomRect.Width / 2), rooms[i].roomRect.Y + (rooms[i].roomRect.Height / 2), hallWidth, hallWidth);

                int roomRectMidX = rooms[i].roomRect.X + (rooms[i].roomRect.Width / 2);
                int roomRectMidY = rooms[i].roomRect.Y + (rooms[i].roomRect.Height / 2);

                int connectedRectMidX = rooms[i].connectedRoom0.roomRect.X + (rooms[i].connectedRoom0.roomRect.Width / 2);
                int connectedRectMidY = rooms[i].connectedRoom0.roomRect.Y + (rooms[i].connectedRoom0.roomRect.Height / 2);

                for (int x = 0; x < Math.Abs(xDiff); x++)
                {
                        for (int j = 0; j < drill.Width; j++)
                        {
                            for (int k = 0; k < drill.Height; k++)
                            {
                                int PosX = j + drill.X;
                                int PosY = k + drill.Y;
                                if(Game.isInWorld(PosX, PosY))
                                {
                                    cellmap[PosX, PosY].Visual = air;
                                    cellmap[PosX, PosY].collideable = false;
                                }
                            }
                        }

                        int xPos = drill.X;

                        if (roomRectMidX > connectedRectMidX)
                        {
                            xPos--;
                        }
                        else if(roomRectMidY < connectedRectMidY)
                        {
                            xPos++;
                        }

                    drill.X = xPos;
                }

                for (int y = 0; y < Math.Abs(yDiff); y++)
                {
                    for (int j = 0; j < drill.Width; j++)
                    {
                        for (int k = 0; k < drill.Height; k++)
                        {
                            int PosX = j + drill.X;
                            int PosY = k + drill.Y;
                            if (Game.isInWorld(PosX, PosY))
                            {
                                cellmap[PosX, PosY].Visual = air;
                                cellmap[PosX, PosY].collideable = false;
                            }
                        }
                    }

                    int yPos = drill.Y;

                    if (rooms[i].roomRect.Y > rooms[i].connectedRoom0.roomRect.Y)
                    {
                        yPos--;
                    }
                    else if (rooms[i].roomRect.Y < rooms[i].connectedRoom0.roomRect.Y)
                    {
                        yPos++;
                    }

                    drill.Y = yPos;
                }

            }

            for (int i = 0; i < rooms.Count; i++)
            {
                if(rooms[i].twoConnectedRooms)
                {
                    int xDiff = rooms[i].roomRect.X - rooms[i].connectedRoom1.roomRect.X;
                    int yDiff = rooms[i].roomRect.Y - rooms[i].connectedRoom1.roomRect.Y;

                    Rectangle drill = new Rectangle(rooms[i].roomRect.X + 1, rooms[i].roomRect.Y + 1, hallWidth, hallWidth);

                    for (int x = 0; x < Math.Abs(xDiff); x++)
                    {
                        for (int j = 0; j < drill.Width; j++)
                        {
                            for (int k = 0; k < drill.Height; k++)
                            {
                                int PosX = j + drill.X;
                                int PosY = k + drill.Y;
                                if (Game.isInWorld(PosX, PosY))
                                {
                                    cellmap[PosX, PosY].Visual = air;
                                    cellmap[PosX, PosY].collideable = false;
                                }
                            }
                        }

                        int xPos = drill.X;

                        if (rooms[i].roomRect.X > rooms[i].connectedRoom1.roomRect.X)
                        {
                            xPos--;
                        }
                        else if (rooms[i].roomRect.X < rooms[i].connectedRoom1.roomRect.X)
                        {
                            xPos++;
                        }

                        drill.X = xPos;
                    }

                    for (int y = 0; y < Math.Abs(yDiff); y++)
                    {
                        for (int j = 0; j < drill.Width; j++)
                        {
                            for (int k = 0; k < drill.Height; k++)
                            {
                                int PosX = j + drill.X;
                                int PosY = k + drill.Y;
                                if (Game.isInWorld(PosX, PosY))
                                {
                                    cellmap[PosX, PosY].Visual = air;
                                    cellmap[PosX, PosY].collideable = false;
                                }
                            }
                        }

                        int yPos = drill.Y;

                        if (rooms[i].roomRect.Y > rooms[i].connectedRoom1.roomRect.Y)
                        {
                            yPos--;
                        }
                        else if (rooms[i].roomRect.Y < rooms[i].connectedRoom1.roomRect.Y)
                        {
                            yPos++;
                        }

                        drill.Y = yPos;
                    }
                }
            }

            return cellmap;
        }

        private static Room WalkRooms(int roomNumber, Room currentRoom)
        {
            if(roomNumber <= 1)
            {
                return currentRoom.connectedRoom0;
            }
            else
            {
                return WalkRooms(roomNumber - 1, currentRoom.connectedRoom0);
            }
        }
    }

    public class Room
    {
        public Rectangle roomRect;
        public Room connectedRoom0;
        public Room connectedRoom1;
        public bool twoConnectedRooms;
    }

    public enum GenerationType
    {
        Caves,
        Rooms,
    }
}