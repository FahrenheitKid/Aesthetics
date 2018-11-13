using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using DG.Tweening;
using SonicBloom.Koreo;
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

    public enum gridBlockPattern
    {
        Single, // 0
        Double_H, // 1
        Double_V, // 2
        Triple_H, //3
        Triple_V, //4
        Cross //5

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


    [SerializeField, Candlelight.PropertyBackingField]
    private gridType _GridType;
    public gridType GridType
    {
        get
        {
            return _GridType;

        }
        set
        {
            _GridType = value;
            
            if(_GridType == gridType.Hole)
            {
                isBlocked = false;
                isPreBlocked = false;

                isFallen = true;
                isPreFallen = true;
                transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
                
            }

            if(_GridType == gridType.Obstacle)
            {
                isBlocked = true;
                isPreBlocked = true;
                
                isFallen = false;
                isPreFallen = false;
                transform.localScale = blockScale;
                changeColor(gridBlockColor.Black);
            }

        }
    }

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

    public struct GridBlockEventStats
    {
        public GridBlock.gridBlockPattern pattern;
        public int countdown;
        public int duration;

        public int countdown_count;

        public int duration_count;

        public bool startCountdown;

        public bool startDuration;

        public GridBlockEventStats (GridBlock.gridBlockPattern pattern,
            int countdown,
            int duration,

            int countdown_count,

            int duration_count,

            bool startCountdown,

            bool startDuration)
        {
            this.pattern = pattern;
            this.countdown = countdown;
            this.duration = duration;

            this.countdown_count = countdown_count;

            this.duration_count = duration_count;

            this.startCountdown = startCountdown;

            this.startDuration = startDuration;

        }

    }

    private GridBlockEventStats fall_data;
    private GridBlockEventStats block_data;
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
    protected Player _stolenOwner = null;
    public Player stolenOwner
    {
        get
        {
            return _stolenOwner;
        }
        set
        {
            _stolenOwner = value;
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
            {
                Item = null;
                GetComponent<MeshRenderer>().material.SetColor("_TintColor2",Color.white);
                //GetComponent<MeshRenderer>().material.SetFloat("_Tint2Gamma",0.3f);
                

            }
                else
                {
                    //GetComponent<MeshRenderer>().material.SetFloat("_Tint2Gamma",0.2f);
                    
                    GetComponent<MeshRenderer>().material.SetColor("_TintColor2",grid_ref.stageHighlightColors.First());
                }
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

    public struct GridBlockStatus
    {
        public bool? hasItem;
        public bool? isOccupied;
        public bool? isPreBlocked;
        public bool? isBlocked;
        public bool? isPreFallen;
        public bool? isFallen;
        public bool? isRespawning;

        public GridBlockStatus (bool? hasItem,
            bool? isOccupied,

            bool? isPreBlocked,
            bool? isBlocked,
            bool? isPreFallen,
            bool? isFallen,
            bool? isRespawning)
        {
            this.hasItem = hasItem;
            this.isOccupied = isOccupied;

            this.isPreBlocked = isPreBlocked;
            this.isBlocked = isBlocked;
            this.isPreFallen = isPreFallen;
            this.isFallen = isFallen;
            this.isRespawning = isRespawning;
        }
    }

    [Tooltip ("Is this gridblock occupied by a Player?")]
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

    [Tooltip ("Is this gridblock blocked (cannot allow any player on it?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isPreBlocked = false;
    public bool isPreBlocked
    {
        get
        {
            return _isPreBlocked;
        }
        set
        {
            _isPreBlocked = value;
        }
    }

    [Tooltip ("Is this gridblock blocked (cannot allow any player on it?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isBlocked = false;
    public bool isBlocked
    {
        get
        {
            return _isBlocked;
        }
        set
        {
            _isBlocked = value;
        }
    }

    private bool haveBlockStunned = false;
    [Tooltip ("blocked scaling vector")]
    [SerializeField]
    private Vector3 blockScale = new Vector3 (1, 3.5f, 1);

    [Tooltip ("Is this gridblock locked (cannot be colored)")]
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

    [Tooltip ("Is this gridblock already fallen? ")]
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

    [Tooltip ("Is this gridblock in the countdown to start falling down?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isPreFallen = false; // while stil on the countdown
    public bool isPreFallen
    {
        get
        {
            return _isPreFallen;
        }
        set
        {
            _isPreFallen = value;
        }
    }

    [Tooltip ("Is this gridblock respawning from a fall?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isRespawning = false; // while stil on the countdown
    public bool isRespawning
    {
        get
        {
            return _isRespawning;
        }
        set
        {
            _isRespawning = value;
        }
    }

    [SerializeField]
    public Countdown respawnTimer;

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
        respawnTimer = gameObject.AddComponent<Countdown> ();
    }

    // Update is called once per frame
    void Update ()
    {
        if (respawnTimer.stop)
            isRespawning = false;
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

            //if tile is "paitable"
            if (!isLocked && !isFallen)
            {
                if (p.item != null || p.hasItem)
                {
                    if (p != null)
                    {
                        //if painting player has lock, need to paint it black and lock the tile
                        if (p.item.GetType () == typeof (Lock))
                        {
                            changeColor (p.blackGridColor);
                            changeOwner (p);
                            stolenOwner = null;

                            isLocked = true;
                        } // if painting player has lipstick, need to paint with random enemy color
                        else if (p.item.GetType () == typeof (RainbowLipstick))
                        {
                            List<Player> plist = grid_ref.GetPlayerList().ToList();
                            plist.Remove (p);
                            int idx = Random.Range (0, plist.Count);

                            if (plist.Count > 0 && idx >= 0 && idx < plist.Count)
                            {
                                // if the selected random enemy has lock, need to painted like a locked tile
                                if (plist[idx].item != null && plist[idx].item.GetType () == typeof (Lock))
                                {

                                    changeColor (plist[idx].blackGridColor);
                                    changeOwner (p);
                                    stolenOwner = null;
                                }
                                else
                                {
                                    changeColor (plist[idx].gridColor);
                                    changeOwner (p);
                                    stolenOwner = null;
                                }
                            }
                        }
                        else
                        {
                            changeColor (p.gridColor);
                            changeOwner (p);
                            stolenOwner = null;
                        }

                    }

                }
                else
                {
                    changeColor (p.gridColor);
                    changeOwner (p);
                    stolenOwner = null;

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

    /// <summary>
    /// OnTriggerStay is called once per frame for every Collider other
    /// that is touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerStay (Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            Player p = other.GetComponent<Player> ();

            if (haveBlockStunned == false && isBlocked)
            {
                haveBlockStunned = p.Stun (grid_ref.blockStunDuration);
                //haveBlockStunned = true;
            }

        }
    }

    //triggers the fall event
    public void Fall (GridBlock.gridBlockPattern pattern, int countdown, int duration)
    {

        if (isFallen || isPreFallen || isBlocked || isPreBlocked) //already falling/fallen, must return
            return;

        fall_data.pattern = pattern;
        fall_data.countdown = countdown;
        fall_data.countdown--; // need to descrease 1 otherwise it will run one additinoal time
        fall_data.duration = duration;
        fall_data.countdown_count = 0;
        fall_data.duration_count = 0;
        fall_data.startCountdown = true;
        fall_data.startDuration = false;
        isPreFallen = true;

        Vector3 pos = startTransform.position;
        //pos.y += this.GetComponent<Renderer> ().bounds.size.y + 0.2f;
        pos.y += 0.2f;

        if (fall_data.countdown > 0)
        {
            int show = fall_data.countdown + 1; // never show 0
            SpawnCountdownText (pos, show.ToString (), Color.green);
        }

        // textMesh.text =    fall_data.countdown.ToString();

        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, FallIncrement);

    }

    public void Block (GridBlock.gridBlockPattern pattern, int countdown, int duration)
    {
        if (isFallen || isPreFallen || isBlocked || isPreBlocked) //already falling/fallen, must return
            return;

        block_data.pattern = pattern;
        block_data.countdown = countdown;
        block_data.countdown--; // need to descrease 1 otherwise it will run one additinoal time
        block_data.duration = duration;
        block_data.countdown_count = 0;
        block_data.duration_count = 0;
        block_data.startCountdown = true;
        block_data.startDuration = false;
        isPreBlocked = true;

        //rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (BlockIncrement);

        Vector3 pos = startTransform.position;
        //pos.y += this.GetComponent<Renderer> ().bounds.size.y + 0.2f;
        pos.y += 0.2f;

        //Color c(0,0,0);
        if (block_data.countdown > 0)
        {
            int show = block_data.countdown + 1; // never show 0
            SpawnCountdownText (pos, show.ToString (), Color.green);
        }
        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, BlockIncrement);
        // textMesh.text =    fall_data.countdown.ToString();

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
    public void FallIncrement (KoreographyEvent evt)
    {
        //print (fall_data.pattern + " " + fall_data.countdown + " " + fall_data.duration);

        if (fall_data.startCountdown) //if can start the countdown
        {
            if (fall_data.countdown_count < fall_data.countdown) //if countdown still going
            {
                fall_data.countdown_count++;
                int result = fall_data.countdown - fall_data.countdown_count + 1; // never show 0
                int crossResult = (100 * result) / fall_data.countdown;

                textCountdown_ref.GetComponent<TextMeshPro> ().text = (result).ToString ();

                Color col = Color.green;
                if (result <= 5 && result > 3)
                {
                    //print("first mudei");
                    col = new Color (0.2f, 0.7f, 0.4f, 1.0f);
                    col = Color.yellow;
                }
                else
                if (result < 3 && result > 2 )
                {
                    //print("second mudei");
                    col = new Color (0.8f, 0.7f, 0.4f, 1.0f);
                    col = Color.red;
                }
                else if (result <= 2)
                {
                    //print("third mudei");
                    col = new Color (0.2f, 0.5f, 0.3f, 1.0f);
                    col = Color.red;
                }
                textCountdown_ref.GetComponent<TextMeshPro> ().color = col;

            }
            else //countdown ended
            {
                //adjust fall_data values
                fall_data.countdown_count = 0;
                fall_data.startCountdown = false;
                fall_data.startDuration = true;
                if (textCountdown_ref)
                {
                    textCountdown_ref.GetComponent<TextMeshPro> ().text = "";
                    textCountdown_ref.GetComponent<GridBlockText> ().isDone = true;
                    textCountdown_ref = null;
                }

                isFallen = true;
                isPreFallen = false;
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
                Koreographer.Instance.UnregisterForEvents (rhythmSystem_ref.mainBeatID, FallIncrement);

                //Kill (null);
            }

        }

    }

    //handles block countdown
    public void BlockIncrement (KoreographyEvent evt)
    {
        //print (fall_data.pattern + " " + fall_data.countdown + " " + fall_data.duration);

        if (block_data.startCountdown) //if can start the countdown
        {
            if (block_data.countdown_count < block_data.countdown) //if countdown still going
            {

                block_data.countdown_count++;
                int result = block_data.countdown - block_data.countdown_count + 1; // never show 0
                int crossResult = (100 * result) / block_data.countdown;

                textCountdown_ref.GetComponent<TextMeshPro> ().text = (result).ToString ();

                Color col = Color.green;
                 if (result <= 5 && result > 3)
                {
                    //print("first mudei");
                    col = new Color (0.2f, 0.7f, 0.4f, 1.0f);
                    col = Color.yellow;
                }
                else
                if (result < 3 && result > 2 )
                {
                    //print("second mudei");
                    col = new Color (0.8f, 0.7f, 0.4f, 1.0f);
                    col = Color.red;
                }
                else if (result <= 2)
                {
                    //print("third mudei");
                    col = new Color (0.2f, 0.5f, 0.3f, 1.0f);
                    col = Color.red;
                }

                textCountdown_ref.GetComponent<TextMeshPro> ().color = col;

            }
            else //countdown ended
            {

                //adjust block_data values
                block_data.countdown_count = 0;
                block_data.startCountdown = false;
                block_data.startDuration = true;
                if (textCountdown_ref)
                {
                    textCountdown_ref.GetComponent<TextMeshPro> ().text = "";
                    textCountdown_ref.GetComponent<GridBlockText> ().isDone = true;
                    textCountdown_ref = null;
                }
                isBlocked = true;
                haveBlockStunned = false;
                isPreBlocked = false;
                //textMeshObject.SetActive(false);

                Vector3 endPosition = startTransform.position;
                endPosition.y = endPosition.y - 10;

                //make gridblock scale up to become a little bump
                transform.DOScale (blockScale, rhythmSystem_ref.rhythmTarget_Ref.duration);

            }
        }

        if (block_data.startDuration)
        {
            if (block_data.duration_count < block_data.duration)
            {
                block_data.duration_count++;
                //print("count " + count);
            }
            else
            {
                //print ("fall timeout ");
                block_data.duration_count = 0;
                block_data.startDuration = false;
                isBlocked = false;

                //startCount = false;
                transform.DOScale (new Vector3 (1, 1, 1), rhythmSystem_ref.rhythmTarget_Ref.duration / 2);
                //rhythmSystem_ref.getRhythmNoteToPoolEvent ().RemoveListener (BlockIncrement);
                Koreographer.Instance.UnregisterForEvents (rhythmSystem_ref.mainBeatID, BlockIncrement);
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

    public bool isSame (GridBlock gb)
    {
        return (gb.X == X && gb.Y == Y);
    }
    // changes directly the color of the gridblock
    public void changeColor (gridBlockColor col)
    {
        if (mainColor == col) return;

        switch (col)
        {
            case gridBlockColor.Blue_W:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Blue_W];

                break;
            case gridBlockColor.Green_W:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Green_W];

                break;

            case gridBlockColor.Pink_W:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Pink_W];

                break;

            case gridBlockColor.Purple_W:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Purple_W];
                break;

            case gridBlockColor.Yellow_W:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Yellow_W];

                break;

            case gridBlockColor.White:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.White];

                break;

            case gridBlockColor.Blue_B:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Blue_B];
                break;
            case gridBlockColor.Green_B:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Green_B];

                break;

            case gridBlockColor.Pink_B:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Pink_B];

                break;

            case gridBlockColor.Purple_B:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Purple_B];
                break;

            case gridBlockColor.Yellow_B:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Yellow_B];

                break;

            case gridBlockColor.Black:
                GetComponent<MeshRenderer> ().sharedMaterial = materials[(int) gridBlockColor.Black];
                print ("painted back");
                break;

            default:
                break;

        }

        mainColor = col;

    }

}