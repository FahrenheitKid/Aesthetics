using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TheGrid : MonoBehaviour
{

    public GameObject gridBlock_prefab1;

    public GameObject gridBlock_prefab2;

    [SerializeField]
    private int mapWidth, mapHeight, xGap, yGap;
    [SerializeField]
    private string fileNameToLoad;
    private int[, ] tiles;

    [SerializeField]
    public List<GridBlock> gridBlockList;

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
               GameObject TilePrefab = Instantiate (gridBlock_prefab1, new Vector3 (xGap * j - mapWidth * xGap, 0, yGap * mapHeight - i * yGap), Quaternion.identity) as GameObject;

                        TilePrefab.transform.parent = transform;
                        TilePrefab.GetComponent<GridBlock> ().init (i, 0, j);
                        TilePrefab.GetComponent<GridBlock> ().changeColor ((GridBlock.gridBlockColor)tiles[i,j]);
                        gridBlockList.Add (TilePrefab.GetComponent<GridBlock> ());
                    
                    

            }
        }
        Debug.Log ("Building Completed!");

      
        //looks at the center gridblock
        if(GetGridBlock (mapWidth / 2, mapHeight / 2))
        {

        Camera.main.transform.parent.transform.position =  GetGridBlock (mapWidth / 2, mapHeight / 2).gameObject.transform.position;
            //Camera.main.transform.parent.LookAt(GetGridBlock (mapWidth / 2, mapHeight / 2).gameObject.transform);
        }

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
                string[] aux = width_height.Split (new [] { ',' });
                int aux_val;
                if (int.TryParse (aux[0], out aux_val))
                    mapWidth = aux_val;
                if (int.TryParse (aux[1], out aux_val))
                    mapHeight = aux_val;

                print ("Map Width: " + mapWidth + " | Map Height: " + mapHeight);
                // read the rest of the file
                string input = sr.ReadToEnd ();
                string[] lines = input.Split (new [] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                int[, ] tiles = new int[lines.Length, mapWidth];
                Debug.Log ("Parsing...");
                for (int i = 0; i < lines.Length; i++)
                {
                    string st = lines[i];
                    string[] nums = st.Split (new [] { ',' });
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
        

        foreach (GridBlock go in gridBlockList)
        {
            if(go.X == x && go.Z == z)
            return go;
            
        }

        return null;

    }

    void foo ()
    {
        GridBlock block = new GridBlock ();

        foreach (GridBlock go in gridBlockList)
        {
            int xx = go.X;

            print (xx);
            //if( == x && go.z == z)

        }
    }
}