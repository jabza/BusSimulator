﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public  Transform   target;
	public  int         zoomSpeed = 4;
    public  int         zoomIncrement = 4;
    public  int         zoomCap = 64;

    private int         zoomTarget = 8;
    private Camera      camera;

	void Awake()
	{
        this.camera = GetComponent<Camera>();
	}

	void Update()
	{
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
            zoomTarget += zoomIncrement;
		else if(Input.GetAxis("Mouse ScrollWheel") > 0)
            zoomTarget -= zoomIncrement;

        if(zoomTarget < zoomIncrement)
            zoomTarget = zoomIncrement;
        else if(zoomTarget > zoomCap)
            zoomTarget = zoomCap;

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomTarget, Time.deltaTime * zoomSpeed);

        if(target)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.rotation = target.rotation;
        }
	}
}
