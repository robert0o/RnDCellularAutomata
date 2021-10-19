using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlacer : MonoBehaviour
{
    public MapGenerator mapGen;
    ConnectionPoinSearch point;

    [Header("Grid")]
    [Range(1, 4)]
    public int xRange = 1;
    [Range(1, 4)]
    public int yRange = 1;

    [Header("Generation")]
    public string Seed;
    public bool randomSeed;

    [Header("Iterations")]
    [Range(1,10)]
    public int maxIterations = 1;
    [Range(1,3)]
    public int startIteration = 1;

    int totalSections;
    List<int[,]> maps;
    DungeonPart[] dungeonParts;

    private void Start()
    {
        totalSections = xRange * yRange;
        maps = new List<int[,]>();
        int xMax = int.MinValue, yMax = int.MinValue;
        if (randomSeed == true)
            Seed = Random.Range(float.MinValue, float.MaxValue).ToString();

        int iterate = startIteration;
        while(maps.Count <= totalSections)
        {
            List<int[,]> _maps = mapGen.GetMaps(Seed, iterate);
            for (int j = 0; j < _maps.Count; j++)
            {
                if (_maps[j].GetLength(0) > xMax)
                    xMax = _maps[j].GetLength(0);
                if (_maps[j].GetLength(1) > yMax)
                    yMax = _maps[j].GetLength(1);
                maps.Add(_maps[j]);
            }
            iterate++;
        }

        dungeonParts = new DungeonPart[maps.Count];
        for (int i = 0; i < maps.Count; i++)
        {
            dungeonParts[i] = new DungeonPart(maps[i]);
        }

        int[,] dungeonMap = new int[xMax * xRange, yMax * yRange];
        int atPart = 0;
        for (int i = 0; i < xRange && atPart < dungeonParts.Length; i++)
        {
            for (int j = 0; j < yRange && atPart < dungeonParts.Length; j++, atPart++)
            {
                int xOffset = i * xMax, yOffset = j * yMax;
                for (int x = 0; x < dungeonParts[atPart].tileMap.GetLength(0); x++)
                {
                    for (int y = 0; y < dungeonParts[atPart].tileMap.GetLength(1); y++)
                    {
                        dungeonMap[x + xOffset, y + yOffset] = dungeonParts[atPart].tileMap[x, y];
                    }
                }

            }
        }
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(dungeonMap,0,0);
    }

    DungeonPart.Dir GetDir(int i,int j, DungeonPart part)
    {
        DungeonPart.Dir dir;
        DungeonPart.Dir[,] directionValue = new DungeonPart.Dir[2, 2];
        directionValue[0, 0] = DungeonPart.Dir.LB;
        directionValue[0, 1] = DungeonPart.Dir.LT;
        directionValue[1, 0] = DungeonPart.Dir.RB;
        directionValue[1, 1] = DungeonPart.Dir.RT;
        int x, y;
        if (i < xRange / 2)
            x = 0;
        else
            x = 1;

        if (j < yRange / 2)
            y = 0;
        else
            y = 1;

        dir = directionValue[x, y];
        return dir;
    }
}
