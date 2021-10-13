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

    public void Update()
    {
        hallways = gethallways();

        List<int[,]> maps = new List<int[,]>();
        ConnectionPoinSearch point = new ConnectionPoinSearch();

        for (int i = 0; i < hallways.Count; i++)
        {
            hallways[i] = InvertMap(hallways[i]);
            hallways[i] = point.findEnds(hallways[i]);
            maps.Add(hallways[i]);
        }
        
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceCells(maps, hight);
    
    }

    public List<int[,]> gethallways()
    {
        GenMap();
        getSectionsLists();
        hallways = ConvertToHallways();
        return hallways;
    }
    private void GenMap()
    {
        map = new int[width, hight];
        GenerateMap();
        for (int x = 0; x < iterations; x++)
        {
            Iterate();
        }
    }

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

    public struct Tile
    {
        public int xPos, yPos;
        public Tile(int x , int y)
        {
            xPos = x;
            yPos = y;
        }
    }

    void getSectionsLists()
    {
        RemoveSmallSections(1, minWallSize, 0);
        RemoveSmallSections(0, minRoomSize, 1);
    }

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
                    foreach(Tile tile in section)
                    {
                        VisitedTiles[tile.xPos, tile.yPos] = 1;
                    }
                }
            }
        }
        return sections;
    }

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
            if (IsInsideMap(tile.xPos - 1, tile.yPos) && map[tile.xPos - 1, tile.yPos] == TileState && VisitedTiles[tile.xPos - 1, tile.yPos] == 0)
            {
                VisitedTiles[tile.xPos - 1, tile.yPos] = 1;
                SectionQueue.Enqueue(new Tile(tile.xPos - 1, tile.yPos));
            }
            if (IsInsideMap(tile.xPos + 1, tile.yPos) && map[tile.xPos + 1, tile.yPos] == TileState && VisitedTiles[tile.xPos + 1, tile.yPos] == 0)
            {
                VisitedTiles[tile.xPos + 1, tile.yPos] = 1;
                SectionQueue.Enqueue(new Tile(tile.xPos + 1, tile.yPos));
            }
            if (IsInsideMap(tile.xPos, tile.yPos - 1) && map[tile.xPos, tile.yPos - 1] == TileState && VisitedTiles[tile.xPos, tile.yPos - 1] == 0)
            {
                VisitedTiles[tile.xPos, tile.yPos - 1] = 1;
                SectionQueue.Enqueue(new Tile(tile.xPos, tile.yPos - 1));
            }
            if (IsInsideMap(tile.xPos, tile.yPos + 1) && map[tile.xPos, tile.yPos + 1] == TileState && VisitedTiles[tile.xPos, tile.yPos + 1] == 0)
            {
                VisitedTiles[tile.xPos, tile.yPos + 1] = 1;
                SectionQueue.Enqueue(new Tile(tile.xPos, tile.yPos + 1));
            }
        }
            return section;
    }

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

    bool IsInsideMap(int x,int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < hight);
    }

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

    private void OnDrawGizmos()
    {
        if (map == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                switch (map[x, y])
                {
                    case 1:
                        Gizmos.color = Color.black;
                        break;
                    case 0:
                        Gizmos.color = Color.white;
                        break;
                    case 2:
                        Gizmos.color = Color.red;
                        break;
                    default:
                        Gizmos.color = Color.yellow;
                        break;
                }
                Vector3 pos = new Vector3(x, y, 0);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }

    }//*/
}
