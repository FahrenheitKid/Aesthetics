using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    public float speed = 4f;


    Vector3 foward, right;
	// Use this for initialization
	void Start () {

        foward = Camera.main.transform.forward;
        foward.y = 0;
        foward = Vector3.Normalize(foward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * foward;
	}
	
	// Update is called once per frame
	void Update () {
        
            Move();
	}

    void Move()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.back * speed * Time.deltaTime;
        }
    }
}
