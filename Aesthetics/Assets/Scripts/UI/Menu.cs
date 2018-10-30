using System.Collections;
using System.Collections.Generic;
using Aesthetics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    //[SerializeField]
    //List <Button>

    public List<MenuScreen> screens;

    [SerializeField]
    MenuScreen currentScreen;
    int cameraAnimIdx = 0;

    [SerializeField]
    private Player.AxisState[] horizontalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };

    [SerializeField]
    private Player.AxisState[] verticalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };

    [Tooltip ("Deadzone for the axis press/down")]
    [SerializeField]
    private float deadZone = 0.01f;

    public Vector2[] input = new Vector2[4];

    public List <Player.InputType> playerControllers = new List<Player.InputType>(4);

    // Use this for initialization
    void Start ()
    {
        horizontalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };
        verticalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };
        //playerControllers = new List<Player.InputType>(4);

    }

    // Update is called once per frame
    void Update ()
    {

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

        handleAxisStates ();
        handleInput ();

    }

    void handleInput ()
    {
        if (Input.GetKeyDown (KeyCode.Q))
        {
            SceneManager.LoadScene (1);

        }

        for (int i = 0; i < currentScreen.currentOptions.Length; i++)
        {
            if (currentScreen.currentOptions[i] && currentScreen.currentOptions[i] != null)
            {

                if (horizontalAxisState[i] == Player.AxisState.Up && input[i].x > 0) // right
                {
                    if (currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i]);
                    }
                    else
                    {
                        if (currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object

                            currentScreen.currentOptions[i].pulsateArrows (false, true);
                            currentScreen.currentOptions[i].GoToMultipleOption (true);
                        }
                    }
                }
                else if (horizontalAxisState[i] == Player.AxisState.Down && input[i].x < 0) // left
                {
                    if (currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i]);
                    }
                    else
                    {
                        if (currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object

                            currentScreen.currentOptions[i].pulsateArrows (true, false);
                            currentScreen.currentOptions[i].GoToMultipleOption (false);
                        }
                    }
                }

                // vertical
                if (verticalAxisState[i] == Player.AxisState.Up && input[i].y > 0) // up
                {
                    if (!currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i]);
                    }
                    else
                    {
                        if (!currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object

                            currentScreen.currentOptions[i].pulsateArrows (false, true);
                            currentScreen.currentOptions[i].GoToMultipleOption (true);
                        }
                    }
                }
                else if (verticalAxisState[i] == Player.AxisState.Down && input[i].y < 0) // down
                {
                    if (!currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i]);
                    }
                    else
                    {
                        if (!currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object

                            currentScreen.currentOptions[i].pulsateArrows (true, false);
                            currentScreen.currentOptions[i].GoToMultipleOption (false);
                        }
                    }
                }

            }
        }

        /*
        	
        }
         */

        if (Input.GetKeyDown (KeyCode.Escape))
        {
            Aesthetics.TheGrid.QuitGame ();

        }

        /*
        if (Input.GetButtonDown ("ActionA1"))
        {

            
                if ((!currentScreen.currentOptions[0].isMultipleOptions))
            {
                DefaultEnter ();
            }
            else if (currentScreen.currentOptions[0].isMultipleOptions)
            {
                GoToOption (currentScreen.currentOptions[0].next, currentScreen.currentOptions[0]);
            }
             

        }

        */

        for (int i = 0; i < currentScreen.currentOptions.Length; i++)
        {
            if (currentScreen.currentOptions[i] && currentScreen.currentOptions[i] != null)
            {
                if (Input.GetButtonDown ("ActionA " + (i + 1).ToString () + " " + playerControllers[i].ToString()))
                {
                    if ((!currentScreen.currentOptions[i].isMultipleOptions))
                    {
                        DefaultEnter ();
                    }
                    else if (currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i]);
                    }

                }
                else if (Input.GetButtonDown ("ActionB " + (i + 1).ToString () + " " + playerControllers[i].ToString()))
                {

                    if ((!currentScreen.currentOptions[i].isMultipleOptions))
                    {
                        DefaultBack ();
                    }
                    else if (currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i]);
                    }
                }

            }

        }

    }

    void handleAxisStates ()
    {

        for (int i = 0; i < input.Length; i++)
        {
            input[i] = new Vector2 (Input.GetAxis ("Horizontal " + (i + 1) + " " + playerControllers[i].ToString()), Input.GetAxis ("Vertical " + (i + 1) + " "  + playerControllers[i].ToString()));

            if (Mathf.Abs (input[i].x) > Mathf.Abs (input[i].y))
            {
                input[i].y = 0;
            }
            else
            {
                input[i].x = 0;
            }

            HandleAxisState (ref horizontalAxisState[i], "Horizontal " + (i + 1) + " " + playerControllers[i].ToString());
            HandleAxisState (ref verticalAxisState[i], "Vertical " + (i + 1) + " " + playerControllers[i].ToString());
        }

        //HandleAxisStateDPad (ref horizontalAxisState, "Horizontal" + 2);
        // HandleAxisStateDPad (ref verticalAxisState, "Vertical" + 2);
    }
    void GoToScreen (MenuScreen Screen, string cameraState)
    {

        currentScreen.isSelected = false;

        currentScreen = Screen;
        currentScreen.isSelected = true;

        Camera.main.GetComponent<Animator> ().Play (cameraState);

    }

    public void GoToOption (MenuOption option, MenuOption lastCurrentOption)
    {
        if ((!option || option == null) && (!lastCurrentOption || lastCurrentOption == null)) return;

        //if it is the last multiple option (destionation is null), go to next screen
        if ((!option || option == null) && lastCurrentOption.isMultipleOptions)
        {
            if (currentScreen.isMultiplePlayers)
            {

                MenuOption[] activeOptions = System.Array.FindAll(currentScreen.currentOptions, op => op.gameObject.activeSelf == true);
                //if multiple players, only go to next screen if all the players are in the last option
                if ((System.Array.TrueForAll (activeOptions, op => op.next == null) && lastCurrentOption.next == null))
                {
                    print ("all is true");
                    GoToScreen (lastCurrentOption.nextScreen, lastCurrentOption.nextScreen.cameraState);
                    lastCurrentOption.isSelected = false;
                    return;
                }
                else if ((System.Array.TrueForAll (activeOptions, op => op.previous == null) && lastCurrentOption.previous == null))
                {
                    print ("all is true Previous");
                    GoToScreen (lastCurrentOption.previousScreen, lastCurrentOption.previousScreen.cameraState);
                    lastCurrentOption.isSelected = false;
                    return;
                }
                else
                {
                    print ("wait for all the players!");
                    return;
                }

            }

            GoToScreen (lastCurrentOption.nextScreen, lastCurrentOption.nextScreen.cameraState);
            lastCurrentOption.isSelected = false;
            return;
        }

        lastCurrentOption.isSelected = false;
        if(option)
        option.isSelected = true;
        return;
        //print(currentScreen.currentOption.name);
    }

    void DefaultEnter ()
    {
        foreach (MenuOption o in currentScreen.options)
        {
            if (o && o.isSelected)
            {

                GoToScreen (o.nextScreen, o.nextScreen.cameraState);

            }
        }
    }

    void DefaultBack ()
    {
        foreach (MenuOption o in currentScreen.options)
        {
            if (o && o.isSelected)
            {
                print ("go to previous");
                GoToScreen (o.previousScreen, o.previousScreen.cameraState);

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