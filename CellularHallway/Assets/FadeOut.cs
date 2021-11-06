using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public SpriteRenderer rend;
    public float fadingTime;
    public bool isFading;
    float alpha;
    void Start()
    {
        alpha = 0;
        Color col = rend.color;
        col.a = alpha;
        rend.color = col;
        isFading = false;
    }
    private void Update()
    {
        if(isFading == true)
        {
            alpha += fadingTime*Time.deltaTime/20;
            Color col = rend.color;
            col.a = alpha;
            rend.color = col;
        }
    }

}
