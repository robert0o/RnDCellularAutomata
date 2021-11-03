using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;

    public float bulletspeed;
    public GameObject bulletPrefab;

    public float aps;
    bool isFiring = false;

    void Start()
    {
        isFiring = false;
    }
    public void StartFiring()
    {
        if(isFiring == false)
        {
            StartCoroutine(firing());
        }
        
    }
    IEnumerator firing()
    {
        isFiring = true;
        while( player != null)
        {
            FireBullet();
            yield return new WaitForSeconds(1/aps);
        }
        isFiring = false;
    }
    public void FireBullet()
    {
        Bull bullet = Instantiate(bulletPrefab).GetComponent<Bull>();
        bullet.transform.position = transform.position;
        Vector2 dir = player.transform.position - bullet.transform.position;
        dir.Normalize();
        dir *= bulletspeed;
        bullet.SetBullet(dir);
    }
}
