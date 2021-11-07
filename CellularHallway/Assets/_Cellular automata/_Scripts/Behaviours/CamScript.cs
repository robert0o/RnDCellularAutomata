using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public GameObject player;
    private void Update()
    {
        if (player != null)
        {
            Vector3 target = player.transform.position;
            target.z = transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position, target, 1);
        }
    }
    public void setCam()
    {
        Vector3 target = player.transform.position;
        target.z = transform.position.z;
        transform.position = target;
    }
}
