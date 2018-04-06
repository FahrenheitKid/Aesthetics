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
    private GameObject _scoreMaker_prefab;
    public GameObject scoreMaker_prefab
    {
        get
        {
            return _scoreMaker_prefab;
        }
        set
        {
            _scoreMaker_prefab = value;
        }
    }

    [SerializeField]
    Countdown timer = new Countdown ();

    [SerializeField, Candlelight.PropertyBackingField]
    private List<GameObject> _PlayerPrefabList = new List<GameObject> (4);
    public List<GameObject> GetPlayerPrefabList ()
    {
        return _PlayerPrefabList;
    }
    public void SetPlayerPrefabList (List<GameObject> value)
    {
        _PlayerPrefabList = new List<GameObject> (value);
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private List<Player> _PlayerList = new List<Player> ();
    public List<Player> GetPlayerList ()
    {
        return _PlayerList;
    }
    public void SetPlayerList (List<Player> value)
    {
        _PlayerList = new List<Player> (value);
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
        private set
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
        private set
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

    void Awake ()
    {
        tiles = Load (Application.dataPath + "\\Resources\\" + fileNameToLoad);
        BuildMap ();

        timer = gameObject.AddComponent<Countdown> ();

        SpawnPlayers ();

    }

    // Use this for initialization
    void Start ()
    {
        timer.startTimer (10f);
        SpawnScoreMaker (Random.Range (0, mapWidth), Random.Range (0, mapHeight));
    }

    // Update is called once per frame
    void Update ()
    {
        if (timer.stop)
        {
            timer.startTimer (10f);
            SpawnScoreMaker (Random.Range (0, mapWidth), Random.Range (0, mapHeight));
        }
    }

    private void SpawnPlayers ()
    {
        for (int i = 0; i < GetPlayerList ().Count; i++)
        {
            Vector3 initial_pos = new Vector3 ();

            if (i == 0)
            {
                initial_pos = GetGridBlock (0, 0).gameObject.transform.position;
                initial_pos.y = 0.1f;
            }
            else if (i == 1)
            {
                initial_pos = GetGridBlock (mapWidth - 1, mapHeight - 1).gameObject.transform.position;
                initial_pos.y = 0.1f;
            }
            GameObject player_prefab = Instantiate (GetPlayerPrefabList () [i], initial_pos, Quaternion.identity) as GameObject;
            player_prefab.GetComponent<Player> ().grid_ref = this;
        }
    }

    private void SpawnScoreMaker (int x, int z)
    {
        GameObject scorePrefab = Instantiate (scoreMaker_prefab, getGridBlockPosition (x, z, 0.8f), Quaternion.identity) as GameObject;
        ScoreMaker sm = scorePrefab.GetComponent<ScoreMaker> ();
        sm.grid_ref = GetComponent<TheGrid> ();
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
                _GridBlockList.Add (TilePrefab.GetComponent<GridBlock> ());

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

    public void Score (Player p)
    {
        if (p == null || _GridBlockList == null) return;

        if (GetGridBlockList ().Count == 0 || GetGridBlockList () == null)
            print ("gridBlockCount vazio ou null!");

        foreach (GridBlock gb in GetGridBlockList ())
        {

            if (gb.owner == null) continue;
            if (gb.owner.ID == p.ID)
            {

                gb.changeColor (GridBlock.gridBlockColor.White);
                gb.changeOwner (null);
                p.score++;
            }
        }

        print ("Player " + p.gameObject.name + " has now: " + p.score + " Points!");
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

    public Vector3 getGridBlockPosition (int x, int z, float y)
    {
        Vector3 pos = GetGridBlock (x, z).gameObject.transform.position;

        pos.y = y;
        return pos;
    }
}