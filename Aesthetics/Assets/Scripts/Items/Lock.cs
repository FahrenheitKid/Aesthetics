using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Aesthetics;

public class Lock : Item
{

    [SerializeField]
    Countdown timer;

    public static new float rarity = 50.0f;

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
        if(timer.stop && startCount)
        {
            Kill(null);
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

        if (current_Item != null)
        {
            if (current_Item.GetType () != typeof (Lock))
            {
                print ("KILL DIFERENTE");
                foreach (GridBlock gb in grid_ref.GetGridBlockList ())
                {
                    if (gb.owner == owner || gb.mainColor == owner.blackGridColor)
                    {
                        gb.changeColor (owner.gridColor);
                        if (gb.isLocked)
                            gb.isLocked = false;

                    }
                }

            }
        }
        else
        {

            foreach (GridBlock gb in grid_ref.GetGridBlockList ())
            {
                if (gb.owner == owner || gb.mainColor == owner.blackGridColor)
                {
                    gb.changeColor (owner.gridColor);
                    if (gb.isLocked)
                        gb.isLocked = false;

                }
            }

            print ("KILL null");
        }

        owner.hasItem = false;
        owner.item = null;

        rhythmSystem_ref.getRhythmNoteToPoolEvent ().RemoveListener (IncreaseCount);

        if (gameObject)
            Destroy (gameObject);
    }
    public override void Activate ()
    {
        base.Activate ();

        gridBlockOwner.hasItem = false;
        //gameObject.GetComponent<MeshRenderer> ().enabled = false;
        gameObject.GetComponent<BoxCollider> ().enabled = false;

        foreach (GridBlock gb in grid_ref.GetGridBlockList ())
        {
            if (gb.owner == owner || gb.mainColor == owner.gridColor)
            {
                if (!gb.isLocked)
                {
                    gb.changeColor (owner.blackGridColor);
                    gb.isLocked = true;
                }

            }
        }
        startCount = true;
        timer.startTimer(duration);
        count = 0;

    }
    public override void Equip (Player p)
    {
        base.Equip (p);

    }

}