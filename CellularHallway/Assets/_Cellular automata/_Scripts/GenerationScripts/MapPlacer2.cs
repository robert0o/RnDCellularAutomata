using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography;

public class MapPlacer2 : MonoBehaviour
{
    MapGenerator mapGen;
    ConnectionPoinSearch point;
    DetailedEventPlacer detailedEvent;
    int[,] levelMap;

    [Range(3, 10)]
    public int xRange =3, yRange=3;
    public string Seed;
    public bool randomSeed;

    [Header("EventPlacement")]
    [Range(0, 10)]
    public int maxEvents = 0;
    public bool repeatableEvents = false;
    public int keyMinDistance;

    public RNGSeed rng;

    int totalSections;
    List<int[,]> maps;
    int xMax, yMax;
    int[,] totalMap;
    int[,] cleanMap;
    int[,] mapsVisited;
    int[,] layout;
    List<int> usedRooms;

    public bool SetTiles;

    Queue<Vector2Int[]> partQueue;
    public EventValues EV;

    //something to hold the min and max values of a int[,]
    struct MinMax
    {
        public Vector2Int Max;
        public Vector2Int Min;
        public MinMax(int xMin, int yMin, int xMax, int yMax)
        {
            Max = new Vector2Int(xMax, yMax);
            Min = new Vector2Int(xMin, yMin);
        }

    }

    public void Start()
    {
        //generate a random seed. this should always be true when building
        if (randomSeed == true) Seed = Random.Range(float.MinValue, float.MaxValue).ToString();
        rng.setSeed(Seed.GetHashCode());
        detailedEvent = new DetailedEventPlacer(rng);

        //get's parts from mapgenerator 
        GetMapGenData();

        //set a layout structure for map placement
        StructureMap();

        //remove uneccesary size of the map
        totalMap = TrimMap(totalMap);
        
        SetCleanMap();

        point = new ConnectionPoinSearch();
        int[,] pathedMap = point.findEnds(totalMap);
        SetDetailedEvents();
        AddEndpoints();

        //for testing purpoces cells can still be placed instead of tiles
        if (SetTiles == true)
        {
            FindObjectOfType<SetTiles>().SetMap(cleanMap);
        }
        else
        {
            FindObjectOfType<CellPlacer>().PlaceMapCells(cleanMap, 0, 0);
        }
    }

    //makes a compy of the total map without events placed
    void SetCleanMap()
    {
        cleanMap = new int[totalMap.GetLength(0), totalMap.GetLength(1)];
        for (int x = 0; x < totalMap.GetLength(0); x++)
        {
            for (int y = 0; y < totalMap.GetLength(1); y++)
            {
                if (totalMap[x, y] <= 0) continue;
                cleanMap[x, y] = 1;
            }
        }
    }

    //get's parts from map generator
    void GetMapGenData()
    {
        maps = new List<int[,]>();
        xMax = int.MinValue; 
        yMax = int.MinValue;
        totalSections = xRange * yRange;
        mapGen = FindObjectOfType<MapGenerator>();
        int iterate = 3;
        
        //when iteration go to far there won't be any more parts so and invinite loop would happen
        while (maps.Count <= totalSections)
        {
            if(iterate >= 10){
                //making a new seed when the loop has gone to long and still hasn't gotten enough parts 
                Seed = "";
                for (int i = 0; i < 10; i++){
                    //using chars for a uniqe seed
                    //Fun fact I calculated this and if every person on earth did this every second for a year they would still take 
                    //a million+ years and not even get a duplicate seed once. or they're REALLY lucky
                    char ch = (char)rng.Next(0, 255);
                    Seed += ch.ToString();
                    iterate = 3;
                }
            }
            List<int[,]> _maps = mapGen.GetMaps(Seed, iterate);
            for (int j = 0; j < _maps.Count; j++)
            {
                //check what is the maximum size of all parts for later use 
                if (_maps[j].GetLength(0) > xMax) xMax = _maps[j].GetLength(0);
                if (_maps[j].GetLength(1) > yMax) yMax = _maps[j].GetLength(1);
                maps.Add(_maps[j]);
            }
            iterate++;
        }
    }

