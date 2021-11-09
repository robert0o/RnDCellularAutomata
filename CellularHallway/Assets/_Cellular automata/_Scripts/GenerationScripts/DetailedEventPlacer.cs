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
    public int[,] SetStartingPosition(int[,]map, int eventValue)
    {
        //chooses a single position to start form
        List<Vector2Int> startingPositions = new List<Vector2Int>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == eventValue)
                {
                    startingPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        map = emptyMap(map);

        Vector2Int pos = startingPositions[rng.Next(0, startingPositions.Count)];
        map[pos.x, pos.y] = eventValue;

        return map;
    }
    public int[,] SetEvent1(int[,] map, int eventValue)
    {
        //loosk for a place with a 3*3 area
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
                banditCampPosition.Add(new Vector2Int(x, y));
            }
        }

        if (banditCampPosition.Count == 0)
        {
            return emptyMap(map);
        }
        else if (banditCampPosition.Count == 1)
        {
            location = banditCampPosition[0];
        }
        else
        {
            int campNr = Mathf.FloorToInt(banditCampPosition.Count * 0.5f);
            location = banditCampPosition[campNr];
        }
        //sets the surrounding area to a empy tile and the center to the event tile. tile placer script does the rest
        map = emptyMap(map);
        Vector2Int pos;
        map[location.x, location.y] = eventValue;
        for (int i = 0; i < DirArray.directions.Length; i++)
        {
            pos = location + DirArray.directions[i];
            map[pos.x, pos.y] = 1;
        }
        for (int i = 0; i < DirArray.diagonalDir.Length; i++)
        {
            pos = location + DirArray.diagonalDir[i];
            map[pos.x, pos.y] = 1;
        }
        return map;
    }
    //tressure
    public int[,] SetEvent2(int[,] map, int eventValue)
    {
        List<Vector2Int> counting = new List<Vector2Int>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != eventValue) continue;
                counting.Add(new Vector2Int(x, y));
            }
        }
        //finds and endpoitn of the area to put a chest
        List<Vector2Int> points = point.GetKeyPositions(map, counting[counting.Count / 2], int.MaxValue);

        map = emptyMap(map);
        map[points[0].x, points[0].y] = eventValue;
        return map;
    }
    //boss Room
    public int[,] SetEvent3(int[,] map, int eventValue)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        //gets the area for a boss room
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != eventValue) continue;
                positions.Add(CountingFreeSpaces(map, new Vector2Int(x, y), eventValue));
            }
        }
        
        //goes trough all and selects saves the one with the most free spaces
        Vector2Int maxFreeSpaces = Vector2Int.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            if(positions[i].z > maxFreeSpaces.y)
            {
                maxFreeSpaces = new Vector2Int(i, positions[i].z);
            }
        }
        //finnaly select the place the boss will be
        Vector2Int pos = new Vector2Int(positions[maxFreeSpaces.x].x, positions[maxFreeSpaces.x].y);

        map = emptyMap(map);
        map = SetEvent(map, pos, eventValue);
        return map;
    }

    //looks around and counts the amount of free spaces for the boss room 
    Vector3Int CountingFreeSpaces(int[,] map, Vector2Int startPos,int eventValue)
    {
        Vector3Int pos = new Vector3Int(startPos.x, startPos.y, 0);
        for (int x = startPos.x -2; x < startPos.x + 2; x++)
        {
            for (int y = startPos.y-2; y < startPos.y + 2; y++)
            {
                if (x < 1 || x >= map.GetLength(0) || y < 1 || y > map.GetLength(1)) 
                { 
                    pos.z = -1;
                    return pos; 
                }
                if (map[x, y] != eventValue) continue;
                pos.z++;
            }
        }
        return pos;
    }

    //set the boss tiles
    int[,] SetEvent(int[,] map, Vector2Int startPos, int eventValue)
    {
        for (int x = startPos.x - 2; x <= startPos.x + 2; x++)
        {
            for (int y = startPos.y - 2; y <= startPos.y + 2; y++)
            {
                map[x, y] = eventValue;
            }
        }
        return map;
    }

    //clean the map to make values back to 1
    int[,] emptyMap(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] <= 0) continue;
                map[x, y] = 1;
            }
        }
        return map;
    }
}
