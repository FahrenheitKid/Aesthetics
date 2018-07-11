#define MAP_LOADING_DEBUG
#undef MAP_LOADING_DEBUG

#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using Aesthetics;
public static class ReflectiveEnumerator
{
    static ReflectiveEnumerator () { }

    public static IEnumerable<T> GetEnumerableOfType<T> (params object[] constructorArgs) where T : class, System.IComparable<T>
    {
        List<T> objects = new List<T> ();
        foreach (System.Type type in
            Assembly.GetAssembly (typeof (T)).GetTypes ()
            .Where (myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf (typeof (T))))
        {
            objects.Add ((T) System.Activator.CreateInstance (type, constructorArgs));
        }
        objects.Sort ();
        return objects;
    }
}

namespace Aesthetics
{

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
        private GameObject fastFoward_prefab;
        [SerializeField]
        private GameObject sloMo_prefab;

        [SerializeField]
        private GameObject floppyDisk_prefab;

        [SerializeField]
        private GameObject compactDisk_prefab;

        [SerializeField]
        private GameObject glasses3D_prefab;

        [SerializeField]
        private GameObject rainbowLipstick_prefab;

        [SerializeField]
        private GameObject hRay_prefab;

        [SerializeField]
        private GameObject vRay_prefab;

        [SerializeField]
        private GameObject revolver_prefab;

        [SerializeField]
        private GameObject sneakers_prefab;
        

                [SerializeField]
        private GameObject[] arrows_prefabs = new GameObject[3];




        [SerializeField]
        private RhythmSystem rhythmSystem_ref;

        [SerializeField]
        private List<GameObject> playerPrefabList = new List<GameObject> (2);

                [SerializeField]
        private GameObject scoreFloatingText_prefab;

        [SerializeField]
        private GameObject missFloatingText_prefab;

        [SerializeField]
        private List<PlayerUI> playerUIList = new List<PlayerUI> (2);

        public List<PlayerUI> GetPlayerUIList ()
        {
            return playerUIList;
        }

        [SerializeField]
        private List<Player> playerList = new List<Player> (2);

        public List<Player> GetPlayerList ()
        {
            return playerList;
        }

        [SerializeField]
        public List<Item> itemList;

        public IEnumerable<Item> typesOfItemList;

        [SerializeField]
        public List<FloatingText> floatingTextList;

        [SerializeField]
        private CameraScript cameraScript;

        public CameraScript GetCameraScript()  {  return cameraScript;  }

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

        [Tooltip ("max amount of concurrent itens on the stage (exluding Scoremakers)")]
        [SerializeField]
        int maxItens = 6;
        [Tooltip ("max amount of concurrent scoremakers on the stage")]
        [SerializeField]
        int maxScoreMakers = 4;

        [Tooltip ("ratio value to spawn items. (when there's no ohter item of this type on stage)")]
        [SerializeField]
        float itemBaseSpawnRatio = 5;

        [Tooltip ("base ratio value to spawn scoreMakers. (when there's no ohter item of this type on stage)")]
        [SerializeField]
        float scoreMakerBaseSpawnRatio = 5;

        [Tooltip ("current ratio value to spawn items. 1 each 5 seconds")]
        [SerializeField]
        float itemCurrentSpawnRatio = 5;

        [Tooltip ("current ratio value to spawn scoreMakers.  1 each 5 seconds")]
        [SerializeField]
        float scoreMakerCurrentSpawnRatio = 5;

        [SerializeField]
        int amountScoreMaker = 0;
        [SerializeField]
        int amountItens = 5;
        [Tooltip ("distance of players to itens to eb spawned")]
        [SerializeField]
        float defaultItemSpawnRange = 4;

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
            typesOfItemList = ReflectiveEnumerator.GetEnumerableOfType<Item> ();

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

            // print("ScoreMaker: " + getItemCurrentCount<ScoreMaker>() + " | " + getItemCurrentPercentage<ScoreMaker>());

            //GambleItemToSpawn ();