    //structures the map to make it easier to place the parts
    void StructureMap()
    {
        //making a new map with some leway to prevent index out of range errors
        totalMap = new int[xMax * (xRange + 1), yMax * (yRange + 1)];
        //getting roughly the middle of the map
        Vector2Int baseOffset = new Vector2Int(
            Mathf.CeilToInt(totalMap.GetLength(0) * .5f - maps[0].GetLength(0) * .5f),
            Mathf.CeilToInt(totalMap.GetLength(1) * .5f - maps[0].GetLength(0) * .5f));

        mapsVisited = new int[xRange, yRange];

        CreateLayout();

        SetEventsInMap();

        partQueue = new Queue<Vector2Int[]>();

        //randomly set the starting point
        int midX = rng.Next(0,xRange-1), midY = Mathf.CeilToInt(rng.Next(0, yRange - 1));
        Vector2Int layoutPosition = new Vector2Int(midX, midY);
        mapsVisited[layoutPosition.x, layoutPosition.y] = 1;

        //set the first part to the map
        int firstPart = layout[layoutPosition.x, layoutPosition.y];
        SetPartToMap(maps[firstPart],baseOffset);
        Vector2Int[] qVector = { layoutPosition, baseOffset };

        //part of the loop to set the other parts
        partQueue.Enqueue(qVector);
        while (partQueue.Count > 0)
        {
            Vector2Int[] inQVector = partQueue.Dequeue();
            PlaceStructuredMap(inQVector[0], inQVector[1]);
        }
    }

    //distributes the parts randomly 
    void CreateLayout()
    {
        layout = new int[xRange, yRange];
        
        List<int> parts = new List<int>();
        for (int i = 0; i < maps.Count; i++) { parts.Add(i); }
        for (int x = 0; x < xRange; x++)
        {
            for (int y = 0; y < yRange; y++)
            {
                int partNr = rng.Next(0, parts.Count);
                layout[x, y] = parts[partNr];
                parts.Remove(parts[partNr]);
            }
        }
    }
    //set's event that are more efficient to place them here
    void SetEventsInMap()
    {
        getUsedRooms();
        if (repeatableEvents == true)
            SetRepeatEvents();
        else
            SetNonRepeatEvents();

        SetEventToRoom(2); //startingRoom
        SetEventToRoom(7); //BossRoom
    }
    //when multiple event's can be placed
    void SetRepeatEvents()
    {
        int eventLimit = (maxEvents < usedRooms.Count -1) ? maxEvents : usedRooms.Count - 1;
        for (int i = 0; i < eventLimit; i++)
        {
            int nr = rng.Next(EV.eventStartIndex, EV.eventList.Length);
            if (nr == 7) continue; //Qick sollution for the boss room as I later placed it to always have 1
            SetEventToRoom(nr);
        }
    }
    //when single event's can be placed
    void SetNonRepeatEvents()
    {
        int[] eventNR = new int[EV.eventList.Length - EV.eventStartIndex];
        for (int i = 0; i < eventNR.Length; i++)
        {
            eventNR[i] = EV.eventStartIndex + i;
        }

        int eventLimit = (maxEvents < eventNR.Length) ? maxEvents : eventNR.Length;
        eventNR = eventNR.OrderBy(x => rng.Next()).ToArray();
        for (int i = 0; i < eventLimit; i++)
        {
            if (eventNR[i] == 7) continue; //same as in repeatEvents
            SetEventToRoom(eventNR[i]);
        }
    }

    //goes through the layout to see what parts are used
    void getUsedRooms()
    {
        usedRooms = new List<int>();
        for (int x = 0; x < xRange; x++)
        {
            for (int y = 0; y < yRange; y++)
            {
                usedRooms.Add(layout[x, y]);
            }
        }
    }

