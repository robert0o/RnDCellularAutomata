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
    System.Random rng;

    int xMax = int.MinValue, yMax = int.MinValue;

    public void Start()
    {
        
        MakeMap();
    }
    private void MakeMap()
    {
        SetDungeonParts();

        //int[,] dungeonMap = new int[xMax * xRange, yMax * yRange];
        int[,] dungeonMap = new int[100, 100];
        int atPart = 0;
        for (int i = 0; i < xRange && atPart < dungeonParts.Length; i++)
        {
            for (int j = 0; j < yRange && atPart < dungeonParts.Length; j++, atPart++)
            {
                int xOffset = 0, yOffset = 0;
                

                for (int x = 0; x < dungeonParts[atPart].tileMap.GetLength(0); x++)
                {
                    for (int y = 0; y < dungeonParts[atPart].tileMap.GetLength(1); y++)
                    {
                        if(dungeonParts[atPart].tileMap[x, y] > 0)
                            dungeonMap[x + xOffset, y + yOffset] = dungeonParts[atPart].tileMap[x, y];

                        xOffset = i * xMax + 20;
                        yOffset = j * yMax + 20;//*/
                    }
                }
                if (atPart + 1 < dungeonParts.Length)
                {
                    Vector2Int connectStart = SelectpointToConnect(i, j, atPart);
                    Vector2Int connectEnd = ConnectToPoint(i, j, atPart + 1);
                    if (connectStart != Vector2Int.zero && connectEnd != Vector2Int.zero)
                    {
                        OverlapTop(dungeonParts[atPart].tileMap, connectStart, dungeonParts[atPart + 1].tileMap, connectEnd);
                    }
                }
                else if (atPart! < dungeonParts.Length) return;//*/

                
            }
        }
        /*CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(dungeonMap,0,0);//*/
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
        dungeonParts = new DungeonPart[maps.Count];


        for (int i = 0; i < maps.Count; i++)
        {
            dungeonParts[i] = new DungeonPart(maps[i]);
        }
    }

    Vector2Int SelectpointToConnect(int i , int j, int atPart)
    {
        DungeonPart.Dir dir = GetDir(i, j);
        DungeonPart part = dungeonParts[atPart];
        List<Vector2Int> points = new List<Vector2Int>();
        for (int l = 0; l < part.cPoints.Count; l++)
        {
            if(part.cPoints[l].dir == dir)
            {
                points.Add(part.cPoints[l].point);
            }
        }
        if (points.Count != 0)
            return points[rng.Next(0, points.Count - 1)];
        else return Vector2Int.zero;
    }

    Vector2Int ConnectToPoint(int i , int j, int atPart)
    {
        DungeonPart.Dir[] dirs = GetConnectTo(GetDir(i,j));
        DungeonPart part = dungeonParts[atPart];
        List<Vector2Int> points = new List<Vector2Int>();
        for (int l = 0; l < part.cPoints.Count; l++)
        {
            if(dirs[0] == part.cPoints[l].dir || dirs[1] == part.cPoints[l].dir)
            {
                points.Add(part.cPoints[l].point);
            }
        }
        
        if (points.Count != 0)
            return points[rng.Next(0, points.Count - 1)];
        else return Vector2Int.zero;
    }

    Vector2Int getPointDifference(Vector2Int start,Vector2Int end)
    {
        int x = end.x - start.x;
        int y = end.y - start.y;
        return new Vector2Int(x,y);
    }

    DungeonPart.Dir GetDir(int i,int j)
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

    void OverlapLeft(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = map1.GetLength(0);

        int yDif =  point1.y - point2.y;

        Vector2Int tem2 = new Vector2Int(point2.x + Offset,point2.y + yDif);
        int[,] temMap = new int[100, 100];
        for (int x = 0; x < map1.GetLength(0); x++)
        {
            for (int y = 0; y < map1.GetLength(1); y++)
            {
                if(map1[x,y] > 0)
                {
                    temMap[x + 20, y + 20] = 1;
                }
            }
        }
        List<Vector2Int> map2List = new List<Vector2Int>();
        for (int x = 0; x < map2.GetLength(0); x++)
        {
            for (int y = 0; y < map2.GetLength(1); y++)
            {
                if(map2[x,y] > 0)
                    map2List.Add(new Vector2Int(x, y));
            }
        }
        bool isOverlapping = false;
        int xOffset = Offset + 20;
        int yOffset = yDif + 20;
        while (isOverlapping == false)
        {
            for (int i = 0; i < map2List.Count; i++)
            {
                if (temMap[map2List[i].x + xOffset, map2List[i].y +yOffset] != 0)
                {
                    isOverlapping = true;
                }
            }
            if(isOverlapping == false)
                xOffset--;
        }
        xOffset++;
        for (int i = 0; i < map2List.Count; i++)
        {
            temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] = 1;
            
        }

        temMap[point1.x + 20, point1.y + 20] = 2;
        temMap[point2.x +xOffset, point2.y + yOffset] = 2;
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(temMap, 0, 0);
    }

    void OverlapRight(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = -map1.GetLength(0);

        int yDif = point1.y - point2.y;
        int UO = 50;

        Vector2Int tem2 = new Vector2Int(point2.x + Offset, point2.y + yDif);
        int[,] temMap = new int[100, 100];
        for (int x = 0; x < map1.GetLength(0); x++)
        {
            for (int y = 0; y < map1.GetLength(1); y++)
            {
                if (map1[x, y] > 0)
                {
                    temMap[x + UO, y + UO] = 1;
                }
            }
        }
        List<Vector2Int> map2List = new List<Vector2Int>();
        for (int x = 0; x < map2.GetLength(0); x++)
        {
            for (int y = 0; y < map2.GetLength(1); y++)
            {
                if (map2[x, y] > 0)
                    map2List.Add(new Vector2Int(x, y));
            }
        }
        bool isOverlapping = false;
        int xOffset = Offset + UO;
        int yOffset = yDif + UO;
        while (isOverlapping == false)
        {
            for (int i = 0; i < map2List.Count; i++)
            {
                if (temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] != 0)
                {
                    isOverlapping = true;
                }
            }
            if (isOverlapping == false)
                xOffset++;
        }
        xOffset--;
        for (int i = 0; i < map2List.Count; i++)
        {
            temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] = 1;

        }

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + xOffset, point2.y + yOffset] = 2;
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(temMap, 0, 0);
    }
    void OverlapBottom(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = -map1.GetLength(1);

        int xDif = point1.x - point2.x;
        int UO = 50;

        Vector2Int tem2 = new Vector2Int(point2.x + xDif, point2.y + Offset);
        int[,] temMap = new int[100, 100];
        for (int x = 0; x < map1.GetLength(0); x++)
        {
            for (int y = 0; y < map1.GetLength(1); y++)
            {
                if (map1[x, y] > 0)
                {
                    temMap[x + UO, y + UO] = 1;
                }
            }
        }
        List<Vector2Int> map2List = new List<Vector2Int>();
        for (int x = 0; x < map2.GetLength(0); x++)
        {
            for (int y = 0; y < map2.GetLength(1); y++)
            {
                if (map2[x, y] > 0)
                    map2List.Add(new Vector2Int(x, y));
            }
        }
        bool isOverlapping = false;
        int xOffset = xDif + UO;
        int yOffset = Offset + UO;
        while (isOverlapping == false)
        {
            for (int i = 0; i < map2List.Count; i++)
            {
                if (temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] != 0)
                {
                    isOverlapping = true;
                }
            }
            if (isOverlapping == false)
                yOffset++;
        }
        yOffset--;
        for (int i = 0; i < map2List.Count; i++)
        {
            temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] = 1;

        }

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + xOffset, point2.y + yOffset] = 2;
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(temMap, 0, 0);
    }

    void OverlapTop(int[,] map1, Vector2Int point1, int[,] map2, Vector2Int point2)
    {
        int Offset = map1.GetLength(1);

        int xDif = point1.x - point2.x;
        int UO = 50;

        Vector2Int tem2 = new Vector2Int(point2.x + xDif, point2.y + Offset);
        int[,] temMap = new int[100, 100];
        for (int x = 0; x < map1.GetLength(0); x++)
        {
            for (int y = 0; y < map1.GetLength(1); y++)
            {
                if (map1[x, y] > 0)
                {
                    temMap[x + UO, y + UO] = 1;
                }
            }
        }
        List<Vector2Int> map2List = new List<Vector2Int>();
        for (int x = 0; x < map2.GetLength(0); x++)
        {
            for (int y = 0; y < map2.GetLength(1); y++)
            {
                if (map2[x, y] > 0)
                    map2List.Add(new Vector2Int(x, y));
            }
        }
        bool isOverlapping = false;
        int xOffset = xDif + UO;
        int yOffset = Offset + UO;
        while (isOverlapping == false)
        {
            for (int i = 0; i < map2List.Count; i++)
            {
                if (temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] != 0)
                {
                    isOverlapping = true;
                }
            }
            if (isOverlapping == false)
                yOffset--;
        }
        yOffset++;
        for (int i = 0; i < map2List.Count; i++)
        {
            temMap[map2List[i].x + xOffset, map2List[i].y + yOffset] = 1;

        }

        temMap[point1.x + UO, point1.y + UO] = 2;
        temMap[point2.x + xOffset, point2.y + yOffset] = 2;
        CellPlacer cells = FindObjectOfType<CellPlacer>();
        cells.PlaceMapCells(temMap, 0, 0);
    }


    enum LRTB
    {
        LEFT,RIGHT,TOP,BOTTOM
    }

}
