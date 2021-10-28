using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public TextMesh text;
    public Color[] colors = new Color[3];
    public SpriteRenderer sprite;

    public void InitCell(int distanceValue, Vector3 pos)
    {
        if(distanceValue == 0) {
            sprite.color = colors[0];
            text.text = "" + distanceValue.ToString();
        }
        else if (distanceValue == 1) {
            sprite.color = colors[1];
            text.text = "1";
        }
        else if (distanceValue == 2)
        {
            sprite.color = colors[2];
            text.text = "2";
        }
        else if (distanceValue == 4)
        {
            sprite.color = colors[4];
            text.text = "4";
        }
        else if (distanceValue < short.MaxValue) {
            sprite.color = colors[2];
            text.text = "" + distanceValue.ToString();
        }
        else if (distanceValue > short.MaxValue) {
            sprite.color = colors[3];
            int newDistV = distanceValue - short.MaxValue;
            text.text = "" + newDistV.ToString();
        }
        transform.position = pos;
    }
}
