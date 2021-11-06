using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bull : MonoBehaviour
{
    Vector2 dir;
    public void SetBullet(Vector2 direction)
    {
        dir = direction;
        Destroy(this.gameObject, 2);
    }
    void Update()
    {
        transform.Translate(dir * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //DoDamage
    }
}
