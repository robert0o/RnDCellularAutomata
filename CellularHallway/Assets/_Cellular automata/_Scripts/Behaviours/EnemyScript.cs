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

    public GameObject healthBar;
    public int maxHp;
    public int currentHp;

    void Start()
    {
        isFiring = false;
        currentHp = maxHp;
    }
    public void StartFiring()
    {
        if (isFiring == false)
        {
            StartCoroutine(firing());
        }

    }
    IEnumerator firing()
    {
        isFiring = true;
        while (player != null)
        {
            FireBullet();
            yield return new WaitForSeconds(1 / aps);
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

    void TakingDamage()
    {
        currentHp -= 1;
        if (currentHp < 0) currentHp = 0;
        Vector3 scale = healthBar.transform.localScale;
        scale.x *= ((float)currentHp / (float)maxHp);
        healthBar.transform.localScale = scale;
        if (currentHp == 0)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            TakingDamage();
        }
    }
}
