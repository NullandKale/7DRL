namespace _7DRL.Managers
{
    using _7DRL.Utils;
    using System;

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
            cellmap = initialiseMap(cellmap, worldSize, 0.46f);

            cellmap = doSimulationStep(cellmap, worldSize);
            cellmap = clearMap(cellmap, worldSize);

            cellmap = generateBorders(cellmap, worldSize);

            return cellmap;
        }
    }

    public enum GenerationType
    {
        Caves,
        Rooms,
    }
}