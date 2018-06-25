using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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

    public static Color getColorOfGridBlockColor (gridBlockColor gbc)
    {
        Color c = new Color ();

        switch (gbc)
        {
            case gridBlockColor.Blue_W:
                c.r = 1;
                c.g = 205;
                c.b = 254;

                break;
            case gridBlockColor.Green_W:
                c.r = 5;
                c.g = 255;
                c.b = 161;
                break;

            case gridBlockColor.Pink_W:
                c.r = 255;
                c.g = 113;
                c.b = 206;

                break;

            case gridBlockColor.Purple_W:
                c.r = 185;
                c.g = 103;
                c.b = 255;

                break;

            case gridBlockColor.Yellow_W:
                c.r = 255;
                c.g = 251;
                c.b = 150;

                break;

            case gridBlockColor.White:
                c.r = 255;
                c.g = 255;
                c.b = 255;
                break;

            case gridBlockColor.Blue_B:
                c.r = 1;
                c.g = 205;
                c.b = 254;

                break;
            case gridBlockColor.Green_B:
                c.r = 5;
                c.g = 255;
                c.b = 161;
                break;

            case gridBlockColor.Pink_B:
                c.r = 255;
                c.g = 113;
                c.b = 206;

                break;

            case gridBlockColor.Purple_B:
                c.r = 185;
                c.g = 103;
                c.b = 255;

                break;

            case gridBlockColor.Yellow_B:
                c.r = 255;
                c.g = 251;
                c.b = 150;

                break;

            case gridBlockColor.Black:
                c.r = 52;
                c.g = 52;
                c.b = 52;
                break;

            default:
                break;

        }

        Color cc = new Color32 ((byte) c.r, (byte) c.g, (byte) c.b, 255);
        return cc;
    }
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

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private RhythmSystem rhythmSystem_ref;

    //[SerializeField]
    //private MeshRenderer meshRenderer;

    [SerializeField]
    private BoxCollider fullCollider, innerCollider;

    [SerializeField]
    private Material[] materials;

    public struct FallStats
    {
        public int pattern;
        public int countdown;
        public int duration;

        public int countdown_count;

        public int duration_count;

        public bool startCountdown;

        public bool startDuration;

    }

    private FallStats fall_data;
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

    public Transform startTransform;

    [SerializeField]
    private GameObject textCountdown_prefab;
    [SerializeField]
    private GameObject textCountdown_ref;

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
    private bool _hasItem = false;
    public bool hasItem
    {
        get
        {
            return _hasItem;
        }
        set
        {
            _hasItem = value;
            if (_hasItem == false)
                Item = null;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private Item _Item;
    public Item Item
    {
        get
        {
            return _Item;
        }
        set
        {
            _Item = value;
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

    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isLocked = false;
    public bool isLocked
    {
        get
        {
            return _isLocked;
        }
        set
        {
            _isLocked = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isFallen = false; // when the block is already fallen/dissapeared
    public bool isFallen
    {
        get
        {
            return _isFallen;
        }
        set
        {
            _isFallen = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isFalling = false; // while stil on the countdown
    public bool isFalling
    {
        get
        {
            return _isFalling;
        }
        set
        {
            _isFalling = value;
        }
    }

    public void init (int x, int y, int z, TheGrid gr, RhythmSystem rs)
    {
        _x = x;
        _y = y;
        _z = z;

        grid_ref = gr;
        rhythmSystem_ref = rs;

    }
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        if (isFallen)
            FallUpdate ();
    }

    private void OnCollisionEnter (Collision other)
    {

        if (other.gameObject.CompareTag ("Player"))
        {

            print ("entrou collider bloco " + X + ", " + Z);

        }

    }
    private void OnCollisionExit (Collision other)
    {

    }
    private void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.CompareTag ("Player"))
        {
            Player p = other.GetComponent<Player> ();
            //print ("entrou trigger bloco " + X + ", " + Z);

            if (!isLocked && !isFallen)
            {
                if (p.item != null)
                {
                    if (p.item.GetType () == typeof (Lock))
                    {
                        changeColor (p.blackGridColor);
                        changeOwner (p);
                        isLocked = true;
                    }
                    else
                    {
                        changeColor (p.gridColor);
                        changeOwner (p);
                    }

                }
                else
                {
                    changeColor (p.gridColor);
                    changeOwner (p);

                }
            }

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

    //triggers the fall event
    public void Fall (int pattern, int countdown, int duration)
    {

        if(isFallen || isFalling) //already falling/fallen, must return
        return;

        fall_data.pattern = pattern;
        fall_data.countdown = countdown;
        fall_data.duration = duration;
        fall_data.countdown_count = 0;
        fall_data.duration_count = 0;
        fall_data.startCountdown = true;
        fall_data.startDuration = false;
        isFalling = true;

        Vector3 pos = startTransform.position;
        //pos.y += this.GetComponent<Renderer> ().bounds.size.y + 0.2f;
        pos.y += 0.2f;

        //Color c(0,0,0);
        SpawnCountdownText (pos, fall_data.countdown.ToString (), Color.green);
        // textMesh.text =    fall_data.countdown.ToString();

        rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (FallIncrement);

    }

    //handles fall movement
    private void FallUpdate ()
    {

    }

    public void SpawnCountdownText (Vector3 pos, string tex, Color texCol) //spawn text above the gridblock
    {
        GameObject countdownPrefab = Instantiate (textCountdown_prefab, pos, Quaternion.identity) as GameObject;
        GridBlockText gt = countdownPrefab.GetComponent<GridBlockText> ();

        gt.transform.position = pos;
        gt.transform.localEulerAngles = new Vector3 (90, 0, 0);
        gt.gridBlock_ref = this;
        if (gt.textMeshPro_ref)
            gt.textMeshPro_ref.text = tex;

        if (texCol != null)
            gt.textMeshPro_ref.color = texCol;

        textCountdown_ref = countdownPrefab;
    }

    //increase fall variables
    public void FallIncrement ()
    {
        //print (fall_data.pattern + " " + fall_data.countdown + " " + fall_data.duration);

        if (fall_data.startCountdown) //if can start the countdown
        {
            if (fall_data.countdown_count < fall_data.countdown) //if countdown still going
            {

                int result = fall_data.countdown - fall_data.countdown_count; // never show 0
                int crossResult = (100 * result) / fall_data.countdown;

                textCountdown_ref.GetComponent<TextMeshPro> ().text = (result).ToString ();

                Color col = new Color (1, 2, 3);
                if (crossResult >= 66)
                {
                    //print("first mudei");
                    col = new Color (0.2f, 0.7f, 0.4f, 1.0f);
                    col = Color.yellow;
                }
                else
                if (crossResult < 66 && crossResult >= 33)
                {
                    //print("second mudei");
                    col = new Color (0.8f, 0.7f, 0.4f, 1.0f);
                    col = Color.red;
                }
                else if (crossResult < 33)
                {
                    //print("third mudei");
                    col = new Color (0.2f, 0.5f, 0.3f, 1.0f);
                    col = Color.red;
                }

                textCountdown_ref.GetComponent<TextMeshPro> ().color = col;
                fall_data.countdown_count++;
            }
            else //countdown ended
            {
                //adjust fall_data values
                fall_data.countdown_count = 0;
                fall_data.startCountdown = false;
                fall_data.startDuration = true;
                textCountdown_ref.GetComponent<TextMeshPro> ().text = "";
                textCountdown_ref.GetComponent<GridBlockText> ().isDone = true;
                textCountdown_ref = null;
                isFallen = true;
                isFalling = false;
                //textMeshObject.SetActive(false);

                Vector3 endPosition = startTransform.position;
                endPosition.y = endPosition.y - 10;

                //make gridblock "invisible" by scaling it down, but not totally because we still need the collider
                transform.DOScale (new Vector3 (0.001f, 0.001f, 0.001f), rhythmSystem_ref.rhythmTarget_Ref.duration);
               
            }
        }

        if (fall_data.startDuration)
        {
            if (fall_data.duration_count < fall_data.duration)
            {
                fall_data.duration_count++;
                //print("count " + count);
            }
            else
            {
                //print ("fall timeout ");
                fall_data.duration_count = 0;
                fall_data.startDuration = false;
                isFallen = false;

                //startCount = false;
                transform.DOScale (new Vector3 (1, 1, 1), rhythmSystem_ref.rhythmTarget_Ref.duration / 2);
                rhythmSystem_ref.getRhythmNoteToPoolEvent ().RemoveListener (FallIncrement);
                //Kill (null);
            }

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