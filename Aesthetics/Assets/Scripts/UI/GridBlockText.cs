using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridBlockText : MonoBehaviour
{

    public TextMeshPro textMeshPro_ref;
    public GridBlock gridBlock_ref;
    public bool isDone = false;

    [Range (0.1f, 3f)]
    public float duration;
    [Range (1f, 72f)]
    public float fontSize = 10;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

        if (isDone)
            Destroy (this.gameObject);
    }
}