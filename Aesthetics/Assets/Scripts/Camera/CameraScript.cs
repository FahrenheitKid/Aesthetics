using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    private TheGrid gridScript;

    [SerializeField]
    private float targetAngle = 0;
    [SerializeField]
    private const float rotationAmount = 1.5f;

    #region Property-Variables

    [SerializeField, Candlelight.PropertyBackingField]
    private float _stageOrtoSize = 0; // the ortographic Size of the camera needed to see the entire stage
    public float stageOrtoSize
    {
        get
        {
            return _stageOrtoSize;
        }
        set
        {
            _stageOrtoSize = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private Vector3[] _ViewportBoundaryPoints = new Vector3[4]; // the 4 boundary points of the stage
    public Vector3[] GetViewportBoundaryPoints ()
    {
        return _ViewportBoundaryPoints;
    }
    public void SetViewportBoundaryPoints (Vector3[] value)
    {
        _ViewportBoundaryPoints = value;
    }

    // how much zoom will change in each iteration
    [SerializeField, Candlelight.PropertyBackingField]
    private float _autoZoomAmount = 0.5f;
    public float autoZoomAmount
    {
        get
        {
            return _autoZoomAmount;
        }
        set
        {
            _autoZoomAmount = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _xVerticalOffset = 0.5f;
    public float xVerticalOffset
    {
        get
        {
            return _xVerticalOffset;
        }
        set
        {
            _xVerticalOffset = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _zVerticalOffset = 0.5f;
    public float zVerticalOffset
    {
        get
        {
            return _zVerticalOffset;
        }
        set
        {
            _zVerticalOffset = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _xHorizontallOffset = 0.5f;
    public float xHorizontallOffset
    {
        get
        {
            return _xHorizontallOffset;
        }
        set
        {
            _xHorizontallOffset = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _zHorizontalOffset = 0.5f;
    public float zHorizontalOffset
    {
        get
        {
            return _zHorizontalOffset;
        }
        set
        {
            _zHorizontalOffset = value;
        }
    }

    #endregion

    private void Awake ()
    {
        GetComponent<PostProcessLayer> ().enabled = true;
    }
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {
        // Rotation method
        if (Input.GetKeyDown (KeyCode.Q))
        {
            targetAngle -= 90.0f;
        }
        if (Input.GetKeyDown (KeyCode.E))
        {
            targetAngle += 90.0f;
        }

        if (Input.GetKeyDown (KeyCode.Z))
        {
            //SetViewportBoundaryPoints();
        }

        if (targetAngle != 0)
        {
            Rotate ();
        }
        else
        {
            ZoomUntilSize (stageOrtoSize);
        }

    }

    public void lookAt (GameObject go)
    {
        transform.LookAt (go.transform);
    }

    protected void Rotate ()
    {

        if (targetAngle > 0)
        {
            transform.RotateAround (transform.position, Vector3.up, -rotationAmount);
            targetAngle -= rotationAmount;
        }
        if (targetAngle < 0)
        {
            transform.RotateAround (transform.position, Vector3.up, rotationAmount);
            targetAngle += rotationAmount;
        }

    }

    //sets the 4 boundary points of the stage for Camera Zoom System
    public void setViewBoundaries ()
    {
        if (!gridScript) return;

        //north 
        foreach (GridBlock block in gridScript.GetGridBlockList ())
        {

            //west
            if (block.X == 0 && block.Z == 0)
            {
                GetViewportBoundaryPoints () [0] = block.transform.position;
                GetViewportBoundaryPoints () [0].x -= xHorizontallOffset;
                GetViewportBoundaryPoints () [0].z += zHorizontalOffset;

                print ("Bloco west = " + block.transform.position);
                print ("West before = " + GetViewportBoundaryPoints () [0]);
                GetViewportBoundaryPoints () [0].x = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [0]).x;
                GetViewportBoundaryPoints () [0].y = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [0]).y;
                GetViewportBoundaryPoints () [0].z = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [0]).z;
                print ("West set to = " + GetViewportBoundaryPoints () [0]);
            }
            //north
            else if (block.X == gridScript.mapWidth - 1 && block.Z == 0)
            {
                GetViewportBoundaryPoints () [1] = block.transform.position;
                GetViewportBoundaryPoints () [1].x += xVerticalOffset;
                GetViewportBoundaryPoints () [1].z += zVerticalOffset;

                GetViewportBoundaryPoints () [1].x = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [1]).x;
                GetViewportBoundaryPoints () [1].y = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [1]).y;
                GetViewportBoundaryPoints () [1].z = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [1]).z;
                print ("North set to = " + GetViewportBoundaryPoints () [1]);
            }
            //east
            else if (block.X == gridScript.mapWidth - 1 && block.Z == gridScript.mapHeight - 1)
            {
                GetViewportBoundaryPoints () [2] = block.transform.position;
                GetViewportBoundaryPoints () [2].x += xHorizontallOffset;
                GetViewportBoundaryPoints () [2].z -= zHorizontalOffset;

                GetViewportBoundaryPoints () [2].x = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [2]).x;
                GetViewportBoundaryPoints () [2].y = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [2]).y;
                GetViewportBoundaryPoints () [2].z = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [2]).z;
                print ("East set to = " + GetViewportBoundaryPoints () [2]);
            }
            //south
            else if (block.X == 0 && block.Z == gridScript.mapHeight - 1)
            {
                GetViewportBoundaryPoints () [3] = block.transform.position;
                GetViewportBoundaryPoints () [3].x -= xVerticalOffset;
                GetViewportBoundaryPoints () [3].z -= zVerticalOffset;

                GetViewportBoundaryPoints () [3].x = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [3]).x;
                GetViewportBoundaryPoints () [3].y = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [3]).y;
                GetViewportBoundaryPoints () [3].z = GetComponent<Camera> ().WorldToViewportPoint (GetViewportBoundaryPoints () [3]).z;
                print ("South set to = " + GetViewportBoundaryPoints () [3]);
            }
        }
    }

    //looks at the center gridblock
    public void cameraParentToCenterPosition ()
    {
        Camera.main.transform.parent.transform.position = gridScript.GetGridBlock (gridScript.mapWidth / 2, gridScript.mapHeight / 2).gameObject.transform.position;
    }

    //zoomOut autoZoomAmount if the view boundaries are unseen
    private void ZoomOutUntilSeen ()
    {

        if (!UtilityTools.isPointInViewport (GetViewportBoundaryPoints ()))
        {
            GetComponent<Camera> ().orthographicSize += autoZoomAmount;
        }

        setViewBoundaries ();
        cameraParentToCenterPosition ();

    }

    //Zooms out until ViewBoundaries are seen inside a loop. loopLimit is how many times the loop can iterate
    public void ZoomOutLoopUntilSeen (uint loopLimit)
    {
        int count = 0;
        while (!UtilityTools.isPointInViewport (GetViewportBoundaryPoints ()) && count <= loopLimit)
        {
            GetComponent<Camera> ().orthographicSize += autoZoomAmount;

            setViewBoundaries ();
            cameraParentToCenterPosition ();
            count++;
        }

        if (count >= loopLimit)
        {
            print ("Loop do zoom passou do limite!!!");
        }
        stageOrtoSize = GetComponent<Camera> ().orthographicSize;
    }

    //Zooms (In or Out) autoZoomAmuount closert to the parameter ortoSize
    private void ZoomUntilSize (float ortographic_size)
    {

        if (GetComponent<Camera> ().orthographicSize < ortographic_size)
        {
            GetComponent<Camera> ().orthographicSize += autoZoomAmount;
        }
        else if (GetComponent<Camera> ().orthographicSize > ortographic_size)
        {
            GetComponent<Camera> ().orthographicSize -= autoZoomAmount;
        }

    }
}