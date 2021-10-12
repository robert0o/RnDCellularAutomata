using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPlacer : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public GameObject cellholder;
    ConnectionPoinSearch point;
    List<Hallway> hallways;
    List<Node> nodes;
    List<List<Node>> pathedHallways;
    public GameObject cell;
    CellScript cellPlacer;
    public void Start()
    {
        point = new ConnectionPoinSearch();

    }
    /*public void PlaceCells(List<Hallway> _hallways)
    {
        CellScript[] oldCells = FindObjectsOfType<CellScript>();
        if (oldCells.Length > 0)
        {
            foreach (CellScript oldCell in oldCells)
            {
                Destroy(oldCell.gameObject);
            }
        }
        ;
        pathedHallways = new List<List<Node>>();
        hallways = _hallways;
        foreach (Hallway hall in hallways)
        {
            nodes = point.FindPaths(hall);
            pathedHallways.Add(nodes);
        }

        int exOffset = 0;
        for (int i = 0; i < pathedHallways.Count; i++)
        {
            nodes = pathedHallways[i];
            int Xoffset = (hallways[i].hWidth + 4);
            int Yoffset = mapGenerator.hight + 4;
            for (int j = 0; j < nodes.Count; j++)
            {
                cellPlacer = Instantiate(cell,cellholder.transform).GetComponent<CellScript>();
                cellPlacer.InitCell(nodes[j].distancevalue, new Vector3(nodes[j].x + exOffset, nodes[j].y + Yoffset, -9));
            }
            exOffset += Xoffset;
        }
    }//*/
    public void PlaceCells(List<int[,]> maps,int yOffset)
    {

        CellScript[] oldCells = FindObjectsOfType<CellScript>();
        if (oldCells.Length > 0)
        {
            foreach (CellScript oldCell in oldCells)
            {
                Destroy(oldCell.gameObject);
            }
        }
        int xOffset = 0;
        for (int i = 0; i < maps.Count; i++)
        {
            PlaceMapCells(maps[i], xOffset, yOffset + 4);
            xOffset += maps[i].GetLength(0) + 4;
        }
    }
    void PlaceMapCells(int[,] map, int xOffset, int yOffset)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                cellPlacer = Instantiate(cell, cellholder.transform).GetComponent<CellScript>();
                cellPlacer.InitCell(map[x,y], new Vector3(x + xOffset, y + yOffset, -9));
            }
        }
    }
}
