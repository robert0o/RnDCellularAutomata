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
    [Header("Cel Rules")]
    [Range(0, 1)]
    public int rule0,rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8;

    private void Update()
    {
        GenMap();

        getSectionsLists();
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
        RemoveSmallSections(1, 3, 0);
        RemoveSmallSections(0, 3, 2);
    }

    void RemoveSmallSections(int targetState, int minimumSize, int changeTo)
    {
        List<List<Tile>> wallSections = GetSections(targetState);
        foreach (List<Tile> section in wallSections)
        {
            if (section.Count <= minimumSize)
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
            /*for (int x = startX-1 ; x <= startX + 1; x++)
            {
                for (int y = startY - 1; y <= startY + 1; y++)
                {
                    if (IsInsideMap(x,y) && (x == tile.xPos || y == tile.yPos))
                    {
                        if(VisitedTiles[x,y] == 0 && map[x,y] == TileState)
                        {
                            VisitedTiles[x, y] = 1;
                            SectionQueue.Enqueue(new Tile(x, y));
                        }
                    }
                }
            }*/
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
    

    bool IsInsideMap(int x,int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < hight);
    }
}
