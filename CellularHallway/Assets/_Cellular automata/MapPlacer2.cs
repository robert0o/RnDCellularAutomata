using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlacer2 : MonoBehaviour
{
    MapGenerator mapGen;
    int[,] levelMap;
    [Range(3, 10)]
    public int xRange =3, yRange=3;
    public string Seed;
    public bool randomSeed;
    System.Random rng;

    int totalSections;
    List<int[,]> maps;
    int xMax, yMax;
    int[,] totalMap;
    int[,] mapsVisited;
    int[,] layout;

    Queue<Vector2Int[]> partQueue;

    struct MinMax
    {
        public Vector2Int Max;
        public Vector2Int Min;
        public MinMax(int xMin, int yMin, int xMax, int yMax)
        {
            Max = new Vector2Int(xMax, yMax);
            Min = new Vector2Int(xMin, yMin);
        }

    }
    public void Start()
    {
        if (randomSeed == true) Seed = Random.Range(float.MinValue, float.MaxValue).ToString();
        rng = new System.Random(Seed.GetHashCode());
        GetMapGenData();
        StructureMap();
        totalMap = TrimMap(totalMap);
        FindObjectOfType<CellPlacer>().PlaceMapCells(totalMap, 0, 0);
    }

    void GetMapGenData()
    {
        maps = new List<int[,]>();
        xMax = int.MinValue; 
        yMax = int.MinValue;
        totalSections = xRange * yRange;
        mapGen = FindObjectOfType<MapGenerator>();
        int iterate = 3;
        while (maps.Count <= totalSections)
        {
            List<int[,]> _maps = mapGen.GetMaps(Seed, iterate);
            for (int j = 0; j < _maps.Count; j++)
            {
                if (_maps[j].GetLength(0) > xMax) xMax = _maps[j].GetLength(0);
                if (_maps[j].GetLength(1) > yMax) yMax = _maps[j].GetLength(1);
                maps.Add(_maps[j]);
            }
            iterate++;
        }
    }

    void StructureMap()
    {
        totalMap = new int[xMax * (xRange + 1), yMax * (yRange + 1)];
        Vector2Int baseOffset = new Vector2Int(
            Mathf.CeilToInt(totalMap.GetLength(0) * .5f - maps[0].GetLength(0) * .5f),
            Mathf.CeilToInt(totalMap.GetLength(1) * .5f - maps[0].GetLength(0) * .5f));

        mapsVisited = new int[xRange, yRange];

        layout = new int[xRange, yRange];
        List<int> parts = new List<int>();
        for (int i = 0; i < maps.Count; i++) {parts.Add(i);}
        for (int x = 0; x < xRange; x++){
            for (int y = 0; y < yRange; y++){
                int partNr = rng.Next(0, parts.Count);
                layout[x, y] = parts[partNr];
                parts.Remove(partNr);
            }
        }

        partQueue = new Queue<Vector2Int[]>();

        //int midX = Mathf.CeilToInt((xRange - 1) * 0.5f), midY = Mathf.CeilToInt((yRange - 1) * 0.5f);
        int midX = rng.Next(0,xRange-1), midY = Mathf.CeilToInt(rng.Next(0, yRange - 1));
        Vector2Int layoutPosition = new Vector2Int(midX, midY);
        mapsVisited[layoutPosition.x, layoutPosition.y] = 1;

        int firstPart = layout[layoutPosition.x, layoutPosition.y];
        SetPartToMap(maps[firstPart],baseOffset);
        Vector2Int[] qVector = { layoutPosition, baseOffset };
        partQueue.Enqueue(qVector);
        while (partQueue.Count > 0)
        {
            Vector2Int[] inQVector = partQueue.Dequeue();
            SubStructureMap(inQVector[0], inQVector[1]);
        }
    }

    void SubStructureMap(Vector2Int position,Vector2Int offset)
    {
        int currentPart = layout[position.x, position.y];
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
        for (int i = 0; i < 4; i++)
        {
            Vector2Int newPos = position + directions[i];
            Vector2Int newOffset = offset;

            if (newPos.x >= layout.GetLength(0) || newPos.x < 0 || newPos.y >= layout.GetLength(1) || newPos.y < 0) continue;
            if (mapsVisited[newPos.x, newPos.y] == 1) continue;

            int nextPart = layout[newPos.x, newPos.y];
            newOffset = newOffset + GetOverlapOffset(maps[currentPart], maps[nextPart], directions[i]);
            mapsVisited[newPos.x, newPos.y] = 1;
            SetPartToMap(maps[nextPart], newOffset);
            Vector2Int[] qVector = { newPos, newOffset };
            partQueue.Enqueue(qVector);
        }
    }

    void SetPartToMap(int[,] part, Vector2Int offset)
    {
        for (int x = 0; x < part.GetLength(0); x++){
            for (int y = 0; y < part.GetLength(1); y++){
                if (part[x, y] <= 0) continue;
                totalMap[x + offset.x, y + offset.y] = 1/*part[x, y]*/;
            }
        }
    }
    
    Vector2Int GetOverlapOffset(int[,] baseMap, int[,] overlappingMap, Vector2Int dir)
    {
        int[,] tempmap = new int[xMax * 3, yMax * 3];
        Vector2Int offset = new Vector2Int(xMax, yMax);
        for (int x = 0; x < baseMap.GetLength(0); x++)
        {
            for (int y = 0; y < baseMap.GetLength(1); y++)
            {
                if (baseMap[x, y] > 0)
                    tempmap[x + offset.x, y + offset.y] = baseMap[x, y];
            }
        }
        List<Vector3Int> OverlapList = new List<Vector3Int>();
        for (int x = 0; x < overlappingMap.GetLength(0); x++)
        {
            for (int y = 0; y < overlappingMap.GetLength(1); y++)
            {
                if (overlappingMap[x, y] > 0)
                    OverlapList.Add(new Vector3Int(x, y, overlappingMap[x, y]));
            }
        }

        int xMid1 = Mathf.CeilToInt(baseMap.GetLength(0) * .5f);
        int yMid1 = Mathf.CeilToInt(baseMap.GetLength(1) * .5f);
        int xMid2 = Mathf.CeilToInt(overlappingMap.GetLength(0) * .5f);
        int yMid2 = Mathf.CeilToInt(overlappingMap.GetLength(1) * .5f);
        offset += new Vector2Int(xMid1 - xMid2, yMid1 - yMid2);

        bool isOverlapping = true;
        
        Vector2Int overlapPos;
        while (isOverlapping == true)
        {
            isOverlapping = false;
            for (int i = 0; i < OverlapList.Count; i++)
            {
                overlapPos = new Vector2Int(OverlapList[i].x + offset.x, OverlapList[i].y + offset.y);
                if (tempmap[overlapPos.x,overlapPos.y] > 0)
                {
                    isOverlapping = true;
                }
            }
            offset.x += dir.x;
            offset.y += dir.y;
        }
        offset.x -= dir.x*2;
        offset.y -= dir.y*2;//*/
        for (int i = 0; i < OverlapList.Count; i++)
        {
            tempmap[OverlapList[i].x + offset.x, OverlapList[i].y+ offset.y] = OverlapList[i].z;
        }
        offset.x -= xMax;
        offset.y -= yMax;
        //FindObjectOfType<CellPlacer>().PlaceMapCells(tempmap, 0, 0);
        return offset;
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
    enum Axis {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        LEFTUP,
        RIGHTUP,
        LEFTDOWN,
        RIGHTDOWN
    }
    Vector2Int GetAxisVector(int axis)
    {
        switch (axis)
        {
            case 0:
                return Vector2Int.left;
            case 1:
                return Vector2Int.right;
            case 2:
                return Vector2Int.up;
            case 3:
                return Vector2Int.down;
            case 4:
                return (Vector2Int.left + Vector2Int.up);
            case 5:
                return (Vector2Int.right + Vector2Int.up);
            case 6:
                return (Vector2Int.left + Vector2Int.down);
            default:
                return (Vector2Int.right + Vector2Int.down);
        }
    }
}