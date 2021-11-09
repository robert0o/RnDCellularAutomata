using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject shield;
    int shieldHP = 4;
    int HP = 4;
    int currentHP, currentShield;
    public float speed;
    Vector2 walk;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //set the payer instance of the cam to this player and setting it's position
        FindObjectOfType<CamScript>().player = this.gameObject;
        FindObjectOfType<CamScript>().setCam();
        currentHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        Walking();
        Vector3 zlayer = transform.position;
        zlayer.z = zlayer.y - .6f;
        transform.position = zlayer;
    }
    void Walking()
    {
        //making a movement direction Vector with input.
        walk = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        { walk += Vector2.up; }
        if (Input.GetKey(KeyCode.S))
        { walk += Vector2.down; }
        if (Input.GetKey(KeyCode.A))
        { walk += Vector2.left; }
        if (Input.GetKey(KeyCode.D))
        { walk += Vector2.right; }
        walk.Normalize();
        walk *= speed;
        rb.velocity = walk;
    }

    public void SetShield()
    {
        //makes the shield active
        shield.SetActive(true);
        currentShield = shieldHP;
    }
    public void TakingDamage()
    {
        //when taking damage
        if (currentShield > 0)
            currentShield--;
        else
            currentHP--;

        if(currentHP == 0)
        {
            //death stuff
        }
    }
}
