using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Aesthetics;
public class FastFoward : Item {


	public static new float rarity = 40.0f;

	// Use this for initialization
	void Start () {
		
	}

	  public override void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        base.Setup (grid, rhythm, gb);
       // rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (IncreaseCount);

    }
	
	 public override bool Activate ()
    {
        base.Activate ();

		rhythmSystem_ref.ChangePitch(true);
		 
		
		Kill(null);
		bool gonnaDie = true;
		return gonnaDie;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	 public override void Kill(Item current_Item){

         foreach (var item in grid_ref.itemList.OfType<FastFoward> ())
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
