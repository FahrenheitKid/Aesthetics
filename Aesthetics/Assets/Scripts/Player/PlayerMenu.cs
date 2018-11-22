using System.Collections;
using System.Collections.Generic;
using Aesthetics;
using UnityEngine;
public class PlayerMenu : ScriptableObject
{

    public int ID;
    public Player.InputType controllerType;
    public int inputID;
    public Player.Character character;

    [SerializeField]
    public Color32[] colorPrim = new Color32[2];
    [SerializeField]
    public Color32[] colorSec = new Color32[2];
    [SerializeField]
    public Color32[] colorTert = new Color32[2];

    public GridBlock.gridBlockColor gridblockColor;

    public GridBlock.gridBlockColor blackGridblockColor;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
}