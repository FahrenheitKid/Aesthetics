using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lock : Item {


	[SerializeField]
    Countdown timer;

	 private int count = 0;
    public int beatsDuration = 32;

	private bool startCount = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	 public void IncreaseCount ()
    {

		if(startCount)
		{
			if (count < beatsDuration)
        {
            count++;
			print("count " + count);
        }
        else
        {
            print("Kill");
            count = 0;
			Kill ();
        }
		}
        

    }

	public override void Kill()
	{
		   foreach (var item in grid_ref.itemList.OfType<Lock> ())
        {
            if (item == this)
            {
                grid_ref.itemList.Remove (item);
                break;
            }

        }

		

		foreach(GridBlock gb in grid_ref.GetGridBlockList())
		{
			if(gb.owner == owner || gb.mainColor == owner.blackGridColor)
			{
				gb.changeColor(owner.gridColor);
				if(gb.isLocked)
				gb.isLocked = false;
				
			}
		}
		
		owner.hasItem = false;
		owner.item = null;

		rhythmSystem_ref.getRhythmNoteToPoolEvent().RemoveListener(IncreaseCount);
        base.Kill();
		Destroy(gameObject);
	}
	public override void Activate ()
    {
        base.Activate ();

		gridBlockOwner.hasItem = false;
		gameObject.GetComponent<MeshRenderer>().enabled = false;

		foreach(GridBlock gb in grid_ref.GetGridBlockList())
		{
			if(gb.owner == owner || gb.mainColor == owner.gridColor)
			{
				if(!gb.isLocked)
				{
					gb.changeColor(owner.blackGridColor);
					gb.isLocked = true;
				}
				

			}
		}
		startCount = true;

	}
	public override void Equip(Player p)
	{
		base.Equip(p);
		

	}


}
