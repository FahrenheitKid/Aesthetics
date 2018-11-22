using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GifScript : MonoBehaviour
{

    public Texture2D[] frames;
    public float framesPerSecond = 10.0f;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        int index = (int) (Time.time * framesPerSecond);
        if (frames.Length > 0)
            index = index % frames.Length;
        GetComponent<Renderer> ().material.mainTexture = frames[index];
    }
}