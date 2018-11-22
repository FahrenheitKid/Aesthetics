using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreen : MonoBehaviour
{

    public List<MenuOption> options;
    public string cameraState;
    public bool isHorizontal;

    public MenuOption[] currentOptions;
    public bool isMultiplePlayers;

    public Menu menu_Ref;

    [SerializeField, Candlelight.PropertyBackingField]
    protected bool _isSelected;
    public bool isSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;

            if (_isSelected)
            {
                if (name == "Player Select Screen") // show only players options
                {
                    //get the number of players selected to diplay only the necessary UI elements
                    MenuScreen scr = menu_Ref.screens.Find (screen => screen.name == "Player Number Screen");

                    int numberOfPlayers = 2;
                    string name = scr.options.Find (opt => opt.isSelected == true).name;

                    numberOfPlayers = (int) System.Char.GetNumericValue (name[name.Length - 1]);

                    bool cleared = true;

                    /*
                
                    if(menu_Ref.players.Count > numberOfPlayers)
                        {
                            cleared =true;
                            for(int i = menu_Ref.players.Count - 1; i >= 0 ; i--)
                            {
                                if(menu_Ref.players.Count > numberOfPlayers)
                                {
                                     menu_Ref.players.RemoveAt(i);
                                }
                                else
                                {
                                    break;
                                }
                                   
                            }

                        }

                     */

                    menu_Ref.players.Clear ();

                    if (menu_Ref.players.Count < numberOfPlayers || cleared)
                    {
                        List<int> xboxJoysticks = Input.GetJoystickNames ().ToList ().FindAllIndex (namee => namee.ToLower ().Contains ("xbox"));
                        List<int> ps4Joysticks = Input.GetJoystickNames ().ToList ().FindAllIndex (namee => !namee.ToLower ().Contains ("xbox") && namee != "");

                         while (ps4Joysticks.Count > 1)
        {
            ps4Joysticks.RemoveAt(0);
        }

                        bool onlyPS4 = false;
                        bool onlyXbox = false;
                        bool onlyKeyboard = false;

                        bool WASD = false;
                        bool Arrows = false;

                        if (ps4Joysticks.Any () && !xboxJoysticks.Any ())
                        {
                            onlyPS4 = true;
                        }
                        else if (!ps4Joysticks.Any () && xboxJoysticks.Any ())
                        {
                            onlyXbox = true;
                        }
                        else if (!ps4Joysticks.Any () && !xboxJoysticks.Any ())
                        {
                            onlyKeyboard = true;
                        }

                        for (int i = 0; i < numberOfPlayers; i++)
                        {

                            PlayerMenu p = (PlayerMenu) ScriptableObject.CreateInstance (typeof (PlayerMenu));
                            p.ID = i;
                            p.inputID = i + 1;

                            p.controllerType = (Player.InputType) i;

                            if (onlyPS4)
                            {
                                if (ps4Joysticks.Any ())
                                {
                                    p.controllerType = Player.InputType.PS4;
                                    ps4Joysticks.Remove (ps4Joysticks.Last ());
                                }
                                else
                                {
                                    if (!WASD)
                                    {
                                        p.controllerType = Player.InputType.WASD;
                                        WASD = true;
                                    }
                                    else if (!Arrows)
                                    {
                                        Arrows = true;
                                        p.controllerType = Player.InputType.Arrows;
                                    }

                                    else p.controllerType = Player.InputType.PS4;
                                }
                            }
                            else if (onlyXbox)
                            {
                                if (xboxJoysticks.Any ())
                                {
                                    p.controllerType = Player.InputType.Xbox;
                                    xboxJoysticks.Remove (xboxJoysticks.Last ());
                                }
                                else
                                {
                                    if (!WASD)
                                    {
                                        p.controllerType = Player.InputType.WASD;
                                        WASD = true;
                                    }
                                    else if (!Arrows)
                                    {
                                        Arrows = true;
                                        p.controllerType = Player.InputType.Arrows;
                                    }
                                    else p.controllerType = Player.InputType.Xbox;
                                }
                            }
                            else if (onlyKeyboard)
                            {
                                if (!WASD)
                                {
                                    p.controllerType = Player.InputType.WASD;
                                    WASD = true;
                                }
                                else if (!Arrows)
                                {
                                    Arrows = true;
                                    p.controllerType = Player.InputType.Arrows;
                                }

                                else p.controllerType = Player.InputType.Xbox;

                            }
                            else
                            {
                                if (xboxJoysticks.Any ())
                                {
                                    p.controllerType = Player.InputType.Xbox;
                                    xboxJoysticks.Remove (xboxJoysticks.Last ());
                                }
                                else if (ps4Joysticks.Any ())
                                {
                                    p.controllerType = Player.InputType.PS4;
                                    ps4Joysticks.Remove (ps4Joysticks.Last ());
                                }
                                else
                                {

                                    if (!WASD)
                                    {
                                        p.controllerType = Player.InputType.WASD;
                                        WASD = true;
                                    }
                                    else if (!Arrows)
                                    {
                                        Arrows = true;
                                        p.controllerType = Player.InputType.Arrows;
                                    }
                                    else p.controllerType = Player.InputType.Xbox;

                                }
                            }

                            p.name = "Player " + i.ToString ();
                            menu_Ref.players.Add (p);

                        }

                        menu_Ref.setupPlayersInputIDs ();

                    }
                    else
                    {

                    }

                    for (int i = 0; i < options.Count; i++)
                    {
                        //if last character is lower than number of players
                        if ((int) System.Char.GetNumericValue (options[i].name[options[i].name.Length - 1]) <= numberOfPlayers)
                            options[i].gameObject.SetActive (true);
                    }

                    menu_Ref.setupPlayersInputIDs ();
                    menu_Ref.correctActiveControllerOptionUIs ();

                }
                else if (name == "Item Screen")
                {
                    MenuScreen scr = menu_Ref.screens.Find (screen => screen.name == "Player Select Screen");

                    //save players controllers schemes
                    if (scr && scr != null)
                    {
                        for (int i = 0; i < scr.transform.childCount; i++)
                        {

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("characteroption p"))
                            {
                                int playerID = (int) System.Char.GetNumericValue (scr.transform.GetChild (i).name[scr.transform.GetChild (i).name.Length - 1]) - 1;

                                PlayerMenu pm = menu_Ref.players.Find (p => p.ID == playerID);
                                if (pm && pm != null && scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption)
                                {
                                    pm.character = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().character;

                                    pm.colorPrim = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().colorPrim;
                                    pm.colorSec = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().colorSec;
                                    pm.colorTert = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().colorTert;
                                    pm.gridblockColor = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().gridblockColor;
                                    pm.blackGridblockColor = scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.GetComponent<PlayerModel> ().blackGridblockColor;

                                }
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("controlleroption p"))
                            {
                                int playerID = (int) System.Char.GetNumericValue (scr.transform.GetChild (i).name[scr.transform.GetChild (i).name.Length - 1]) - 1;

                                //the name of the object is the enum so we need this command to convert
                                Player.InputType ip = (Player.InputType) System.Enum.Parse (typeof (Player.InputType), scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.name, true);

                                PlayerMenu pm = menu_Ref.players.Find (p => p.ID == playerID);
                                if (pm && pm != null)
                                    pm.controllerType = ip;

                            }

                        }

                    }

                    menu_Ref.setupPlayersInputIDs ();

                    SetOptionsActive (true);

                }
                else if (name == "Song Select Screen")
                {
                    MenuScreen scr = menu_Ref.screens.Find (screen => screen.name == "Item Screen");

                    //menu_Ref.itemSetup.Clear();

                    //save items setup scheme
                    if (scr && scr != null)
                    {
                        for (int i = 0; i < scr.transform.childCount; i++)
                        {

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("arrow") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Arrow));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("compact") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (CompactDisk));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("3d") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Glasses3D));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("fast") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (FastFoward));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("slomo") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (SloMo));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("rainbow") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (RainbowLipstick));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("lock") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Lock));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("ray") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Ray));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("revolver") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Revolver));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("sneakers") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (Sneakers));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("floppy") && !scr.transform.GetChild (i).GetComponent<MenuOption> ().isToggleOn)
                            {
                                menu_Ref.itemSetup.Add (typeof (FloppyDisk));
                            }

                            if (scr.transform.GetChild (i).name.ToLower ().Contains ("frequency"))
                            {

                                if (scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.name.ToLower ().Contains ("high"))
                                {
                                    menu_Ref.itemFrequency = 3;

                                }

                                if (scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.name.ToLower ().Contains ("medium"))
                                {
                                    menu_Ref.itemFrequency = 5;

                                }

                                if (scr.transform.GetChild (i).GetComponent<MenuOption> ().currentMultipleOption.name.ToLower ().Contains ("low"))
                                {
                                    menu_Ref.itemFrequency = 8;

                                }
                            }

                        }

                    }

                    //menu_Ref.itemSetup.ForEach(t => print( t.Name + "off"));

                    SetOptionsActive (true);

                }
                else if (name == "Confirmation Screen")
                {

                    MenuScreen scr = menu_Ref.screens.Find (screen => screen.name == "Stage Select Screen");

                    if (scr && scr != null)
                    {
                        if (currentOptions.Length > 0)
                            menu_Ref.stage = Instantiate (scr.currentOptions[0].currentMultipleOption.GetComponent<Stage> ());

                        DontDestroyOnLoad (menu_Ref.stage.gameObject);
                        menu_Ref.stage.gameObject.SetActive (false);

                    }

                    scr = menu_Ref.screens.Find (screen => screen.name == "Song Select Screen");
                    if (scr && scr != null)
                    {
                        if (currentOptions.Length > 0)
                            menu_Ref.gameSong = Instantiate (scr.currentOptions[0].currentMultipleOption.GetComponent<Song> ());

                        DontDestroyOnLoad (menu_Ref.gameSong.gameObject);
                        menu_Ref.gameSong.gameObject.SetActive (false);

                    }

                }
                else
                {
                    SetOptionsActive (true);

                }

                MenuOption op = options.Find (opp => opp.isMultipleOptions);
                if (op && options.Count == 1)
                {
                    op.isSelected = true;

                }

            }
            else
            {
                SetOptionsActive (false);
            }
        }
    }

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void SetOptionsActive (bool active)
    {

        foreach (MenuOption o in options)
        {

            o.gameObject.SetActive (active);
        }
    }

    public void SetChildrenActive (bool active)
    {

        foreach (Transform o in transform)
        {

            o.gameObject.SetActive (active);
        }
    }

}