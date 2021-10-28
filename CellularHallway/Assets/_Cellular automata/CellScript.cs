using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public TextMesh text;
    public EventValues EV;
    public SpriteRenderer sprite;

    public void InitCell(int cellType, Vector3 pos)
    {
        if (cellType > EV.eventList.Length - 1) return;
        sprite.color = EV.eventList[cellType].eventColor;
        text.text = "" + cellType.ToString();
        transform.position = pos;
    }
}