    //setting the parts form the layout into the map
    void PlaceStructuredMap(Vector2Int position,Vector2Int offset)
    {
        int currentPart = layout[position.x, position.y];
        Vector2Int[] directions = { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down }; //should be form dirArray
        //suffeling the direction to look at first, making it a little more random
        directions = directions.OrderBy(x => rng.Next()).ToArray();

        //looping rough the direction to look at
        for (int i = 0; i < 4; i++)
        {
            //for ease of use making new vectors with valus that need to be used
            Vector2Int newPos = position + directions[i];
            Vector2Int newOffset = offset;

            //some checks to prefent errors
            if (newPos.x >= layout.GetLength(0) || newPos.x < 0 || newPos.y >= layout.GetLength(1) || newPos.y < 0) continue;
            if (mapsVisited[newPos.x, newPos.y] == 1) continue;

            //getting the part for the layout, chekking for overlap and setting it used so it won't be used again
            int nextPart = layout[newPos.x, newPos.y];
            newOffset = newOffset + GetOverlapOffset(maps[currentPart], maps[nextPart], directions[i]);
            mapsVisited[newPos.x, newPos.y] = 1;

            SetPartToMap(maps[nextPart], newOffset);

            //add to the queue so other part will be set
            Vector2Int[] qVector = { newPos, newOffset };
            partQueue.Enqueue(qVector);
        }
    }

    //add the part to the total map
    void SetPartToMap(int[,] part, Vector2Int offset)
    {
        for (int x = 0; x < part.GetLength(0); x++){
            for (int y = 0; y < part.GetLength(1); y++){
                if (part[x, y] <= 0) continue;
                totalMap[x + offset.x, y + offset.y] = part[x, y];
            }
        }
    }

