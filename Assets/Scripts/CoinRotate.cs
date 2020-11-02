using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(new Vector3(45, 0, 0)*Time.deltaTime);
    }
}