            // print("ScoreMaker: " + getItemCurrentCount<ScoreMaker>() + " | " + getItemCurrentPercentage<ScoreMaker>() + " max: " + maxScoreMakers);

        }

        void itemTimersAwake ()
        {
            scoreMakerTimer = gameObject.AddComponent<Countdown> ();
            itemsTimer = gameObject.AddComponent<Countdown> ();

        }
        void itemTimersStart ()
        {
            // updateItemSpawnRatio();

            float delta = itemCurrentSpawnRatio;
            float interpol = (float) UtilityTools.linear (getItemCurrentCount<ScoreMaker> (), 0, maxScoreMakers, scoreMakerBaseSpawnRatio, 0);
            if (interpol == 0)
                scoreMakerCurrentSpawnRatio = 0;
            else
                scoreMakerCurrentSpawnRatio = (scoreMakerBaseSpawnRatio / 2) / interpol * scoreMakerBaseSpawnRatio;

            interpol = itemCurrentSpawnRatio = (float) UtilityTools.linear (getItemCurrentCount<Item> (), 0, maxItens, itemBaseSpawnRatio, 0);

            if (interpol == 0)
                itemCurrentSpawnRatio = 0;
            else
                itemCurrentSpawnRatio = (itemBaseSpawnRatio / 2) / interpol * itemBaseSpawnRatio;

            scoreMakerTimer.startTimer (scoreMakerCurrentSpawnRatio);
            itemsTimer.startTimer (itemCurrentSpawnRatio);

        }

        void itemTimersUpdate ()
        {
            if (scoreMakerTimer.stop)
            {

                //need to fixxxxxxxx
                if (getItemCurrentCount<ScoreMaker> () < maxScoreMakers)
                    SpawnItem<ScoreMaker> (defaultItemSpawnRange);

                updateScoreMakerSpawnRatio ();

            }

            if (itemsTimer.stop)
            {
                //print ("cabou tempo");
                if (getItemCurrentCount<Item> () < maxItens)
                {
                    ReGamble:

                        if (!GambleItemToSpawn ())
                        {
                            print ("GAMBLE FAILED!");
                            goto ReGamble;
                        }
                    else
                    {
                        // print("GAMBLE win!");
                        updateItemSpawnRatio ();
                    }

                }
                else
                {
                    print ("current items = " + getItemCurrentCount<Item> () + " | max: " + maxItens);
                }

            }

        }

        public void updateItemSpawnRatio ()
        {

            float delta = itemCurrentSpawnRatio;
            float interpol = itemCurrentSpawnRatio = (float) UtilityTools.linear (getItemCurrentCount<Item> (), 0, maxItens, itemBaseSpawnRatio, 0);

            if (interpol == 0)
                itemCurrentSpawnRatio = 0;
            else
                itemCurrentSpawnRatio = (itemBaseSpawnRatio / 2) / interpol * itemBaseSpawnRatio;

            if (((itemsTimer.timeLeft > itemCurrentSpawnRatio) && (Mathf.Abs (delta - itemCurrentSpawnRatio) > 0)) || (itemsTimer.stop && itemCurrentSpawnRatio > 0))
            {

                if (itemCurrentSpawnRatio > 0)
                {
                    // print("reestartei timer" + itemCurrentSpawnRatio);
                    //itemsTimer.stop = true;
                    itemsTimer.startTimer (itemCurrentSpawnRatio);
                }
                else
                {
                    //   print("tinha que reestartar");
                }

            }

        }

        public void updateScoreMakerSpawnRatio ()
        {
            float interpol = (float) UtilityTools.linear (getItemCurrentCount<ScoreMaker> (), 0, maxScoreMakers, scoreMakerBaseSpawnRatio, 0);
            if (interpol == 0)
                scoreMakerCurrentSpawnRatio = 0;
            else
                scoreMakerCurrentSpawnRatio = (scoreMakerBaseSpawnRatio / 2) / interpol * scoreMakerBaseSpawnRatio;

            if (scoreMakerTimer.timeLeft > scoreMakerCurrentSpawnRatio || scoreMakerTimer.stop)
                scoreMakerTimer.startTimer (scoreMakerCurrentSpawnRatio);

        }

        bool SpawnItem<T> (float range)
        {
            //caso não seja descendende da classe Item, ou a própria classe Item, não spawne
            if (!UtilityTools.IsSameOrSubclass (typeof (Item), typeof (T)) || typeof (T) == typeof (Item))
                return false;

            if (typeof (T) == typeof (Lock))
            {
                SpawnLock (range);
                return true;
            }
            else if (typeof (T) == typeof (Arrow))
            {
                SpawnArrow (range, null);
                return true;
            }

            else if (typeof (T) == typeof (FastFoward))
            {
                SpawnFastFoward (range);
                return true;
            }

            else if (typeof (T) == typeof (SloMo))
            {
                SpawnSloMo (range);
                return true;
            }

            else if (typeof (T) == typeof (FloppyDisk))
            {
                SpawnFloppyDisk (range);
                return true;
            }

            else if (typeof (T) == typeof (Glasses3D))
            {
                SpawnGlasses3D (range);
                return true;
            }

            else if (typeof (T) == typeof (CompactDisk))
            {
                SpawnCompactDisk (range);
                return true;
            }
            else if (typeof (T) == typeof (Ray))
            {
                SpawnRay (range, null);
                return true;
            }
            else if (typeof (T) == typeof (Sneakers))
            {
                SpawnSneakers (range);
                return true;
            }

            else if (typeof (T) == typeof (ScoreMaker))
            {
                SpawnScoreMaker (defaultItemSpawnRange);

                return true;

            }

            

            return false;
        }

        //determines what item should we spawn given each items probabilities
        bool GambleItemToSpawn ()
        {
            float chance_total = 0;
            float arrow_currentRarity = 0;
            float compactDisk_currentRarity = 0;
            float fastFoward_currentRarity = 0;
            float floppyDisk_currentRarity = 0;
            float glases3D_currentRarity = 0;
            float lock_currentRarity = 0;
            float rainbow_currentRarity = 0;
            float ray_currentRarity = 0;
            float revolver_currentRarity = 0;
            float slowmo_currentRarity = 0;
            float sneakers_currentRarity = 0;

            foreach (var itemtype in typesOfItemList)
            {
                switch (itemtype.GetType ().Name)
                {
                    case "Arrow":
                        arrow_currentRarity = Arrow.rarity;
                        arrow_currentRarity -= Item.rarityReduction (Arrow.rarity, getItemCurrentCount<Arrow> ());
                        chance_total += arrow_currentRarity;
                        break;

                    case "CompactDisk":
                        compactDisk_currentRarity = CompactDisk.rarity;
                        compactDisk_currentRarity -= Item.rarityReduction (CompactDisk.rarity, getItemCurrentCount<CompactDisk> ());
                        chance_total += compactDisk_currentRarity;
                        break;

                    case "FastFoward":
                        fastFoward_currentRarity = FastFoward.rarity;
                        fastFoward_currentRarity -= Item.rarityReduction (FastFoward.rarity, getItemCurrentCount<FastFoward> ());
                        chance_total += fastFoward_currentRarity;
                        break;

                    case "FloppyDisk":
                        floppyDisk_currentRarity = FloppyDisk.rarity;
                        floppyDisk_currentRarity -= Item.rarityReduction (FloppyDisk.rarity, getItemCurrentCount<FloppyDisk> ());
                        chance_total += floppyDisk_currentRarity;
                        break;

                    case "Glasses3D":
                        glases3D_currentRarity = Glasses3D.rarity;
                        glases3D_currentRarity -= Item.rarityReduction (Glasses3D.rarity, getItemCurrentCount<Glasses3D> ());
                        chance_total += glases3D_currentRarity;
                        break;

                    case "Lock":
                        lock_currentRarity = Lock.rarity;
                        lock_currentRarity -= Item.rarityReduction (Lock.rarity, getItemCurrentCount<Lock> ());
                        chance_total += lock_currentRarity;
                        break;

                    case "RainbowLipstick":
                        rainbow_currentRarity = RainbowLipstick.rarity;
                        rainbow_currentRarity -= Item.rarityReduction (RainbowLipstick.rarity, getItemCurrentCount<RainbowLipstick> ());
                        chance_total += rainbow_currentRarity;
                        break;

                    case "Ray":
                        ray_currentRarity = Ray.rarity;
                        ray_currentRarity -= Item.rarityReduction (Ray.rarity, getItemCurrentCount<Ray> ());
                        chance_total += ray_currentRarity;
                        break;

                    case "Revolver":
                        revolver_currentRarity = Revolver.rarity;
                        revolver_currentRarity -= Item.rarityReduction (Revolver.rarity, getItemCurrentCount<Revolver> ());
                        chance_total += revolver_currentRarity;
                        break;

                    case "SloMo":
                        slowmo_currentRarity = SloMo.rarity;
                        slowmo_currentRarity -= Item.rarityReduction (SloMo.rarity, getItemCurrentCount<SloMo> ());
                        chance_total += slowmo_currentRarity;
                        break;

                    case "Sneakers":
                        sneakers_currentRarity = Sneakers.rarity;
                        if(getItemCurrentCount<Sneakers> () > 0)
                        {
                            int a = 3;
                        }
                        sneakers_currentRarity -= Item.rarityReduction (Sneakers.rarity, getItemCurrentCount<Sneakers> ());
                        chance_total += sneakers_currentRarity;
                        break;

                }

            }

            print("Sneakers chance: "  + sneakers_currentRarity);
            float val = UnityEngine.Random.Range (0, chance_total);

            bool exitLoop = false;
            foreach (var itemtype in typesOfItemList)
            {
                switch (itemtype.GetType ().Name)
                {
                    case "Arrow":
                        if (val <= arrow_currentRarity && Arrow.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            //print(Arrow.ruleCheck(this));
                            return SpawnItem<Arrow> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= arrow_currentRarity;
                        }

                        break;

                    case "CompactDisk":
                        if (val <= compactDisk_currentRarity && CompactDisk.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<CompactDisk> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= compactDisk_currentRarity;
                        }
                        break;

                    case "FastFoward":
                        if (val <= fastFoward_currentRarity && FastFoward.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<FastFoward> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= fastFoward_currentRarity;
                        }
                        break;

                    case "FloppyDisk":
                        if (val <= floppyDisk_currentRarity && FloppyDisk.ruleCheck (this))
                        {
                           // print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<FloppyDisk> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= floppyDisk_currentRarity;
                        }
                        break;

                    case "Glasses3D":
                        if (val <= glases3D_currentRarity && Glasses3D.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<Glasses3D> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= glases3D_currentRarity;
                        }
                        break;

                    case "Lock":
                        if (val <= lock_currentRarity && Lock.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<Lock> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= lock_currentRarity;
                        }
                        break;

                    case "RainbowLipstick":
                        if (val <= rainbow_currentRarity && RainbowLipstick.ruleCheck (this))
                        {
                            print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<RainbowLipstick> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= rainbow_currentRarity;
                        }
                        break;

                    case "Ray":
                        if (val <= ray_currentRarity && Ray.ruleCheck (this))
                        {
                            //print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<Ray> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= ray_currentRarity;
                        }
                        break;

                    case "Revolver":
                        if (val <= revolver_currentRarity && Revolver.ruleCheck (this))
                        {
                            print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<Revolver> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= revolver_currentRarity;
                        }
                        break;

                    case "SloMo":
                        if (val <= slowmo_currentRarity && SloMo.ruleCheck (this))
                        {
                           // print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<SloMo> (defaultItemSpawnRange);
                            exitLoop = true;
                        }
                        else
                        {
                            val -= slowmo_currentRarity;
                        }
                        break;

                    case "Sneakers":
                        if (val <= sneakers_currentRarity && Sneakers.ruleCheck (this))
                        {
                            print ("Rolled" + itemtype.GetType ().Name);
                            return SpawnItem<Sneakers> (defaultItemSpawnRange);
                            //exitLoop = true;
                        }
                        else
                        {
                            val -= sneakers_currentRarity;
                        }
                        break;

                }

                if (exitLoop) break;
            }

            return false;
        }

        // Update is called once per frame
        void Update ()
        {

            amountScoreMaker = getItemCurrentCount<ScoreMaker> ();
            amountItens = getItemCurrentCount<Item> ();
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
                playerList[0].Stun (4);
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
           
            a.x = gb.X;
            a.y = gb.Y;
            a.z = gb.Z;
             a.Setup(this,rhythmSystem_ref,gb);
            gb.hasItem = true;
            itemList.Add (a);

            return a;
        }

        private Arrow SpawnArrow (float range, Arrow.arrowType? typeOfArrow)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject prefab;
            if (typeOfArrow == null)
            {
                int ran = Random.Range (0, 3);
                if (ran == 0) typeOfArrow = Arrow.arrowType.Single;
                if (ran == 1) typeOfArrow = Arrow.arrowType.Double;
                if (ran == 2) typeOfArrow = Arrow.arrowType.Quadruple;
                else
                    typeOfArrow = Arrow.arrowType.Single;

            }

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
            a.arrow_type = (Arrow.arrowType) typeOfArrow;
            a.gridBlockOwner = gb;

            a.x = gb.X;
            a.y = gb.Y;
            a.z = gb.Z;

             a.Setup(this,rhythmSystem_ref,gb);

            gb.hasItem = true;
            itemList.Add (a);

            return a;
        }

      

        private Ray SpawnRay (float range, Ray.rayType? typeOfRay)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject prefab;
            if (typeOfRay == null)
            {
                int ran = Random.Range (0, 2);
                if (ran == 0) typeOfRay = Ray.rayType.HRay;
                if (ran == 1) typeOfRay = Ray.rayType.Vray;
                else
                    typeOfRay = Ray.rayType.HRay;

            }

            switch (typeOfRay)
            {


                case Ray.rayType.Vray:
                    prefab = vRay_prefab;
                    break;

                case Ray.rayType.HRay:
                default:
                    prefab = hRay_prefab;
                    break;

            }

            GameObject rayPrefab = Instantiate (prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            Ray a = rayPrefab.GetComponent<Ray> ();
            a.grid_ref = GetComponent<TheGrid> ();
            a.rhythmSystem_ref = rhythmSystem_ref;
            a.ray_type = (Ray.rayType) typeOfRay;
            a.gridBlockOwner = gb;

            a.x = gb.X;
            a.y = gb.Y;
            a.z = gb.Z;

             a.Setup(this,rhythmSystem_ref,gb);

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

       private FastFoward SpawnFastFoward (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject ffPrefab = Instantiate (fastFoward_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            ffPrefab.GetComponent<FastFoward> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (ffPrefab.GetComponent<FastFoward> ());

            return ffPrefab.GetComponent<FastFoward> ();
        }

        private SloMo SpawnSloMo (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject smPrefab = Instantiate (sloMo_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            smPrefab.GetComponent<SloMo> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (smPrefab.GetComponent<SloMo> ());

            return smPrefab.GetComponent<SloMo> ();
        }

        
          private CompactDisk SpawnCompactDisk (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject smPrefab = Instantiate (compactDisk_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            smPrefab.GetComponent<CompactDisk> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (smPrefab.GetComponent<CompactDisk> ());

            return smPrefab.GetComponent<CompactDisk> ();
        }
        private FloppyDisk SpawnFloppyDisk (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject smPrefab = Instantiate (floppyDisk_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            smPrefab.GetComponent<FloppyDisk> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (smPrefab.GetComponent<FloppyDisk> ());

            return smPrefab.GetComponent<FloppyDisk> ();
        }

         private Glasses3D SpawnGlasses3D (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject spPrefab = Instantiate (glasses3D_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            spPrefab.GetComponent<Glasses3D> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (spPrefab.GetComponent<Glasses3D> ());

            return spPrefab.GetComponent<Glasses3D> ();
        }

        

         private Sneakers SpawnSneakers (float range)
        {
            GridBlock gb = null;
            while (gb == null)
                gb = GetRandomGridBlock (range, new GridBlock.GridBlockStatus (false, false, false, false, false, false, false));

            if (gb.isOccupied || gb.hasItem) return null;

            GameObject spPrefab = Instantiate (sneakers_prefab, getGridBlockPosition (gb.X, gb.Z, 0.8f), Quaternion.identity) as GameObject;
            spPrefab.GetComponent<Sneakers> ().Setup (GetComponent<TheGrid> (), rhythmSystem_ref, gb);

            gb.hasItem = true;
            itemList.Add (spPrefab.GetComponent<Sneakers> ());

            return spPrefab.GetComponent<Sneakers> ();
        }

       
// north z--
// south z++
// west x--
// east x++
        

        public GameObject getPrefabOfType<T> (Arrow.arrowType? typeOfArrow)
        {
            if (typeof (T) == typeof (Lock))
                return lock_prefab;

            if (typeof (T) == typeof (Arrow))
            {
                if (typeOfArrow != null)
                {
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
                    return prefab;
                }
                else
                {
                    return arrows_prefabs[Random.Range (0, arrows_prefabs.Count ())];
                }
            }

            if (typeof (T) == typeof (ScoreMaker))
                return scoreMaker_prefab;

            return null;
        }

        public float getItemCurrentPercentage<T> ()
        {

            if (!UtilityTools.IsSameOrSubclass (typeof (Item), typeof (T)))
                return 0;

            if (typeof (T) == typeof (ScoreMaker))
            {
                if (getItemCurrentCount<T> () != 0)
                {
                    //print("" + (getItemCurrentCount<T> () + " / " + maxScoreMakers + " x 100 = " + (getItemCurrentCount<T> () / maxScoreMakers) * 100) );
                    return ((float) getItemCurrentCount<T> () / (float) maxScoreMakers) * 100;
                }
                else return 0;

            }
            else
            {

                if (getItemCurrentCount<T> () != 0)
                    return ((float) getItemCurrentCount<T> () / (float) maxItens) * 100;
                else return 0;

            }

        }

        public int getItemCurrentCountHelper<T> (T obj)
        {
            return getItemCurrentCount<T> ();
        }

        public int getItemCurrentCount<T> ()
        {
            if (typeof (T) != (typeof (Item)))
                return itemList.OfType<T> ().Count ();
            else // if we want to get the count of the Itens, we need to exlude ScoreMaker since it is a "special" item.
            {
                //print("nao sou item");
                int count = 0;
                foreach (var item in itemList)
                {
                    if (item.GetType () == typeof (ScoreMaker)) continue;

                    count++;
                }

                return count;
            }

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
                            temp.Add (gb);
                            temp.Add (vb);

                            validPatterns.Add (temp);

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
                            temp.Add (gb);
                            temp.Add (vb);

                            validPatterns.Add (temp);

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
}