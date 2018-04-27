using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SonicBloom.Koreo;

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

    [SerializeField]
    private BoxCollider charCollider, blockCollider;

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

    [SerializeField, Candlelight.PropertyBackingField]
    private AxisState _horizontalAxisState = AxisState.Idle;
    public AxisState horizontalAxisState
    {
        get
        {
            return _horizontalAxisState;
        }
        set
        {
            _horizontalAxisState = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private AxisState _verticalAxisState = AxisState.Idle;
    public AxisState verticalAxisState
    {
        get
        {
            return _verticalAxisState;
        }
        set
        {
            _verticalAxisState = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _deadZone = 0.02f;
    public float deadZone
    {
        get
        {
            return _deadZone;
        }
        set
        {
            _deadZone = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _gridSize = 1f;
    public float gridSize
    {
        get
        {
            return _gridSize;
        }
        set
        {
            _gridSize = value;
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
        }
    }

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

            _isStunned = value;

        }
    }

    [SerializeField]
    Countdown stunTimer;

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
            if(combo == 0) _multiplierCombo = 0;
            int i = (ID > 0) ? ID - 1 : 0;
            grid_ref.GetPlayerUIList () [i].setCombo (_combo);
        }
    }

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
            else if(_multiplierCombo == 0) multiplier = 1;

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

    [SerializeField, Candlelight.PropertyBackingField]
    private TheGrid _grid_ref;
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
    private RhythmSystem _rhythmSystem_ref;
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

    [Range (0, 2)]
    [SerializeField]
    private float beatPunchScale = 0.05f;

    [Range (0, 500)]
    [SerializeField]
    private int vibrato = 0;

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
        Koreographer.Instance.RegisterForEvents ("MaybeBeats", OnMainBeat);

    }

    private void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.CompareTag ("GridBlock"))
        {
            GridBlock gb = other.GetComponent<GridBlock> ();
            x = gb.X;
            z = gb.Z;

        }
    }

    public void Update ()
    {

        HandleAxisState (ref _horizontalAxisState, "Horizontal" + ID);
        HandleAxisState (ref _verticalAxisState, "Vertical" + ID);

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
                if ((input.y != 0 && verticalAxisState == AxisState.Down) !=
                    (input.x != 0 && horizontalAxisState == AxisState.Down))
                {

                    //Move ();
                    // combo++;

                    if (rhythmSystem_ref.WasNoteHit ())
                    {
                        print("Player" + ID.ToString() + " Hit it!");
                        Move ();
                        combo++;
                        multiplierCombo++;

                    }
                    else
                    {
                        combo = 0;
                    }

                }
                //StartCoroutine (move (transform));

            }
        }
    }

    public void isNextBlockEmpty (Vector3 start_pos, Vector3 end_pos)
    {

    }
    public void setIsMoving (bool value)
    {
        isMoving = value;
    }

    public void Stun ()
    {

    }
    public void Move ()
    {
        CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

        isMoving = true;

        Vector3 startGridPosition = new Vector3 (x, y, z);
        Vector3 endGridPosition = new Vector3 ();

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

        endGridPosition = grid_ref.getGridBlockPosition ((int) endGridPosition.x, (int) endGridPosition.z, endGridPosition.y);
        endGridPosition.y = 0.1f;

        transform.DOMove (endGridPosition, rhythmSystem_ref.rhythmTarget_Ref.duration).OnComplete (() => isMoving = false).SetEase (Ease.OutQuart);
        transform.DOLookAt (endGridPosition, 0.2f);

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

    void OnMainBeat (KoreographyEvent evt)
    {

        transform.DOPunchScale (transform.localScale * beatPunchScale, duration, vibrato, elasticity);

    }

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
}