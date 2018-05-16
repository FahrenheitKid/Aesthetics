using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    [SerializeField]
    protected TheGrid _grid_ref;
    public TheGrid grid_ref
    {
        get
        {
            return _grid_ref;
        }
        set
        {
            _grid_ref = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected int _x;
    public int x
    {
        get
        {
            return _x;
        }
        set
        {
            _x = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected int _y;
    public int y
    {
        get
        {
            return _y;
        }
        set
        {
            _y = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected int _z;
    public int z
    {
        get
        {
            return _z;
        }
        set
        {
            _z = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected Player _owner;
    public Player owner
    {
        get
        {
            return _owner;
        }
        set
        {
            _owner = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected GridBlock _gridBlockOwner;
    public GridBlock gridBlockOwner
    {
        get
        {
            return _gridBlockOwner;
        }
        set
        {
            _gridBlockOwner = value;
        }
    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    protected void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.CompareTag ("Player"))
        {
            owner = other.gameObject.GetComponent<Player> ();

            //activate in case it is not an equippable item
            Activate ();
            //Equip(owner);
        }

    }

    public virtual void Use ()
    {
        print ("Base item used");
    }

    public virtual void Activate ()
    {
        // print ("Base item activated");
    }

    public virtual void Equip (Player p)
    {
        if (p.hasItem)
            Destroy (p.item);

        p.hasItem = true;
        p.item = this;
        owner = p;
        print ("Base item equiped");
    }
}