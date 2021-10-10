using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoinSearch : MonoBehaviour
{
    Hallway hallway;
    List<Node> VisitedNodes;
    void FindPaths(Hallway _hallway)
    {
        Queue<Node> nodes = new Queue<Node>();
        VisitedNodes = new List<Node>();
        hallway = _hallway;
        int yMid = hallway.hHight / 2;
        int[,] visitedTiles = new int[hallway.hWidth, hallway.hHight];

        for (int x = 1; x < hallway.hWidth; x++)
        {
            if (hallway.hallway[x, yMid] == 0)
            {
                nodes.Enqueue(new Node(x, yMid, 0));
                break;
            }
        }

        while (nodes.Count > 0)
        {
            Node node = nodes.Dequeue();
            Node otherNode = null;
            visitedTiles[node.x, node.y] = 1;
            if (hallway.hallway[node.x-1, node.y] == 0)
            {
                otherNode = GetNode(node.x - 1, node.y);
                if (visitedTiles[node.x - 1, node.y] == 0)
                {
                    visitedTiles[node.x - 1, node.y] = 1;
                    nodes.Enqueue(new Node(node.x - 1, node.y, node.distancevalue + 1));
                }
                else if(otherNode != null && otherNode.distancevalue > node.distancevalue + 1)
                {
                   
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
            }

        }

        
    }
    Node GetNode(int x,int y)
    {
        Node node;
        for (int i = 0; i < VisitedNodes.Count; i++)
        {
            if(VisitedNodes[i].x == x && VisitedNodes[i].y == y)
            {
                return VisitedNodes[i];
            }
        }
        return null;
    }
    
}

public class Node
{
    public readonly int x, y;
    bool hasPreviousNode = false;
    //int xConnectedNode, yConnectedNode;
    public int distancevalue;
    public Node(int _x, int _y, int _distanceValue)
    {
        x = _x;
        y = _y;
        distancevalue = _distanceValue;
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

