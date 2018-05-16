using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBackup : MonoBehaviour
{

    [SerializeField]
    public float speed = 4f;

    Vector3 foward, right;
    // Use this for initialization
    void Start ()
    {

        foward = Camera.main.transform.forward;
        foward.y = 0;
        foward = Vector3.Normalize (foward);
        right = Quaternion.Euler (new Vector3 (0, 90, 0)) * foward;
    }

    // Update is called once per frame
    void Update ()
    {

        if (Input.GetButton ("Horizontal") || Input.GetButton ("Vertical"))
            Move ();
    }

    void Move ()
    {

        //assuming we only using the single camera:
        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = Quaternion.Euler (new Vector3 (0, 45, 0)) * camera.transform.forward;
        var right = Quaternion.Euler (new Vector3 (0, 45, 0)) * camera.transform.right * -1;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize ();
        right.Normalize ();

        Vector3 rightMovement = right * speed * Time.deltaTime * Input.GetAxis ("Vertical");
        Vector3 upMovement = forward * speed * Time.deltaTime * Input.GetAxis ("Horizontal");

        Vector3 heading = Vector3.Normalize (rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;

    }

}