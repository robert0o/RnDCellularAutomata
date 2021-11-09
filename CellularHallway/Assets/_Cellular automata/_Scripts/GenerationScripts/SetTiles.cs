using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTiles : MonoBehaviour
{
    //Not very clean but it works
    public GameObject tilePrefeb;
    public GameObject stairs;
    public GameObject key;
    public GameObject bandit;
    public GameObject tresureMap;
    public GameObject chest;
    public GameObject boss;
    public GameObject player;

    bool hasBoss = false;
    Vector2Int bossPos;

    //Calling this will instantiate all tiles
    public void SetMap(int[,] map)
    {
        //first is getting all floor and wall tiles then instantiate them
        List<Vector3Int> tileList = getTiles(map);
        for (int i = 0; i < tileList.Count; i++)
        {
            CreateFloornWalls(tileList[i]);
        }
        //same with things like the player, chests, ect.
        List<Vector3Int> EntityList = getEntities(map);
        for (int i = 0; i < EntityList.Count; i++)
        {
            CreateEntities(EntityList[i]);
        }
        //in case bosstiles got placed. is always 5*5 so -2 of the middle
        if(hasBoss == true)
        {
            bossPos = new Vector2Int(bossPos.x - 2, bossPos.y - 2);
            Instantiate(boss).transform.position = new Vector3(bossPos.x, bossPos.y + .5f, bossPos.y - .5f);
        }
    }
    //instantiates the floor and walls
    void CreateFloornWalls(Vector3Int tile)
    {
        TileScript tileScript;
        tileScript = Instantiate(tilePrefeb).GetComponent<TileScript>();
        float depth = (tile.z == 0||tile.z ==1) ? 100 : -.7f; //floor tiles are set far beck so they won't render in front of anything
        Vector3 pos = new Vector3(tile.x, tile.y, tile.y + depth) ;
        tileScript.SetSprite(tile.z);
        tileScript.transform.position = pos;
    }
    //instantiates the other things
    void CreateEntities(Vector3Int tile)
    {
        if (tile.z == 2)
        {
            //player
            Instantiate(player).transform.position = new Vector3(tile.x, tile.y, tile.y -.6f);
        }
        if (tile.z == 3)
        {
            //stairs
            Instantiate(stairs).transform.position = new Vector3(tile.x, tile.y, tile.y +.1f);
        }
        if (tile.z == 4)
        {
            //key
            Instantiate(key).transform.position = new Vector3(tile.x, tile.y, tile.y - .5f);
        }
        if (tile.z == 5)
        {
            //bandit, these are plact in a '+' pattarn
            Instantiate(tresureMap).transform.position = new Vector3(tile.x, tile.y, tile.y - .5f);
            for (int i = 0; i < DirArray.directions.Length; i++)
            {
                Vector2 newVector = new Vector2(tile.x, tile.y) + new Vector2(DirArray.directions[i].x, DirArray.directions[i].y);
                Instantiate(bandit).transform.position = new Vector3(newVector.x, newVector.y +.3f, newVector.y - .5f);
            }
        }
        if (tile.z == 6)
        {
            //chest
            Instantiate(chest).transform.position = new Vector3(tile.x, tile.y, tile.y - .5f);
        }
        if (tile.z == 7)
        {
            //boss
            hasBoss = true;
            bossPos = new Vector2Int(tile.x, tile.y);
        }
    }

    //makes a list of everything that is not a wall or empty tile
    List<Vector3Int> getEntities(int[,] map)
    {
        List<Vector3Int> entityList = new List<Vector3Int>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if(map[x,y] > 1)
                {
                    entityList.Add(new Vector3Int(x, y, map[x, y]));
                }
            }
        }
        return entityList;
    }
    //makes a list of floor and wall tiles to place later the .z is for what sprite to use
    //0 = normal floor tile, 1 = boss floor tile, 2 = black wall sprite and 3 = brick wall sprite
    List<Vector3Int> getTiles(int[,] map)
    {
        List<Vector3Int> tileList = new List<Vector3Int>();
        bool IsWallAfterFloor = false;                                              //To know what sprite to use
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                //Tiles that are empty or simply have an entitiy on top of it.
                if (map[x, y] == 1 || map[x, y] == 2 || map[x, y] == 4 || map[x, y] == 5 || map[x, y] == 6)
                {
                    tileList.Add(new Vector3Int(x, y, 0));
                    IsWallAfterFloor = true;
                }
                //for the stairs doesn't need a floor tile
                if (map[x, y] == 3)
                {
                    IsWallAfterFloor = true;
                }
                if (map[x, y] <= 0)
                {
                    //if the tile in front of the wall is a floor tile it will use the brick wall as sprite.
                    if (IsWallAfterFloor == true)
                    {
                        tileList.Add(new Vector3Int(x, y, 3));
                        IsWallAfterFloor = false;
                    }
                    //else it will use the black wall sprite
                    else
                    {
                        //Looping to see if there is a floor tile above or next to it. else it won't be added as it's not needed
                        for (int i = 0; i < DirArray.directions.Length; i++)
                        {
                            Vector2Int pos = new Vector2Int(x, y) + DirArray.directions[i];
                            if (pos.x < 0 || pos.x >= map.GetLength(0) || pos.y < 0 || pos.y > map.GetLength(1)-1) { continue; }
                            if (map[pos.x, pos.y] > 0)
                            {
                                tileList.Add(new Vector3Int(x, y, 2));
                                break;
                            }
                        }
                    }
                }
                //tiles for the boss area
                if (map[x, y] == 7)
                {
                    tileList.Add(new Vector3Int(x, y, 1));
                    IsWallAfterFloor = true;
                }
            }
        }
        return tileList;
    }
}
