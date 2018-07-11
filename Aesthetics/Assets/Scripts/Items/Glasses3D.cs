using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using UnityEngine;
public class Glasses3D : Item
{

    public static new float rarity = 30.03f;

    public int multiplier_increase = 3;

    // Use this for initialization
    void Start ()
    {

    }

    public override void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        base.Setup (grid, rhythm, gb);
        // rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (IncreaseCount);

    }

    public override bool Activate ()
    {

        //rhythmSystem_ref.ChangePitch(false);
        //owner.isShielded = true;
        //gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
        base.Activate ();

        owner.multiplier += 3;
        grid_ref.GetCameraScript ().Rotate ((Random.value > 0.5f));
        Kill (null);
        bool gonnaDie = true;
        return gonnaDie;

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public override void Kill (Item current_Item)
    {

        foreach (var item in grid_ref.itemList.OfType<Glasses3D> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        base.Kill (current_Item);
    }
}