using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using UnityEngine;

public class Lock : Item
{

    [SerializeField]
    Countdown timer;

    public static new float rarity = 20.0f;

    [SerializeField]
    private int count = 0;
    public int beatsDuration = 32;
    public float duration = 10f;

    [SerializeField]
    private bool startCount = false;
    [SerializeField]
    private bool didSetup = false;
    // Use this for initialization
    void Start ()
    {
        timer = gameObject.AddComponent<Countdown> ();
    }

    public override void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        base.Setup (grid, rhythm, gb);
        //if(rhythmSystem_ref!= null)
        //rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (IncreaseCount);
        didSetup = true;

    }
    // Update is called once per frame
    void Update ()
    {
        if (timer.stop && startCount)
        {
            Kill (null);
        }
    }

    public void IncreaseCount ()
    {
        if (!didSetup) return;

        if (startCount)
        {
            if (count < beatsDuration)
            {
                count++;
                //print("count " + count);
            }
            else
            {
                print ("timeout ");
                count = 0;
                startCount = false;

                Kill (null);
            }
        }

    }

    public override void Kill (Item current_Item)
    {
        foreach (var item in grid_ref.itemList.OfType<Lock> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        foreach (GridBlock gb in grid_ref.GetGridBlockList ())
        {
            if (gb.owner == owner)
            {
                gb.changeColor (owner.gridColor);
                if (gb.isLocked)
                    gb.isLocked = false;

            }
            else if (gb.stolenOwner == owner)
            {
                if (gb.owner.item)
                {
                    if (gb.owner.item.GetType () == typeof (Lock))
                    {
                        continue;
                    }

                }

                gb.changeColor (gb.owner.gridColor);
                gb.isLocked = false;

                gb.stolenOwner = null;
            }
            else if (gb.mainColor == owner.blackGridColor)
            {
                gb.changeColor (gb.owner.gridColor);
            }

        }

        owner.hasItem = false;
        owner.item = null;

        base.Kill (null);

    }
    public override bool Activate ()
    {

        // gridBlockOwner.hasItem = false;
        //gameObject.GetComponent<MeshRenderer> ().enabled = false;
        gameObject.GetComponent<BoxCollider> ().enabled = false;

        foreach (GridBlock gb in grid_ref.GetGridBlockList ())
        {
            if (gb.owner == owner)
            {
                if (!gb.isLocked)
                {

                    gb.isLocked = true;
                }

            }

            //change by the color so lipstick colored tiles disguise as locked as well
            if (gb.mainColor == owner.gridColor)
                gb.changeColor (owner.blackGridColor);
        }
        startCount = true;
        timer.startTimer (duration);
        count = 0;
        gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
        return base.Activate ();

    }
    public override void Equip (Player p)
    {
        gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
        base.Equip (p);
        gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);

    }

}