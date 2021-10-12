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
    [Header("Cel Rules")]
    [Range(0, 1)]
    public int rule0,rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8;
    List<Hallway> hallways;

    public void Update()
    {
        List<Hallway> _hallways = gethallways();
        if (useTileMap == false)
        {
            CellPlacer cells = FindObjectOfType<CellPlacer>();
            cells.PlaceCells(_hallways);
        }
        else
        {
            ConnectionPoinSearch poin = new ConnectionPoinSearch();
            foreach (Hallway hallway in hallways)
            {
                poin.FindPaths(hallway);
                poin.SetFurthestPooints(hallway);
            }//*/
        }
    }

    public List<Hallway> gethallways()
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
                iterationMap[x, y] = rules(SurroundWalls(x, y));
            }
        }
        map = iterationMap;
    }
    
    int rules(int walls)
    {
        int state = 0;

        switch (walls)
        {
            case 0:
                state = rule0;
                break;
            case 1:
                state = rule1;
                break;
            case 2:
                state = rule2;
                break;
            case 3:
                state = rule3;
                break;
            case 4:
                state = rule4;
                break;
            case 5:
                state = rule5;
                break;
            case 6:
                state = rule6;
                break;
            case 7:
                state = rule7;
                break;
            case 8:
                state = rule8;
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
                Vector3 pos = new Vector3(x, y,0);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
        if (useTileMap == true)
        {
            int offset = hight + 4;
            int exOffset = 0;
            int previousTotalOffset = 0;
            foreach (Hallway hallway in hallways)
            {
                for (int x = 0; x < hallway.hWidth; x++)
                {
                    for (int y = 0; y < hallway.hHight; y++)
                    {
                        switch (hallway.hallway[x, y])
                        {
                            case 0:
                                Gizmos.color = Color.white;
                                break;
                            case 2:
                                Gizmos.color = Color.red;
                                break;
                            default:
                                Gizmos.color = Color.black;
                                break;
                        }
                        Vector3 pos = new Vector3(x + exOffset, y + offset, 0);
                        Gizmos.DrawCube(pos, Vector3.one);
                    }
                }
                exOffset = hallway.hWidth + previousTotalOffset + 4;
                previousTotalOffset = exOffset;
            }//*/
        }
    }//*/
    
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

    List<Hallway> ConvertToHallways()
    {
        List<Hallway> hallways = new List<Hallway>();
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
            hallways.Add(new Hallway(hallMap, hallWidth, hallHight));
        }
        return hallways;
    }

    bool IsInsideMap(int x,int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < hight);
    }
}

public class Hallway
{
    public int[,] hallway;
    public readonly int hWidth;
    public readonly int hHight;

    public Hallway(int[,] _hallway, int width, int hight)
    {
        hallway = _hallway;
        hWidth = width;
        hHight = hight;
    }
}
