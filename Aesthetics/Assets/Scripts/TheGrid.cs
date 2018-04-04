using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TheGrid : MonoBehaviour
{

    #region Property-Variables

    [SerializeField, Candlelight.PropertyBackingField]
    private GameObject _gridBlock_prefab1;
    public GameObject gridBlock_prefab1
    {
        get
        {
            return _gridBlock_prefab1;
        }
        set
        {
            _gridBlock_prefab1 = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private GameObject _gridBlock_prefab2;
    public GameObject gridBlock_prefab2
    {
        get
        {
            return _gridBlock_prefab2;
        }
        set
        {
            _gridBlock_prefab2 = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private CameraScript _cameraScript;
    public CameraScript cameraScript
    {
        get
        {
            return _cameraScript;
        }
        set
        {
            _cameraScript = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _mapWidth = 0;
    public int mapWidth
    {
        get
        {
            return _mapWidth;
        }
        set
        {
            _mapWidth = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _mapHeight = 0;
    public int mapHeight
    {
        get
        {
            return _mapHeight;
        }
        set
        {
            _mapHeight = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _xOffset = 1;
    public int xOffset
    {
        get
        {
            return _xOffset;
        }
        set
        {
            _xOffset = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private int _zOffset = 1;
    public int zOffset
    {
        get
        {
            return _zOffset;
        }
        set
        {
            _zOffset = value;
        }
    }

    [SerializeField]
    private string fileNameToLoad;
    private int[, ] tiles;

    [SerializeField, Candlelight.PropertyBackingField]
    private List<GridBlock> _GridBlockList = new List<GridBlock> ();
    public List<GridBlock> GetGridBlockList ()
    {
        return _GridBlockList;
    }
    public void SetGridBlockList (List<GridBlock> value)
    {
        _GridBlockList = new List<GridBlock> (value);
    }

    #endregion

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    void Awake ()
    {
        tiles = Load (Application.dataPath + "\\Resources\\" + fileNameToLoad);
        BuildMap ();
    }

    void BuildMap ()
    {
        Debug.Log ("Building Map...");
        for (int i = 0; i < tiles.GetLength (0); i++)
        {
            for (int j = 0; j < tiles.GetLength (1); j++)
            {
                GameObject TilePrefab = Instantiate (gridBlock_prefab1, new Vector3 (xOffset * j - mapWidth * xOffset, 0, zOffset * mapHeight - i * zOffset), Quaternion.identity) as GameObject;

                TilePrefab.transform.parent = transform;
                TilePrefab.GetComponent<GridBlock> ().init (j, 0, i);
                TilePrefab.GetComponent<GridBlock> ().changeColor ((GridBlock.gridBlockColor) tiles[i, j]);
                GetGridBlockList ().Add (TilePrefab.GetComponent<GridBlock> ());

            }
        }
        Debug.Log ("Building Completed!");

        print (mapWidth / 2 + " | " + mapHeight / 2);

        cameraScript.cameraParentToCenterPosition ();
        //Camera.main.transform.parent.LookAt(GetGridBlock (mapWidth / 2, mapHeight / 2).gameObject.transform);

        cameraScript.setViewBoundaries ();
        cameraScript.ZoomOutLoopUntilSeen (100);

    }

    private int[, ] Load (string filePath)
    {
        try
        {
            Debug.Log ("Loading File...");
            using (StreamReader sr = new StreamReader (filePath))
            {

                // read first line to detect width and height size
                string width_height = sr.ReadLine ();
                string[] aux = width_height.Split (new []
                {
                    ','
                });
                int aux_val;
                if (int.TryParse (aux[0], out aux_val))
                    mapWidth = aux_val;
                if (int.TryParse (aux[1], out aux_val))
                    mapHeight = aux_val;

                print ("Map Width: " + mapWidth + " | Map Height: " + mapHeight);
                // read the rest of the file
                string input = sr.ReadToEnd ();
                string[] lines = input.Split (new []
                {
                    '\r',
                    '\n'
                }, System.StringSplitOptions.RemoveEmptyEntries);
                int[, ] tiles = new int[lines.Length, mapWidth];
                Debug.Log ("Parsing...");
                for (int i = 0; i < lines.Length; i++)
                {
                    string st = lines[i];
                    string[] nums = st.Split (new []
                    {
                        ','
                    });
                    if (nums.Length != mapWidth)
                    {

                    }
                    for (int j = 0; j < Mathf.Min (nums.Length, mapWidth); j++)
                    {
                        int val;
                        if (int.TryParse (nums[j], out val))
                        {
                            tiles[i, j] = val;
                        }
                        else
                        {
                            tiles[i, j] = 1;
                        }
                    }
                }
                Debug.Log ("Parsing Completed!");
                return tiles;
            }
        }
        catch (IOException e)
        {
            Debug.Log (e.Message);
        }
        return null;
    }

    public GridBlock GetGridBlock (int x, int z)
    {

        foreach (GridBlock go in GetGridBlockList ())
        {
            if (go.X == x && go.Z == z)
                return go;

        }

        return null;

    }
}