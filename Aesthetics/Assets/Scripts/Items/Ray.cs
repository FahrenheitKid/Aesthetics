using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using UnityEngine;
public class Ray : Item
{

    public enum rayType
    {
        HRay,
        Vray
    }

    public static new float rarity = 50.05f;
    public rayType ray_type;
    Countdown rayRenderTimer;
    public float rayTimeDuration = 0.35f;
    public float stunDuration = 3.5f;
    public LineRenderer lineRend;

    bool alreadyShot = false;

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

        Shoot ();
        //Kill(null);
    }

    public void Shoot ()
    {

        if (ray_type == Ray.rayType.Vray)
        {

            Vector3 start, end;

            start = grid_ref.getGridBlockPosition (owner.x, 0, 1f);
            end = grid_ref.getGridBlockPosition (owner.x, grid_ref.mapHeight - 1, 1f);

            lineRend.SetPosition (0, start);
            lineRend.SetPosition (1, end);

            foreach (Player p in grid_ref.GetPlayerList ())
            {
                if (p == owner) continue;
                if (p.x == owner.x)
                {
                    p.Stun (stunDuration);
                }
            }

            lineRend.enabled = true;
            rayRenderTimer.startTimer (rayTimeDuration);
        }
        else if (ray_type == Ray.rayType.HRay)
        {
            Vector3 start, end;

            start = grid_ref.getGridBlockPosition (0, owner.z, 1f);
            end = grid_ref.getGridBlockPosition (grid_ref.mapWidth - 1, owner.z, 1f);

            lineRend.SetPosition (0, start);
            lineRend.SetPosition (1, end);

            foreach (Player p in grid_ref.GetPlayerList ())
            {
                if (p == owner) continue;
                if (p.z == owner.z)
                {
                    p.Stun (stunDuration);
                }
            }

            lineRend.enabled = true;
            rayRenderTimer.startTimer (rayTimeDuration);

        }

        alreadyShot = true;
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

        foreach (var item in grid_ref.itemList.OfType<Ray> ())
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