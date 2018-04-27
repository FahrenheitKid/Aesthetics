using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlock : MonoBehaviour
{

    #region enums
    public enum gridBlockColor
    {
        White,
        Pink_W,
        Purple_W,
        Yellow_W,
        Blue_W,
        Green_W,
        Black,
        Pink_B,
        Purple_B,
        Yellow_B,
        Blue_B,
        Green_B

    }
    public static gridBlockColor GridBlockColor;

    /* 
    public enum owner
    {
        Nobody,
        Player1,
        Player2,
        Player3,
        Player4
    }
    public static owner Owner;
*/
    public enum gridType
    {
        Hole,
        Normal,
        Obstacle,
        Void
    }
    #endregion

    public static gridType GridType;

    //[SerializeField]
    //private MeshRenderer meshRenderer;

    [SerializeField]
    private BoxCollider fullCollider, innerCollider;

    [SerializeField]
    private Material[] materials;

    [SerializeField]
    private int _x, _y, _z;
    public int X
    {
        get
        {
            return _x;
        }
        private set
        {
            _x = value;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }
        private set
        {
            _y = value;
        }
    }
    public int Z
    {
        get
        {
            return _z;
        }
        private set
        {
            _z = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private GridBlock.gridBlockColor _mainColor = GridBlock.gridBlockColor.Black;
    public GridBlock.gridBlockColor mainColor
    {
        get
        {
            return _mainColor;
        }
        set
        {
            _mainColor = value;
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
    private bool _isOccupied = false;
    public bool isOccupied
    {
        get
        {
            return _isOccupied;
        }
        set
        {
            _isOccupied = value;
        }
    }
    public void init (int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void OnCollisionEnter (Collision other)
    {

        if (other.gameObject.CompareTag ("Player"))
        {

            print ("entrou collider bloco " + X + ", " + Z);

        }

    }
    private void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.CompareTag ("Player"))
        {

            //print ("entrou trigger bloco " + X + ", " + Z);
            changeColor (other.GetComponent<Player> ().gridColor);
            changeOwner (other.GetComponent<Player> ());
            isOccupied = true;

        }
    }
    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            isOccupied = false;
        }

    }

    public void changeOwner (Player p)
    {
        if (p == null)
        {
            owner = null;

            return;
        }
        owner = p;

    }
    // changes directly the color of the gridblock
    public void changeColor (gridBlockColor col)
    {
        if (mainColor == col) return;

        switch (col)
        {
            case gridBlockColor.Blue_W:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Blue_W];

                break;
            case gridBlockColor.Green_W:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Green_W];

                break;

            case gridBlockColor.Pink_W:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Pink_W];

                break;

            case gridBlockColor.Purple_W:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Purple_W];
                break;

            case gridBlockColor.Yellow_W:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Yellow_W];

                break;

            case gridBlockColor.White:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.White];

                break;

            case gridBlockColor.Blue_B:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Blue_B];
                break;
            case gridBlockColor.Green_B:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Green_B];

                break;

            case gridBlockColor.Pink_B:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Pink_B];

                break;

            case gridBlockColor.Purple_B:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Purple_B];
                break;

            case gridBlockColor.Yellow_B:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Yellow_B];

                break;

            case gridBlockColor.Black:
                GetComponent<MeshRenderer> ().material = materials[(int) gridBlockColor.Black];
                print ("painted back");
                break;

            default:
                break;

        }

        mainColor = col;
    }
}