#define MAP_LOADING_DEBUG
#undef MAP_LOADING_DEBUG

#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class TheGrid : MonoBehaviour
{

    #region Property-Variables

    [SerializeField]
    private GameObject gridBlock_prefab1;

    [SerializeField]
    private GameObject scoreMaker_prefab;

    [SerializeField]
    private GameObject scoreFloatingText_prefab;

    [SerializeField]
    private GameObject missFloatingText_prefab;

    [SerializeField]
    private RhythmSystem rhythmSystem_ref;

    [SerializeField]
    Countdown timer;

    [SerializeField]
    private List<GameObject> playerPrefabList = new List<GameObject> (2);

    [SerializeField]
    private List<PlayerUI> playerUIList = new List<PlayerUI> (2);

    public List<PlayerUI> GetPlayerUIList()
    {
        return playerUIList;
    }

    [SerializeField]
    private List<Player> playerList = new List<Player> (2);

    [SerializeField]
    public List<Item> itemList;

    [SerializeField]
    public List<FloatingText> floatingTextList;

    [SerializeField]
    private CameraScript cameraScript;

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

    [SerializeField]
    private int xOffset = 1;

    [SerializeField]
    private int zOffset = 1;

    [SerializeField]
    private bool isRandomScoreMakerSpawnTime = true;

    // corresponds to [Range(0f, 1f)]
    [Range(3.0f,10.0f)]
    [SerializeField]
    private float ScoreMakerSpawnTime;


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

       #if DEBUG
		prefabsAssertions();
		#endif

        if (isRandomScoreMakerSpawnTime)
            timer.startTimer (Random.Range (3f, ScoreMakerSpawnTime));
        else
            timer.startTimer (ScoreMakerSpawnTime);

        
        
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

            if (itemList.Count < 5)
            {
                ScoreMaker sm = SpawnScoreMaker (Random.Range (0, mapWidth), Random.Range (0, mapHeight));

            }

        }
    }

    //Spawn Players (currrently 2)
    private void SpawnPlayers ()
    {
        for (int i = 0; i < playerList.Count; i++)
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
            GameObject player_prefab = Instantiate (playerPrefabList [i], initial_pos, Quaternion.identity) as GameObject;
            player_prefab.GetComponent<Player> ().setGridRef(this);
            player_prefab.GetComponent<Player> ().setRhythmSystemRef(rhythmSystem_ref);
            playerList[i] = player_prefab.GetComponent<Player>();

        }
    }

    //spawn ScoreMaker Item
    private ScoreMaker SpawnScoreMaker (int x, int z)
    {
        GridBlock gb = GetGridBlock (x, z);
        if (gb.isOccupied || gb.hasItem) return null;

        GameObject scorePrefab = Instantiate (scoreMaker_prefab, getGridBlockPosition (x, z, 0.8f), Quaternion.identity) as GameObject;
        ScoreMaker sm = scorePrefab.GetComponent<ScoreMaker> ();
        sm.grid_ref = GetComponent<TheGrid> ();
        sm.gridBlockOwner = gb;
        gb.hasItem = true;
        itemList.Add (sm);

        return sm;
    }

    //Build the Map
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

    //load txt file with the map
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

    // Score points for the player
    public void Score (Player p)
    {
        if (p == null || _GridBlockList == null) return;

        if (GetGridBlockList ().Count == 0 || GetGridBlockList () == null)
            print ("gridBlockCount vazio ou null!");

        int result = 0;

        //clear all blocks painted with the player's color
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

        Vector3 pos = p.transform.position;
        pos.y += p.GetComponent<Renderer> ().bounds.size.y + 0.0f;
        SpawnScoreFloatingText (pos, result.ToString (), GridBlock.getColorOfGridBlockColor (p.gridColor));
        p.score += result;

        p.multiplierCombo = 0;

    }


    public void SpawnScoreFloatingText (Vector3 pos, string tex, Color texCol)
    {

        GameObject floatingPrefab = Instantiate (scoreFloatingText_prefab, pos, Quaternion.identity) as GameObject;
        FloatingText ft = floatingPrefab.GetComponent<FloatingText> ();
        ft.grid_ref = this;
        ft.SpawnText (tex, texCol, pos);
        floatingTextList.Add (ft);
        //m_current_floatingText = m_floatingText;

    }

    public void SpawnMissFloatingText (Vector3 pos)
    {

        GameObject floatingPrefab = Instantiate (missFloatingText_prefab, pos, Quaternion.identity) as GameObject;
        FloatingText ft = floatingPrefab.GetComponent<FloatingText> ();
        ft.grid_ref = this;
        ft.SpawnText ("Miss!", Color.black, pos);
        ft.duration = 0.8f;
        ft.fontSize = 8f;
        floatingTextList.Add (ft);
        //m_current_floatingText = m_floatingText;

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


    private void prefabsAssertions()
    {


        Assert.IsNotNull(gridBlock_prefab1);
        Assert.IsNotNull(scoreMaker_prefab);
        Assert.IsNotNull(scoreFloatingText_prefab);
        Assert.IsNotNull(missFloatingText_prefab);
        Assert.IsNotNull(rhythmSystem_ref);

        for(int i = 0; i < playerList.Count; i++)
        {
            if(i < 2)
            Assert.IsNotNull(playerList[i]);
        }

        for(int i = 0; i < playerPrefabList.Count; i++)
        {
            if(i < 2)
            Assert.IsNotNull(playerPrefabList[i]);
        }

        for(int i = 0; i < playerUIList.Count; i++)
        {
            if(i < 2)
            Assert.IsNotNull(playerUIList[i]);
        }

        /*
        
    private GameObject gridBlock_prefab1;

    [SerializeField]
    private GameObject scoreMaker_prefab;

    [SerializeField]
    private GameObject scoreFloatingText_prefab;

    [SerializeField]
    private GameObject missFloatingText_prefab;

    [SerializeField]
    private RhythmSystem rhythmSystem_ref;
         */
        
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