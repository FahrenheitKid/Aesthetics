using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {


	//[SerializeField]
	//List <Button>


	public List<MenuScreen> screens;

	[SerializeField]
	MenuScreen currentScreen;
	int cameraAnimIdx = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


		/*
			if(Input.GetKeyDown(KeyCode.D))
		{
			foreach (AnimatorControllerParameter parameter in Camera.main.GetComponent<Animator>().parameters) {
    		if (parameter.type == AnimatorControllerParameterType.Bool)
        			Camera.main.GetComponent<Animator>().SetBool(parameter.name, false);
					}

			if(cameraAnimIdx >=0 && cameraAnimIdx + 1 < Camera.main.GetComponent<Animator>().parameters.Length)
			{
				cameraAnimIdx++;
			}

			Camera.main.GetComponent<Animator>().SetBool(Camera.main.GetComponent<Animator>().parameters[cameraAnimIdx].name,true);
			//print(Camera.main.GetComponent<Animator>().parameters[cameraAnimIdx].name + " " + Camera.main.GetComponent<Animator>().GetBool(cameraAnimIdx));
		}

		if(Input.GetKeyDown(KeyCode.A))
		{
			foreach (AnimatorControllerParameter parameter in Camera.main.GetComponent<Animator>().parameters) {
    		if (parameter.type == AnimatorControllerParameterType.Bool)
        			Camera.main.GetComponent<Animator>().SetBool(parameter.name, false);
					}

			if(cameraAnimIdx - 1 >= 0 && cameraAnimIdx < Camera.main.GetComponent<Animator>().parameters.Length)
			{
				cameraAnimIdx--;
			}

			Camera.main.GetComponent<Animator>().SetBool(Camera.main.GetComponent<Animator>().parameters[cameraAnimIdx].name,true);
			//print(Camera.main.GetComponent<Animator>().parameters[cameraAnimIdx].name + " " + Camera.main.GetComponent<Animator>().GetBool(cameraAnimIdx));
		}
		 */
		

		if(Input.GetKeyDown(KeyCode.Q))
		{
			SceneManager.LoadScene(1);

		}

		if(Input.GetAxis("Horizontal1") < 0)
		{
			if(currentScreen.isHorizontal)
			{
				GoToOption(currentScreen.currentOption.next);
			}
		}
		else if(Input.GetAxis("Horizontal1") > 0)
		{
			if(currentScreen.isHorizontal)
			{
				GoToOption(currentScreen.currentOption.previous);
			}
		}

		if(Input.GetAxis("Vertical1") > 0)
		{
			if(!currentScreen.isHorizontal)
			{
				
				GoToOption(currentScreen.currentOption.next);
			}
		}
		else if(Input.GetAxis("Vertical1") < 0)
		{
			if(!currentScreen.isHorizontal)
			{
				GoToOption(currentScreen.currentOption.previous);
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Aesthetics.TheGrid.QuitGame();
			
		}

		if(Input.GetKeyDown(KeyCode.Return))
		{
			DefaultEnter();
		}
		
	}


	void GoToScreen(MenuScreen Screen, string cameraState)
	{

		currentScreen.SetOptionsActive(false);

		currentScreen = Screen;

		Camera.main.GetComponent<Animator>().Play(cameraState);

		currentScreen.SetOptionsActive(true);

		if(currentScreen.options[0])
		{
			//currentScreen.options[0].isSelected = true;
			//currentScreen.options[0].pulse = true;
		}
		
		
		
	}

	public void GoToOption(MenuOption option)
	{
		if(!option) return;
		currentScreen.currentOption.isSelected = false;
		
		option.isSelected = true;
		currentScreen.currentOption = option;
	}

	void DefaultEnter()
	{
		foreach(MenuOption o in currentScreen.options)
		{
			if(o && o.isSelected)
			{
				o.isSelected = false;
				o.pulse = false;
				GoToScreen(o.nextScreen, o.nextScreen.cameraState);

				

			}
		}
	}


}
