using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureChestScript : MonoBehaviour
{
    public GameObject shieldItem;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponentInChildren<ShieldPickupScript>().PickedUp();
    }
}
