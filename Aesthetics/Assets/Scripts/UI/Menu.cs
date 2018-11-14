using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aesthetics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using TMPro;

public class Menu : MonoBehaviour
{

    //[SerializeField]
    //List <Button>

     [Header("Music/Sfx Settings")]
     [SerializeField]
     public AudioSource audioSource_Ref;
     public SimpleMusicPlayer musicPlayer;

     public Song menuSong;

     public TextMeshProUGUI currentPlayingText;

    public AudioClip confirm_Sfx;
    public AudioClip back_Sfx;

    public AudioClip nextOption_Sfx;

    public List<AudioSource> audioSources;

    [Header("Screen Settings")]
    public List<MenuScreen> screens;

    [SerializeField]
    MenuScreen currentScreen;
    int cameraAnimIdx = 0;


    [Header("Input Settings")]
    [SerializeField]
    private Player.AxisState[] horizontalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };

    [SerializeField]
    private Player.AxisState[] verticalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };

    [Tooltip ("Deadzone for the axis press/down")]
    [SerializeField]
    private float deadZone = 0.01f;

    public Vector2[] input = new Vector2[4];

    [Header("Setup Settings")]

    // public List <Player.InputType> playerControllers = new List<Player.InputType>(4);
    public List<PlayerMenu> players = new List<PlayerMenu> (4);

    public Stage stage;
    public Song gameSong;

    public List<System.Type> itemSetup = new List<System.Type> (11);
    public float itemFrequency = 50f;


    private void Awake() {

        DontDestroyOnLoad(this);
        for(int i = 0; i < 4; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
        setCurrentPlayingText(menuSong.songName, menuSong.artistsName);
    }
    // Use this for initialization
    void Start ()
    {   
        if(stage && stage != null)
        SceneManager.MoveGameObjectToScene(stage.gameObject, SceneManager.GetActiveScene());
        
        horizontalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };
        verticalAxisState = new Player.AxisState[4] { Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle, Player.AxisState.Idle };
        //playerControllers = new List<Player.InputType>(4);

        PlayerMenu p = (PlayerMenu) ScriptableObject.CreateInstance (typeof (PlayerMenu));
        p.ID = 0;
        p.inputID = 1;
        p.controllerType = (Player.InputType) 0;
        p.name = "Player " + p.ID.ToString ();
        players.Add (p);


        if(!audioSource_Ref.isPlaying)
        {
            audioSource_Ref.clip = menuSong.song;
            audioSource_Ref.Play();
        }


    }

    // Update is called once per frame
    void Update ()
    {
        if(SceneManager.GetActiveScene().name.ToLower().Contains("game")) return;

        if(currentScreen.name == "Confirmation Screen")
        {
            
            if(Camera.main.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                //DontDestroyOnLoad(stage.gameObject);
                //stage.gameObject.SetActive(false);
                if(audioSource_Ref.isPlaying)
                    {
            
                        audioSource_Ref.Stop();
                    }

                SceneManager.LoadScene(1);
            }
        }
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

                if (horizontalAxisState[i] == Player.AxisState.Down && input[i].x > 0) // right
                {
                    if (currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                        if(audioSources.Count >= i + 1)
                        audioSources[i].PlayOneShot(nextOption_Sfx);

                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i], true);
                    }
                    else
                    {
                        if (currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                              if(audioSources.Count >= i + 1)
                                audioSources[i].PlayOneShot(nextOption_Sfx);
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
                          if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i], false);
                    }
                    else
                    {
                        if (currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object

                             if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
                            currentScreen.currentOptions[i].pulsateArrows (true, false);
                            currentScreen.currentOptions[i].GoToMultipleOption (false);
                        }
                    }
                }

                // vertical
                if (verticalAxisState[i] == Player.AxisState.Down && input[i].y > 0) // up
                {
                    if (!currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                         if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i], true);
                    }
                    else
                    {
                        if (!currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object
                             if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
                            currentScreen.currentOptions[i].pulsateArrows (false, true);
                            currentScreen.currentOptions[i].GoToMultipleOption (true);
                        }
                    }
                }
                else if (verticalAxisState[i] == Player.AxisState.Down && input[i].y < 0) // down
                {
                    
                    if (!currentScreen.isHorizontal && !currentScreen.currentOptions[i].isMultipleOptions)
                    {
                         if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i], false);
                    }
                    else
                    {
                        if (!currentScreen.currentOptions[i].isHorizontal && currentScreen.currentOptions[i].isMultipleOptions)
                        {
                            // move to next object
                            if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(nextOption_Sfx);
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

        if (Input.GetKeyDown (KeyCode.R))
        {
            SceneManager.LoadScene(0);

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
            if (!(i < players.Count)) continue;

            if (currentScreen.currentOptions[i] && currentScreen.currentOptions[i] != null)
            {
                if (Input.GetButtonDown ("ActionA " + players[i].inputID.ToString () + " " + players[i].controllerType.ToString ()))
                {
                    if ((!currentScreen.currentOptions[i].isMultipleOptions) && (!currentScreen.currentOptions[i].isToggleOption))
                    {
                         if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(confirm_Sfx);
                        DefaultEnter ();
                    }
                    else if (currentScreen.currentOptions[i].isMultipleOptions)
                    {
                         if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(confirm_Sfx);
                        GoToOption (currentScreen.currentOptions[i].next, currentScreen.currentOptions[i], true);
                    }
                    else if (currentScreen.currentOptions[i].isToggleOption)
                    {
                          if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(confirm_Sfx);
                        currentScreen.currentOptions[i].isToggleOn ^= true;
                    }

                }
                else if (Input.GetButtonDown ("ActionB " + players[i].inputID.ToString () + " " + players[i].controllerType.ToString ()))
                {

                    if ((!currentScreen.currentOptions[i].isMultipleOptions) && (!currentScreen.currentOptions[i].isToggleOption))
                    {
                          if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(back_Sfx);
                        DefaultBack ();
                    }
                    else if (currentScreen.currentOptions[i].isMultipleOptions)
                    {
                         if(audioSources.Count >= i + 1)
                            audioSources[i].PlayOneShot(back_Sfx);
                        GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i], false);
                    }
                    else if (currentScreen.currentOptions[i].isToggleOption)
                    {
                        //  GoToOption (currentScreen.currentOptions[i].previous, currentScreen.currentOptions[i]);
                    }
                }

            }

        }

    }

    void handleAxisStates ()
    {

        for (int i = 0; i < players.Count; i++)
        {
            string horizontalName = "Horizontal " + (players[i].inputID) + " " + players[i].controllerType.ToString ();
            string verticalName = "Vertical " + (players[i].inputID) + " " + players[i].controllerType.ToString ();
            input[i] = new Vector2 (Input.GetAxis (horizontalName), Input.GetAxis (verticalName));

            if (Mathf.Abs (input[i].x) > Mathf.Abs (input[i].y))
            {
                input[i].y = 0;
            }
            else
            {
                input[i].x = 0;
            }

            HandleAxisState (ref horizontalAxisState[i], horizontalName);
            HandleAxisState (ref verticalAxisState[i], verticalName);
        }

        //HandleAxisStateDPad (ref horizontalAxisState, "Horizontal" + 2);
        // HandleAxisStateDPad (ref verticalAxisState, "Vertical" + 2);
    }
    void GoToScreen (MenuScreen Screen, string cameraState)
    {
        if(!Screen && Screen!= null) return;

        currentScreen.isSelected = false;

        currentScreen = Screen;
        currentScreen.isSelected = true;

        Camera.main.GetComponent<Animator> ().Play (cameraState);

    }

    public void GoToOption (MenuOption option, MenuOption lastCurrentOption, bool goNext)
    {
        if ((!option || option == null) && (!lastCurrentOption || lastCurrentOption == null)) return;

        //if it is the last multiple option (destionation is null), go to next screen
        if ((!option || option == null) && lastCurrentOption.isMultipleOptions)
        {
            if (currentScreen.isMultiplePlayers)
            {

                MenuOption[] activeOptions = System.Array.FindAll (currentScreen.currentOptions, op => op.gameObject.activeSelf == true);
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

            if(goNext)
            {
                if (lastCurrentOption.next == null)
                GoToScreen (lastCurrentOption.nextScreen, lastCurrentOption.nextScreen.cameraState);
            }
            else
            {
                if (lastCurrentOption.previous == null)
                GoToScreen (lastCurrentOption.previousScreen, lastCurrentOption.previousScreen.cameraState);

            }

           // if (lastCurrentOption.previous == null)
           //     GoToScreen (lastCurrentOption.previousScreen, lastCurrentOption.previousScreen.cameraState);
           // else if (lastCurrentOption.next == null)
           //     GoToScreen (lastCurrentOption.nextScreen, lastCurrentOption.nextScreen.cameraState);
            lastCurrentOption.isSelected = false;
            return;
        }

        lastCurrentOption.isSelected = false;
        if (option)
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
                
                if(o.previousScreen && o.previousScreen != null)
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

    public void setupPlayersInputIDs ()
    {

        // get list with all the players using controllers and sort inputID by it
        List<PlayerMenu> controllers = players.FindAll (p => p.controllerType == Player.InputType.Xbox || p.controllerType == Player.InputType.PS4);
        List<PlayerMenu> keyboards = players.FindAll (p => p.controllerType != Player.InputType.Xbox && p.controllerType != Player.InputType.PS4);

        List<int> xboxJoysticks = Input.GetJoystickNames ().ToList ().FindAllIndex (name => name.ToLower ().Contains ("xbox"));
        List<int> ps4Joysticks = Input.GetJoystickNames ().ToList ().FindAllIndex (name => !name.ToLower ().Contains ("xbox"));

        List<int> possibleIDs = new List<int> ();

        for (int i = 1; i <= players.Count; i++) possibleIDs.Add (i);

        foreach (PlayerMenu p in controllers)
        {
            //  if(p.controllerType != Player.InputType.Xbox && p.controllerType != Player.InputType.PS4)

            if (p.controllerType == Player.InputType.Xbox && xboxJoysticks.Any ())
            {

                p.inputID = xboxJoysticks.First () + 1;
                xboxJoysticks.Remove (xboxJoysticks.First ());
                possibleIDs.Remove (p.inputID);
            }
            else if (p.controllerType == Player.InputType.PS4 && ps4Joysticks.Any ())
            {
                p.inputID = ps4Joysticks.First () + 1;
                ps4Joysticks.Remove (ps4Joysticks.First ());
                possibleIDs.Remove (p.inputID);

            }

        }

        foreach (PlayerMenu p in keyboards)
        {
            //  if(p.controllerType != Player.InputType.Xbox && p.controllerType != Player.InputType.PS4)
            p.inputID = possibleIDs.First ();
            possibleIDs.Remove (possibleIDs.First ());

        }

    }


    public void setCurrentPlayingText(string songName, string artistsName)
    {
        currentPlayingText.text = "[playing " + songName + " by " + artistsName + " in the background]";
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