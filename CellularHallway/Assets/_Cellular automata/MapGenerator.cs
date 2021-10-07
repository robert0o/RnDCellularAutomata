using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int hight;
    [Range(0, 100)]
    public int fillPercent;
    [Range(0, 10)]
    public int iterations;
    public string seed;
    int[,] map;
    [Header("Cel Rules")]
    [Range(0, 1)]
    public int rule0,rule1, rule2, rule3, rule4, rule5, rule6, rule7, rule8;

    private void Start()
    {
        GenMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenMap();
        }
    }
    private void GenMap()
    {
        map = new int[width, hight];
        GenerateMap();
        for (int x = 0; x < iterations; x++)
        {
            Iterate();
        }
    }

    void GenerateMap()
    {
        System.Random rng = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == hight - 1)
                {
                    map[x, y] = 1; 
                }
                else
                    map[x, y] = (rng.Next(0, 100) < fillPercent) ? 1 : 0;
            }
        }
    }

    void Iterate()
    {
        int[,] iterationMap = new int[width, hight];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                iterationMap[x, y] = rules(SurroundWalls(x, y));
            }
        }
        map = iterationMap;
    }
    
    int rules(int walls)
    {
        int state = 0;

        switch (walls)
        {
            case 0:
                state = rule0;
                break;
            case 1:
                state = rule1;
                break;
            case 2:
                state = rule2;
                break;
            case 3:
                state = rule3;
                break;
            case 4:
                state = rule4;
                break;
            case 5:
                state = rule5;
                break;
            case 6:
                state = rule6;
                break;
            case 7:
                state = rule7;
                break;
            case 8:
                state = rule8;
                break;
            default:
                state = 0;
                break;
        }
        return state;
    }
    int SurroundWalls(int xPos, int yPos)
    {
        int wallCount = 0;
        for (int x = xPos-1; x <= xPos+1; x++)
        {
            for (int y = yPos-1; y <= yPos +1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < hight)
                {
                    if (x != xPos || y != yPos)
                        wallCount += map[x, y];
                }
                else
                {
                    wallCount++;
                }
                
                
            }
        }

        return wallCount;
    }

    private void OnDrawGizmos()
    {
        if (map == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < hight; y++)
            {
                Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                Vector3 pos = new Vector3(x, y,0);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }
}
