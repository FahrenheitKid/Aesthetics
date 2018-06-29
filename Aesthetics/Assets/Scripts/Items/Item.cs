using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aesthetics{




public class Item : MonoBehaviour, System.IComparable<Item>
{

     public int CompareTo(Item obj) {
           // return Less than zero if this object 
           // is less than the object specified by the CompareTo method.

           // return Zero if this object is equal to the object 
           // specified by the CompareTo method.

           // return Greater than zero if this object is greater than 
           // the object specified by the CompareTo method.
           return 0;
    }

    public struct SpawnRules
    {
        // % needed to be on stage currently for this item to be spawned

        public float scoreMaker;
        public float arrow;

        public float locks;

        public float ray;
        public float revolver;

        public float rainbowLipstick;

        public float fastFoward;
        public float sloMo;
        public float sneakers;
        public float floppyDisk;
        public float compactDisk;

        public float glasses3D;

        public float maxItemCapacityUsage;

        public SpawnRules (float scoreMaker,
            float arrow,
            float locks,
            float ray,
            float revolver,
            float rainbowLipstick,
            float fastFoward,
            float sloMo,
            float sneakers,
            float floppyDisk,
            float compactDisk,
            float glasses3D,
            float maxItemCapacityUsage)
        {

            this.scoreMaker = scoreMaker;
            this.arrow = arrow;
            this.locks = locks;
            this.ray = ray;
            this.revolver = revolver;
            this.rainbowLipstick = rainbowLipstick;
            this.fastFoward = fastFoward;
            this.sloMo = sloMo;
            this.sneakers = sneakers;
            this.floppyDisk = floppyDisk;
            this.compactDisk = compactDisk;
            this.glasses3D = glasses3D;
            this.maxItemCapacityUsage = maxItemCapacityUsage;

        }


    }

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

    [SerializeField]
    protected RhythmSystem _rhythmSystem_ref;
    public RhythmSystem rhythmSystem_ref
    {
        get
        {
            return _rhythmSystem_ref;
        }
        set
        {
            _rhythmSystem_ref = value;
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

    //[Tooltip ("chance to spawn")]
    [SerializeField]
    public static float rarity = 0.0f;
    [SerializeField]
    public static float current_rarity = 0.0f;

    public static SpawnRules rules = new SpawnRules(0,0,0,0,0,0,0,0,0,0,0,0,0);

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

            Equip (owner);
        }

    }

    public virtual void Setup (TheGrid grid, RhythmSystem rhythm, GridBlock gb)
    {
        grid_ref = grid;
        rhythmSystem_ref = rhythm;
        gridBlockOwner = gb;
        x = gb.X;
        y = gb.Y;
        z = gb.Z;

        gb.hasItem = true;
        //grid_ref.itemList.Add(this);

    }

    //multiple time use items use Use
    public virtual void Use ()
    {
        print ("Base item used");
    }

    //one time use only itens use activate. All itens use Activate upon getting  picked up
    public virtual void Activate ()
    {
        // print ("Base item activated");
    }

    public virtual void Equip (Player p)
    {
        if (p.hasItem)
        {
            if (p.item)
                p.item.Kill (this);
        }

        p.hasItem = true;
        p.item = this;
        owner = p;

        //make item stay above player s head
        transform.parent = p.transform;
        transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
        float player_height = p.gameObject.GetComponent<MeshRenderer> ().bounds.max.y;
        transform.localPosition = new Vector3 (0.0f, player_height, 0.0f);

        grid_ref.updateItemSpawnRatio();
        //print ("Base item equiped");
    }

    public virtual void Kill (Item current_Item)
    {

            
         if (gameObject)
            Destroy (gameObject);
    }

        public static bool ruleCheck(TheGrid grid_ref)
        {
            
            if(rules.arrow  > grid_ref.getItemCurrentPercentage<Arrow>())
                return false;
            if(rules.locks  > grid_ref.getItemCurrentPercentage<Lock>())
                return false;
            if(rules.ray  > grid_ref.getItemCurrentPercentage<Ray>())
                return false;
            if(rules.revolver  > grid_ref.getItemCurrentPercentage<Revolver>())
                return false;
            if(rules.rainbowLipstick  > grid_ref.getItemCurrentPercentage<RainbowLipstick>())
                return false;
            if(rules.fastFoward  > grid_ref.getItemCurrentPercentage<FastFoward>())
                return false;
            if(rules.sloMo  > grid_ref.getItemCurrentPercentage<SloMo>())
                return false;
            if(rules.sneakers  > grid_ref.getItemCurrentPercentage<Sneakers>())
                return false;
            if(rules.floppyDisk  > grid_ref.getItemCurrentPercentage<FloppyDisk>())
                return false;
            if(rules.compactDisk  > grid_ref.getItemCurrentPercentage<CompactDisk>())
                return false;
            if(rules.glasses3D  > grid_ref.getItemCurrentPercentage<Glasses3D>())
                return false;
            
            if(rules.maxItemCapacityUsage > grid_ref.getItemCurrentPercentage<Item>())
                return false;
           

           // print("passou");
            return true;

        }

        public static float rarityReduction(float rarity, int currentCount)
        {
            float percentageToReduce = (rarity / 2) * currentCount;
            float reductionActualValue = (percentageToReduce * 0.01f) * rarity;

            if(currentCount == 1)
            {
              //  print("arrow need to reduce in: " + percentageToReduce + "% | " + percentageToReduce + "% is actually " + reductionActualValue + " of " + rarity);
            }
            if(reductionActualValue < rarity)
            return reductionActualValue;
            else
            return rarity;
        }
}
}