using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aesthetics;
using TMPro;

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
                    
                    if(menu_Ref.players.Count < numberOfPlayers)
                    {
                        menu_Ref.players.Clear();



                    for(int i = 0; i < numberOfPlayers; i++)
                    {
                        
                        PlayerMenu p = (PlayerMenu)ScriptableObject.CreateInstance(typeof(PlayerMenu));
                        p.ID = i;
                        p.inputID = i + 1;
                        p.controllerType = (Player.InputType)i;
                        p.name = "Player " + i.ToString();
                        menu_Ref.players.Add(p);


                    }

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

                    menu_Ref.setupPlayersInputIDs();

                }
                else if(name == "Stage Select Screen")
                {
                    MenuScreen scr = menu_Ref.screens.Find (screen => screen.name == "Player Select Screen");
                    
                    if(scr && scr != null)
                    {
                        for(int i= 0; i < scr.transform.childCount; i++)
                    {
                        
                        if(scr.transform.GetChild(i).name.ToLower().Contains("characteroption p"))
                        {
                            int playerID = (int)System.Char.GetNumericValue (scr.transform.GetChild(i).name[scr.transform.GetChild(i).name.Length - 1]) - 1;

                            PlayerMenu pm =  menu_Ref.players.Find(p => p.ID == playerID);
                            if(pm && pm != null)
                            pm.character = scr.transform.GetChild(i).GetComponent<MenuOption>().currentMultipleOption.GetComponent<PlayerModel>().character;
                        }

                        if(scr.transform.GetChild(i).name.ToLower().Contains("controlleroption p"))
                        {
                            int playerID = (int)System.Char.GetNumericValue (scr.transform.GetChild(i).name[scr.transform.GetChild(i).name.Length - 1]) - 1;
                            
                            //the name of the object is the enum so we need this command to convert
                            Player.InputType ip = (Player.InputType ) System.Enum.Parse(typeof(Player.InputType ), scr.transform.GetChild(i).GetComponent<MenuOption>().currentMultipleOption.name, true);
                          
                         
                            
                             PlayerMenu pm =   menu_Ref.players.Find(p => p.ID == playerID);
                            if(pm && pm != null)
                            pm.controllerType = ip;
                            

                        }

                    }
                    }
                    

                    menu_Ref.setupPlayersInputIDs();
                }
                else
                {
                    SetOptionsActive (true);

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