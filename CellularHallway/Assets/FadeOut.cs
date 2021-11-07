using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    public SpriteRenderer rend;
    public float fadingTime;
    public bool isFading, fadeIn;
    float alpha;
    AsyncOperation a;
    void Start()
    {
        Color col = rend.color;
        col.a = 1.2f;
        rend.color = col;
        SwitchFade(true);
    }
    private void Update()
    {
        Color col = rend.color;
        if (isFading == true)
        {
            col.a += 1 / fadingTime * Time.deltaTime;
            if(col.a >= 1)
            {
                col.a = 1;
                isFading = false;
                a.allowSceneActivation = true;
            }
            rend.color = col;
        }
        if (fadeIn == true)
        {
            col.a -= 1 / fadingTime * Time.deltaTime;
            if(col.a <= 0)
            {
                col.a = 0;
                fadeIn = false;
            }
            rend.color = col;
        }
        
    }
    public void SwitchFade(bool fadeingIn)
    {
        if(fadeingIn == true)
        {
            isFading = false;
            fadeIn = true;
        }
        else
        {
            isFading = true;
            fadeIn = false;
            a = SceneManager.LoadSceneAsync(0);
            a.allowSceneActivation = false;
        }
    }

}
