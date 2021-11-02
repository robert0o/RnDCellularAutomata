using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTiles : MonoBehaviour
{
    public GameObject tilePrefeb;
    public void SetMap(int[,] map)
    {
        List<Vector3Int> tileList = getTiles(map);
        TileScript tilesc;
        for (int i = 0; i < tileList.Count; i++)
        {
            tilesc = Instantiate(tilePrefeb).GetComponent<TileScript>();
            Vector3 pos = new Vector3(tileList[i].x, tileList[i].y, tileList[i].y);
            tilesc.SetSprite(tileList[i].z);
            tilesc.transform.position = pos;
        }
    }
    List<Vector3Int> getTiles(int[,] map)
    {
        List<Vector3Int> tileList = new List<Vector3Int>();
        bool IsWallAfterFloor = false;
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 1 || map[x, y] == 2 || map[x, y] == 4 || map[x, y] == 5 || map[x, y] == 6)
                {
                    tileList.Add(new Vector3Int(x, y, 0));
                    IsWallAfterFloor = true;
                }
                if (map[x, y] <= 0)
                {
                    if (IsWallAfterFloor == true)
                    {
                        tileList.Add(new Vector3Int(x, y, 3));
                        IsWallAfterFloor = false;
                    }
                    else
                    {
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
                if (map[x, y] == 7)
                {
                    tileList.Add(new Vector3Int(x, y, 1));
                    IsWallAfterFloor = true;
                }
                if (map[x, y] == 3)
                {
                    tileList.Add(new Vector3Int(x, y, 4));
                }
            }
        }
        return tileList;
    }
}
