using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPart {
    public int[,] tileMap;
    public List<cPoint> cPoints;
    public Vector2Int startPoint;

    public DungeonPart(int[,] _tileMap)
    {
        tileMap = _tileMap;
        SetConnectPoints();
    }

    void SetConnectPoints()
    {
        List<Vector2Int> points = new List<Vector2Int>();
        
        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                if (tileMap[x, y] > short.MaxValue)
                    points.Add(new Vector2Int(x, y));
                if (tileMap[x, y] == 1)
                    startPoint = new Vector2Int(x, y);
                if (tileMap[x, y] > 1 && tileMap[x, y] > short.MaxValue)
                    tileMap[x, y] = 1;
                if (tileMap[x, y] > short.MaxValue)
                    tileMap[x, y] = 2;
            }
        }
        int halfx = tileMap.GetLength(0) / 2, halfY = tileMap.GetLength(1) / 2;
        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].x < halfx && points[i].y < halfY) 
            {
                //bottemLeft
                cPoints.Add(new cPoint(points[i], Dir.BL));
            }
            if (points[i].x >= halfx && points[i].y < halfY)
            {
                //bottomRight
                cPoints.Add(new cPoint(points[i], Dir.BR));
            }
            if (points[i].x < halfx && points[i].y >= halfY)
            {
                //topLeft
                cPoints.Add(new cPoint(points[i], Dir.TL));
            }
            if (points[i].x >= halfx && points[i].y >= halfY)
            {
                //topRight
                cPoints.Add(new cPoint(points[i], Dir.TR));
            }
        }
    }
    public enum Dir
    {
        TL,
        TR,
        BL,
        BR
    }
    public struct cPoint
    {
        public Vector2Int point;
        public Dir dir;

        public cPoint(Vector2Int _point, Dir _dir)
        {
            point = _point;
            dir = _dir;
        }
    }
}
