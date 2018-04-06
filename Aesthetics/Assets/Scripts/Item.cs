using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

 [SerializeField, Candlelight.PropertyBackingField]
		private int _x;
	    public int X
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
		private int _y;
    public int Y
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
		private int _z;
    public int Z
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



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void Use()
	{

	}
}
