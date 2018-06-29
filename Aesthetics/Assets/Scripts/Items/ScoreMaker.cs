using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Aesthetics;
public class ScoreMaker : Item
{

    // Use this for initialization
    void Start ()
    {
        //rarity = 100;
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public override void Equip (Player p)
    {

    }

    public override void Activate ()
    {
        base.Activate ();
        //GameObject.FindGameObjectWithTag ("Grid").GetComponent<TheGrid> ().Score (owner);
        grid_ref.Score (owner);
        //owner.hasItem = false;
        gridBlockOwner.hasItem = false;

        foreach (var item in grid_ref.itemList.OfType<ScoreMaker> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

        grid_ref.updateItemSpawnRatio();
        base.Kill(null);
        //Destroy (gameObject);
    }

}