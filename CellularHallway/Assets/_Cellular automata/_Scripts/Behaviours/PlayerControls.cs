using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    Vector2 walk;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindObjectOfType<CamScript>().player = this.gameObject;
        FindObjectOfType<CamScript>().setCam();
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
}
