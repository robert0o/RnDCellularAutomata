using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch
{
    //Hallway hallway;
    List<Node> VisitedNodes;
    int startingX, startingY;
    /*public List<Node> FindPaths(Hallway _hallway)
    {
        Queue<Node> nodes = new Queue<Node>();
        VisitedNodes = new List<Node>();
        hallway = _hallway;
        int yMid = Mathf.RoundToInt(hallway.hHight / 2f);
        int[,] visitedTiles = new int[hallway.hWidth, hallway.hHight];


        List<int> xOpenSpaces = new List<int>();
        List<int> yOpenSpaces = new List<int>();
        for (int x = 0; x < hallway.hWidth; x++)
        {
            if (hallway.hallway[x, yMid] == 0)
                xOpenSpaces.Add(x);
        }
        int middleXopenspaces = Mathf.RoundToInt((xOpenSpaces.Count - 1) / 2f);
        int middleX = xOpenSpaces[middleXopenspaces];
        for (int y = 0; y < hallway.hHight; y++)
        {
            if (hallway.hallway[middleX, y] == 0)
            {
                yOpenSpaces.Add(y);
            }
        }
        int middleY = yOpenSpaces[Mathf.RoundToInt((yOpenSpaces.Count - 1) / 2f)];
        nodes.Enqueue(new Node(middleX, middleY, 0));
        startingX = middleX;
        startingY = middleY;

        int[,] PathTileMap = new int[hallway.hWidth, hallway.hHight];

        while (nodes.Count > 0)
        {
            Node node = nodes.Dequeue();
            VisitedNodes.Add(node);
            Node otherNode = null;
            visitedTiles[node.x, node.y] = 1;
            int[] look;
            for (int i = 0; i < 4; i++)
            {
                look = LookNext(node.x, node.y, i);
                if (hallway.hallway[look[0], look[1]] == 0)
                {
                    otherNode = GetNode(look[0], look[1]);
                    if (visitedTiles[look[0], look[1]] == 0)
                    {
                        visitedTiles[look[0], look[1]] = 1;
                        PathTileMap[look[0], look[1]] = node.distancevalue + 1;
                        nodes.Enqueue(new Node(look[0], look[1], node.distancevalue + 1,GetDirectionOfNode(look[0], look[1])));
                    }
                }
            }
        }
        //hallway = SetFurthestPooints(hallway);
        return VisitedNodes;
    }
    //*/
    public int[,] findEnds(int [,] invertedMap)
    {
        int[,] newMap = new int[invertedMap.GetLength(0), invertedMap.GetLength(1)];
        int[,] visitedMap = new int[invertedMap.GetLength(0), invertedMap.GetLength(1)];
        Queue<Vector3Int> PointsToLookAt = new Queue<Vector3Int>();
        
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
        int[] look;
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
                if (invertedMap[look[0], look[1]] > 0)
                {
                    if (visitedMap[look[0], look[1]] == 0)
                    {
                        visitedMap[look[0], look[1]] = 1;
                        newMap[look[0], look[1]] = node.z + 1;
                        PointsToLookAt.Enqueue(new Vector3Int(look[0], look[1], node.z + 1));
                        isEndPoint = false;
                    }
                }
            }//*/
            if (isEndPoint == true)
            {
                //newMap[node.x, node.y] = short.MaxValue + node.z;
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

        Queue<Vector3Int> testQueue = new Queue<Vector3Int>();

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
                //endMap[node.x, node.y] = short.MaxValue + node.z;
                testQueue.Enqueue(node);
            }
        }
        
        while(testQueue.Count> 0)
        {
            Vector3Int lastNode = testQueue.Dequeue();
            endMap[lastNode.x, lastNode.y] = short.MaxValue + lastNode.z;
        }

        return endMap;
    }

    Node GetNode(int x,int y)
    {
        for (int i = 0; i < VisitedNodes.Count; i++)
        {
            if(VisitedNodes[i].x == x && VisitedNodes[i].y == y)
            {
                return VisitedNodes[i];
            }
        }
        return null;
    }

    float GetXtoYRatio(float _x, float _y)
    {
        float ratio;

        ratio = _y / _x;

        return ratio;
    }
    Node.Direction GetDirectionOfNode(int _x, int _y)
    {
        Node.Direction dir = Node.Direction.STARTorERROR;
        float x = _x;
        float y = _y;

        if (System.Math.Abs(x) * GetXtoYRatio(x, y) >= System.Math.Abs(y))
        {
            if (x >= startingX)
                dir = Node.Direction.RIGHT;
            else
                dir = Node.Direction.LEFT;
        }
        else
        {
            if (y >= startingY)
                dir = Node.Direction.UP;
            else
                dir = Node.Direction.DOWN;
        }

        return dir;
    }
    public Hallway SetFurthestPooints(Hallway _hall)
    {
        Hallway hall = _hall;
        Node[] nodeArray = new Node[4];
        Node tempNode;
        int up = int.MinValue;
        int down = int.MinValue;
        int left = int.MinValue;
        int right = int.MinValue;
        for (int i = 0; i < VisitedNodes.Count; i++)
        {
            tempNode = VisitedNodes[i];
            if (tempNode.dir == Node.Direction.UP && tempNode.distancevalue > up)
            {
                up = tempNode.distancevalue;
                nodeArray[(int)Node.Direction.UP] = tempNode;
            }
            if (tempNode.dir == Node.Direction.DOWN && tempNode.distancevalue > down)
            {
                down = tempNode.distancevalue;
                nodeArray[(int)Node.Direction.DOWN] = tempNode;
            }
            if (tempNode.dir == Node.Direction.LEFT && tempNode.distancevalue > left)
            {
                left = tempNode.distancevalue;
                nodeArray[(int)Node.Direction.LEFT] = tempNode;
            }
            if (tempNode.dir == Node.Direction.RIGHT && tempNode.distancevalue > right)
            {
                right = tempNode.distancevalue;
                nodeArray[(int)Node.Direction.RIGHT] = tempNode;
            }
        }
        for (int i = 0; i < nodeArray.Length; i++)
        {
            if (nodeArray[i] == null) continue;
            hall.hallway[nodeArray[i].x, nodeArray[i].y] = 2;
        }
        return hall;
    }

    int[] LookNext(int x, int y, int iteration)
    {
        int[] XnY = new int[2];
        switch (iteration)
        {
            case 0:
                XnY[0] = x - 1;
                XnY[1] = y;
                break;
            case 1:
                XnY[0] = x + 1;
                XnY[1] = y;
                break;
            case 2:
                XnY[0] = x;
                XnY[1] = y - 1;
                break;
            default:
                XnY[0] = x;
                XnY[1] = y + 1;
                break;
        }
        return XnY;
    }
}

public class Node
{
    public readonly int x, y;
    bool hasPreviousNode = false;
    //int xConnectedNode, yConnectedNode;
    public int distancevalue;
    public Direction dir;
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        STARTorERROR
    }

    public Node(int _x, int _y, int _distanceValue, Direction _dir = Direction.STARTorERROR)
    {
        x = _x;
        y = _y;
        distancevalue = _distanceValue;
        dir = _dir;
    }
    void SetNodePath()
    {
        hasPreviousNode = true;
    }
    public bool isNotStart()
    {
        return hasPreviousNode;
    }
}

