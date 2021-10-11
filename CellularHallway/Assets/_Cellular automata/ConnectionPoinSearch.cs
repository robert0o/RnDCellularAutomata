using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch
{
    Hallway hallway;
    List<Node> VisitedNodes;
    int startingX, startingY;
    public Hallway FindPaths(Hallway _hallway)
    {
        Queue<Node> nodes = new Queue<Node>();
        VisitedNodes = new List<Node>();
        hallway = _hallway;
        int yMid = hallway.hHight / 2;
        int xMid = hallway.hHight / 3;
        int[,] visitedTiles = new int[hallway.hWidth, hallway.hHight];

        for (int x = xMid; x < hallway.hWidth; x++)
        {
            if (hallway.hallway[x, yMid] == 0)
            {
                nodes.Enqueue(new Node(x, yMid, 0));
                startingX = x;
                startingY = yMid;
                break;
            }
        }

        while (nodes.Count > 0)
        {
            Node node = nodes.Dequeue();
            VisitedNodes.Add(node);
            Node otherNode = null;
            visitedTiles[node.x, node.y] = 1;
            /*if (hallway.hallway[node.x-1, node.y] == 0)
            {
                otherNode = GetNode(node.x - 1, node.y);
                if (visitedTiles[node.x - 1, node.y] == 0)
                {
                    visitedTiles[node.x - 1, node.y] = 1;
                    nodes.Enqueue(new Node(node.x - 1, node.y, node.distancevalue + 1));
                }
            }
            if (hallway.hallway[node.x+1, node.y] == 0)
            {
                otherNode = GetNode(node.x + 1, node.y);
                if (visitedTiles[node.x + 1, node.y] == 0)
                {
                    visitedTiles[node.x + 1, node.y] = 1;
                    nodes.Enqueue(new Node(node.x + 1, node.y, node.distancevalue + 1));
                }
            }
            if (hallway.hallway[node.x, node.y-1] == 0)
            {
                otherNode = GetNode(node.x, node.y -1);
                if (visitedTiles[node.x, node.y - 1] == 0)
                {
                    visitedTiles[node.x, node.y - 1] = 1;
                    nodes.Enqueue(new Node(node.x, node.y - 1, node.distancevalue + 1));
                }
            }
            if (hallway.hallway[node.x, node.y+1] == 0)
            {
                otherNode = GetNode(node.x, node.y + 1);
                if (visitedTiles[node.x, node.y + 1] == 0)
                {
                    visitedTiles[node.x, node.y + 1] = 1;
                    nodes.Enqueue(new Node(node.x, node.y + 1, node.distancevalue + 1));
                }
            }//*/

            int nodeX, nodeY;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        nodeX = node.x - 1;
                        nodeY = node.y;
                        break;
                    case 1:
                        nodeX = node.x + 1;
                        nodeY = node.y;
                        break;
                    case 2:
                        nodeX = node.x;
                        nodeY = node.y - 1;
                        break;
                    default:
                        nodeX = node.x;
                        nodeY = node.y + 1;
                        break;
                }
                if (hallway.hallway[nodeX, nodeY] == 0)
                {
                    otherNode = GetNode(nodeX, nodeY);
                    if (visitedTiles[nodeX, nodeY] == 0)
                    {
                        visitedTiles[nodeX, nodeY] = 1;
                        nodes.Enqueue(new Node(nodeX, nodeY, node.distancevalue + 1,GetDirectionOfNode(nodeX,nodeY)));
                    }
                }
            }//*/
        }
        hallway = SetFurthestPooints(hallway);
        return hallway;
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
    Hallway SetFurthestPooints(Hallway _hall)
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

