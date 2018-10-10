﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {


	//[SerializeField]
	//List <Button>


	public List<GameObject> screens;
	int cameraAnimIdx = 0;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

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

		if(Input.GetKeyDown(KeyCode.S))
		{
			SceneManager.LoadScene(1);

		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Aesthetics.TheGrid.QuitGame();
			
		}
		
	}


	void GoToScreen(Menu Screen, string cameraState)
	{

	}


}
