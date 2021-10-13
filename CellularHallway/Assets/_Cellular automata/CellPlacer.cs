using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPlacer : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public GameObject cellholder;
    public GameObject cell;
    CellScript cellPlacer;
    public void PlaceCells(List<int[,]> maps,int yOffset)
    {
        RemovePreviousCells();

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
    void RemovePreviousCells()
    {
        CellScript[] oldCells = FindObjectsOfType<CellScript>();
        if (oldCells.Length <= 0) return;

        foreach (CellScript oldCell in oldCells)
        {
            Destroy(oldCell.gameObject);
        }
    }
}
