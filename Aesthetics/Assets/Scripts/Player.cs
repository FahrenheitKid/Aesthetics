using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        private bool isMoving = false;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float t;
        private float factor;

        #endregion

        private void Awake() {
            ID = ++globalID;
        }

        private void Start ()
        {

        }
    public void Update ()
    {
        if (!isMoving)
        {
            input = new Vector2 (Input.GetAxis ("Horizontal" + ID), Input.GetAxis ("Vertical" +ID));
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
                StartCoroutine (move (transform));
            }
        }
    }

    public IEnumerator move (Transform transform)
    {
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