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

    public void Start()
    {
        point = new ConnectionPoinSearch();

    }
    public void PlaceCells(List<Hallway> _hallways)
    {
        CellScript[] oldCells = FindObjectsOfType<CellScript>();
        if (oldCells.Length > 0)
        {
            foreach (CellScript oldCell in oldCells)
            {
                Destroy(oldCell.gameObject);
            }
        }
        CellScript cellPlacer;
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
                cellPlacer.InitCell(0, nodes[j].distancevalue, new Vector2(nodes[j].x + exOffset, nodes[j].y + Yoffset));
            }
            exOffset += Xoffset;
        }
    }
}
