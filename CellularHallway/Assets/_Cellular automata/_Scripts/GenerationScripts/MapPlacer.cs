using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlacer : MonoBehaviour
{
    public MapGenerator mapGen;

    [Header("Grid")]
    [Range(2, 5)]
    public int xRange = 2;
    [Range(2, 5)]
    public int yRange = 2;

    [Header("Generation")]
    public string Seed;
    public bool randomSeed;

    [Header("Iterations")]
    [Range(1, 10)]
    public int maxIterations = 1;
    [Range(1, 3)]
    public int startIteration = 1;

    int totalSections;
    List<int[,]> maps;
    List<DungeonPart> dungeonParts;
    System.Random rng;
    DungeonBit[] Dbits;

    int[,] combinedMap;

    struct layout
    {
        public int[,] map1;
        public int[,] map2;
        public Vector2Int point1;
        public Vector2Int point2;
        public Vector2Int offsetFor2;
        bool left;
        public layout(int[,] _map1,int[,] _map2,Vector2Int _point1, Vector2Int _point2, Vector2Int _offsetFor2, bool goesLeft)
        {
            map1 = _map1;
            map2 = _map2;
            point1 = _point1;
            point2 = _point2;
            offsetFor2 = _offsetFor2;
            left = goesLeft;
        }
    }

    struct MappingOrder
    {
        public readonly int ID1, ID2;
        public readonly LRTB directionFrom1;
        public MappingOrder(int id1,int id2,LRTB direction1)
        {
            ID1 = id1;
            ID2 = id2;
            directionFrom1 = direction1;
        }
    }

    int xMax = int.MinValue, yMax = int.MinValue;
    public void Start()
    {

        MakeMap();
    }
    private void MakeMap()
    {
        SetDungeonParts();
        PlaceMap();
    }

    void PlaceMap()
    {
        int Currenpart = 0;
        DungeonPart roomcenter, roomRight, roomTop;      //ConnectToPoint()
        Vector2Int[,] combinedoffsets = new Vector2Int[xRange, yRange];
        combinedoffsets[0, 0] = Vector2Int.zero;
        for (int y = 0; y < yRange; y++)
        {
            Vector2Int offset = Vector2Int.zero;
            for (int x = 0; x < xRange; x++)
            {
                roomcenter = dungeonParts[Currenpart];
                if (x != xRange - 1)
                {
                    roomRight = dungeonParts[Currenpart + 1];
                    combinedoffsets[x + 1, y] += ReturnOverlap(roomcenter, roomRight, LRTB.LEFT);
                }

                if (y != yRange - 1)
                {
                    roomTop = dungeonParts[Currenpart + xRange];
                    combinedoffsets[x , y + 1] += ReturnOverlap(roomcenter, roomTop, LRTB.TOP);
                }
                Currenpart++;
            }
        }
        combinedMap = new int[(xRange+2 )* xMax + 2, (2+yRange) * yMax + 2];
        int index = 0;
        combinedoffsets = CombineOffset(combinedoffsets);
        int xMinOffset = int.MaxValue, yMinOffset = int.MaxValue;
        for (int x = 0; x < combinedoffsets.GetLength(0); x++)
        {
            for (int y = 0; y < combinedoffsets.GetLength(1); y++)
            {
                if (combinedoffsets[x, y].x < xMinOffset) xMinOffset = combinedoffsets[x, y].x;
                if (combinedoffsets[x, y].y < yMinOffset) yMinOffset = combinedoffsets[x, y].y;
            }
        }
        xMinOffset = Mathf.Abs(xMinOffset);
        yMinOffset = Mathf.Abs(yMinOffset);
        for (int y = 0; y < yRange; y++)
        {
            for (int x = 0; x < xRange; x++)
            {
                Debug.Log("inbounce, x:" + x + " Y:" + y);
                if (x == 3 && y == 0)
                {

                }
                Vector2Int tempOffset = combinedoffsets[x, y];
                tempOffset.x += xMinOffset + 1; tempOffset.y += yMinOffset+1;
                Debug.Log("Index: " + index);
                combinedMap = PartPlacer(combinedMap, dungeonParts[index].tileMap, tempOffset);
                index++;
            }
        }
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(combinedMap,0,0);//*/
    }

    Vector2Int[,] CombineOffset(Vector2Int[,] offset)
    {
        Vector2Int[,] newOffset = new Vector2Int[offset.GetLength(1), offset.GetLength(1)];

        for (int y = 1; y < offset.GetLength(1); y++)
        {
            newOffset[0, y] = offset[0, y] + offset[0, y - 1];
        }
        for (int y = 0; y < offset.GetLength(1); y++)
        {
            Vector2Int tempOffset = Vector2Int.zero;
            for (int x = 0; x < offset.GetLength(0); x++)
            {
                newOffset[x, y] = offset[x, y] + tempOffset;
                tempOffset += offset[x, y];
            }
        }

        return newOffset;
    }

    int[,] PartPlacer(int[,] Map,int[,] part, Vector2Int offset)
    {
        for (int x = 0; x < part.GetLength(0); x++)
        {
            for (int y = 0; y < part.GetLength(1); y++)
            {
                if(part[x,y] > 0 && x + offset.x < Map.GetLength(0) && y+offset.y < Map.GetLength(1))
                {
                    Map[x + offset.x, y + offset.y] = 1;
                }
            }
        }
        return Map;
    }

    Vector2Int ReturnOverlap(DungeonPart part1,DungeonPart part2, LRTB axis)
    {
        DungeonPart.cPoint center, Other;
        center = ConnectToPoint(axis, part1);
        Other = ConnectToPoint(GetOpisiteAxis(axis), part2);
        Vector2Int point1, point2;
        int[,] room1 = part1.tileMap;
        int[,] room2 = part2.tileMap;
        point1 = center.point;
        point2 = Other.point;
        if(axis == LRTB.LEFT)
            return OverlapLeft(room1, point1, room2, point2);
        else
            return SlotInFromTopRight(room1, point1, room2, point2);
    }


    void GetDirForLayout(DungeonPart.Dir dir, LRTB axis)
    {
        switch (dir)
        {
            case DungeonPart.Dir.LB:
                if (axis == LRTB.LEFT) { }
                if (axis == LRTB.BOTTOM) { }
                break;
            case DungeonPart.Dir.RB:
                if (axis == LRTB.RIGHT) { }
                if (axis == LRTB.BOTTOM) { }
                break;
            case DungeonPart.Dir.LT:
                if (axis == LRTB.LEFT) { }
                if (axis == LRTB.TOP) { }
                break;
            case DungeonPart.Dir.RT:
                if (axis == LRTB.RIGHT) { }
                if (axis == LRTB.TOP) { }
                break;
        }
    }

    DungeonPart.Dir[] GetAxisDirs(LRTB axis)
    {
        DungeonPart.Dir[] dirs = new DungeonPart.Dir[2];
        if (axis == LRTB.BOTTOM) { dirs[0] = DungeonPart.Dir.LB; dirs[1] = DungeonPart.Dir.RB; }
        if (axis == LRTB.TOP) { dirs[0] = DungeonPart.Dir.LT; dirs[1] = DungeonPart.Dir.RT; }
        if (axis == LRTB.LEFT) { dirs[0] = DungeonPart.Dir.LB; dirs[1] = DungeonPart.Dir.LT; }
        if (axis == LRTB.RIGHT) { dirs[0] = DungeonPart.Dir.RB; dirs[1] = DungeonPart.Dir.RT; }
        return dirs;
    }
    LRTB GetOpisiteAxis(LRTB axis)
    {
        LRTB newAxis;
        switch (axis)
        {
            case LRTB.LEFT:
                newAxis = LRTB.RIGHT;
                break;
            case LRTB.RIGHT:
                newAxis = LRTB.LEFT;
                break;
            case LRTB.TOP:
                newAxis = LRTB.BOTTOM;
                break;
            default:
                newAxis = LRTB.TOP;
                break;
        }
        return newAxis;
    }

    void SetDungeonParts()
    {

        totalSections = xRange * yRange;
        maps = new List<int[,]>();
        if (randomSeed == true)
            Seed = Random.Range(float.MinValue, float.MaxValue).ToString();
        rng = new System.Random(Seed.GetHashCode());
        int iterate = startIteration;
        while (maps.Count <= totalSections)
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
        dungeonParts = new List<DungeonPart>();
        Dbits = new DungeonBit[maps.Count];

        for (int i = 0; i < maps.Count; i++)
        {
            dungeonParts.Add(new DungeonPart(maps[i]));
            Dbits[i] = new DungeonBit(i);
        }
    }

    DungeonPart.cPoint ConnectToPoint(LRTB axis, DungeonPart part)
    {
        List<DungeonPart.cPoint> points = new List<DungeonPart.cPoint>();
        DungeonPart.Dir[] dirs = GetAxisDirs(axis);
        for (int l = 0; l < part.cPoints.Count; l++)
        {
            if (dirs[0] == part.cPoints[l].dir || dirs[1] == part.cPoints[l].dir)
            {
                    points.Add(part.cPoints[l]);
            }
        }
        if (points.Count != 0 )
            return points[rng.Next(0, points.Count - 1)];
        else return new DungeonPart.cPoint(Vector2Int.zero,dirs[0]);
    }//*/

    int[,] SetPartToMap(int[,] mapToSetTo, int[,] PartToSet, int universalOffset = 0)
    {
        for (int x = 0; x < PartToSet.GetLength(0); x++)
        {
            for (int y = 0; y < PartToSet.GetLength(1); y++)
            {
                if (PartToSet[x, y] > 0)
                {
                    mapToSetTo[x + universalOffset, y + universalOffset] = 1;
                }
            }
        }
        return mapToSetTo;
    }
    int[,] SetPartToMap(int[,] mapToSetTo, List<Vector2Int> PartToSet, Vector2Int offset)
    {
        for (int i = 0; i < PartToSet.Count; i++)
        {
            mapToSetTo[PartToSet[i].x + offset.x, PartToSet[i].y + offset.y] = 1;
        }
        return mapToSetTo;
    }
    List<Vector2Int> ConvertPartToList(List<Vector2Int> mapList, int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] > 0)
                    mapList.Add(new Vector2Int(x, y));
            }
        }
        return mapList;
    }
    bool PartsOverlap(int[,] temMap, List<Vector2Int> map2List, Vector2Int offset)
    {
        bool isOverlapping = false;
        for (int i = 0; i < map2List.Count; i++)
        {
            if (temMap[map2List[i].x + offset.x, map2List[i].y + offset.y] != 0)
            {
                isOverlapping = true;
            }
        }
        return isOverlapping;
    }


    Vector2Int OverlapLeft(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int xDif = point1.x - point2.x;
        int yDif = point1.y - point2.y;
        int UO = 50;

        int[,] temMap = new int[120, 120];
        temMap = SetPartToMap(temMap, map1, UO);

        List<Vector2Int> map2List = new List<Vector2Int>();
        map2List = ConvertPartToList(map2List, map2);

        bool isOverlapping = false;
        Vector2Int offset = new Vector2Int(xDif + UO, yDif + UO);

        while (isOverlapping == false)
        {
            if (PartsOverlap(temMap, map2List, offset) == false)
                offset.x++;
            else
                isOverlapping = true;
        }
        
        temMap = SetPartToMap(temMap, map2List, offset);

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + offset.x, point2.y + offset.y] = 2;
        offset.x -= UO;
        offset.y -= UO;
        return offset;
    }
    Vector2Int OverlapTop(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = map1.GetLength(1);
        int xDif = point1.x - point2.x;
        int UO = 50;

        int[,] temMap = new int[100, 100];
        temMap = SetPartToMap(temMap, map1, UO);

        List<Vector2Int> map2List = new List<Vector2Int>();
        map2List = ConvertPartToList(map2List, map2);

        bool isOverlapping = false;
        Vector2Int offset = new Vector2Int(xDif + UO, Offset + UO);
        while (isOverlapping == false)
        {
            if (PartsOverlap(temMap, map2List, offset) == false)
                offset.y--;
            else
                isOverlapping = true;
        }
        offset.y++;
        temMap = SetPartToMap(temMap, map2List, offset);

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + offset.x, point2.y + offset.y] = 2;
        offset.x -= UO;
        offset.y -= UO;
        return offset;
    }

    Vector2Int SlotInFromTopRight(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        Vector2Int[] minMax1 = GetMinMax(map1);
        Vector2Int[] minMax2 = GetMinMax(map2);
        int xmax = (minMax1[1].x > minMax2[1].x) ? minMax1[1].x : minMax2[1].x;
        int ymax = (minMax1[1].y > minMax2[1].y) ? minMax1[1].y : minMax2[1].y;
        int[,] tempMap = new int[xmax * 4, ymax * 4];

        for (int x = 0; x < map1.GetLength(0); x++)
        {
            for (int y = 0; y < map1.GetLength(1); y++)
            {
                if (map1[x, y] > 0)
                    tempMap[x, y] = 1;
            }
        }
        List<Vector2Int> map2Tiles = new List<Vector2Int>();
        for (int x = 0; x < map2.GetLength(0); x++)
        {
            for (int y = 0; y < map2.GetLength(1); y++)
            {
                if (map2[x, y] > 0)
                    map2Tiles.Add(new Vector2Int(x, y));
            }
        }

        Vector2Int offset = new Vector2Int( minMax1[1].x + minMax2[0].x , minMax1[1].y + minMax2[0].y);
        bool isOverlapping = false;
        while (isOverlapping == false)
        {
            for (int i = 0; i < map2Tiles.Count; i++)
            {
                if (tempMap[map2Tiles[i].x + offset.x, map2Tiles[i].y + offset.y] != 0)
                {
                    isOverlapping = true;
                }
            }
            offset.x--; offset.y--;
        }
        offset.x++; offset.y++;

        bool overlapX = false;
        while (overlapX == false)
        {
            for (int i = 0; i < map2Tiles.Count; i++)
            {
                if (tempMap[map2Tiles[i].x + offset.x, map2Tiles[i].y + offset.y] != 0)
                {
                    overlapX = true;
                }
            }
            offset.x--;
        }
        offset.x++;

        bool overlapY = false;
        while (overlapY == false)
        {
            for (int i = 0; i < map2Tiles.Count; i++)
            {
                if (tempMap[map2Tiles[i].x + offset.x, map2Tiles[i].y + offset.y] != 0)
                {
                    overlapY = true;
                }
            }
            offset.y--;
        }
        offset.y++;
        return offset;

    }//*/

    Vector2Int[] GetMinMax(int[,] _map)
    {
        Vector2Int[] MinMax = { new Vector2Int(int.MaxValue, int.MaxValue), new Vector2Int(int.MinValue, int.MinValue) };

        for (int x = 0; x < _map.GetLength(0); x++)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                if(_map[x,y] > 0)
                {
                    if (x < MinMax[0].x) MinMax[0].x = x;
                    if (y < MinMax[0].y) MinMax[0].y = y;
                    if (x > MinMax[1].x) MinMax[1].x = x;
                    if (y > MinMax[1].y) MinMax[1].y = y;
                }
            }
        }

        return MinMax;
    }


    struct DungeonBit
    {
        public readonly int ID;
        public bool left, right, top, bottom;
        public DungeonBit(int _ID)
        {
            ID = _ID;
            left = true;
            right = true;
            top = true;
            bottom = true;
        }
        public void SetToUsed(LRTB axis)
        {
            switch (axis)
            {
                case LRTB.LEFT:
                    left = false;
                    break;
                case LRTB.RIGHT:
                    right = false;
                    break;
                case LRTB.TOP:
                    top = false;
                    break;
                case LRTB.BOTTOM:
                    bottom = false;
                    break;
            }
        }
        public bool IsAvalible(LRTB axis)
        {
            switch (axis)
            {
                case LRTB.LEFT:
                    return left;
                case LRTB.RIGHT:
                    return right;
                case LRTB.TOP:
                    return top;
                default:
                    return bottom;
            }
        }
    }
   
    enum LRTB
    {
        LEFT,RIGHT,TOP,BOTTOM
    }
}
