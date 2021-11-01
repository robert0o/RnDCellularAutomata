using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch
{
    int startingX, startingY;
    List<Vector2Int> endPointList;
    
    public int[,] findEnds(int [,] map)
    {
        int[,] newMap = new int[map.GetLength(0), map.GetLength(1)];
        int[,] visitedMap = new int[map.GetLength(0), map.GetLength(1)];
        Queue<Vector3Int> PointsToLookAt = new Queue<Vector3Int>();
        endPointList = new List<Vector2Int>();
        //
        Vector2Int startingPos = LookForMiddle(map);
        //
        PointsToLookAt.Enqueue(new Vector3Int(startingPos.x, startingPos.y, 1));
        newMap[startingPos.x, startingPos.y] = 1;
        Vector2Int look;
        Queue<Vector3Int> endPoints = new Queue<Vector3Int>();
        bool isEndPoint;
        while (PointsToLookAt.Count > 0)
        {
            isEndPoint = true;
            Vector3Int node = PointsToLookAt.Dequeue();
            visitedMap[node.x, node.y] = 1;
            for (int i = 0; i < 4; i++)
            {
                look = LookNext(node.x, node.y, i);
                if (map[look.x, look.y] > 0)
                {
                    if (visitedMap[look.x, look.y] == 0)
                    {
                        visitedMap[look.x, look.y] = 1;
                        newMap[look.x, look.y] = node.z + 1;
                        PointsToLookAt.Enqueue(new Vector3Int(look.x,look.y, node.z + 1));
                        isEndPoint = false;
                    }
                }
            }//*/
            if (isEndPoint == true)
            {
                endPoints.Enqueue(node);
                
            }
        }
        
        if (endPoints.Count > 0)
            newMap = CycleToEndPoints(newMap, endPoints);//*/

        return newMap;
    }

    

    int[,] CycleToEndPoints(int[,] travelMap, Queue<Vector3Int> endpoints)
    {
        int[,] endMap = travelMap;
        int[,] visitedMap = new int[travelMap.GetLength(0), travelMap.GetLength(1)];
        Queue<Vector3Int> endQueue = new Queue<Vector3Int>();
        

        while (endpoints.Count > 0)
        {
            Vector3Int node = endpoints.Dequeue();
            visitedMap[node.x, node.y] = 1;
            endQueue.Enqueue(node);
        }
        int LookingAtNode;
        int thisNode;
        while (endQueue.Count > 0)
        {
            bool hasHigherOrEqualNeighbor = false;
            Vector3Int node = endQueue.Dequeue();
            for (int x = node.x -1; x <= node.x + 1; x++)
            {
                for (int y = node.y - 1; y <= node.y + 1; y++)
                {
                    LookingAtNode = endMap[x, y];  thisNode = endMap[node.x, node.y];
                    if(endMap[x, y] >= endMap[node.x, node.y] && (x != node.x || y!= node.y))
                    {
                        hasHigherOrEqualNeighbor = true;
                        if (visitedMap[x,y] == 0)
                        {
                            visitedMap[x, y] = 1;
                            endQueue.Enqueue(new Vector3Int(x, y, endMap[x, y]));
                        }
                    }
                }
            }
            if (hasHigherOrEqualNeighbor == false)
            {
                endPointList.Add(new Vector2Int(node.x, node.y));
                endMap[node.x, node.y] = short.MaxValue + node.z;
            }
        }

        return endMap;
    }

    public List<Vector2Int> GetKeyPositions(int[,] lockMap, Vector2Int lockPosition, int minDistance)
    {
        Queue<Vector3Int> nodeQue = new Queue<Vector3Int>();
        List<Vector2Int> keyPositions = new List<Vector2Int>();
        int width = lockMap.GetLength(0);
        int length = lockMap.GetLength(1);
        int[,] visitedMap = new int[width, length];
        int[,] distanceMap = new int[width, length];
        Vector3Int[] dir = { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };
        int maximumDistance = 0;

        nodeQue.Enqueue(new Vector3Int(lockPosition.x,lockPosition.y,1));
        visitedMap[lockPosition.x, lockPosition.y] = 1;
        Vector3Int node;

        while (nodeQue.Count  > 0)
        {
            node = nodeQue.Dequeue();
            
            for (int i = 0; i < dir.Length; i++)
            {
                Vector2Int pos = new Vector2Int(node.x + dir[i].x, node.y + dir[i].y);
                if (IsInMap(width, length, pos) == false) continue; 
                if( visitedMap[pos.x,pos.y] == 1 || lockMap[pos.x, pos.y] == 0) continue;

                if (node.z + 1 >= minDistance)
                    keyPositions.Add(new Vector2Int(node.x, node.y));
                if (node.z + 1 > maximumDistance)
                    maximumDistance = node.z + 1;

                visitedMap[pos.x, pos.y] = 1;
                distanceMap[pos.x, pos.y] = node.z + 1;
                nodeQue.Enqueue(node + dir[i]+Vector3Int.forward);
            }
        }
        if(keyPositions.Count == 0)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (distanceMap[i, j] == maximumDistance)
                        keyPositions.Add(new Vector2Int(i, j));
                }
            }
        }
        return keyPositions;
    }

    public Vector2Int LookForMiddle(int[,] map)
    {
        //Can find a rough middle point of a given map
        int yMid = Mathf.RoundToInt((map.GetLength(1) - 1) / 2f);

        List<int> xOpenSpaces = new List<int>();
        List<int> yOpenSpaces = new List<int>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            if (map[x, yMid] > 0)
                xOpenSpaces.Add(x);
        }
        int middleXopenspaces = Mathf.RoundToInt((xOpenSpaces.Count - 1) / 2f);
        int middleX = xOpenSpaces[middleXopenspaces];
        for (int y = 0; y < map.GetLength(1); y++)
        {
            if (map[middleX, y] > 0)
            {
                yOpenSpaces.Add(y);
            }
        }
        int middleY = yOpenSpaces[Mathf.RoundToInt((yOpenSpaces.Count - 1) / 2f)];
        return new Vector2Int(middleX, middleY);
    }

    bool IsInMap(int width,int length, Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < length);
    }

    Vector2Int LookNext(int x, int y, int iteration)
    {
        Vector2Int xny = new Vector2Int(x,y);
        switch (iteration)
        {
            case 0:
                xny += Vector2Int.left;
                break;
            case 1:
                xny += Vector2Int.right;
                break;
            case 2:
                xny += Vector2Int.down;
                break;
            default:
                xny += Vector2Int.up;
                break;
        }
        return xny;
    }
    public List<Vector2Int> GetEndPointsPosition()
    {
        return (endPointList == null) ? null : endPointList;
    }

    public List<List<Vector3Int>> DetectEventRegions(int[,] map)
    {
        List<List<Vector3Int>> regions = new List<List<Vector3Int>>();
        int[,] visitedMap = new int[map.GetLength(0), map.GetLength(1)];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] <= 1 || visitedMap[x,y] == 1) continue;
                regionReturn re = GetRegion(x, y, map[x, y], map, visitedMap);
                visitedMap = re.visitedMap;
                if (re.succes == false) continue;
                regions.Add(re.region);
            }
        }
        return regions;
    }
    regionReturn GetRegion(int xStart, int yStart, int eventvalue, int[,] map, int[,] visitedmap)
    {
        List<Vector3Int> region = new List<Vector3Int>();
        Queue<Vector2Int> regionQueue = new Queue<Vector2Int>();
        Vector2Int tempPos;
        region.Add(new Vector3Int(xStart, yStart, eventvalue));
        regionQueue.Enqueue(new Vector2Int(xStart, yStart));
        visitedmap[xStart, yStart] = 1;

        while (regionQueue.Count > 0)
        {
            Vector2Int node = regionQueue.Dequeue();
            for (int i = 0; i < DirArray.directions.Length; i++)
            {
                tempPos = node + DirArray.directions[i];
                if (map[tempPos.x, tempPos.y] != eventvalue || visitedmap[tempPos.x, tempPos.y] != 0) continue;
                visitedmap[tempPos.x, tempPos.y] = 1;
                regionQueue.Enqueue(new Vector2Int(tempPos.x, tempPos.y));
                region.Add(new Vector3Int(tempPos.x, tempPos.y, eventvalue));
            }
        }
        bool succes = true;
        if (region.Count < 10) succes = false;
        return new regionReturn(region,visitedmap,succes);
    }
    struct regionReturn
    {
        public bool succes;
        public List<Vector3Int> region;
        public int[,] visitedMap;
        public regionReturn(List<Vector3Int> _region, int[,] _visitedMap,bool _succes)
        {
            succes = _succes;
            region = _region;
            visitedMap = _visitedMap;
        }
    }
}

