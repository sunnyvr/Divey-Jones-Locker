using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;

    private float upRotation = 0;
    private GameObject player;

    private float waterLevel = 28.5f;
    public bool isUnderwater;
    public Color32 underwaterCol;
    
    void Start()
    {
        player = this.transform.parent.gameObject;
        Cursor.lockState = CursorLockMode.Locked; // hide cursor
        underwaterCol = new Color32(29, 51, 140, 128);
    }

    void Update()
    {
        isUnderwater = transform.position.y < waterLevel;
        if(!isUnderwater) RenderSettings.fog = false;

        if(isUnderwater)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = underwaterCol;
            RenderSettings.fogDensity = 0.01f;
        }

        // looks up but can't rotate too far
        upRotation -= Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        upRotation = Mathf.Clamp(upRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(upRotation, 0f, 0f);

        // look around
        float x = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        player.transform.Rotate(Vector3.up*x);
    }
}
