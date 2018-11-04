using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using UnityEngine;

public class Sneakers : Item
{

    [SerializeField]
    Countdown timer;

    public static new float rarity = 20.03f;

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
        foreach (var item in grid_ref.itemList.OfType<Sneakers> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        if (owner)
        {

            if (owner.hasItem)
            {
                if (owner.item == this)
                {
                    owner.item = null;
                    owner.hasItem = false;
                }
            }

        }

        base.Kill (null);
    }
    public override bool Activate ()
    {

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