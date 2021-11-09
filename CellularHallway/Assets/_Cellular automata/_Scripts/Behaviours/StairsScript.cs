using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsScript : MonoBehaviour
{
    public bool keyPickedUp;
    public Sprite lockedDoor;
    public Sprite stairs;
    SpriteRenderer rend;
    private void Start()
    {
        keyPickedUp = false;
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = lockedDoor;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //change to stairs prite when the key got picked up
        if (keyPickedUp == true)
        {
            rend.sprite = stairs;
            //start the fading to black and loading of the scene
            FindObjectOfType<FadeOut>().SwitchFade(false);
        }
    }
}
