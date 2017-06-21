namespace _7DRL.Managers
{
    using System;

    public static class WorldManager
    {
        private static float chanceToStartAlive = 0.45f;

        private static char[,] initialiseMap(char[,] map, int worldSize)
        {
            Random r = new Random();
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    if (r.NextDouble() < chanceToStartAlive)
                    {
                        map[x, y] = '#';
                    }
                }
            }
            return map;
        }

        //Returns the number of cells in a ring around (x,y) that are alive.
        private static int countAliveNeighbours(char[,] map, int x, int y, int worldSize)
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
                    else if (map[neighbour_x, neighbour_y] == '#')
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }

        private static char[,] doSimulationStep(char[,] oldMap, int worldSize)
        {
            char[,] newMap = new char[worldSize, worldSize];
            //Loop over each row and column of the map
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    if (x == 0 || y == 0 || x == worldSize || y == worldSize)
                    {
                        newMap[x, y] = '#';
                        continue;
                    }


                    int nbs = countAliveNeighbours(oldMap, x, y, worldSize);
                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (oldMap[x, y] == '#')
                    {
                        if (nbs < 4)
                        {
                            newMap[x, y] = ' ';
                        }
                        else
                        {
                            newMap[x, y] = '#';
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > 4)
                        {
                            newMap[x, y] = '#';
                        }
                        else
                        {
                            newMap[x, y] = ' ';
                        }
                    }
                }
            }
            return newMap;
        }

        public static char[,] GenerateWorld(int worldSize)
        {
            //Create a new map
            char[,] cellmap = new char[worldSize, worldSize];
            //Set up the map with random values
            cellmap = initialiseMap(cellmap, worldSize);
            //And now run the simulation for a set number of steps
            for (int i = 0; i < 10; i++)
            {
                cellmap = doSimulationStep(cellmap, worldSize);
            }

            return cellmap;
        }
    }
}