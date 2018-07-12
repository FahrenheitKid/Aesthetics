using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using UnityEngine;
public class Revolver : Item
{

    public static new float rarity = 1500.0f;
    Countdown rayRenderTimer;
    public float rayTimeDuration = 0.35f;
    public float stunDuration = 3.5f;
    public LineRenderer lineRend;

    bool alreadyShot = false;
    public CameraScript.windRose? shot_direction;

    // Use this for initialization
    void Start ()
    {
        rayRenderTimer = gameObject.AddComponent<Countdown> ();
        lineRend = GetComponent<LineRenderer> ();
    }

    public override void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        base.Setup (grid, rhythm, gb);
        // rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (IncreaseCount);

    }

    public override bool Activate ()
    {

        return base.Activate ();
        gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);

    }

    public override void Use ()
    {
        //owner.Immune(owner.multiplier * immunity_multiplier);

        Shoot (shot_direction);
        //Kill(null);
    }

    public void Shoot (CameraScript.windRose? shotDir)
    {
		if(shotDir == null) return;
        Vector3 start, end;

        start = grid_ref.getGridBlockPosition (owner.x, owner.z, 1f);
        end = grid_ref.getGridBlockPosition (owner.x, owner.z, 1f);

        switch (shotDir)
        {
            case CameraScript.windRose.North:

                end = grid_ref.getGridBlockPosition (owner.x, 0, 1f);

                break;

            case CameraScript.windRose.South:
                end = grid_ref.getGridBlockPosition (owner.x, grid_ref.mapHeight - 1, 1f);

                break;

            case CameraScript.windRose.West:

                 end = grid_ref.getGridBlockPosition (0, owner.z, 1f);
                break;

            case CameraScript.windRose.East:

                end = grid_ref.getGridBlockPosition (grid_ref.mapWidth - 1, owner.z, 1f);
                break;

        }

        lineRend.SetPosition (0, start);
        lineRend.SetPosition (1, end);

        // make list with all targets in the shot direction

        List<Player> targets = new List<Player> ();

        print("Atirei :" + shot_direction + "| sizelist: " + grid_ref.GetPlayerList ().Count);
        
        foreach (Player p in grid_ref.GetPlayerList ())
        {
            if (p == owner) continue;

            switch (shotDir)
            {

                case CameraScript.windRose.North:

                    if (p.x == owner.x && p.z < owner.z)
                    {
						print("acertei norteeee");
                        targets.Add (p);
                    }

                    break;

                case CameraScript.windRose.South:

                    if (p.x == owner.x && p.z > owner.z)
                    {
                        targets.Add (p);
						print("acertei south");
                    }

                    break;

                case CameraScript.windRose.West:

                    if (p.z == owner.z && p.x < owner.x)
                    {
                        targets.Add (p);
						print("acertei west");
                    }

                    break;

                case CameraScript.windRose.East:
                    if (p.z == owner.z && p.x > owner.x)
                    {
                        targets.Add (p);
						print("acertei east");
                    }
                    break;

            }

            
        }
		
		Player realTarget = null;
		int dis = grid_ref.mapHeight * grid_ref.mapWidth;

		//find closest os the targets
		foreach (Player p in targets)
        {
				if (p == owner) continue;

			if(shotDir == CameraScript.windRose.North || shotDir == CameraScript.windRose.South)
			{
				if (Mathf.Abs(p.z - owner.z) < dis)
                    {
						realTarget = p;
						dis = Mathf.Abs(p.z - owner.z);
                    }
			}
			else
			{
				if (Mathf.Abs(p.x - owner.x) < dis)
                    {
						realTarget = p;
						dis = Mathf.Abs(p.x - owner.x);
                    }
			}
			
		}


		if(realTarget != null)
		{
			// stun and steal realTarget
			  realTarget.Stun (stunDuration);
			  owner.Steal(realTarget, true);
		}

        lineRend.enabled = true;
        rayRenderTimer.startTimer (rayTimeDuration);

        alreadyShot = true;
		owner.isAiming = false;

    }

    public override void Equip (Player p)
    {

        //rhythmSystem_ref.ChangePitch(false);
        base.Equip (p);

        gameObject.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);

    }

    // Update is called once per frame
    void Update ()
    {
        if (rayRenderTimer.stop && alreadyShot)
        {
            lineRend.enabled = false;
            Kill (null);
        }
    }

    public override void Kill (Item current_Item)
    {

        foreach (var item in grid_ref.itemList.OfType<Revolver> ())
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
        //Destroy(owner.GetComponent<LineRenderer>());
        base.Kill (current_Item);
    }
}