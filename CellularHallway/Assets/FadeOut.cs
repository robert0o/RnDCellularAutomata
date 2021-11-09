using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour //basic way for a fade-in and out screen
{
    public SpriteRenderer rend;
    public float fadingTime;
    public bool isFading, fadeIn;
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
        //for fading from transparent to black
        if (isFading == true)
        {
            col.a += 1 / fadingTime * Time.deltaTime;
            if(col.a >= 1)
            {
                col.a = 1;
                isFading = false;
                //allowing the preloaded scene to be loaded
                a.allowSceneActivation = true;
            }
            rend.color = col;
        }
        //for fadign from black to transparent
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
            //preloading next scene. it's the same scene but will have a new random seed
            a = SceneManager.LoadSceneAsync(0);
            a.allowSceneActivation = false;
        }
    }

}
