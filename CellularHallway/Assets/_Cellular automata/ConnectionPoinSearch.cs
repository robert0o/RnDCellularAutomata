using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch
{
    Hallway hallway;
    List<Node> VisitedNodes;
    int startingX, startingY;
    public List<Node> FindPaths(Hallway _hallway)
    {
        Queue<Node> nodes = new Queue<Node>();
        VisitedNodes = new List<Node>();
        hallway = _hallway;
        int yMid = Mathf.RoundToInt(hallway.hHight / 2f);
        int xMid = Mathf.RoundToInt(hallway.hWidth / 2f);
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
                        nodes.Enqueue(new Node(look[0], look[1], node.distancevalue + 1,GetDirectionOfNode(look[0], look[1])));
                    }
                }
            }//*/
        }
        //hallway = SetFurthestPooints(hallway);
        return VisitedNodes;//*/
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

