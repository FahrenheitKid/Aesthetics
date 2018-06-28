#define MAP_LOADING_DEBUG
#undef MAP_LOADING_DEBUG

#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;


public static class ReflectiveEnumerator
{
    static ReflectiveEnumerator() { }

    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, System.IComparable<T>
    {
        List<T> objects = new List<T>();
        foreach (System.Type type in 
            Assembly.GetAssembly(typeof(T)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
            objects.Add((T)System.Activator.CreateInstance(type, constructorArgs));
        }
        objects.Sort();
        return objects;
    }
}

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
    public float playerInitY = 0.1f;

     [SerializeField]
    public float blockStunDuration = 3.0f;

    [SerializeField]
    Countdown scoreMakerTimer;

    [SerializeField]
    Countdown itemsTimer;

    [Tooltip("max amount of concurrent itens on the stage (exluding Scoremakers)")]
    [SerializeField]
    int maxItens = 0;
   [Tooltip("max amount of concurrent scoremakers on the stage")]
    [SerializeField]
    int maxScoreMakers = 0;

    [Tooltip("ratio value to spawn items. 1 = 1 each 5 seconds")]
    [SerializeField]
    float itemSpawnRatio = 0;

    [Tooltip("ratio value to spawn scoreMakers.  1 = 1 each 5 seconds")]
    [SerializeField]
    float scoreMakerSpawnRatio = 0;

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

        QualitySettings.vSyncCount = 0; // VSync must be disabled
        Application.targetFrameRate = 45;

#if DEBUG
        prefabsAssertions ();
#endif

        itemTimersStart ();



