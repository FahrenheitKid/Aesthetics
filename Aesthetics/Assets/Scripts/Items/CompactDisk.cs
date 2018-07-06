using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Aesthetics;
public class CompactDisk : Item {

	public static new float rarity = 90.0f;

	public float immunity_multiplier = 6.0f;

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
       

		
		return  base.Activate ();
		gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);

	}

	public override void Use()
	{
		owner.Immune(owner.multiplier * immunity_multiplier);
		Kill(null);
	}

	 public override void Equip (Player p)
    {
       

		//rhythmSystem_ref.ChangePitch(false);
		base.Equip(p);
		
		
		gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	 public override void Kill(Item current_Item){

         foreach (var item in grid_ref.itemList.OfType<CompactDisk> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

       if(owner)
	   {
		
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