    //getting the offset for the parts to not overlap. doesn't check for overlap with already placed parts
    //make a temporary map, placing bolth parts at roughly there center ontop of eachoter then move them away untill they don't overlap
    Vector2Int GetOverlapOffset(int[,] baseMap, int[,] overlappingMap, Vector2Int dir)
    {
        int[,] tempmap = new int[xMax * 3, yMax * 3];
        Vector2Int offset = new Vector2Int(xMax, yMax);
        //setting part one to roughly the middle of the map
        for (int x = 0; x < baseMap.GetLength(0); x++)
        {
            for (int y = 0; y < baseMap.GetLength(1); y++)
            {
                if (baseMap[x, y] > 0)
                    tempmap[x + offset.x, y + offset.y] = baseMap[x, y];
            }
        }
        //making a list of empty tiles to make it easier to check if they overlap
        List<Vector3Int> OverlapList = new List<Vector3Int>();
        for (int x = 0; x < overlappingMap.GetLength(0); x++)
        {
            for (int y = 0; y < overlappingMap.GetLength(1); y++)
            {
                if (overlappingMap[x, y] > 0)
                    OverlapList.Add(new Vector3Int(x, y, overlappingMap[x, y]));
            }
        }

        //calculating htere middel
        int xMid1 = Mathf.CeilToInt(baseMap.GetLength(0) * .5f);
        int yMid1 = Mathf.CeilToInt(baseMap.GetLength(1) * .5f);
        int xMid2 = Mathf.CeilToInt(overlappingMap.GetLength(0) * .5f);
        int yMid2 = Mathf.CeilToInt(overlappingMap.GetLength(1) * .5f);
        offset += new Vector2Int(xMid1 - xMid2, yMid1 - yMid2);

        bool isOverlapping = true;
        
        Vector2Int overlapPos;
        //while loop to keep going untill they don't overlap anymore
        while (isOverlapping == true)
        {
            isOverlapping = false;
            for (int i = 0; i < OverlapList.Count; i++)
            {
                overlapPos = new Vector2Int(OverlapList[i].x + offset.x, OverlapList[i].y + offset.y);
                if (tempmap[overlapPos.x,overlapPos.y] > 0)
                {
                    isOverlapping = true;
                }
            }
            offset.x += dir.x; // dir is the direction for it to go at. taking bolth x and y togeter, 
            offset.y += dir.y; //so there won't have to be one for horzintal and one for vertical
        }
        //from testing going back 1 makes it till it connects and going back 2 connects it on multiple places
        offset.x -= dir.x*2;
        offset.y -= dir.y*2;
        // calculating the offset and returning it
        for (int i = 0; i < OverlapList.Count; i++)
        {
            tempmap[OverlapList[i].x + offset.x, OverlapList[i].y+ offset.y] = OverlapList[i].z;
        }
        offset.x -= xMax;
        offset.y -= yMax;
        return offset;
    }
    //removing uneccecary data from the map
    int[,] TrimMap(int[,] map)
    {
        MinMax mm = GetMinMax(map);
        int xLength = mm.Max.x - mm.Min.x, yLength = mm.Max.y - mm.Min.y;

        int[,] trimmedMap = new int[xLength+2, yLength+2];
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (map[mm.Min.x + x, mm.Min.y + y] > 0)
                    trimmedMap[x+1, y+1] = map[mm.Min.x + x, mm.Min.y + y];
            }
        }
        return trimmedMap;
    }
    //function to get min max not used as much as i chould
    MinMax GetMinMax(int[,] map)
    {
        int xMin = int.MaxValue, yMin = int.MaxValue;
        int xMax = int.MinValue, yMax = int.MinValue;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] > 0)
                {
                    if (x > xMax) xMax = x;
                    if (x < xMin) xMin = x;
                    if (y > yMax) yMax = y;
                    if (y < yMin) yMin = y;
                }
            }
        }
        return new MinMax(xMin, yMin, xMax, yMax);
    }
    //add and exit and keys
    void AddEndpoints()
    {
        //looks for end points and randomly selects one
        if (point == null) return;
        List<Vector2Int> ends = point.GetEndPointsPosition();
        if (ends == null) return;
        Vector2Int endPoint = ends[rng.Next(0, ends.Count)];
        cleanMap[endPoint.x,endPoint.y] = 3;

        //then looks for a location to place the keys
        List<Vector2Int> keys = point.GetKeyPositions(cleanMap,endPoint,keyMinDistance);
        if (keys.Count <= 0) return;
        Vector2Int keyPoint = keys[rng.Next(0, keys.Count)];
        cleanMap[keyPoint.x, keyPoint.y] = 4;
    }
    //takes a random room and then changes the empty spaces to the event's value
    void SetEventToRoom(int eventIndex)
    {
        int eventRoom;

        int index = rng.Next(0, usedRooms.Count);
        eventRoom = usedRooms[index];
        usedRooms.Remove(usedRooms[index]);
        setRoomValue(eventRoom, eventIndex);
    }

    //sets the detailed events
    void SetDetailedEvents()
    {
        //get the regions for events then makes them a clean map and later set it to a detailed event
        List<List<Vector3Int>> eventsList = point.DetectEventRegions(totalMap);
        if (eventsList.Count <= 0) return;
        int[] regionValue = new int[eventsList.Count];
        List<int[,]> eventparts = new List<int[,]>();
        for (int i = 0; i < eventsList.Count; i++)
        {
            int[,] partMap = new int[totalMap.GetLength(0), totalMap.GetLength(1)];
            for (int j = 0; j < eventsList[i].Count; j++)
            {
                Vector3Int pos = eventsList[i][j];
                partMap[pos.x, pos.y] = pos.z;
                if (j == 0) regionValue[i] = pos.z;
            }
            eventparts.Add(partMap);
        }

        //calling the right function for every event
        for (int i = 0; i < eventparts.Count; i++)
        {
            switch (regionValue[i])
            {
                case 2:
                    eventparts[i] = detailedEvent.SetStartingPosition(eventparts[i], 2);
                    break;
                case 5:
                    eventparts[i] = detailedEvent.SetEvent1(eventparts[i], 5);
                    break;
                case 6:
                    eventparts[i] = detailedEvent.SetEvent2(eventparts[i], 6);
                    break;
                case 7:
                    eventparts[i] = detailedEvent.SetEvent3(eventparts[i], 7);
                    break;
                default:
                    break;
            }
        }

        //place parts to  clean map
        foreach (int[,] part in eventparts)
        {
            for (int x = 0; x < part.GetLength(0); x++)
            {
                for (int y = 0; y < part.GetLength(1); y++)
                {
                    if (part[x, y] <= 1) continue;
                    cleanMap[x, y] = part[x, y];
                }
            }
        }
        
    }

    //set's a part to the value of a event
    void setRoomValue(int eventRoom,int eventIndex)
    {
        for (int x = 0; x < maps[eventRoom].GetLength(0); x++)
        {
            for (int y = 0; y < maps[eventRoom].GetLength(1); y++)
            {
                if (maps[eventRoom][x, y] <= 0) continue;
                maps[eventRoom][x, y] = eventIndex;
            }
        }
    }
}