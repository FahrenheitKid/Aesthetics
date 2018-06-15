#define MAP_LOADING_DEBUG
#undef MAP_LOADING_DEBUG

#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TheGrid : MonoBehaviour
{

    #region Property-Variables

    [SerializeField]
    private GameObject gridBlock_prefab1;

    [SerializeField]
    private GameObject scoreMaker_prefab;

    [SerializeField]
    private GameObject lock_prefab;

    [SerializeField]
    private GameObject scoreFloatingText_prefab;

    [SerializeField]
    private GameObject missFloatingText_prefab;

    [SerializeField]
    private GameObject[] arrows_prefabs = new GameObject[3];

    [SerializeField]
    private RhythmSystem rhythmSystem_ref;

    [SerializeField]
    private List<GameObject> playerPrefabList = new List<GameObject> (2);

    [SerializeField]
    private List<PlayerUI> playerUIList = new List<PlayerUI> (2);

    public List<PlayerUI> GetPlayerUIList ()
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
    Countdown scoreMakerTimer;

    [SerializeField]
    private bool isRandomScoreMakerSpawnTime = true;

    // corresponds to [Range(0f, 1f)]
    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float ScoreMakerSpawnTimeMax;

    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float ScoreMakerSpawnTimeMin;

    [SerializeField]
    private int scoreMakerCountLimit = 5;

    [SerializeField]
    Countdown lockTimer;

    [SerializeField]
    private bool isRandomLockSpawnTime = true;

    // corresponds to [Range(0f, 1f)]
    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float lockSpawnTimeMax;

    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float lockSpawnTimeMin;

    [SerializeField]
    private int lockCountLimit = 2;

    [SerializeField]
    Countdown arrowTimer;

    [SerializeField]
    private bool isRandomArrowSpawnTime = true;

    // corresponds to [Range(0f, 1f)]
    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float ArrowSpawnTimeMax;

    [Range (0.1f, 15.0f)]
    [SerializeField]
    private float ArrowSpawnTimeMin;

    [SerializeField]
    private int arrowCountLimit = 4;

    [SerializeField]
    private float singleArrowRatio = 0.5f;

    [SerializeField]
    private float doubleArrowRatio = 0.35f;

    [SerializeField]
    private float quadrupleArrowRatio = 0.15f;

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

        itemTimersAwake ();
        SpawnPlayers ();

        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    void Start ()
    {

#if DEBUG
        prefabsAssertions ();
#endif

        itemTimersStart ();

    }

    void itemTimersAwake ()
    {
        scoreMakerTimer = gameObject.AddComponent<Countdown> ();
        arrowTimer = gameObject.AddComponent<Countdown> ();
        lockTimer = gameObject.AddComponent<Countdown> ();

    }
    void itemTimersStart ()
    {
        if (isRandomScoreMakerSpawnTime)
            scoreMakerTimer.startTimer (Random.Range (ScoreMakerSpawnTimeMin, ScoreMakerSpawnTimeMax));
        else
            scoreMakerTimer.startTimer ((ScoreMakerSpawnTimeMax + ScoreMakerSpawnTimeMin) / 2);

        if (isRandomArrowSpawnTime)
            arrowTimer.startTimer (Random.Range (ArrowSpawnTimeMin, ArrowSpawnTimeMax));
        else
            arrowTimer.startTimer ((ArrowSpawnTimeMin + ArrowSpawnTimeMax) / 2);

        if (isRandomLockSpawnTime)
            lockTimer.startTimer (Random.Range (lockSpawnTimeMin, lockSpawnTimeMax));
        else
            lockTimer.startTimer ((lockSpawnTimeMin + lockSpawnTimeMax) / 2);

    }

    void itemTimersUpdate ()
    {
        if (scoreMakerTimer.stop)
        {
            if (isRandomScoreMakerSpawnTime)
                scoreMakerTimer.startTimer (Random.Range (ScoreMakerSpawnTimeMin, ScoreMakerSpawnTimeMax));
            else
                scoreMakerTimer.startTimer ((ScoreMakerSpawnTimeMax + ScoreMakerSpawnTimeMin) / 2);

            // if more than 5 scoremakers in the stage
            if (itemList.OfType<ScoreMaker> ().Count () < scoreMakerCountLimit)
            {
                ScoreMaker sm = SpawnScoreMaker (2.0f);

            }

        }

        if (arrowTimer.stop)
        {
            if (isRandomLockSpawnTime)
                lockTimer.startTimer (Random.Range (lockSpawnTimeMin, lockSpawnTimeMax));
            else
                lockTimer.startTimer ((lockSpawnTimeMin + lockSpawnTimeMax) / 2);

            // if more than 5 scoremakers in the stage
            if (itemList.OfType<Lock> ().Count () < lockCountLimit)
            {
                Lock sm = SpawnLock (2.0f);

            }

        }

        if (arrowTimer.stop)
        {
            if (isRandomArrowSpawnTime)
                arrowTimer.startTimer (Random.Range (ArrowSpawnTimeMin, ArrowSpawnTimeMax));
            else
                arrowTimer.startTimer ((ArrowSpawnTimeMin + ArrowSpawnTimeMax) / 2);

            if (itemList.OfType<Arrow> ().Count () < arrowCountLimit)
            {
                float ratioTotal = singleArrowRatio + doubleArrowRatio + quadrupleArrowRatio;
                float ar = Random.Range (0f, ratioTotal);
                Arrow.arrowType t = Arrow.arrowType.Single;
                if ((ar - quadrupleArrowRatio) < 0)
                {
                    t = Arrow.arrowType.Quadruple;
                }
                else if ((ar - doubleArrowRatio) < 0)
                {
                    t = Arrow.arrowType.Double;
                }
                else if ((ar - singleArrowRatio) < 0)
                {
                    t = Arrow.arrowType.Single;
                }

                Arrow sm = SpawnArrow (1.0f, t);

            }

        }

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

        itemTimersUpdate ();

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
            GameObject player_prefab = Instantiate (playerPrefabList[i], initial_pos, Quaternion.identity) as GameObject;
            player_prefab.GetComponent<Player> ().setGridRef (this);
            player_prefab.GetComponent<Player> ().setRhythmSystemRef (rhythmSystem_ref);
            playerList[i] = player_prefab.GetComponent<Player> ();

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
        sm.rhythmSystem_ref = rhythmSystem_ref;
        sm.gridBlockOwner = gb;
        sm.x = gb.X;
        sm.y = gb.Y;
        sm.z = gb.Z;

        gb.hasItem = true;
        gb.Item = sm;
        itemList.Add (sm);

        return sm;
    }

    private ScoreMaker SpawnScoreMaker (float range)
    {
        GridBlock gb = null;
        while (gb == null)
            gb = GetEmptyIsolatedGridBlock (range);

        if (gb.isOccupied || gb.hasItem) return null;

        GameObject scorePrefab = Instantiate (scoreMaker_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
        ScoreMaker sm = scorePrefab.GetComponent<ScoreMaker> ();
        sm.grid_ref = GetComponent<TheGrid> ();
        sm.rhythmSystem_ref = rhythmSystem_ref;
        sm.gridBlockOwner = gb;
        sm.x = gb.X;
        sm.y = gb.Y;
        sm.z = gb.Z;

        gb.hasItem = true;
        gb.Item = sm;
        itemList.Add (sm);

        return sm;
    }

    private Arrow SpawnArrow (int x, int z, Arrow.arrowType typeOfArrow)
    {
        GridBlock gb = GetGridBlock (x, z);
        if (gb.isOccupied || gb.hasItem) return null;

        GameObject prefab;
        switch (typeOfArrow)
        {
            case Arrow.arrowType.Quadruple:
                prefab = arrows_prefabs[2];
                break;

            case Arrow.arrowType.Double:
                prefab = arrows_prefabs[1];
                break;

            case Arrow.arrowType.Single:
            default:
                prefab = arrows_prefabs[0];
                break;

        }
        GameObject arrowPrefab = Instantiate (prefab, getGridBlockPosition (x, z, 0.8f), Quaternion.identity) as GameObject;
        Arrow a = arrowPrefab.GetComponent<Arrow> ();
        a.grid_ref = GetComponent<TheGrid> ();
        a.rhythmSystem_ref = rhythmSystem_ref;
        a.gridBlockOwner = gb;
        a.arrow_type = typeOfArrow;
        rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (a.IncreaseCount);
        a.x = gb.X;
        a.y = gb.Y;
        a.z = gb.Z;

        gb.hasItem = true;
        itemList.Add (a);

        return a;
    }

    private Arrow SpawnArrow (float range, Arrow.arrowType typeOfArrow)
    {
        GridBlock gb = null;
        while (gb == null)
            gb = GetEmptyIsolatedGridBlock (range);

        if (gb.isOccupied || gb.hasItem) return null;

        GameObject prefab;
        switch (typeOfArrow)
        {
            case Arrow.arrowType.Quadruple:
                prefab = arrows_prefabs[2];
                break;

            case Arrow.arrowType.Double:
                prefab = arrows_prefabs[1];
                break;

            case Arrow.arrowType.Single:
            default:
                prefab = arrows_prefabs[0];
                break;

        }
        GameObject arrowPrefab = Instantiate (prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
        Arrow a = arrowPrefab.GetComponent<Arrow> ();
        a.grid_ref = GetComponent<TheGrid> ();
        a.rhythmSystem_ref = rhythmSystem_ref;
        a.arrow_type = typeOfArrow;
        a.gridBlockOwner = gb;

        rhythmSystem_ref.getRhythmNoteToPoolEvent ().AddListener (a.IncreaseCount);
        a.x = gb.X;
        a.y = gb.Y;
        a.z = gb.Z;

        gb.hasItem = true;
        itemList.Add (a);

        return a;
    }

    private Lock SpawnLock (int x, int z)
    {
        GridBlock gb = GetGridBlock (x, z);
        if (gb.isOccupied || gb.hasItem) return null;

        GameObject lockPrefab = Instantiate (lock_prefab, getGridBlockPosition (x, z, 0.8f), Quaternion.identity) as GameObject;

        lockPrefab.GetComponent<Lock> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

        gb.hasItem = true;
        itemList.Add (lockPrefab.GetComponent<Lock> ());

        return lockPrefab.GetComponent<Lock> ();
    }

    private Lock SpawnLock (float range)
    {
        GridBlock gb = null;
        while (gb == null)
            gb = GetEmptyIsolatedGridBlock (range);

        if (gb.isOccupied || gb.hasItem) return null;

        GameObject lockPrefab = Instantiate (lock_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
        lockPrefab.GetComponent<Lock> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

        gb.hasItem = true;
        itemList.Add (lockPrefab.GetComponent<Lock> ());

        return lockPrefab.GetComponent<Lock> ();
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
                TilePrefab.GetComponent<GridBlock> ().init (j, 0, i, GetComponent<TheGrid>(),rhythmSystem_ref);
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
                gb.isLocked = false;
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

    //returns random empty gridblock with no players in that range
    public GridBlock GetEmptyIsolatedGridBlock (float range)
    {

        bool selected = false;
        int it = -1;
        while (!selected && GetGridBlockList ().Count > 0 && playerList.Count > 0)
        {
            it = Random.Range (0, GetGridBlockList ().Count);

            foreach (Player p in playerList)
            {
                if (!p) continue;

                Vector3 gb_pos = new Vector3 (GetGridBlockList () [it].X, 0, GetGridBlockList () [it].Z);
                Vector3 p_pos = new Vector3 (p.x, 0, p.z);

                if (Vector3.Distance (gb_pos, p_pos) >= range && GetGridBlockList () [it].hasItem == false && GetGridBlockList () [it].isOccupied == false)
                {
                    selected = true;
                    break;
                }

            }

        }

        if (it > 0)
            return GetGridBlockList () [it];
        else
            return null;
    }

    //returns random empty gridblock with no players in that range
    public GridBlock GetRandomGridBlock (float range, bool? hasItem, bool? isOccupied, bool? isFallen)
    {
        int while_max_count = 0;
        bool selected = false;
        int it = -1;
        while (!selected && GetGridBlockList ().Count > 0 && playerList.Count > 0 && while_max_count <= 200)
        {
            it = Random.Range (0, GetGridBlockList ().Count);

            foreach (Player p in playerList)
            {
                if (!p) continue;

                Vector3 gb_pos = new Vector3 (GetGridBlockList () [it].X, 0, GetGridBlockList () [it].Z);
                Vector3 p_pos = new Vector3 (p.x, 0, p.z);

                if (Vector3.Distance (gb_pos, p_pos) >= range)
                {
                    if ((hasItem != null && GetGridBlockList () [it].hasItem == hasItem) || hasItem == null)
                    {
                        if ((isOccupied != null && GetGridBlockList () [it].isOccupied == isOccupied) || isOccupied == null)
                        {
                            if ((isFallen != null && GetGridBlockList () [it].isFallen == isOccupied) || isFallen == null)
                            {
                                selected = true;
                                break;
                            }
                        }
                    }

                }

            }

            while_max_count++;

        }

        if (it > 0)
            return GetGridBlockList () [it];
        else
        {
            print("Não achou ou explodiu");
             return null;
        }
           
    }

    public Vector3 getGridBlockPosition (int x, int z, float y)
    {
        Vector3 pos = GetGridBlock (x, z).gameObject.transform.position;

        pos.y = y;
        return pos;
    }

    private void prefabsAssertions ()
    {

        Assert.IsNotNull (gridBlock_prefab1);
        Assert.IsNotNull (scoreMaker_prefab);
        Assert.IsNotNull (scoreFloatingText_prefab);
        Assert.IsNotNull (missFloatingText_prefab);
        Assert.IsNotNull (rhythmSystem_ref);

        for (int i = 0; i < playerList.Count; i++)
        {
            if (i < 2)
                Assert.IsNotNull (playerList[i]);
        }

        for (int i = 0; i < playerPrefabList.Count; i++)
        {
            if (i < 2)
                Assert.IsNotNull (playerPrefabList[i]);
        }

        for (int i = 0; i < playerUIList.Count; i++)
        {
            if (i < 2)
                Assert.IsNotNull (playerUIList[i]);
        }

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