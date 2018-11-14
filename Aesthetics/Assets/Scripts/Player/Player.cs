using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aesthetics
{

    public class Player : MonoBehaviour
    {

        public enum AxisState
        {
            Idle,
            Down,
            Held,
            Up
        }

        public enum InputType
        {
            WASD,
            Arrows,
            Xbox,
            PS4
        }

        public enum Character
        {
            AnimeGirl,
            David,
            Skull,
            Afro
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

         [SerializeField, Candlelight.PropertyBackingField]
        private int _inputID = 0;
        public int inputID
        {
            get
            {
                return _inputID;
            }
            set
            {
                _inputID = value;
            }
        }

        [SerializeField, Candlelight.PropertyBackingField]
        private InputType _controllerType = 0;
        public InputType controllerType
        {
            get
            {
                return _controllerType;
            }
            set
            {
                _controllerType = value;
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

                if (value)
                {
                    combo = 0;
                    multiplierCombo = 0;
                    disableInput = true;
                    setShaderColors( stunColor, stunColor, stunColor, stunColor, stunColor, stunColor);
                }
                else
                {

                   setShaderColors(colorPrim[0],colorPrim[1],colorSec[0], colorSec[1], colorTert[0], colorTert[1]);


                    disableInput = false;
                }

                _isStunned = value;

            }
        }

        [SerializeField]
        Color32 stunColor;

        [SerializeField]
        Color32 [] colorPrim = new Color32[2];
        [SerializeField]
        Color32 [] colorSec = new Color32[2];
        [SerializeField]
        Color32 [] colorTert = new Color32[2];

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

        [Tooltip ("Is the player aiming with an item?")]
        [SerializeField, Candlelight.PropertyBackingField]
        private bool _isAiming = false;
        public bool isAiming
        {
            get
            {
                return _isAiming;
            }
            set
            {

                _isAiming = value;

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
        [Tooltip ("shield Immunity duration?")]
        [SerializeField, ]
        public float shieldImmunity_duration = 3f;

        [Tooltip ("the block where the player fell?")]
        [SerializeField, ]
        public GridBlock fall_gridBlock_Ref = null;

        [Tooltip ("model renderer ref")]
        [SerializeField, ]
        public Renderer renderer_Ref = null;

        [Tooltip ("model renderer ref")]
        [SerializeField, ]
        public Animator animator_Ref = null;

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
                    renderer_Ref.sharedMaterials[0].shader = glass;

                }
                else
                {
                    Shader normal = Shader.Find ("Toon/Lit ColorMask Gradient");
                    renderer_Ref.sharedMaterials[0].shader = normal;
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
                if (_multiplier <= 0) _multiplier = 1;
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

             public int inputCountX;
            public int inputCountY;
            private bool X_isAxisInUse = false;
            private bool Y_isAxisInUse = false;
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
            if (globalID >= 4) globalID = 0;
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

            setShaderColors(colorPrim[0],colorPrim[1],colorSec[0], colorSec[1], colorTert[0], colorTert[1]);

        }

        private void OnTriggerEnter (Collider other)
        {
            // update player's current grid
            if (other.gameObject.CompareTag ("GridBlock"))
            {
                GridBlock gb = other.GetComponent<GridBlock> ();
                if(x != gb.X)
                x = gb.X;
                if(z != gb.Z)
                z = gb.Z;

                if (other.GetComponent<GridBlock> ().isFallen) //if stepped on fallen gridblock
                {
                    if (!isImmune && !isShielded && !isFallen)
                    {
                        //make player fall down on the next beat
                        disableInput = true;
                        fall_gridBlock_Ref = other.GetComponent<GridBlock> ();
                        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, Fall);
                    }

                    if (isShielded)
                    {
                        if (item)
                        {
                            if (item.GetType () == typeof (FloppyDisk))
                            {

                                isShielded = false;

                                Immune (shieldImmunity_duration);
                                item.Kill (null);
                                hasItem = false;
                                item = null;
                            }
                        }

                    }

                }

            }

        }

        private void OnTriggerStay (Collider other)
        {

            if (other.gameObject.CompareTag ("GridBlock"))
            {

                 GridBlock gb = other.GetComponent<GridBlock> ();
                 if(x != gb.X)
                x = gb.X;
                if(z != gb.Z)
                z = gb.Z;

                if (other.GetComponent<GridBlock> ().isFallen) //if stepped on fallen gridblock
                {
                    if (!isImmune && !isShielded && !isFallen)
                    {
                        //make player fall down on the next beat
                        disableInput = true;
                        fall_gridBlock_Ref = other.GetComponent<GridBlock> ();
                        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.mainBeatID, Fall);
                    }

                    if (isShielded)
                    {
                        if (item)
                        {
                            if (item.GetType () == typeof (FloppyDisk))
                            {
                                Immune (shieldImmunity_duration);
                                item.Kill (null);
                                hasItem = false;
                                item = null;
                            }
                        }
                        isShielded = false;

                    }

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

                    Immune (respawnImmunity_duration); // make player imune for a few more seconds
                    //immunityTimer.startTimer (respawnImmunity_duration); 
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

            if (isStunned)
            {
                if (stunTimer.stop)
                {
                    isStunned = false;
                }
            }
        }


        string getHorizontalInputName()
        {
            return "Horizontal " + inputID + " " + controllerType.ToString();
        }

        string getVerticalInputName()
        {
            return "Vertical " + inputID + " " + controllerType.ToString();
        }
        void handleInput ()
        {
            //int numPressed = 0;
            
                HandleAxisState (ref horizontalAxisState,getHorizontalInputName());
                HandleAxisState (ref verticalAxisState, getVerticalInputName());
            
            

            if (isStunned)
            {
                return;
            }

            if (!isMoving)
            {
                input = new Vector2 (Input.GetAxis (getHorizontalInputName()), Input.GetAxis (getVerticalInputName()));
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

                            //if aiming, shoot in direction pressed
                            if (isAiming && hasItem && item && item.GetType () == typeof (Revolver))
                            {
                                Revolver r = (Revolver) item;
                                r.shot_direction = getDirectionFromInput ();
                                r.Shoot (getDirectionFromInput ());
                                //print("NEW SHOT = " + r.shot_direction);

                            }
                            else
                            {
                                //move player and increase combo
                                if (Move ())
                                {
                                    combo++;
                                    multiplierCombo++;
                                }
                            }

                        }
                        else
                        {
                            //lose combo

                            Vector3 pos = transform.position;
                            pos.y += renderer_Ref.bounds.size.y + 0.0f;
                            grid_ref.SpawnMissFloatingText (pos);

                            combo = 0;
                        }

                    }
                    //StartCoroutine (move (transform));

                }
                else

                if (Input.GetButtonDown ("ActionA " + inputID + " " + controllerType) &&
                    ((verticalAxisState == AxisState.Idle) &&
                        (horizontalAxisState == AxisState.Idle)))
                {

                    //not allow action with movement

                    if (hasItem && item && item.GetType () != typeof (FloppyDisk))
                    {

                        //if Pressed on the beat
                        if (rhythmSystem_ref.WasNoteHit ())
                        {
                            //print ("Player" + ID.ToString () + " Hit it!");

                            if (item.GetType () == typeof (Revolver))
                            {
                                isAiming = true;
                            }
                            else
                            {
                                if (!isAiming)
                                    item.Use ();
                            }

                        }
                        else
                        {
                            //lose combo

                            Vector3 pos = transform.position;
                            pos.y += renderer_Ref.bounds.size.y + 0.0f;
                            grid_ref.SpawnMissFloatingText (pos);

                            combo = 0;
                        }
                    }

                }
            }

        }
        public void setIsMoving (bool value)
        {
            isMoving = value;
        }

        public void Immune (float duration)
        {
          
            if (duration == 0) return;
            if (immunityTimer.timeLeft >= duration && !immunityTimer.stop) return;
            
            isImmune = true;
            immunityTimer.startTimer (duration);
        }
        void Fall (KoreographyEvent evt)
        {
            isFallen = true;
            disableInput = true;

            //lose item when falling
            if (hasItem)
                item.Kill (null);

            Vector3 startPos = transform.position;
            startPos.y -= 15;

            combo = 0;
            multiplierCombo = 0;
            multiplier = 0;
            //transform.DOMove (startPos, rhythmSystem_ref.rhythmTarget_Ref.duration * 2);
            //transform.DOMoveY (startPos.y, respawn_duration);
            transform.DOMoveY (startPos.y, fall_duration / 1.5f).SetEase (Ease.InOutCubic);

            Koreographer.Instance.UnregisterForEvents (rhythmSystem_ref.mainBeatID, Fall);

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
            respawnBlock.respawnTimer.startTimer (respawn_duration);

        }
        public bool Stun (float duration)
        {
            if (isImmune) return false;
            if (isShielded)
            {
                if (item)
                {
                    if (item.GetType () == typeof (FloppyDisk))
                    {
                        item.Kill (null);
                    }
                }

                Immune (shieldImmunity_duration);
                isShielded = false;
                return false;
            }

            stunTimer.startTimer (duration);
            isStunned = true;
            return true;

        }

        public bool Steal (Player target, bool stealLocked)
        {
            print ("Stealing " + target.name);
            if (target.isImmune) return false;
            if (target.isShielded)
            {
                if (target.item)
                {
                    if (target.item.GetType () == typeof (FloppyDisk))
                    {
                        target.item.Kill (null);
                    }
                }

                target.Immune (target.shieldImmunity_duration);
                target.isShielded = false;
                return false;
            }

            foreach (GridBlock gb in grid_ref.GetGridBlockList ())
            {

                if (gb.owner == null || gb.owner != target) continue;

                if (!stealLocked)
                    if (gb.isLocked)
                        continue;

                if (gb.isLocked)
                    gb.changeColor (this.blackGridColor);
                else gb.changeColor (this.gridColor);

                gb.changeOwner (this);
                gb.stolenOwner = target;

            }

            return true;

        }
        public bool Move ()
        {
            CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

            Vector3 startGridPosition = new Vector3 (x, y, z);
            Vector3 endGridPosition = new Vector3 ();

            GridBlock endGridBlock;
            GridBlock startGridBlock;
            startGridBlock = grid_ref.GetGridBlock (x, z);

            CameraScript.windRose? player_direction = null;

            player_direction = getDirectionFromInput ();

            endGridBlock = getDestinationBlock (player_direction, 1);

            //if the player is with sneakers, need to move double
            if (hasItem && item.GetType ().Name == typeof (Sneakers).Name)
            {
                
                // if both blocks are availaable, move to second, otherwise move only one.
                if (isMyDestinationBlockAvailable (player_direction, 1) && isMyDestinationBlockAvailable (player_direction, 2))
                {
                    if (!getDestinationBlock (player_direction, 1).isFallen)
                        endGridBlock = getDestinationBlock (player_direction, 2);
                }

            }

            //if destination gridBlock is already occupied
            if (endGridBlock == null)
            {
               

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
                x = endGridBlock.X;
                z = endGridBlock.Z;
                return true;

            }

        }


        void setShaderColors(Color32 colorPrim1, Color32 colorPrim2, Color32 colorSec1,Color32 colorSec2, Color32 colorTert1, Color32 colorTert2)
        {
            if(!renderer_Ref) return;

            Shader shader_ref = renderer_Ref.sharedMaterials[0].shader;

                    renderer_Ref.materials[0].SetColor ("_ColorPrim1", colorPrim1);
                    renderer_Ref.materials[0].SetColor ("_ColorPrim2", colorPrim2);
                    
                    renderer_Ref.materials[0].SetColor ("_ColorSec1", colorSec1);
                    renderer_Ref.materials[0].SetColor ("_ColorSec2", colorSec2);

                    renderer_Ref.sharedMaterials[0].SetColor ("_ColorTert1", colorTert1);
                    renderer_Ref.materials[0].SetColor ("_ColorTert2", colorTert2);
        }

        public void setModelColors(Color32[] prim, Color32[] sec, Color32[] tert)
        {
            if(!renderer_Ref) return;

            colorPrim = prim;
            colorSec =  sec;
            colorTert = tert;

            setShaderColors(colorPrim[0],colorPrim[1],colorSec[0], colorSec[1], colorTert[0], colorTert[1]);
        }


        CameraScript.windRose? getDirectionFromInput ()
        {
            CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

            CameraScript.windRose? player_direction = null;
            //defines player move direction according to input and camera orientation
            switch (cameraScript.orientation)
            {
                case CameraScript.windRose.North:

                    if (input.x > 0)
                    {

                        player_direction = CameraScript.windRose.East;

                    }
                    else if (input.x < 0)
                    {

                        player_direction = CameraScript.windRose.West;

                    }
                    else if (input.y > 0)
                    {

                        player_direction = CameraScript.windRose.North;

                    }
                    else if (input.y < 0)
                    {

                        player_direction = CameraScript.windRose.South;
                    }

                    break;

                case CameraScript.windRose.East:
                    if (input.x > 0)
                    {

                        player_direction = CameraScript.windRose.South;

                    }
                    else if (input.x < 0)
                    {

                        player_direction = CameraScript.windRose.North;
                    }
                    else if (input.y > 0)
                    {

                        player_direction = CameraScript.windRose.East;

                    }
                    else if (input.y < 0)
                    {

                        player_direction = CameraScript.windRose.West;
                    }

                    break;

                case CameraScript.windRose.South:

                    if (input.x > 0)
                    {

                        player_direction = CameraScript.windRose.West;

                    }
                    else if (input.x < 0)
                    {

                        player_direction = CameraScript.windRose.East;
                    }
                    else if (input.y > 0)
                    {

                        player_direction = CameraScript.windRose.South;

                    }
                    else if (input.y < 0)
                    {

                        player_direction = CameraScript.windRose.North;

                    }

                    break;

                case CameraScript.windRose.West:

                    if (input.x > 0)
                    {

                        player_direction = CameraScript.windRose.North;

                    }
                    else if (input.x < 0)
                    {

                        player_direction = CameraScript.windRose.South;
                    }
                    else if (input.y > 0)
                    {

                        player_direction = CameraScript.windRose.West;

                    }
                    else if (input.y < 0)
                    {

                        player_direction = CameraScript.windRose.East;
                    }

                    break;

                default:
                    break;

            }

            return player_direction;
        }

        bool isMyDestinationBlockAvailable (CameraScript.windRose? move_direction, int movement_amount)
        {
            if (move_direction == null) return false;
            CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

            Vector3 startGridPosition = new Vector3 (x, y, z);
            Vector3 endGridPosition = new Vector3 ();

            GridBlock endGridBlock;
            GridBlock startGridBlock;
            startGridBlock = grid_ref.GetGridBlock (x, z);

            switch (move_direction)
            {
                case CameraScript.windRose.North:

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z -= movement_amount;

                    break;

                case CameraScript.windRose.South:

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - movement_amount)
                        endGridPosition.z += movement_amount;

                    break;
                case CameraScript.windRose.West:

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x -= movement_amount;

                    break;
                case CameraScript.windRose.East:
                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - movement_amount)
                        endGridPosition.x += movement_amount;

                    break;

            }

            endGridBlock = grid_ref.GetGridBlock ((int) endGridPosition.x, (int) endGridPosition.z);

            if (!endGridBlock) return false;
            //if destination gridBlock is already occupied
            if (endGridBlock.isOccupied || endGridBlock.isBlocked || endGridBlock.isRespawning)
            {

                //print ("Block (" + endGridBlock.X + ", " + endGridBlock.Z + ") occupied!");
                return false;

            }
            else return true;

        }

        // returns the destination grid block, given a direction the player is going and how many tiles he is going to move
        // returns null if block is blocked by something or unavailable
        GridBlock getDestinationBlock (CameraScript.windRose? move_direction, int movement_amount)
        {
            if (move_direction == null) return null;
            CameraScript cameraScript = Camera.main.gameObject.GetComponent<CameraScript> ();

            Vector3 startGridPosition = new Vector3 (x, y, z);
            Vector3 endGridPosition = new Vector3 ();

            GridBlock endGridBlock;
            GridBlock startGridBlock;
            startGridBlock = grid_ref.GetGridBlock (x, z);

            switch (move_direction)
            {
                case CameraScript.windRose.North:

                    endGridPosition = startGridPosition;
                    if (endGridPosition.z > 0)
                        endGridPosition.z -= movement_amount;

                    break;

                case CameraScript.windRose.South:

                    endGridPosition = startGridPosition;

                    if (endGridPosition.z < grid_ref.mapHeight - movement_amount)
                        endGridPosition.z += movement_amount;

                    break;
                case CameraScript.windRose.West:

                    endGridPosition = startGridPosition;

                    if (endGridPosition.x > 0)
                        endGridPosition.x -= movement_amount;

                    break;
                case CameraScript.windRose.East:
                    endGridPosition = startGridPosition;

                    if (endGridPosition.x < grid_ref.mapWidth - movement_amount)
                        endGridPosition.x += movement_amount;

                    break;

            }

            endGridBlock = grid_ref.GetGridBlock ((int) endGridPosition.x, (int) endGridPosition.z);

            if (!endGridBlock) 
            {
               
                return null;
            }

            //if destination gridBlock is already occupied
            if (endGridBlock.isOccupied || endGridBlock.isBlocked || endGridBlock.isRespawning)
            {
               
                //print ("Block (" + endGridBlock.X + ", " + endGridBlock.Z + ") occupied!");
                return null;

            }
            else return endGridBlock;

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

        void HandleAxisStateDPad (ref AxisState state, string axi)
        {

                /*
                if (Input.GetAxisRaw ("DpadX") != 0)
            {
                if (X_isAxisInUse == false)
                {
                    if (Input.GetAxisRaw ("DpadX") == +1)
                    {
                        inputCountX += 1;
                    }
                    else if (Input.GetAxisRaw ("DpadX") == -1)
                    {
                        inputCountX -= 1;
                    }
                    X_isAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw ("DpadX") == 0)
            {
                X_isAxisInUse = false;
            }
            //----------------------------------------
            if (Input.GetAxisRaw ("DpadY") != 0)
            {
                if (Y_isAxisInUse == false)
                {
                    if (Input.GetAxisRaw ("DpadY") == +1)
                    {
                        inputCountY += 1;
                    }
                    else if (Input.GetAxisRaw ("DpadY") == -1)
                    {
                        inputCountY -= 1;
                    }
                    Y_isAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw ("DpadY") == 0)
            {
                Y_isAxisInUse = false;
            }
                 */
            

            switch (state)
            {
                case AxisState.Idle:
                    if (Input.GetAxisRaw (axi) < -deadZone || Input.GetAxisRaw (axi) > deadZone)
                    {
                        state = AxisState.Down;
                    }
                    break;

                case AxisState.Down:
                    state = AxisState.Held;
                    break;

                case AxisState.Held:
                    if (Input.GetAxisRaw (axi) > -deadZone && Input.GetAxisRaw (axi) < deadZone)
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
        public IEnumerator xmoveOld (Transform transform)
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