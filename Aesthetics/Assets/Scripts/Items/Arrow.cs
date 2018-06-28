using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

using Aesthetics;
public class Arrow : Item
{

    public enum arrowType
    {
        Single,
        Double,
        Triple,
        Quadruple
    }

    private int count = 0;
    public int beatsPerRotation = 4;
    [SerializeField]
    private CameraScript.windRose orientation;

    [SerializeField]
    public arrowType arrow_type;
    // Use this for initialization
    void Start ()
    {
        orientation = CameraScript.windRose.South;
        //rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (IncreaseCount);

    }

    public void IncreaseCount ()
    {

        if (count < beatsPerRotation)
        {
            count++;
        }
        else
        {
            Rotate ();
            count = 0;
        }

    }

    void Rotate ()
    {

        turnOrientation (true);

    }
    // Update is called once per frame
    void Update ()
    {

    }

    public void turnOrientation (bool clockwise)
    {
        switch (orientation)
        {
            case CameraScript.windRose.North:
                if (clockwise)
                    orientation = CameraScript.windRose.West;
                else
                    orientation = CameraScript.windRose.East;

                break;

            case CameraScript.windRose.East:
                if (clockwise)
                    orientation = CameraScript.windRose.North;
                else
                    orientation = CameraScript.windRose.South;

                break;

            case CameraScript.windRose.South:
                if (clockwise)
                    orientation = CameraScript.windRose.East;
                else
                    orientation = CameraScript.windRose.West;

                break;

            case CameraScript.windRose.West:
                if (clockwise)
                    orientation = CameraScript.windRose.South;
                else
                    orientation = CameraScript.windRose.North;

                break;

            default:
                break;
        }

        if (clockwise)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y -= 90;
            gameObject.transform.DORotate (rot, rhythmSystem_ref.rhythmTarget_Ref.duration);
        }
        else
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.y += 90;
            gameObject.transform.DORotate (rot, rhythmSystem_ref.rhythmTarget_Ref.duration);
        }
    }

    public override void Equip (Player p)
    {

    }
    public override void Activate ()
    {
        base.Activate ();

        bool doNorth = false;
        bool doEast = false;
        bool doSouth = false;
        bool doWest = false;

        switch (arrow_type)
        {
            case arrowType.Single:
                switch (orientation)
                {
                    case CameraScript.windRose.North:
                        doNorth = true;
                        break;
                    case CameraScript.windRose.East:
                        doEast = true;
                        break;
                    case CameraScript.windRose.South:
                        doSouth = true;
                        break;
                    case CameraScript.windRose.West:
                        doWest = true;
                        break;
                    default:
                        break;
                }
                break;

            case arrowType.Double:
                switch (orientation)
                {
                    case CameraScript.windRose.North:
                    case CameraScript.windRose.South:
                        doNorth = true;
                        doSouth = true;
                        break;
                    case CameraScript.windRose.East:
                    case CameraScript.windRose.West:
                        doEast = true;
                        doWest = true;
                        break;

                    default:
                        break;
                }
                break;

            case arrowType.Triple:
                switch (orientation)
                {

                    default : break;
                }
                break;

            case arrowType.Quadruple:

                doNorth = true;
                doSouth = true;
                doEast = true;
                doWest = true;

                break;

            default:
                break;
        }

        foreach (GridBlock gb in grid_ref.GetGridBlockList ())
        {
            if (doNorth) // -- Z
            {

                if (gb.Z <= gridBlockOwner.Z && gb.X == gridBlockOwner.X)
                {
                    if (owner.hasItem && owner.item && owner.item.GetType () == typeof (Lock))
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.blackGridColor);
                            gb.changeOwner (owner);
                        }

                    }
                    else
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.gridColor);
                            gb.changeOwner (owner);
                        }
                    }

                }
            }

            if (doEast) // >> X
            {
                if (gb.X >= gridBlockOwner.X && gb.Z == gridBlockOwner.Z)
                {
                    if (owner.hasItem && owner.item && owner.item.GetType () == typeof (Lock))
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.blackGridColor);
                            gb.changeOwner (owner);
                        }

                    }
                    else
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.gridColor);
                            gb.changeOwner (owner);
                        }
                    }
                }
            }

            if (doSouth) // >> Z
            {

                if (gb.Z >= gridBlockOwner.Z && gb.X == gridBlockOwner.X)
                {
                    if (owner.hasItem && owner.item && owner.item.GetType () == typeof (Lock))
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.blackGridColor);
                            gb.changeOwner (owner);
                        }

                    }
                    else
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.gridColor);
                            gb.changeOwner (owner);
                        }
                    }
                }

            }

            if (doWest) // << X
            {
                if (gb.X <= gridBlockOwner.X && gb.Z == gridBlockOwner.Z)
                {
                    if (owner.hasItem && owner.item && owner.item.GetType () == typeof (Lock))
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.blackGridColor);
                            gb.changeOwner (owner);
                        }

                    }
                    else
                    {
                        if (gb.isLocked == false)
                        {
                            gb.changeColor (owner.gridColor);
                            gb.changeOwner (owner);
                        }
                    }
                }
            }
        }

        foreach (var item in grid_ref.itemList.OfType<Arrow> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        gridBlockOwner.hasItem = false;

        rhythmSystem_ref.getRhythmNoteToPoolEvent ().RemoveListener (IncreaseCount);
        Destroy (gameObject);
    }

}