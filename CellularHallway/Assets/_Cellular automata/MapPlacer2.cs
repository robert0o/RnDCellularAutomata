using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlacer2 : MonoBehaviour
{
    MapGenerator mapGen;
    int[,] levelMap;
    [Range(2,10)]
    public int xRange,yRange;

    struct MinMax
    {
        public Vector2Int Max;
        public Vector2Int Min;
        public MinMax(int xMin,int yMin,int xMax, int yMax)
        {
            Max = new Vector2Int(xMax, yMax);
            Min = new Vector2Int(xMin, yMin);
        }

    }
    public void Start()
    {
        mapGen = FindObjectOfType<MapGenerator>();

        //FindObjectOfType<CellPlacer>().PlaceMapCells(levelMap, 0, 0);
    }

    void CombineMaps(List<int[,]> mapList)
    {
        
    }

    
    int[,] TrimMap(int[,] map)
    {
        MinMax mm = GetMinMax(map);
        int xLength = mm.Max.x - mm.Min.x, yLength = mm.Max.y - mm.Min.y;

        int[,] trimmedMap = new int[xLength, yLength];
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (map[mm.Min.x + x, mm.Min.y + y] > 0)
                    trimmedMap[x, y] = 1;
            }
        }
        return trimmedMap;
    }
    MinMax GetMinMax(int[,] map)
    {
        int xMin = int.MaxValue, yMin = int.MaxValue;
        int xMax = int.MinValue, yMax = int.MinValue;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] > 0)
                {
                    if (x > xMax) xMax = x;
                    if (x < xMin) xMin = x;
                    if (y > yMax) yMax = y;
                    if (y < yMin) yMin = y;
                }
            }
        }
        return new MinMax(xMin, yMin, xMax, yMax);
    }

    MinMax GetMinMax(List<int[,]> maps)
    {
        int xMin = int.MaxValue, xMax = int.MinValue, yMin = int.MaxValue, yMax = int.MinValue;
        for (int i = 0; i < maps.Count; i++)
        {
            MinMax mm = GetMinMax(maps[i]);
            if (mm.Max.x > xMax) xMax = mm.Max.x;
            if (mm.Min.x < xMin) xMin = mm.Min.x;
            if (mm.Max.y > yMax) yMax = mm.Max.y;
            if (mm.Min.y < yMin) yMin = mm.Min.y;
        }
        return new MinMax(xMin, yMin, xMax, yMax);
    }

}

