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
    Queue<MappingOrder> mappingQueue;

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
        Currenpart = 0;
        combinedoffsets = combineOffset(combinedoffsets);
        for (int y = 0; y < yRange; y++)
        {
            for (int x = 0; x < xRange; x++)
            {
                Debug.Log("inbounce, x:" + x + " Y:" + y);
                if (x == 1 && y == 0)
                {

                }
                combinedMap = PartPlacer(combinedMap, dungeonParts[Currenpart].tileMap, combinedoffsets[x, y]);
                Currenpart++;
            }
        }
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(combinedMap,0,0);//*/
    }

    Vector2Int[,] combineOffset(Vector2Int[,] offset)
    {
        Vector2Int[,] newOffset = offset;
        newOffset[0, 0] += new Vector2Int(20, 20);
        for (int y = 1; y < offset.GetLength(1); y++)
        {
            newOffset[0, y] += newOffset[0, y - 1];
        }

        for (int y = 0; y < offset.GetLength(1); y++)
        {
            for (int x = 1; x < offset.GetLength(0); x++)
            {
                newOffset[x, y] += newOffset[x - 1, y];
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
                if(part[x,y] > 0)
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
            return OverlapTop(room1, point1, room2, point2);
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

    int GetBit(DungeonPart part)
    {
        for (int i = 0; i < dungeonParts.Count; i++)
        {
            if (dungeonParts[i] == part)
                return i;
        }
        return -1;
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

    DungeonPart.cPoint SelectpointToConnect(DungeonPart.Dir dir, DungeonPart part)
    {
        List<DungeonPart.cPoint> points = new List<DungeonPart.cPoint>();
        for (int l = 0; l < part.cPoints.Count; l++)
        {
            if (part.cPoints[l].dir == dir)
            {
                points.Add(part.cPoints[l]);
            }
        }
        if (points.Count != 0)
            return points[rng.Next(0, points.Count - 1)];
        else return new DungeonPart.cPoint(Vector2Int.zero,dir);
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


    DungeonPart.Dir GetDir(int i, int j)
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
    DungeonPart.Dir[] GetConnectTo(DungeonPart.Dir _dir)
    {
        DungeonPart.Dir[] dir = new DungeonPart.Dir[2];
        switch (_dir)
        {
            case DungeonPart.Dir.LB:
                dir[0] = DungeonPart.Dir.RT;
                dir[0] = DungeonPart.Dir.RB;
                break;
            case DungeonPart.Dir.LT:
                dir[0] = DungeonPart.Dir.RT;
                dir[0] = DungeonPart.Dir.RB;
                break;
            case DungeonPart.Dir.RB:
                dir[0] = DungeonPart.Dir.LT;
                dir[0] = DungeonPart.Dir.LB;
                break;
            case DungeonPart.Dir.RT:
                dir[0] = DungeonPart.Dir.LT;
                dir[0] = DungeonPart.Dir.LB;
                break;
        }
        return dir;
    }

    int[,] TrimMap(int[,] currentMap)
    {
        int[,] trimmedMap;
        Vector2Int xyMax = new Vector2Int(int.MinValue, int.MinValue);
        Vector2Int xyMin = new Vector2Int(int.MaxValue, int.MaxValue);
        for (int x = 0; x < currentMap.GetLength(0); x++)
        {
            for (int y = 0; y < currentMap.GetLength(1); y++)
            {
                if(currentMap[x,y] > 0)
                {
                    if (x > xyMax.x)
                        xyMax.x = x;
                    if (y > xyMax.y)
                        xyMax.y = y;
                    if (x < xyMin.x)
                        xyMin.x = x;
                    if (y < xyMin.y)
                        xyMin.y = y;
                }
            }
        }

        Vector2Int mapSize = new Vector2Int(xyMin.x - xyMax.x, xyMin.y - xyMax.y);
        trimmedMap = new int[mapSize.x + 2, mapSize.y + 2];
        for (int x = xyMin.x; x <= xyMax.x; x++)
        {
            for (int y = xyMin.y; y <= xyMax.y; y++)
            {
                trimmedMap[x - xyMin.x + 1, y - xyMin.y + 1] = currentMap[x, y];
            }
        }
                return trimmedMap;
    }

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
        int Offset = map1.GetLength(0);
        int yDif = point1.y - point2.y;
        int UO = 50;

        int[,] temMap = new int[100, 100];
        temMap = SetPartToMap(temMap, map1, UO);

        List<Vector2Int> map2List = new List<Vector2Int>();
        map2List = ConvertPartToList(map2List, map2);

        bool isOverlapping = false;
        Vector2Int offset = new Vector2Int(Offset + UO, yDif + UO);

        while (isOverlapping == false)
        {
            if (PartsOverlap(temMap, map2List, offset) == false)
                offset.x--;
            else
                isOverlapping = true;
        }
        offset.x++;
        temMap = SetPartToMap(temMap, map2List, offset);

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + offset.x, point2.y + offset.y] = 2;
        offset.x -= UO;
        offset.y -= UO;
        return offset;
    }

    Vector2Int OverlapRight(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = -map1.GetLength(0);
        int yDif = point1.y - point2.y;
        int UO = 50;

        int[,] temMap = new int[100, 100];
        temMap = SetPartToMap(temMap, map1, UO);

        List<Vector2Int> map2List = new List<Vector2Int>();
        map2List = ConvertPartToList(map2List, map2);

        bool isOverlapping = false;
        Vector2Int offset = new Vector2Int(Offset + UO, yDif + UO);

        while (isOverlapping == false)
        {
            if (PartsOverlap(temMap, map2List, offset) == false)
                offset.x++;
            else
                isOverlapping = true;
        }
        offset.x--;

        temMap = SetPartToMap(temMap, map2List, offset);

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + offset.x, point2.y + offset.y] = 2;
        offset.x -= UO;
        offset.y -= UO;
        return offset;
    }
    Vector2Int OverlapBottom(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = -map1.GetLength(1);
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
                offset.y++;
            else
                isOverlapping = true;
        }
        offset.y--;
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
    /*
   void SetLayoutStructure()
   {
       if (dungeonParts.Count <= 0) return;

       List<DungeonPart> dParts = dungeonParts;

       int Connections = rng.Next(1, 4);
       int StartingRoom = rng.Next(0, dParts.Count - 1);

       DungeonPart room = dParts[StartingRoom];
       dParts.Remove(dParts[StartingRoom]);

       List<DungeonPart.Dir> PlacedDirs = new List<DungeonPart.Dir>();
       DungeonPart.Dir dir;

       if (Connections == 4)
       {
           PlacedDirs.Add(DungeonPart.Dir.LB);
           PlacedDirs.Add(DungeonPart.Dir.LT);
           PlacedDirs.Add(DungeonPart.Dir.RB);
           PlacedDirs.Add(DungeonPart.Dir.RT);
       }
       else
       {
           for (int i = 0; i < Connections; i++)
           {
               dir = (DungeonPart.Dir)rng.Next(0, 3);
               if (!PlacedDirs.Contains(dir))
               {
                   PlacedDirs.Add(dir);
               }
           }
       }

       for (int i = 0; i < PlacedDirs.Count; i++)
       {
           DungeonPart.cPoint connectPoint = SelectpointToConnect(PlacedDirs[i], room);
           if (connectPoint.point == Vector2Int.zero) continue;
           int newRoom = rng.Next(0, dParts.Count);
           DungeonPart.cPoint targetConnectPoint = ConnectToPoint(connectPoint.dir, dParts[newRoom]);
           if (targetConnectPoint.point == Vector2Int.zero) continue;
           if (DbitAvalible(connectPoint.dir, GetDBit(room)) == false || DbitAvalible(targetConnectPoint.dir, GetDBit(dParts[newRoom])) == false) continue;
           SetTargetDbitDirs(connectPoint.dir, targetConnectPoint.dir, GetDBit(room));
           SetTargetDbitDirs(targetConnectPoint.dir, connectPoint.dir, GetDBit(dParts[newRoom]));

       }

   }

   void SetTargetDbitDirs(DungeonPart.Dir dir1, DungeonPart.Dir dir2,int dbit)
   {
       if(dir1 == DungeonPart.Dir.LB)
       {
           if(dir2 == DungeonPart.Dir.RB)
           {
               //straitNextToIt
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RB);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RT);
           }
           if (dir2 == DungeonPart.Dir.RT)
           {
               //Schuin tegenaan
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RT);
           }
           if (dir2 == DungeonPart.Dir.LT)
           {
               //straitUnderIt
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LT);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RT);
           }

       }
       if (dir1 == DungeonPart.Dir.LT)
       {
           if (dir2 == DungeonPart.Dir.RB)
           {
               //schuin tegenaan
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RB);
           }
           if (dir2 == DungeonPart.Dir.RT)
           {
               //recht tegenaan
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RB);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RT);
           }
           if (dir2 == DungeonPart.Dir.LB)
           {
               //recht boven
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LB);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RB);
           }
       }
       if (dir1 == DungeonPart.Dir.RB)
       {
           if (dir2 == DungeonPart.Dir.RT)
           {
               //recht onder
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RT);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LT);
           }
           if (dir2 == DungeonPart.Dir.LB)
           {
               //recht naast
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LB);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LT);
           }
           if (dir2 == DungeonPart.Dir.LT)
           {
               //Schuin onder
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LT);
           }
       }
       if (dir1 == DungeonPart.Dir.RT)
       {
           if (dir2 == DungeonPart.Dir.RB)
           {
               //recht boven
               Dbits[dbit].SetToUsed(DungeonPart.Dir.RB);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LB);
           }
           if (dir2 == DungeonPart.Dir.LB)
           {
               //schuin naast
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LB);
           }
           if (dir2 == DungeonPart.Dir.LT)
           {
               //recht naast
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LT);
               Dbits[dbit].SetToUsed(DungeonPart.Dir.LB);
           }
       }
   }

   int GetDBit(DungeonPart part)
   {
       for (int i = 0; i < dungeonParts.Count; i++)
       {
           if (part == dungeonParts[i])
               return i;
       }
       return -1;
   }

   bool DbitAvalible(DungeonPart.Dir dir,int dbit)
   {
       switch (dir)
       {
           case DungeonPart.Dir.LB:
               return Dbits[dbit].LBAvalible;
           case DungeonPart.Dir.LT:
               return Dbits[dbit].LTAvalible;
           case DungeonPart.Dir.RB:
               return Dbits[dbit].RBAvalible;
           case DungeonPart.Dir.RT:
               return Dbits[dbit].RTAvalible;
           default:
               return false;
       }
   }
   //*/


    /*
    void SetLayout()
    {   //standalone part where list get's copied so dparts can delete some
        List<DungeonPart> dParts = dungeonParts;

        //desice a room to start with and remove it form dparts so it woun't be chosen again
        int partNr = rng.Next(0, dParts.Count - 1);
        DungeonPart room = dParts[partNr];
        dParts.Remove(room);

        //deside the ammout of connecting rooms and make an array for that amount
        int connections = rng.Next(0, 4);
        LRTB[] axisArray = new LRTB[connections];

        
        for (int i = 0; i < connections; i++)                   //should be filling the array with different sides
        {
            bool isUsed = true;
            while (isUsed == true)
            {
                isUsed = false;                                 //insurance for leaving the loop
                LRTB axis = (LRTB)rng.Next(0, 3);               //find a random direction
                for (int j = 0; j < axisArray.Length; j++)      //check if it hasn't already been picked
                {
                    if (axis == axisArray[i])
                        isUsed = true;                          //if it has make the loop try again
                }
            }
        }

        for (int i = 0; i < axisArray.Length; i++)              //when directions are picked
        {
            DungeonPart newRoom = null;                         //go trough them and give each dpart to connect to
            for (int j = 0; j < dParts.Count; j++)
            {
                newRoom = dungeonParts[i];                      //get the new part and break if it's a valid target
                if ((Dbits[GetBit(newRoom)].IsAvalible(GetOpisiteAxis(axisArray[i])) == true)) break;
            }
            if (newRoom == null) continue;
            dParts.Remove(newRoom);                             //delete it so it won't be chosen again
            mappingQueue.Enqueue(new MappingOrder(GetBit(room), //Create a new mappingoerder and put it-
                GetBit(newRoom), axisArray[i]));                //in the queue for later puposes
        }
        
    }//*/
}
