using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<StairsScript>().keyPickedUp = true;
        this.gameObject.SetActive(false);
    }
}
