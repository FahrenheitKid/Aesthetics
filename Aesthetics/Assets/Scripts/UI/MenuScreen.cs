using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                    
                    for (int i = 0; i < options.Count; i++)
                    {
                        //if last character is lower than number of players
                        if ((int) System.Char.GetNumericValue (options[i].name[options[i].name.Length - 1]) <= numberOfPlayers)
                            options[i].gameObject.SetActive (true);
                    }  

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