using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int width;
    public int hight;
    [Range(0, 100)]
    public int fillPercent;
    [Range(0, 10)]
    public int iterations;
    public string seed;
    int[,] map;
    [Header("Fill sizes")]
    public int minWallSize;
    public int minRoomSize;
    [Header("HallwayType")]
    public bool useTileMap = false;
    [Header("Cell Rules")]
    [Range(0, 1)]
    public int[] rules = new int[9];
    List<int[,]> hallways;

    //Get a list of parts taken from Cellular Automata
    public List<int[,]> GetMaps(string _seed, int _iterations)
    {
        seed = _seed;
        iterations = _iterations;
        hallways = gethallways();
        //get parts from the map
        List<int[,]> maps = new List<int[,]>();
        ConnectionPoinSearch point = new ConnectionPoinSearch();

        //invert the maps then add them to the list of return maps
        for (int i = 0; i < hallways.Count; i++)
        {
            hallways[i] = InvertMap(hallways[i]);
            hallways[i] = point.findEnds(hallways[i]);
            maps.Add(hallways[i]);
        }

        return maps;
    }

    //It was supposed to be hallways at first but later i changed it to be the rooms and corredors in one
    public List<int[,]> gethallways()
    {
        GenMap();
        getSectionsLists();
        hallways = ConvertToHallways();
        return hallways;
    }
    //simply generates the map an iterates to it requested iteration.
    //An optimization woud be to go trough iterations wile adding them then sending them all back at once.
    private void GenMap()
    {
        map = new int[width, hight];
        GenerateMap();
        for (int x = 0; x < iterations; x++)
        {
            Iterate();
        }
    }

    //Fill the map randomly with 1's and 0's
    void GenerateMap()
    {
        System.Random rng = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == hight - 1)
                {
                    map[x, y] = 1; 
                }
                else
                    map[x, y] = (rng.Next(0, 100) < fillPercent) ? 1 : 0;
            }
        }
    }

    //looping trough all the cells in the map and applying the rules on them then setting the map to the iteration
    //and better solution would be to have it return the map it made.
    void Iterate()
    {
        int[,] iterationMap = new int[width, hight];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                iterationMap[x, y] = Rules(SurroundWalls(x, y));
            }
        }
        map = iterationMap;
    }
    //the rules for the cells in a switch statement
    int Rules(int walls)
    {
        int state = 0;

        switch (walls)
        {
            case 0:
                state = rules[0];
                break;
            case 1:
                state = rules[1];
                break;
            case 2:
                state = rules[2];
                break;
            case 3:
                state = rules[3];
                break;
            case 4:
                state = rules[4];
                break;
            case 5:
                state = rules[5];
                break;
            case 6:
                state = rules[6];
                break;
            case 7:
                state = rules[7];
                break;
            case 8:
                state = rules[8];
                break;
            default:
                state = 0;
                break;
        }
        return state;
    }
    //checks the surrounding walls
    int SurroundWalls(int xPos, int yPos)
    {
        int wallCount = 0;
        for (int x = xPos-1; x <= xPos+1; x++)
        {
            for (int y = yPos-1; y <= yPos +1; y++)
            {
                if (IsInsideMap(x,y))
                {
                    if (x != xPos || y != yPos)
                        wallCount += map[x, y];
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }
    //a simple holder I made bevore I figured out Vector2Int was a thing
    public struct Tile
    {
        public int xPos, yPos;
        public Tile(int x , int y)
        {
            xPos = x;
            yPos = y;
        }
    }

    //removing small areas so unesecary parts won't be added to the map
    void getSectionsLists()
    {
        RemoveSmallSections(1, minWallSize, 0);
        RemoveSmallSections(0, minRoomSize, 1);
    }

    //Loops trough the map to se if a small section is present then changes it
    void RemoveSmallSections(int targetState, int minimumSize, int changeTo)
    {
        List<List<Tile>> wallSections = GetSections(targetState);
        foreach (List<Tile> section in wallSections)
        {
            if (section.Count < minimumSize)
            {
                foreach (Tile tile in section)
                {
                    map[tile.xPos, tile.yPos] = changeTo;
                }
            }
        }
    }

    //looks for areas in the map.
    List<List<Tile>> GetSections(int tileState)
    {
        List<List<Tile>> sections = new List<List<Tile>>();
        int[,] VisitedTiles = new int[width, hight];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if(VisitedTiles[x,y] == 0 && map[x,y] == tileState)
                {
                    List<Tile> section = Setsection(x, y);
                    sections.Add(section);
                    //set the tiles to visited so they won't be added to a section again.
                    foreach(Tile tile in section)
                    {
                        VisitedTiles[tile.xPos, tile.yPos] = 1;
                    }
                }
            }
        }
        return sections;
    }

    //the searching for a section I recently improved this one with the Static DirArray class. other pathfinding is based on this
    List<Tile> Setsection(int startX, int startY)
    {
        List<Tile> section = new List<Tile>();
        int[,] VisitedTiles = new int[width, hight];
        Queue<Tile> SectionQueue = new Queue<Tile>();
        SectionQueue.Enqueue(new Tile(startX, startY));
        int TileState = map[startX, startY];
        
        VisitedTiles[startX, startY] = 1;
        Tile tile;
        while (SectionQueue.Count > 0)
        {
            tile = SectionQueue.Dequeue();
            section.Add(tile);
            for (int i = 0; i < DirArray.directions.Length; i++)
            {
                Vector2Int dir = new Vector2Int(tile.xPos,tile.yPos) + DirArray.directions[i];
                if (IsInsideMap(dir.x,dir.y) && map[dir.x, dir.y] == TileState && VisitedTiles[dir.x, dir.y] == 0)
                {
                    VisitedTiles[dir.x, dir.y] = 1;
                    SectionQueue.Enqueue(new Tile(dir.x, dir.y));
                }
            }
        }
            return section;
    }

    //making a niew int[,] for a section with and added boarder of walls
    List<int[,]> ConvertToHallways()
    {
        List<int[,]> hallways = new List<int[,]>();
        List<List<Tile>> sectionList = GetSections(0);
        foreach (List<Tile> section in sectionList)
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;
            for (int i = 0; i < section.Count; i++)
            {
                if (section[i].xPos < minX)
                    minX = section[i].xPos;
                if (section[i].xPos > maxX)
                    maxX = section[i].xPos;
                if (section[i].yPos < minY)
                    minY = section[i].yPos;
                if (section[i].yPos > maxY)
                    maxY = section[i].yPos;
            }

            int hallWidth = 2 + (maxX - (minX - 1));
            int hallHight = 2 + (maxY - (minY - 1));
            int[,] hallMap = new int[hallWidth, hallHight];
            for (int x = 0; x < hallWidth; x++)
            {
                for (int y = 0; y < hallHight; y++)
                {
                    hallMap[x, y] = 1;
                }
            }

            int xOffset = 1 - minX;
            int yOffset = 1 - minY;
            int xPlacement, yPlacement;
            for (int i = 0; i < section.Count; i++)
            {
                xPlacement = section[i].xPos + xOffset;
                yPlacement = section[i].yPos + yOffset;
                hallMap[xPlacement, yPlacement] = 0;
            }
            hallways.Add(hallMap);
        }
        return hallways;
    }
    //a simple check and the name says all
    bool IsInsideMap(int x,int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < hight);
    }

    //since i used 1 = wall and 0 = empty space and later i used 0 as wall and 1 and up for avalible spaces I wanted to invert the map.
    //i could actually just turn it around for all function but this was easier and saved time
    int[,] InvertMap(int[,] _map)
    {
        int[,] newMap = new int[_map.GetLength(0), _map.GetLength(1)];
        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                newMap[x, y] = (_map[x, y] == 0) ? 1 : 0;
            }
        }
        return newMap;
    }
}
