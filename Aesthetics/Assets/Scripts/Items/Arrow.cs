using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Events;
public class Arrow : Item
{

    public enum arrowType
    {
        Single,
        Double,
        Triple,
        Quadruple
    }

    public static new float rarity = 25.02f;
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

    public override void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        base.Setup (grid, rhythm, gb);
        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, IncreaseCount);

    }

    public void IncreaseCount (KoreographyEvent evt)
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

    public override bool Activate ()
    {

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
                    else if (owner.hasItem && owner.item && owner.item.GetType () == typeof (RainbowLipstick))
                    {
                        List<Player> plist = grid_ref.GetPlayerList ().ToList ();
                        plist.Remove (owner);
                        int idx = Random.Range (0, plist.Count);

                        if (plist.Count > 0 && idx >= 0 && idx < plist.Count)
                        {
                            if (!gb.isLocked)
                            {

                               
                                gb.changeOwner (owner);
                                
                                if(plist[idx].item && plist[idx].item.GetType() == typeof(Lock))
                                {
                                     
                                      gb.changeColor (plist[idx].blackGridColor);
                                }
                                else
                                {
                                    gb.changeColor (plist[idx].gridColor);
                                }
                            }
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
                    else if (owner.hasItem && owner.item && owner.item.GetType () == typeof (RainbowLipstick))
                    {
                        List<Player> plist = grid_ref.GetPlayerList ().ToList ();
                        plist.Remove (owner);
                        int idx = Random.Range (0, plist.Count);

                        if (plist.Count > 0 && idx >= 0 && idx < plist.Count)
                        {
                            if (!gb.isLocked)
                            {

                               
                                gb.changeOwner (owner);
                                
                                if(plist[idx].item && plist[idx].item.GetType() == typeof(Lock))
                                {
                                     
                                      gb.changeColor (plist[idx].blackGridColor);
                                }
                                else
                                {
                                    gb.changeColor (plist[idx].gridColor);
                                }
                            }
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
                    else if (owner.hasItem && owner.item && owner.item.GetType () == typeof (RainbowLipstick))
                    {
                        List<Player> plist = grid_ref.GetPlayerList ().ToList ();
                        plist.Remove (owner);
                        int idx = Random.Range (0, plist.Count);

                        if (plist.Count > 0 && idx >= 0 && idx < plist.Count)
                        {
                            if (!gb.isLocked)
                            {

                               
                                gb.changeOwner (owner);
                                
                                if(plist[idx].item && plist[idx].item.GetType() == typeof(Lock))
                                {
                                     
                                      gb.changeColor (plist[idx].blackGridColor);
                                }
                                else
                                {
                                    gb.changeColor (plist[idx].gridColor);
                                }
                            }
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
                    else if (owner.hasItem && owner.item && owner.item.GetType () == typeof (RainbowLipstick))
                    {
                        List<Player> plist = grid_ref.GetPlayerList ().ToList ();
                        plist.Remove (owner);
                        int idx = Random.Range (0, plist.Count);

                        if (plist.Count > 0 && idx >= 0 && idx < plist.Count)
                        {
                            if (!gb.isLocked)
                            {

                               
                                gb.changeOwner (owner);
                                
                                if(plist[idx].item && plist[idx].item.GetType() == typeof(Lock))
                                {
                                     
                                      gb.changeColor (plist[idx].blackGridColor);
                                }
                                else
                                {
                                    gb.changeColor (plist[idx].gridColor);
                                }
                            }
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

        base.Activate ();
        Kill (null);

        bool gonnaDie = true;
        return gonnaDie;
        //Destroy (gameObject);
    }

    public override void Kill (Item current_Item)
    {

        foreach (var item in grid_ref.itemList.OfType<Arrow> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        Koreographer.Instance.UnregisterForEvents (rhythmSystem_ref.mainBeatID, IncreaseCount);

        base.Kill (current_Item);
    }

}