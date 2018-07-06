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



    public override bool Activate ()
    {
        base.Activate ();
        //GameObject.FindGameObjectWithTag ("Grid").GetComponent<TheGrid> ().Score (owner);
        grid_ref.Score (owner);
        //owner.hasItem = false;
        grid_ref.updateScoreMakerSpawnRatio();

        bool gonnaDie = true;
		
        Kill(null);

        return gonnaDie;
        //Destroy (gameObject);
    }

    public override void Kill(Item current_Item){

         foreach (var item in grid_ref.itemList.OfType<ScoreMaker> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

       

        base.Kill(current_Item);
    }

}