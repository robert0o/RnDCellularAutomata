using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public TextMesh text;
    public Color[] colors = new Color[3];
    public SpriteRenderer sprite;

    public void InitCell(int cellType,int distanceValue, Vector3 pos)
    {
        switch (cellType)
        {
            case 0:
                sprite.color = colors[0];
                text.text = "" + distanceValue.ToString();
                break;
            case 2:
                sprite.color = colors[2];
                text.text = "" + distanceValue.ToString();
                break;
            case 1:
                sprite.color = colors[1];
                text.gameObject.SetActive(false);
                break;

        }
        transform.position = pos;
    }
}
