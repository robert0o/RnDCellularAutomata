using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickupScript : MonoBehaviour
{
    bool pickedUp;
    int loops;
    void Update()
    {
        if(pickedUp == true)
        {
            //a small animation for the shield
            transform.Translate(Vector3.up * Time.deltaTime / 2);
            loops++;
        }
        //little of an ugly way to set a time but it's fine for this
        if(loops >= 30)
        {
            FindObjectOfType<PlayerControls>().SetShield();
            this.gameObject.SetActive(false);
        }
    }
    public void PickedUp()
    {
        pickedUp = true;
        loops = 0;
    }
}
