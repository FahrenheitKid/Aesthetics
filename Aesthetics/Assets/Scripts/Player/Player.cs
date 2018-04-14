using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        }
    }

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
    private TheGrid _grid_ref = new TheGrid ();
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
            if(globalID >=2) globalID = 0;
        ID = ++globalID;
    }

    private void Start ()
    {

    }

    private void OnTriggerEnter (Collider other)
    {
        print ("triggerei com " + other.gameObject.name);
        if (other.gameObject.CompareTag ("GridBlock"))
        {
            GridBlock gb = other.GetComponent<GridBlock> ();
            x = gb.X;
            z = gb.Z;

        }
    }

    public void Update ()
    {
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

            if (input != Vector2.zero)
            {
                //StartCoroutine (move (transform));
                Move ();
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

        transform.DOMove (endGridPosition, 0.4f).OnComplete (() => isMoving = false).SetEase (Ease.InQuint);
        transform.DOLookAt (endGridPosition, 0.2f);

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