using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour {


	public List<MenuOption> options;
	public string cameraState;
	public bool isHorizontal;

	public MenuOption currentOption;

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

			if(_isSelected)
			{
				if(name == "Player Select Screen") // show only players options
				{
					//get the number of players selected to diplay only the necessary UI elements
					MenuScreen scr = menu_Ref.screens.Find(screen => screen.name == "Player Number Screen");
					
					int optionIndex = 1;
					optionIndex =  scr.options.FindIndex(opt => opt.isSelected == true);

					
					for(int i = 0; i < optionIndex + 2; i++)
					{
						options[i].gameObject.SetActive(true);
					}

				}
				else
				{
					SetOptionsActive(true);

				}
				
				
			}
			else
			{
				SetOptionsActive(false);
			}
        }
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		
		
	}

	public void SetOptionsActive(bool active)
	{
		
		foreach(MenuOption o in options)
		{
			
			o.gameObject.SetActive(active);
		}
	}

	public void SetChildrenActive(bool active)
	{
		
		foreach(Transform o in transform)
		{
			
			o.gameObject.SetActive(active);
		}
	}

	
}