        Item t;
        Lock l;
        int lele;
        print("Item" + getItemCurrentPercentage<Item>());
        print("Lock" + getItemCurrentPercentage<Lock>());
        print("int" + getItemCurrentPercentage<int>());
       
    }

    void itemTimersAwake ()
    {
        scoreMakerTimer = gameObject.AddComponent<Countdown> ();
        itemsTimer = gameObject.AddComponent<Countdown> ();

    }
    void itemTimersStart ()
    {
        scoreMakerTimer.startTimer(scoreMakerSpawnRatio);
        itemsTimer.startTimer(itemSpawnRatio);

    }

    void itemTimersUpdate ()
    {
        if (scoreMakerTimer.stop)
        {
           

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

        if (Input.GetKeyDown (KeyCode.G))
        {
            playerList[0].Stun(4);
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
                initial_pos.y = playerInitY;

            }
            else if (i == 1)
            {
                initial_pos = GetGridBlock (mapWidth - 1, mapHeight - 1).gameObject.transform.position;
                initial_pos.y = playerInitY;
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
            gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

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
            gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

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
            gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

        if (gb.isOccupied || gb.hasItem) return null;

        GameObject lockPrefab = Instantiate (lock_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
        lockPrefab.GetComponent<Lock> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

        gb.hasItem = true;
        itemList.Add (lockPrefab.GetComponent<Lock> ());

        return lockPrefab.GetComponent<Lock> ();
    }

    float getItemCurrentPercentage<T>()
    {
        
        if(!UtilityTools.IsSameOrSubclass(typeof(Item),typeof(T)))
            return 0;

      //  continue hereeeeeee
      return 8000f;
    }

    int getItemCurrentCount<T>()
    {
        return itemList.OfType<T>().Count();
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
                TilePrefab.GetComponent<GridBlock> ().init (j, 0, i, GetComponent<TheGrid> (), rhythmSystem_ref);
                TilePrefab.GetComponent<GridBlock> ().changeColor ((GridBlock.gridBlockColor) tiles[i, j]);
                TilePrefab.GetComponent<GridBlock> ().startTransform = TilePrefab.transform;
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
    public List<GridBlock> GetNeighbourGridBlocks (int x, int z, bool allowDiagonals, GridBlock.GridBlockStatus status)
    {
        List<GridBlock> neighbours = new List<GridBlock> ();

        /*

        0,0 | 1,0 | 2,0
        0,1 | 1,1 | 2,1
        0,2 | 1,2 | 2,2

        */
        foreach (GridBlock gb in _GridBlockList)
        {
            //skip block we are comparing

            if (gb.X == x && gb.Z == z) continue;
            if (!gb) continue;

            int x_delta = Mathf.Abs (gb.X - x);
            int z_delta = Mathf.Abs (gb.Z - z);
            int delta_sum = x_delta + z_delta;

            if ((status.hasItem != null && gb.hasItem == status.hasItem) || status.hasItem == null)
            {

                if ((status.isOccupied != null && gb.isOccupied == status.isOccupied) || status.isOccupied == null)
                {
                    if ((status.isBlocked != null && gb.isBlocked == status.isBlocked) || status.isBlocked == null)
                    {

                        if ((status.isPreBlocked != null && gb.isPreBlocked == status.isPreBlocked) || status.isPreBlocked == null)
                        {

                            if ((status.isFallen != null && gb.isFallen == status.isFallen) || status.isFallen == null)
                            {

                                if ((status.isPreFallen != null && gb.isPreFallen == status.isPreFallen) || status.isPreFallen == null)
                                {

                                    if ((status.isRespawning != null && gb.isRespawning == status.isRespawning) || status.isRespawning == null)
                                    {
                                        if ((allowDiagonals && delta_sum <= 2) || (!allowDiagonals && delta_sum <= 1))
                                        {
                                            neighbours.Add (gb);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        return neighbours;
    }

    public GridBlock GetRandomNeighbourGridBlock (int x, int z, bool allowDiagonals, GridBlock.GridBlockStatus status)
    {
        List<GridBlock> neighbours = GetNeighbourGridBlocks (x, z, allowDiagonals, status);

        return neighbours[Random.Range (0, neighbours.Count)];
    }

    //returns random empty gridblock with no players in that range
    public GridBlock GetRandomGridBlock (float range, GridBlock.GridBlockStatus status)
    {
        List<GridBlock> selecteds = new List<GridBlock> ();

        int selected_count = 0; // count how many blocks are valid trhoughout the players
        int it = -1;

        for (int i = 0; i < GetGridBlockList ().Count; i++)
        {
            it = i;
            if (!GetGridBlockList () [it]) continue;

            foreach (Player p in playerList)
            {
                if (!p) continue;

                Vector3 gb_pos = new Vector3 (GetGridBlockList () [it].X, 0, GetGridBlockList () [it].Z);
                Vector3 p_pos = new Vector3 (p.x, 0, p.z);

                if (Vector3.Distance (gb_pos, p_pos) >= range)
                {

                    if ((status.hasItem != null && GetGridBlockList () [it].hasItem == status.hasItem) || status.hasItem == null)
                    {

                        if ((status.isOccupied != null && GetGridBlockList () [it].isOccupied == status.isOccupied) || status.isOccupied == null)
                        {
                            if ((status.isBlocked != null && GetGridBlockList () [it].isBlocked == status.isBlocked) || status.isBlocked == null)
                            {

                                if ((status.isPreBlocked != null && GetGridBlockList () [it].isPreBlocked == status.isPreBlocked) || status.isPreBlocked == null)
                                {

                                    if ((status.isFallen != null && GetGridBlockList () [it].isFallen == status.isFallen) || status.isFallen == null)
                                    {

                                        if ((status.isPreFallen != null && GetGridBlockList () [it].isPreFallen == status.isPreFallen) || status.isPreFallen == null)
                                        {

                                            if ((status.isRespawning != null && GetGridBlockList () [it].isRespawning == status.isRespawning) || status.isRespawning == null)
                                            {
                                                //print("1 pelo menos");
                                                selected_count++;
                                                //break;
                                            }
                                            //break;
                                        }
                                        //break;
                                    }
                                }
                            }
                        }

                    }

                }

            }

            if (selected_count >= playerList.Count)
            {
                selecteds.Add (GetGridBlockList () [it]);
            }

            selected_count = 0;
        }

        if (selecteds.Count > 0)
            return selecteds[Random.Range (0, selecteds.Count)];
        else
        {
            //print("nao achei");
            return null;
        }

    }

    //returns random empty gridblock with no players in that range
    public List<GridBlock> GetRandomGridBlocks (float range, GridBlock.GridBlockStatus status)
    {
        List<GridBlock> selecteds = new List<GridBlock> ();

        int selected_count = 0; // count how many blocks are valid trhoughout the players
        int it = -1;

        for (int i = 0; i < GetGridBlockList ().Count; i++)
        {
            it = i;
            if (!GetGridBlockList () [it]) continue;

            foreach (Player p in playerList)
            {
                if (!p) continue;

                Vector3 gb_pos = new Vector3 (GetGridBlockList () [it].X, 0, GetGridBlockList () [it].Z);
                Vector3 p_pos = new Vector3 (p.x, 0, p.z);

                if (Vector3.Distance (gb_pos, p_pos) >= range)
                {

                    if ((status.hasItem != null && GetGridBlockList () [it].hasItem == status.hasItem) || status.hasItem == null)
                    {

                        if ((status.isOccupied != null && GetGridBlockList () [it].isOccupied == status.isOccupied) || status.isOccupied == null)
                        {
                            if ((status.isBlocked != null && GetGridBlockList () [it].isBlocked == status.isBlocked) || status.isBlocked == null)
                            {

                                if ((status.isPreBlocked != null && GetGridBlockList () [it].isPreBlocked == status.isPreBlocked) || status.isPreBlocked == null)
                                {

                                    if ((status.isFallen != null && GetGridBlockList () [it].isFallen == status.isFallen) || status.isFallen == null)
                                    {

                                        if ((status.isPreFallen != null && GetGridBlockList () [it].isPreFallen == status.isPreFallen) || status.isPreFallen == null)
                                        {

                                            if ((status.isRespawning != null && GetGridBlockList () [it].isRespawning == status.isRespawning) || status.isRespawning == null)
                                            {

                                                selected_count++;
                                                //break;
                                            }
                                        }
                                        //break;
                                    }
                                    //break;
                                }
                            }
                        }
                    }

                }

            }

            if (selected_count >= playerList.Count)
            {
                selecteds.Add (GetGridBlockList () [it]);
            }

            selected_count = 0;

        }

        if (selecteds.Count > 0)
            return selecteds;
        else
        {
            return null;
        }

    }

    public List<GridBlock> GetRandomPatternGridBlocks (GridBlock.gridBlockPattern pattern, float range, GridBlock.GridBlockStatus status)
    {
        List<GridBlock> list = null;
        List<List<GridBlock>> availablePatterns = GetPatternGridBlocks (pattern, range, status);

        if (pattern != GridBlock.gridBlockPattern.Single)
        {
            if (availablePatterns != null)
            {

                list = availablePatterns[Random.Range (0, availablePatterns.Count)];
            }
        }
        else //if(pattern == GridBlock.gridBlockPattern.Single)
        {
            list = GetRandomGridBlocks (range, status);
        }

        return list;

    }

    public List<List<GridBlock>> GetPatternGridBlocks (GridBlock.gridBlockPattern pattern, float range, GridBlock.GridBlockStatus status)
    {
        List<List<GridBlock>> list = null;

        List<GridBlock> availableBlocks = GetRandomGridBlocks (range, status);
        List<List<GridBlock>> validPatterns = new List<List<GridBlock>> ();
        List<GridBlock> already_added = new List<GridBlock> ();

        switch (pattern)
        {

            case GridBlock.gridBlockPattern.Cross:

                //print("first size " + availableBlocks.Count);
                //for each of the possible blocks to check
                foreach (GridBlock gb in availableBlocks)
                {
                    //dont check for blocks we already added
                    //if(already_added.Contains(gb)) continue;

                    List<GridBlock> neighbours = GetNeighbourGridBlocks (gb.X, gb.Z, false, status);

                    List<GridBlock> validNeighbours = new List<GridBlock> ();

                    // if we have the 4 sides neighbours
                    if (neighbours.Count == 4)
                    {
                        neighbours.Add (gb);
                        validPatterns.Add (neighbours);
                    }

                }

                list = validPatterns;

                break;

            case GridBlock.gridBlockPattern.Triple_V: // 4

                //print("first size " + availableBlocks.Count);
                //for each of the possible blocks to check
                foreach (GridBlock gb in availableBlocks)
                {
                    //dont check for blocks we already added
                    //if(already_added.Contains(gb)) continue;

                    List<GridBlock> neighbours = GetNeighbourGridBlocks (gb.X, gb.Z, false, status);

                    List<GridBlock> validNeighbours = new List<GridBlock> ();

                    foreach (GridBlock nb in neighbours)
                    {
                        //if the neighbours are in the same horizontal lane
                        if (nb.X == gb.X)
                        {
                            validNeighbours.Add (nb);
                        }
                    }

                    //to be a triplet we need 2 neighbours
                    if (validNeighbours.Count == 2)
                    {
                        //print("achei 2 vizinhos!");
                        List<GridBlock> temp = new List<GridBlock> ();
                        temp.Add (gb);
                        temp.AddRange (validNeighbours);

                        validPatterns.Add (temp);

                    }

                }

                list = validPatterns;
                break;
            case GridBlock.gridBlockPattern.Triple_H: // 3

                //print("first size " + availableBlocks.Count);
                //for each of the possible blocks to check
                foreach (GridBlock gb in availableBlocks)
                {
                    //dont check for blocks we already added
                    //if(already_added.Contains(gb)) continue;

                    List<GridBlock> neighbours = GetNeighbourGridBlocks (gb.X, gb.Z, false, status);

                    List<GridBlock> validNeighbours = new List<GridBlock> ();

                    foreach (GridBlock nb in neighbours)
                    {
                        //if the neighbours are in the same horizontal lane
                        if (nb.Z == gb.Z)
                        {
                            validNeighbours.Add (nb);
                        }
                    }

                    //to be a triplet we need 2 neighbours
                    if (validNeighbours.Count == 2)
                    {
                        //print("achei 2 vizinhos!");
                        List<GridBlock> temp = new List<GridBlock> ();
                        temp.Add (gb);
                        temp.AddRange (validNeighbours);

                        validPatterns.Add (temp);

                    }

                }

                list = validPatterns;
                break;

            case GridBlock.gridBlockPattern.Double_V: // 2

                  foreach (GridBlock gb in availableBlocks)
                {
                    //dont check for blocks we already added
                    //if(already_added.Contains(gb)) continue;

                    List<GridBlock> neighbours = GetNeighbourGridBlocks (gb.X, gb.Z, false, status);

                    List<GridBlock> validNeighbours = new List<GridBlock> ();

                    foreach (GridBlock nb in neighbours)
                    {
                        //if the neighbours are in the same horizontal lane
                        if (nb.X == gb.X)
                        {
                            validNeighbours.Add (nb);
                        }
                    }

                    //to be a triplet we need 2 neighbours
                    foreach (GridBlock vb in validNeighbours)
                    {
                        //print("achei 2 vizinhos!");
                        List<GridBlock> temp = new List<GridBlock> ();
                        temp.Add(gb);
                        temp.Add(vb);

                        validPatterns.Add(temp);

                    }

                }

                list = validPatterns;
                break;

            case GridBlock.gridBlockPattern.Double_H: // 1

              foreach (GridBlock gb in availableBlocks)
                {
                    //dont check for blocks we already added
                    //if(already_added.Contains(gb)) continue;

                    List<GridBlock> neighbours = GetNeighbourGridBlocks (gb.X, gb.Z, false, status);

                    List<GridBlock> validNeighbours = new List<GridBlock> ();

                    foreach (GridBlock nb in neighbours)
                    {
                        //if the neighbours are in the same horizontal lane
                        if (nb.Z == gb.Z)
                        {
                            validNeighbours.Add (nb);
                        }
                    }

                    //to be a triplet we need 2 neighbours
                    foreach (GridBlock vb in validNeighbours)
                    {
                        //print("achei 2 vizinhos!");
                        List<GridBlock> temp = new List<GridBlock> ();
                        temp.Add(gb);
                        temp.Add(vb);

                        validPatterns.Add(temp);

                    }

                }

                list = validPatterns;
                break;
            default:
                break;

        }
        return list;
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