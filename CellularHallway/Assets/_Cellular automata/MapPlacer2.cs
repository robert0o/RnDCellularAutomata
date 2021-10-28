using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;

public class MapPlacer2 : MonoBehaviour
{
    MapGenerator mapGen;
    ConnectionPoinSearch point;
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
    List<int> usedRooms;
    List<int> eventRooms;

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
        usedRooms = new List<int>();
        eventRooms = new List<int>();
        if (randomSeed == true) Seed = Random.Range(float.MinValue, float.MaxValue).ToString();
        rng = new System.Random(Seed.GetHashCode());

        GetMapGenData();

        StructureMap();

        totalMap = TrimMap(totalMap);

        point = new ConnectionPoinSearch();
        int[,] pathedMap = point.findEnds(totalMap);
        AddEndpoints();

        

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
            if(iterate >= 10){
                Seed = "";
                for (int i = 0; i < 10; i++){
                    char ch = (char)rng.Next(0, 255);
                    Seed += ch.ToString();
                    iterate = 3;
                }
            }
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

        CreateLayout();
        SetEventToRoom(4); //banditcampish
        SetEventToRoom(2); //startingRoom
       

        partQueue = new Queue<Vector2Int[]>();

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
            PlaceStructuredMap(inQVector[0], inQVector[1]);
        }
    }

    void CreateLayout()
    {
        layout = new int[xRange, yRange];
        usedRooms = new List<int>();
        List<int> parts = new List<int>();
        for (int i = 0; i < maps.Count; i++) { parts.Add(i); }
        for (int x = 0; x < xRange; x++)
        {
            for (int y = 0; y < yRange; y++)
            {
                int partNr = rng.Next(0, parts.Count-1);
                layout[x, y] = parts[partNr];
                parts.Remove(parts[partNr]);
                usedRooms.Add(parts[partNr]);
            }
        }
    }

    void PlaceStructuredMap(Vector2Int position,Vector2Int offset)
    {
        int currentPart = layout[position.x, position.y];
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
        int[] index = { 0, 1, 2, 3 };
        //System.Array.Sort(directions,);
        directions = directions.OrderBy(x => rng.Next()).ToArray();

        //somehow messes stuff up
        List<int> dirs = new List<int>();
        for (int i = 0; i < directions.Length; i++){
            dirs.Add(i);}
        int[] rngDir = new int[dirs.Count];
        for (int i = 0; i < rngDir.Length; i++){
            int j = rng.Next(0, dirs.Count - 1);
            rngDir[i] = j;
            dirs.Remove(dirs[j]);
        }


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
                totalMap[x + offset.x, y + offset.y] = part[x, y];
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

        int[,] trimmedMap = new int[xLength+2, yLength+2];
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (map[mm.Min.x + x, mm.Min.y + y] > 0)
                    trimmedMap[x+1, y+1] = map[mm.Min.x + x, mm.Min.y + y];
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
    void AddEndpoints()
    {
        if (point == null) return;
        List<Vector2Int> ends = point.GetEndPointsPosition();
        if (ends == null) return;
        for (int i = 0; i < ends.Count; i++)
        {
            totalMap[ends[i].x, ends[i].y] = 3;
        }
    }
    void SetEventToRoom(int eventIndex)
    {
        int eventRoom;

        int index = rng.Next(0, usedRooms.Count - 1);
        eventRoom = usedRooms[index];
        usedRooms.Remove(usedRooms[index]);
            
        eventRooms.Add(eventRoom);
        setRoomValue(eventRoom, eventIndex);
    }
    void setRoomValue(int eventRoom,int eventIndex)
    {
        for (int x = 0; x < maps[eventRoom].GetLength(0); x++)
        {
            for (int y = 0; y < maps[eventRoom].GetLength(1); y++)
            {
                if (maps[eventRoom][x, y] <= 0) continue;
                maps[eventRoom][x, y] = eventIndex;
            }
        }
    }
}