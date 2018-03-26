using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
private float targetAngle = 0;
const float rotationAmount = 1.5f;
public float rSpeed = 1.0f;
	
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
		// Rotation method
        if (Input.GetKeyDown(KeyCode.Q)) {
            targetAngle -= 90.0f;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            targetAngle += 90.0f;
        }
 
        if(targetAngle !=0)
        {
            Rotate();
        }

    }

    public void lookAt (GameObject go)
    {
        transform.LookAt (go.transform);
    }

	protected void Rotate()
    {
     
 
     
        if (targetAngle>0)
        {
            transform.RotateAround(transform.position, Vector3.up, -rotationAmount);
            targetAngle -= rotationAmount;
        }
        if(targetAngle <0)
        {
            transform.RotateAround(transform.position, Vector3.up, rotationAmount);
            targetAngle += rotationAmount;
        }
     
    }
}