#define MAP_LOADING_DEBUG
#undef MAP_LOADING_DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    Countdown timer;

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
    private List<PlayerUI> _PlayerUIList = new List<PlayerUI> (4);
    public List<PlayerUI> GetPlayerUIList ()
    {
        return _PlayerUIList;
    }
    public void SetPlayerUIList (List<PlayerUI> value)
    {
        _PlayerUIList = new List<PlayerUI> (value);
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

    [SerializeField, Candlelight.PropertyBackingField]
    private bool _isRandomScoreMakerSpawnTime = true;
    public bool isRandomScoreMakerSpawnTime
    {
        get
        {
            return _isRandomScoreMakerSpawnTime;
        }
        set
        {
            _isRandomScoreMakerSpawnTime = value;
        }
    }

    // corresponds to [Range(0f, 1f)]
    [SerializeField, Candlelight.PropertyBackingField (typeof (RangeAttribute), 3f, 10f)]
    private float _ScoreMakerSpawnTime;
    public float ScoreMakerSpawnTime
    {
        get { return _ScoreMakerSpawnTime; }
        set { _ScoreMakerSpawnTime = value; }
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
        tiles = Load (Application.streamingAssetsPath + "\\" + fileNameToLoad);
        BuildMap ();

        timer = gameObject.AddComponent<Countdown> ();

        SpawnPlayers ();

    }

    // Use this for initialization
    void Start ()
    {
        if (isRandomScoreMakerSpawnTime)
            timer.startTimer (Random.Range (3f, ScoreMakerSpawnTime));
        else
            timer.startTimer (ScoreMakerSpawnTime);

        SpawnScoreMaker (Random.Range (0, mapWidth), Random.Range (0, mapHeight));
    }

    // Update is called once per frame
    void Update ()
    {

        if (Input.GetKeyDown (KeyCode.Escape))
        {
            QuitGame ();
        }

        if (Input.GetKeyDown (KeyCode.R))
        {

            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
        }

        if (timer.stop)
        {
            if (isRandomScoreMakerSpawnTime)
                timer.startTimer (Random.Range (3f, ScoreMakerSpawnTime));
            else
                timer.startTimer (ScoreMakerSpawnTime);
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
            player_prefab.GetComponent<Player> ().rhythmSystem_ref = rhythmSystem_ref;
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
        #if MAP_LOADING_DEBUG
        Debug.Log ("Building Map...");
        #endif
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
         #if MAP_LOADING_DEBUG
        Debug.Log ("Building Completed!");
       
        print (mapWidth / 2 + " | " + mapHeight / 2);
        #endif
        cameraScript.cameraParentToCenterPosition ();
        //Camera.main.transform.parent.LookAt(GetGridBlock (mapWidth / 2, mapHeight / 2).gameObject.transform);

        cameraScript.setViewBoundaries ();
        cameraScript.ZoomOutLoopUntilSeen (100);

    }

    private int[, ] Load (string filePath)
    {
        try
        {
             #if MAP_LOADING_DEBUG
            Debug.Log ("Loading File...");
            #endif

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

                 #if MAP_LOADING_DEBUG
                print ("Map Width: " + mapWidth + " | Map Height: " + mapHeight);
                #endif

                // read the rest of the file
                string input = sr.ReadToEnd ();
                string[] lines = input.Split (new []
                {
                    '\r',
                    '\n'
                }, System.StringSplitOptions.RemoveEmptyEntries);
                int[, ] tiles = new int[lines.Length, mapWidth];

                 #if MAP_LOADING_DEBUG
                Debug.Log ("Parsing...");
                #endif

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
                 #if MAP_LOADING_DEBUG
                Debug.Log ("Parsing Completed!");
                #endif
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

        int result = 0;
        foreach (GridBlock gb in GetGridBlockList ())
        {

            if (gb.owner == null) continue;
            if (gb.owner.ID == p.ID)
            {

                gb.changeColor (GridBlock.gridBlockColor.White);
                gb.changeOwner (null);
                result++;
            }
        }

        result *= p.multiplier;
        p.multiplierCombo = 0;
        p.score += result;
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

    public void QuitGame ()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}