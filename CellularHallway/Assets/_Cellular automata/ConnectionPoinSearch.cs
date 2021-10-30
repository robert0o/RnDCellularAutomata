using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch
{
    int startingX, startingY;
    List<Vector2Int> endPointList;
    
    public int[,] findEnds(int [,] invertedMap)
    {
        int[,] newMap = new int[invertedMap.GetLength(0), invertedMap.GetLength(1)];
        int[,] visitedMap = new int[invertedMap.GetLength(0), invertedMap.GetLength(1)];
        Queue<Vector3Int> PointsToLookAt = new Queue<Vector3Int>();
        endPointList = new List<Vector2Int>();

        int yMid = Mathf.RoundToInt((invertedMap.GetLength(1)-1) / 2f);
        int xStart, yStart;

        List<int> xOpenSpaces = new List<int>();
        List<int> yOpenSpaces = new List<int>();
        for (int x = 0; x < invertedMap.GetLength(0); x++)
        {
            if (invertedMap[x, yMid] > 0)
                xOpenSpaces.Add(x);
        }
        int middleXopenspaces = Mathf.RoundToInt((xOpenSpaces.Count -1) / 2f);
        int middleX = xOpenSpaces[middleXopenspaces];
        for (int y = 0; y < invertedMap.GetLength(1); y++)
        {
            if (invertedMap[middleX, y] > 0)
            {
                yOpenSpaces.Add(y);
            }
        }
        int middleY = yOpenSpaces[Mathf.RoundToInt((yOpenSpaces.Count -1) / 2f)];
        xStart = middleX;
        yStart = middleY;
        PointsToLookAt.Enqueue(new Vector3Int(xStart, yStart,1));
        newMap[xStart, yStart] = 1;
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
                if (invertedMap[look.x, look.y] > 0)
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
        Vector3Int[] dir = { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };

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
                visitedMap[pos.x, pos.y] = 1;
                nodeQue.Enqueue(node + dir[i]+Vector3Int.forward);
            }
        }
        return keyPositions;
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
}

