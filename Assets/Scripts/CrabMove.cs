using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMove : MonoBehaviour
{
    private float distanceWalked;
    public float changeDir;
    private Vector3 direction;
    public float speed;

    void Start()
    {
        direction = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(distanceWalked >= changeDir)
        {
            distanceWalked = 0f;
            direction = -direction;
        }

        transform.position += direction*speed*Time.deltaTime;
        distanceWalked += speed*Time.deltaTime;
    }
}
