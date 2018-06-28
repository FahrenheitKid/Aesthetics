using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aesthetics {


public class Player : MonoBehaviour
{

    public enum AxisState
    {
        Idle,
        Down,
        Held,
        Up
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private static int _globalID = 0;
    public int globalID
    {
        get
        {
            return _globalID;
        }
        private set
        {
            _globalID = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _ID = 0;
    public int ID
    {
        get
        {
            return _ID;
        }
        set
        {
            _ID = value;
        }
    }

    [SerializeField]
    private AxisState horizontalAxisState = AxisState.Idle;

    [SerializeField]
    private AxisState verticalAxisState = AxisState.Idle;

    [Tooltip ("Deadzone for the axis press/down")]
    [SerializeField]
    private float deadZone = 0.02f;

    [Tooltip ("The size of one girdblock, it used in movement calculations")]
    [SerializeField]
    private float gridSize = 1f;

    [Tooltip ("If player has an Itemw ith him or not")]
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
        }
    }

    [Tooltip (" The Item the player is holding. Null if it is not holding anyhting ")]
    [SerializeField, Candlelight.PropertyBackingField]
    private Item _item;
    public Item item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
        }
    }

    [Tooltip ("Is the player stunned?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isStunned = false;
    public bool isStunned
    {
        get
        {
            return _isStunned;
        }
        set
        {
            
            if(value)
            {
                combo = 0;
                multiplierCombo = 0;
                multiplier = 0;
                disableInput = true;
                Shader shader_ref = gameObject.GetComponent<Renderer> ().materials[0].shader;
               
                gameObject.GetComponent<Renderer> ().materials[0].SetColor("_Color",Color.blue);
            }
            else
            {

                Shader shader_ref = gameObject.GetComponent<Renderer> ().materials[0].shader;
               
                gameObject.GetComponent<Renderer> ().materials[0].SetColor("_Color",Color.white);

                disableInput = false;
            }

            _isStunned = value;

        }
    }

    [Tooltip ("Is the player fallen?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isFallen = false;
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

    [Tooltip ("fallen/death duration")]
    [SerializeField, ]
    public float fall_duration = 3;

    [Tooltip ("respawn duration")]
    [SerializeField, ]
    public float respawn_duration = 1.5f;

    [Tooltip ("respawn Immunity duration?")]
    [SerializeField, ]
    public float respawnImmunity_duration = 2f;

    [Tooltip ("the block where the player fell?")]
    [SerializeField, ]
    public GridBlock fall_gridBlock_Ref = null;

    [Tooltip ("Is the player respawning?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isRespawning = false;
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

    [Tooltip ("Is the player immune?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isImmune = false;
    public bool isImmune
    {
        get
        {
            return _isImmune;
        }
        set
        {
            if (value)
            {
                
                Shader glass = Shader.Find ("FX/Glass/Stained BumpDistort");
                gameObject.GetComponent<Renderer> ().materials[0].shader = glass;

            }
            else
            {
                Shader normal = Shader.Find ("Standard");
                gameObject.GetComponent<Renderer> ().materials[0].shader = normal;
            }

            _isImmune = value;

        }
    }

    [Tooltip ("Is the player shielded(nullifies one stun/effect)?")]
    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isShielded = false;
    public bool isShielded
    {
        get
        {
            return _isShielded;
        }
        set
        {

            _isShielded = value;

        }
    }

    [Tooltip ("disable all player inputs")]
    [SerializeField]
    private bool disableInput = false;

    [SerializeField]
    Countdown stunTimer;

    [SerializeField]
    Countdown fallenTimer;

    [SerializeField]
    Countdown respawnTimer;

    [SerializeField]
    Countdown immunityTimer;

    [Tooltip ("ThPlayer's current score")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _score = 0;
    public int score
    {
        get
        {
            return _score;
        }
        set
        {

            _score = value;
            int i = (ID > 0) ? ID - 1 : 0;
            grid_ref.GetPlayerUIList () [i].setScore (_score);

        }
    }

    [Tooltip ("Player's current Combo")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _combo = 0;
    public int combo
    {
        get
        {
            return _combo;
        }
        set
        {

            _combo = value;
            if (combo == 0) _multiplierCombo = 0;
            int i = (ID > 0) ? ID - 1 : 0;
            grid_ref.GetPlayerUIList () [i].setCombo (_combo);
        }
    }

    [Tooltip ("Player's current multiplayer Combo")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _multiplierCombo = 0;
    public int multiplierCombo
    {
        get
        {
            return _multiplierCombo;
        }
        set
        {
            _multiplierCombo = value;

            if (_multiplierCombo / 10 >= 1) multiplier = _multiplierCombo / 10;
            else if (_multiplierCombo == 0) multiplier = 1;

            //int i = (ID > 0) ? ID - 1 : 0;
            //grid_ref.GetPlayerUIList () [i].setCombo (_combo);
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _multiplier = 1;
    public int multiplier
    {
        get
        {
            return _multiplier;
        }
        set
        {

            _multiplier = value;
            int i = (ID > 0) ? ID - 1 : 0;
            grid_ref.GetPlayerUIList () [i].setMultiplier (_multiplier);
        }
    }

    [Tooltip ("X value in internal grid matrix")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _x;
    public int x
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

    [Tooltip ("Y value in internal grid matrix")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _y;
    public int y
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

    [Tooltip ("Z Value in internal grid matrix")]
    [SerializeField, Candlelight.PropertyBackingField]
    private int _z;
    public int z
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

    [Tooltip ("start y value")]
    [SerializeField, Candlelight.PropertyBackingField]
    private float _initY;
    public float initY
    {
        get
        {
            return _initY;
        }
        private set
        {
            _initY = value;
        }
    }

    [Tooltip ("Player's move speed")]
    [SerializeField, Candlelight.PropertyBackingField]
    private float _moveSpeed = 10f;
    public float moveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }

    [SerializeField]
    private TheGrid grid_ref;

    [SerializeField]
    private RhythmSystem rhythmSystem_ref;

    [Tooltip ("Player's GridBlockColor")]
    [SerializeField, Candlelight.PropertyBackingField]
    private GridBlock.gridBlockColor _gridColor = GridBlock.gridBlockColor.Pink_B;
    public GridBlock.gridBlockColor gridColor
    {
        get
        {
            return _gridColor;
        }
        set
        {
            _gridColor = value;
        }
    }

    [Tooltip ("Player's GridBlockColor")]
    [SerializeField, Candlelight.PropertyBackingField]
    private GridBlock.gridBlockColor _blackGridColor = GridBlock.gridBlockColor.Pink_B;
    public GridBlock.gridBlockColor blackGridColor
    {
        get
        {
            return _blackGridColor;
        }
        set
        {
            _blackGridColor = value;
        }
    }

    [Tooltip (" How much scaling of the Punch Scale done in the main beat")]
    [Range (0, 2)]
    [SerializeField]
    private float beatPunchScale = 0.05f;

    [Tooltip ("How much the players vibrates with the scaling")]
    [Range (0, 500)]
    [SerializeField]
    private int vibrato = 0;

    [Tooltip ("PunchScale elasticity")]
    [Range (0, 1)]
    [SerializeField]
    private float elasticity = 0.0f;

    //[Range(0,1)]s
    [SerializeField, Candlelight.PropertyBackingField]
    private float _duration = 0.5f;
    public float duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }

    private enum Orientation
    {
        Horizontal,
        Vertical
        };

        #region movement variables
        private Orientation gridOrientation = Orientation.Horizontal;
        private bool allowDiagonals = false;
        private bool correctDiagonalSpeed = true;
        private Vector2 input;
        [SerializeField]
        private bool isMoving = false;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private float t;
        private float factor;

        #endregion

        //Sets the player's ID
        private void Awake ()
        {
        if (globalID >= 2) globalID = 0;
        ID = ++globalID;

        stunTimer = gameObject.AddComponent<Countdown> ();
    }

    private void Start ()
    {
        _multiplier = 1;
        _score = 1;
        _combo = 0;
        _multiplierCombo = 0;
        beatPunchScale = 0.05f;

        float musicBPM = (float) rhythmSystem_ref.currentMusicBPM;

        duration = (float) (60 / musicBPM) / 2;
        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, OnMainBeat);
        initY = grid_ref.playerInitY;

        stunTimer = gameObject.AddComponent<Countdown> ();
        immunityTimer = gameObject.AddComponent<Countdown> ();
        fallenTimer = gameObject.AddComponent<Countdown> ();
        respawnTimer = gameObject.AddComponent<Countdown> ();

    }

    private void OnTriggerEnter (Collider other)
    {
        // update player's current grid
        if (other.gameObject.CompareTag ("GridBlock"))
        {
            GridBlock gb = other.GetComponent<GridBlock> ();
            x = gb.X;
            z = gb.Z;

            if (other.GetComponent<GridBlock> ().isFallen && !isImmune && !isShielded && !isFallen) //if stepped on fallen gridblock
            {
                //make player fall down on the next beat
                fall_gridBlock_Ref = other.GetComponent<GridBlock> ();
                rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (Fall);
            }

        }

    }

    private void OnTriggerStay (Collider other)
    {

        if (other.gameObject.CompareTag ("GridBlock"))
        {

            if (other.GetComponent<GridBlock> ().isFallen && !isImmune && !isShielded && !isFallen) //if stepped on fallen gridblock
            {
                //make player fall down on the next beat
                fall_gridBlock_Ref = other.GetComponent<GridBlock> ();
                rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (Fall);
            }

        }
    }

    public void Update ()
    {
        if (!disableInput)
            handleInput ();

        if (isFallen) // if is death/fallen
        {
            if (fallenTimer.stop) // if death time ended
            {

                Respawn (); // respawn charcter

            }
        }

        if (isRespawning) // if is respawning
        {
            if (respawnTimer.stop) // if respawn time ended
            {

                isRespawning = false;
                //let player move again;
                disableInput = false;
                fall_gridBlock_Ref = null;

                immunityTimer.startTimer (respawnImmunity_duration); // make player imune for a few more seconds

            }
        }

        if (isImmune)
        {

            if (immunityTimer.stop) // if our immune time ended
            {
                if (!isRespawning)
                    isImmune = false;

            }
        }

        if(isStunned)
        {
            if(stunTimer.stop)
            {
                isStunned = false;
            }
        }
    }

    void handleInput ()
    {
        HandleAxisState (ref horizontalAxisState, "Horizontal" + ID);
        HandleAxisState (ref verticalAxisState, "Vertical" + ID);

        if (isStunned)
        {

        }

        if (!isMoving)
        {
            input = new Vector2 (Input.GetAxis ("Horizontal" + ID), Input.GetAxis ("Vertical" + ID));
            if (!allowDiagonals)
            {
                if (Mathf.Abs (input.x) > Mathf.Abs (input.y))
                {
                    input.y = 0;
                }
                else
                {
                    input.x = 0;
                }
            }

            if (input != Vector2.zero && !_isStunned)
            {
                //only compute durinf pressed Down, not on hold
                if ((input.y != 0 && verticalAxisState == AxisState.Down) !=
                    (input.x != 0 && horizontalAxisState == AxisState.Down))
                {

                    //Move ();
                    // combo++;

                    //if Pressed on the beat
                    if (rhythmSystem_ref.WasNoteHit ())
                    {
                        //print ("Player" + ID.ToString () + " Hit it!");

                        //move player and increase combo
                        if (Move ())
                        {
                            combo++;
                            multiplierCombo++;
                        }

                    }
                    else
                    {
                        //lose combo

                        Vector3 pos = transform.position;
                        pos.y += GetComponent<Renderer> ().bounds.size.y + 0.0f;
                        grid_ref.SpawnMissFloatingText (pos);

                        combo = 0;
                    }

                }
                //StartCoroutine (move (transform));

            }
        }

    }
    public void setIsMoving (bool value)
    {
        isMoving = value;
    }

    void Fall ()
    {
        isFallen = true;
        disableInput = true;

        //lose item when falling
        if (hasItem)
            item.Kill (null);

        Vector3 startPos = transform.position;
        startPos.y -= 20;

        transform.DOMove (startPos, rhythmSystem_ref.rhythmTarget_Ref.duration / 2);

        rhythmSystem_ref.getRhythmNoteToPoolEvent ().RemoveListener (Fall);

        fallenTimer.startTimer (fall_duration);

    }

    void Respawn ()
    {
        int safe_count = 0;
        //find neighbour gridblock to serve as spawn point
        RetryToFindNeighbour:

            GridBlock respawnBlock;
        if (fall_gridBlock_Ref)
        {

            respawnBlock = grid_ref.GetRandomNeighbourGridBlock (fall_gridBlock_Ref.X, fall_gridBlock_Ref.Z, true, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));
        }

        else
        {
            respawnBlock = grid_ref.GetRandomGridBlock (1, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));
        }

        if (!respawnBlock && safe_count < 450)
        {
            safe_count++;
            goto RetryToFindNeighbour;
        }

        if (safe_count >= 450)
        {
            //timer to research for new spawnpoint
            print ("NÃO ACHOU SPAWN POINT, EU SABIAAAAA!");
        }

        respawnBlock.isRespawning = true;
        isRespawning = true;
        isFallen = false;

        isImmune = true;

        Vector3 respawnPos = respawnBlock.transform.position;
        respawnPos.y += 10;

        transform.position = respawnPos;

        //start moving player down from above and start respawn timer
        transform.DOMoveY (initY, respawn_duration);
        respawnTimer.startTimer (respawn_duration);

    }
    public void Stun (float duration)
    {
            if(isImmune) return;


            stunTimer.startTimer(duration);
            isStunned = true;
            
    }
    public bool Move ()
    {
        CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

        Vector3 startGridPosition = new Vector3 (x, y, z);
        Vector3 endGridPosition = new Vector3 ();

        GridBlock endGridBlock;
        GridBlock startGridBlock;
        startGridBlock = grid_ref.GetGridBlock (x, z);

        switch (cameraScript.orientation)
        {
            case CameraScript.windRose.North:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;
                }

                break;

            case CameraScript.windRose.East:
                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z > 0)
                        endGridPosition.z--;
                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;
                }

                break;

            case CameraScript.windRose.South:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;
                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }

                break;

            case CameraScript.windRose.West:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;
                }

                break;

            default:
                break;

        }

        endGridBlock = grid_ref.GetGridBlock ((int) endGridPosition.x, (int) endGridPosition.z);

        //if destination gridBlock is already occupied
        if (endGridBlock.isOccupied || endGridBlock.isBlocked || endGridBlock.isRespawning)
        {

            //print ("Block (" + endGridBlock.X + ", " + endGridBlock.Z + ") occupied!");
            return false;

        }
        else
        {
            endGridBlock.isOccupied = true;

            endGridPosition = endGridBlock.transform.position;
            endGridPosition.y = 0.1f;

            isMoving = true;
            transform.DOMove (endGridPosition, rhythmSystem_ref.rhythmTarget_Ref.duration).OnComplete (() => isMoving = false).SetEase (Ease.OutQuart);
            transform.DOLookAt (endGridPosition, 0.2f);
            return true;

        }

    }

    //Returns Destination GridBlock
    GridBlock PeekDestinationGridBlock ()
    {
        CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

        Vector3 startGridPosition = new Vector3 (x, y, z);
        Vector3 endGridPosition = new Vector3 ();

        GridBlock endGridBlock = null;
        GridBlock startGridBlock;
        startGridBlock = grid_ref.GetGridBlock (x, z);

        switch (cameraScript.orientation)
        {
            case CameraScript.windRose.North:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;
                }

                break;

            case CameraScript.windRose.East:
                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z > 0)
                        endGridPosition.z--;
                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;
                }

                break;

            case CameraScript.windRose.South:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;
                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }

                break;

            case CameraScript.windRose.West:

                if (input.x > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z > 0)
                        endGridPosition.z--;

                }
                else if (input.x < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - 1)
                        endGridPosition.z++;

                }
                else if (input.y > 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x--;

                }
                else if (input.y < 0)
                {

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - 1)
                        endGridPosition.x++;
                }

                break;

            default:
                break;

        }

        endGridBlock = grid_ref.GetGridBlock ((int) endGridPosition.x, (int) endGridPosition.z);
        return endGridBlock;

    }

    void HandleAxisState (ref AxisState state, string axi)
    {
        switch (state)
        {
            case AxisState.Idle:
                if (Input.GetAxis (axi) < -deadZone || Input.GetAxis (axi) > deadZone)
                {
                    state = AxisState.Down;
                }
                break;

            case AxisState.Down:
                state = AxisState.Held;
                break;

            case AxisState.Held:
                if (Input.GetAxis (axi) > -deadZone && Input.GetAxis (axi) < deadZone)
                {
                    state = AxisState.Up;
                }
                break;

            case AxisState.Up:
                state = AxisState.Idle;
                break;
        }

    }

    //Main beta callback
    void OnMainBeat (KoreographyEvent evt)
    {
        //Ounch sacle player to the beat
        transform.DOPunchScale (transform.localScale * beatPunchScale, duration, vibrato, elasticity);

    }

    //movement coroutine stuff | currently not using
    public IEnumerator move (Transform transform)
    {

        CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

        isMoving = true;
        startPosition = transform.position;
        t = 0;

        if (gridOrientation == Orientation.Horizontal)
        {
            endPosition = new Vector3 (startPosition.x + System.Math.Sign (input.x) * gridSize,
                startPosition.y, startPosition.z + System.Math.Sign (input.y) * gridSize);
        }
        else
        {
            endPosition = new Vector3 (startPosition.x + System.Math.Sign (input.x) * gridSize,
                startPosition.y + System.Math.Sign (input.y) * gridSize, startPosition.z);
        }

        if (allowDiagonals && correctDiagonalSpeed && input.x != 0 && input.y != 0)
        {
            factor = 0.7071f;
        }
        else
        {
            factor = 1f;
        }

        transform.DOLookAt (endPosition, 0.2f);

        while (t < 1f)
        {
            t += Time.deltaTime * (moveSpeed / gridSize) * factor;
            transform.position = Vector3.Lerp (startPosition, endPosition, t);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }

    public void setGridRef (TheGrid reference)
    {
        grid_ref = reference;
    }

    public void setRhythmSystemRef (RhythmSystem reference)
    {
        rhythmSystem_ref = reference;
    }

}
}