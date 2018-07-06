using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Aesthetics;
public class FloppyDisk : Item {

	public static new float rarity = 90.0f;

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
       

		//rhythmSystem_ref.ChangePitch(false);
		 owner.isShielded = true;
		
		return  base.Activate ();

	}

	 public override void Equip (Player p)
    {
       

		//rhythmSystem_ref.ChangePitch(false);
		base.Equip(p);
		if(!owner.isShielded)
		 owner.isShielded = true;
		
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	 public override void Kill(Item current_Item){

         foreach (var item in grid_ref.itemList.OfType<FloppyDisk> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

       if(owner)
	   {
		   if(owner.isShielded)
	   		owner.isShielded = false;

			   if(owner.hasItem)
			   {
				   if(owner.item == this)
				   	{
						   owner.item = null;
						   owner.hasItem = false;
					   }
			   }

	   }

        base.Kill(current_Item);
    }
}
