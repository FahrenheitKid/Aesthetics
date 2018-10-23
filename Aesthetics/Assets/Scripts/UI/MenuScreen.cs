using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour {


	public List<MenuOption> options;
	public string cameraState;
	public bool isHorizontal;

	public MenuOption currentOption;

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

	
}
