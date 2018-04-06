using System.Collections;
using UnityEngine;

public class RotateItself : MonoBehaviour
{

    public bool rotatex = false;
    public bool rotatey = false;
    public bool rotatez = false;
    public int speed = 20;
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        int xval = 0;
        int yval = 0;
        int zval = 0;

        if (rotatex)
        {
            xval = 1;
        }

        if (rotatey)
        {
            yval = 1;
        }

        if (rotatez)
        {
            zval = 1;
        }

        transform.Rotate (xval * speed * Time.deltaTime, yval * speed * Time.deltaTime, zval * speed * Time.deltaTime);
    }
}