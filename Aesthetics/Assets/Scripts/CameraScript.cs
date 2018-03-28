using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    
[SerializeField]
private float targetAngle = 0;
const float rotationAmount = 1.5f;

 [SerializeField, Candlelight.PropertyBackingField]
    private int _rotationSpeed = 0;
    public int rotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
    }


	
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
            targetAngle -= rotationAmount * rotationSpeed;
        }
        if(targetAngle <0)
        {
            transform.RotateAround(transform.position, Vector3.up, rotationAmount);
            targetAngle += rotationAmount* rotationSpeed;
        }
     
    }

    void ZoomOutUntilSeen()
    {

    }
}