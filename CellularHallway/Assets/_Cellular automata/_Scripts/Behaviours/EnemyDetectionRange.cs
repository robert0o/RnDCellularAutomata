using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRange : MonoBehaviour
{
    public EnemyScript host;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        host.player = collision.gameObject;
        host.StartFiring();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        host.player = null;
    }
}
