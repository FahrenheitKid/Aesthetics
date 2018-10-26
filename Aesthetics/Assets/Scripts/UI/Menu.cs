using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Aesthetics;
using DG.Tweening;

public class Menu : MonoBehaviour {


	//[SerializeField]
	//List <Button>


	public List<MenuScreen> screens;

	[SerializeField]
	MenuScreen currentScreen;
	int cameraAnimIdx = 0;

	
        [SerializeField]
        private Player.AxisState[] horizontalAxisState = new Player.AxisState[4] {Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle };

        [SerializeField]
        private Player.AxisState[] verticalAxisState = new Player.AxisState[4] {Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle };

        [Tooltip ("Deadzone for the axis press/down")]
        [SerializeField]
        private float deadZone = 0.01f;

		public Vector2[] input = new Vector2[4];
	
	// Use this for initialization
	void Start () {
		horizontalAxisState = new Player.AxisState[4] {Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle };
		verticalAxisState = new Player.AxisState[4] {Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle,Player.AxisState.Idle };

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
		
			handleAxisStates();
			handleInput();
		
		
	}

	void handleInput()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			SceneManager.LoadScene(1);

		}
		
		
		
                
		

		if(horizontalAxisState[0] == Player.AxisState.Up && input[0].x > 0) // right
		{
			if(currentScreen.isHorizontal && !currentScreen.currentOption.isMultipleOptions)
			{
				GoToOption(currentScreen.currentOption.next);
			}
			else
			{
				if(currentScreen.currentOption.isHorizontal && currentScreen.currentOption.isMultipleOptions)
				{
					// move to next object

					currentScreen.currentOption.pulsateArrows(false,true);
					currentScreen.currentOption.GoToMultipleOption(true);
				}
			}
		}
		else if(horizontalAxisState[0] == Player.AxisState.Down && input[0].x < 0) // left
		{
			if(currentScreen.isHorizontal && !currentScreen.currentOption.isMultipleOptions)
			{
				GoToOption(currentScreen.currentOption.previous);
			}
			else
			{
				if(currentScreen.currentOption.isHorizontal && currentScreen.currentOption.isMultipleOptions)
				{
					// move to next object


					currentScreen.currentOption.pulsateArrows(true,false);
					currentScreen.currentOption.GoToMultipleOption(false);
				}
			}
		}

		if(verticalAxisState[0] == Player.AxisState.Down && input[0].y > 0)
		{
			if(!currentScreen.isHorizontal && !currentScreen.currentOption.isMultipleOptions)
			{
				
				GoToOption(currentScreen.currentOption.next);
			}
		}
		else if((verticalAxisState[0] == Player.AxisState.Down )&& input[0].y < 0)
		{
			if(!currentScreen.isHorizontal && !currentScreen.currentOption.isMultipleOptions)
			{
				GoToOption(currentScreen.currentOption.previous);
			}
		}

                
            

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Aesthetics.TheGrid.QuitGame();
			
		}

		if(Input.GetButtonDown("ActionA1"))
		{
			if((!currentScreen.currentOption.isMultipleOptions) )
			{
				DefaultEnter();
			}
			else if(currentScreen.currentOption.isMultipleOptions)
			{
				GoToOption(currentScreen.currentOption.next);
			}
			
		}

		if(Input.GetButtonDown("ActionB1"))
		{
			if((!currentScreen.currentOption.isMultipleOptions) )
			{
				DefaultBack();
			}
			else if(currentScreen.currentOption.isMultipleOptions)
			{
				GoToOption(currentScreen.currentOption.previous);
			}
		}
	}

	void handleAxisStates()
	{

		
			for(int i = 0; i < input.Length; i++)
		{
			input[i] = new Vector2 (Input.GetAxis ("Horizontal" + (i +1)), Input.GetAxis ("Vertical" + (i +1)));
        
                    if (Mathf.Abs (input[i].x) > Mathf.Abs (input[i].y))
                    {
                        input[i].y = 0;
                    }
                    else
                    {
                        input[i].x = 0;
                    }

			HandleAxisState (ref horizontalAxisState[i], "Horizontal" + (i+1));
            HandleAxisState (ref verticalAxisState[i], "Vertical" + (i+1));
		}
		
			
        
         
                //HandleAxisStateDPad (ref horizontalAxisState, "Horizontal" + 2);
               // HandleAxisStateDPad (ref verticalAxisState, "Vertical" + 2);
	}
	void GoToScreen(MenuScreen Screen, string cameraState)
	{
		
		currentScreen.isSelected = false;
		

		currentScreen = Screen;
		currentScreen.isSelected = true;

		Camera.main.GetComponent<Animator>().Play(cameraState);
		
		
		
	}

	public void GoToOption(MenuOption option)
	{
		if(!option && !currentScreen.currentOption.isMultipleOptions) return;
	
		currentScreen.currentOption.isSelected = false;
		

		if(!option && currentScreen.currentOption.isMultipleOptions)
		{
			GoToScreen(currentScreen.currentOption.nextScreen, currentScreen.currentOption.nextScreen.cameraState);
		}

		currentScreen.currentOption = option;
		option.isSelected = true;
		
		//print(currentScreen.currentOption.name);
	}

	void DefaultEnter()
	{
		foreach(MenuOption o in currentScreen.options)
		{
			if(o && o.isSelected)
			{
				
				GoToScreen(o.nextScreen, o.nextScreen.cameraState);

				

			}
		}
	}

	void DefaultBack()
	{
		foreach(MenuOption o in currentScreen.options)
		{
			if(o && o.isSelected)
			{
				
				GoToScreen(o.previousScreen, o.previousScreen.cameraState);

				

			}
		}
	}


	        void HandleAxisState (ref Player.AxisState state, string axi)
        {
            switch (state)
            {
                case Player.AxisState.Idle:
                    if (Input.GetAxis (axi) < -deadZone || Input.GetAxis (axi) > deadZone)
                    {
                        state = Player.AxisState.Down;
                    }
                    break;

                case Player.AxisState.Down:
                    state = Player.AxisState.Held;
                    break;

                case Player.AxisState.Held:
                    if (Input.GetAxis (axi) > -deadZone && Input.GetAxis (axi) < deadZone)
                    {
                        state = Player.AxisState.Up;
                    }
                    break;

                case Player.AxisState.Up:
                    state = Player.AxisState.Idle;
                    break;
            }

        }

        void HandleAxisStateDPad (ref Player.AxisState state, string axi)
        {

                /*
                if (Input.GetAxisRaw ("DpadX") != 0)
            {
                if (X_isAxisInUse == false)
                {
                    if (Input.GetAxisRaw ("DpadX") == +1)
                    {
                        inputCountX += 1;
                    }
                    else if (Input.GetAxisRaw ("DpadX") == -1)
                    {
                        inputCountX -= 1;
                    }
                    X_isAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw ("DpadX") == 0)
            {
                X_isAxisInUse = false;
            }
            //----------------------------------------
            if (Input.GetAxisRaw ("DpadY") != 0)
            {
                if (Y_isAxisInUse == false)
                {
                    if (Input.GetAxisRaw ("DpadY") == +1)
                    {
                        inputCountY += 1;
                    }
                    else if (Input.GetAxisRaw ("DpadY") == -1)
                    {
                        inputCountY -= 1;
                    }
                    Y_isAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw ("DpadY") == 0)
            {
                Y_isAxisInUse = false;
            }
                 */
            

            switch (state)
            {
                case Player.AxisState.Idle:
                    if (Input.GetAxisRaw (axi) < -deadZone || Input.GetAxisRaw (axi) > deadZone)
                    {
                        state = Player.AxisState.Down;
                    }
                    break;

                case Player.AxisState.Down:
                    state = Player.AxisState.Held;
                    break;

                case Player.AxisState.Held:
                    if (Input.GetAxisRaw (axi) > -deadZone && Input.GetAxisRaw (axi) < deadZone)
                    {
                        state = Player.AxisState.Up;
                    }
                    break;

                case Player.AxisState.Up:
                    state = Player.AxisState.Idle;
                    break;
            }

        }

}
