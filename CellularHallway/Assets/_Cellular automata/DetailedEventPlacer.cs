using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailedEventPlacer 
{
    RNGSeed rng;
    ConnectionPoinSearch point;
    
    public DetailedEventPlacer(RNGSeed seed)
    {
        rng = seed;
        point = new ConnectionPoinSearch();
    }
    public int[,] SetEvent1(int[,] map, int eventValue)
    {
        List<Vector2Int> banditCampPosition = new List<Vector2Int>();
        bool possiblePosition;
        Vector2Int location;
        //bandits
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != eventValue) continue;
                possiblePosition = true;
                for (int i = 0; i < DirArray.aroundDir.Length; i++)
                {
                    Vector2Int position = new Vector2Int(x, y) + DirArray.aroundDir[i];
                    if (map[position.x, position.y] != eventValue) possiblePosition = false; continue;
                    
                }
                if (possiblePosition == false) continue;
                banditCampPosition.Add(new Vector2Int(x,y));
            }
        }

        if (banditCampPosition.Count == 0)
        {
            location = point.LookForMiddle(map);
        }
        else if(banditCampPosition.Count == 1)
        {
            location = banditCampPosition[0];
        }
        else
        {
            int campNr = Mathf.FloorToInt(banditCampPosition.Count * 0.5f);
            location = banditCampPosition[campNr];
        }

        Vector2Int pos;
        map[location.x, location.y] = eventValue;
        for (int i = 0; i < DirArray.directions.Length; i++)
        {
            pos = location + DirArray.directions[i];
            map[pos.x, pos.y] = eventValue;
        }
        for (int i = 0; i < DirArray.diagonalDir.Length; i++)
        {
            pos = location + DirArray.diagonalDir[i];
            map[pos.x, pos.y] = 1;
        }
        return map;
    }
    public int[,] SetEvent2(int[,] map, int eventValue)
    {
        // tresure event
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] <= 0) continue;
            }
        }
        return map;
    }
    public int[,] SetEvent3(int[,] map, int eventValue)
    {
        //Secret Passage?
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] <= 0) continue;
            }
        }
        return map;
    }

    
}
