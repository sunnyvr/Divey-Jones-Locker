using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleMove : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        target = transform.Find("WhaleTarget").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.position, Vector3.up, 20*Time.deltaTime);
    }
}
