using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class RhythmSystem : MonoBehaviour
{

    #region Fields

    [Tooltip ("The Event ID of the track to use for target generation.")]
    [EventID]
    public string eventID;

    [Tooltip ("The number of milliseconds (both early and late) within which input will be detected as a Hit.")]
    [Range (8f, 150f)]
    public float hitWindowRangeInMS = 80;

    [Tooltip ("The number of units traversed per second by Note Objects.")]
    public float noteSpeed = 1f;

    [Tooltip ("The archetype (blueprints) to use for generating notes.  Can be a prefab.")]
    public RhythmNote rhythmNotePrefab;


    [SerializeField]
    private GameObject rhyhtmNotesParent_ref;

	 [SerializeField]
    private GameObject MusicPlayback_ref;

     [SerializeField]
    private SimpleMusicPlayer musicPlayer_ref;

     [SerializeField]
    private List <Koreography> koreographyList;

    [Tooltip ("The amount of time in seconds to provide before playback of the audio begins.  Changes to this value are not immediately handled during the lead-in phase while playing in the Editor.")]
    public float leadInTime;

    [Tooltip ("The Audio Source through which the Koreographed audio will be played.  Be sure to disable 'Auto Play On Awake' in the Music Player.")]
    public AudioSource audioCom;

    [Range (0.1f, 2f)]
    [SerializeField]
    private float pitch;

    [Tooltip ("The Color of Note Objects and Buttons in this Lane.")]
    public Color color = Color.magenta;

    [Tooltip ("A reference to the visuals for the \"target\" location.")]
    public SpriteRenderer targetVisuals;

    [Tooltip ("The Keyboard Button used by this lane.")]
    public KeyCode keyboardButton;

    public float angleX;
    public float angleY;

    // The amount of leadInTime left before the audio is audible.
    float leadInTimeLeft;

    // The amount of time left before we should play the audio (handles Event Delay).
    float timeLeftToPlay;

    // Local cache of the Koreography loaded into the Koreographer component.
    Koreography playingKoreo;

    public double currentMusicBPM
    {
        get
        {
            return Koreographer.Instance.GetMusicBPM ();
        }
    }

    // Koreographer works in samples.  Convert the user-facing values into sample-time.  This will simplify
    //  calculations throughout.
    int hitWindowRangeInSamples; // The sample range within which a viable event may be hit.

    // The pool for containing note objects to reduce unnecessary Instantiation/Destruction.
    Stack<RhythmNote> noteObjectPool = new Stack<RhythmNote> ();

    // list with all beat events
    List<KoreographyEvent> beatEvents;

    // A Queue that contains all of the Note Objects currently active (on-screen) within this lane.  Input and
    //  lifetime validity checks are tracked with operations on this Queue.
    List<RhythmNote> trackedNotes = new List<RhythmNote> ();

    public RhythmTarget rhythmTarget_Ref;

    public bool enableMirrorNotes = true;
    // Lifetime boundaries.  This game goes from the top of the screen to the bottom.
    public Vector3 spawnPosition;

    public Vector3 despawnPosition;

    public Vector3 mirrorSpawnPosition;

    public Vector3 mirrorDespawnPosition;

    // Index of the next event to check for spawn timing in this lane.
    int pendingEventIdx = 0;

    // Feedback Scales used for resizing the buttons on press.
    Vector3 defaultScale;
    float scaleNormal = 1f;
    float scalePress = 1.4f;
    float scaleHold = 1.2f;

    #endregion
    #region Properties

    public Vector3 TargetPosition
    {
        get
        {
            return new Vector3 (rhythmTarget_Ref.transform.position.x,
                rhythmTarget_Ref.transform.position.y,
                rhythmTarget_Ref.transform.position.z);
        }
    }

    public Vector3 LocalTargetPosition
    {
        get
        {
            return new Vector3 (rhythmTarget_Ref.transform.localPosition.x,
                rhythmTarget_Ref.transform.localPosition.y,
                rhythmTarget_Ref.transform.localPosition.z);
        }
    }
    // Public access to the hit window.
    public int HitWindowSampleWidth
    {
        get
        {
            return hitWindowRangeInSamples;
        }
    }

    // Access to the current hit window size in Unity units.
    public float WindowSizeInUnits
    {
        get
        {
            return noteSpeed * (hitWindowRangeInMS * 0.001f);
        }
    }

    // The Sample Rate specified by the Koreography.
    public int SampleRate
    {
        get
        {
            return playingKoreo.SampleRate;
        }
    }

    // The current sample time, including any necessary delays.
    public int DelayedSampleTime
    {
        get
        {
            // Offset the time reported by Koreographer by a possible leadInTime amount.
            return playingKoreo.GetLatestSampleTime () - (int) (audioCom.pitch * leadInTimeLeft * SampleRate);
        }
    }

    #endregion
    #region Methods


    private void Awake() {
        
        if(Random.Range(0,3) == 0 )
		{
           if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[0],0,true);
             //gets main beat track
            eventID = koreographyList[0].GetEventIDs()[0];
            //musicPlayer_ref.Play();

           }
			
		}
        else if(Random.Range(0,3) == 1)
        {
            if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[1],0,true);
             //gets main beat track
            eventID = koreographyList[1].GetEventIDs()[0];
             //musicPlayer_ref.Play();

           }
        }
        else if(Random.Range(0,3) == 2)
        {
            if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[2],0,true);
             //gets main beat track
            eventID = koreographyList[2].GetEventIDs()[0];
             //musicPlayer_ref.Play();

           }
        }
         
         

    }
    void Start ()
    {

        
        // Ensure the slider and the readout are properly in sync with the AudioSource on Start!
        pitch = audioCom.pitch;

        InitializeLeadIn ();


       if(Random.Range(0,3) == 0 )
		{
           if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[0],0,true);
             //gets main beat track
            eventID = koreographyList[0].GetEventIDs()[0];
            musicPlayer_ref.Play();

           }
			
		}
        else if(Random.Range(0,3) == 1)
        {
            if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[1],0,true);
             //gets main beat track
            eventID = koreographyList[1].GetEventIDs()[0];
             musicPlayer_ref.Play();

           }
        }
        else if(Random.Range(0,3) == 2)
        {
            if(!musicPlayer_ref.IsPlaying)

           {
                musicPlayer_ref.LoadSong(koreographyList[2],0,true);
             //gets main beat track
            eventID = koreographyList[2].GetEventIDs()[0];
             musicPlayer_ref.Play();

           }
        }

        targetVisuals = rhythmTarget_Ref.GetComponent<SpriteRenderer> ();

		
        // Initialize events.
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex (0);
        

        // Grab all the events out of the Koreography.
        KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID (eventID);

        beatEvents = rhythmTrack.GetAllEvents ();

        /*------------------------------------------------------------- */

        SetupSpawnPositions ();

        // Update our visual color.
        //targetVisuals.color = color;

        // Capture the default scale set in the Inspector.
        defaultScale = targetVisuals.transform.localScale;
    }

    // Sets up the lead-in-time.  Begins audio playback immediately if the specified lead-in-time is zero.
    void InitializeLeadIn ()
    {
        // Initialize the lead-in-time only if one is specified.
        if (leadInTime > 0f)
        {
            // Set us up to delay the beginning of playback.
            leadInTimeLeft = leadInTime;
            timeLeftToPlay = leadInTime - Koreographer.Instance.EventDelayInSeconds;
        }
        else
        {
            // Play immediately and handle offsetting into the song.  Negative zero is the same as
            //  zero so this is not an issue.
            audioCom.time = -leadInTime;
            musicPlayer_ref.Play ();
        }
    }

    void Update ()
    {

        audioCom.pitch = pitch;
        // Clear out invalid entries.
        while (trackedNotes.Count > 0 && trackedNotes[0].IsNoteMissed ())
        {
            trackedNotes.RemoveAt(0);
        }

        // This should be done in Start().  We do it here to allow for testing with Inspector modifications.
        UpdateInternalValues ();

        // Count down some of our lead-in-time.
        if (leadInTimeLeft > 0f)
        {
            leadInTimeLeft = Mathf.Max (leadInTimeLeft - Time.unscaledDeltaTime, 0f);
        }

        // Count down the time left to play, if necessary.
        if (timeLeftToPlay > 0f)
        {
            timeLeftToPlay -= Time.unscaledDeltaTime;

            // Check if it is time to begin playback.
            if (timeLeftToPlay <= 0f)
            {
                audioCom.time = -timeLeftToPlay;
                audioCom.Play ();

                timeLeftToPlay = 0f;
            }
        }

        /* ---------------------------------  */

        // Check for new spawns.
        CheckSpawnNext ();

        // Check for input.  Note that touch controls are handled by the Event System, which is all
        //  configured within the Inspector on the buttons themselves, using the same functions as
        //  what is found here.  Touch input does not have a built-in concept of "Held", so it is not
        //  currently supported.
        if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.D))
        {
           // CheckNoteHit ();
            //SetScalePress();
        }
        else if (Input.GetKey (keyboardButton))
        {
            //SetScaleHold();
        }
        else if (Input.GetKeyUp (keyboardButton))
        {
            //SetScaleDefault();
        }

    }

    // Update any internal values that depend on externally accessible fields (public or Inspector-driven).
    void UpdateInternalValues ()
    {
        hitWindowRangeInSamples = (int) (0.001f * hitWindowRangeInMS * SampleRate);
    }

    // Retrieves a frehsly activated Note Object from the pool.
    public RhythmNote GetFreshNoteObject ()
    {
        RhythmNote retObj;

        if (noteObjectPool.Count > 0)
        {
            retObj = noteObjectPool.Pop ();
        }
        else
        {
            retObj = GameObject.Instantiate<RhythmNote> (rhythmNotePrefab);
        }

        retObj.gameObject.SetActive (true);
        retObj.enabled = true;
        retObj.rhythmSystem_ref = this;
        retObj.gameObject.transform.SetParent (rhyhtmNotesParent_ref.transform);
        retObj.gameObject.transform.localRotation = Quaternion.Euler (0, 0, 0);
        //retObj.gameObject.transform.rotation = rhythmTarget_Ref.transform.rotation;

        //rhyhtmNotesParent_ref.transform.rotation = Camera.main.transform.parent.rotation;

        return retObj;
    }

	public RhythmNote GetFreshNoteObject (KoreographyEvent ev, RhythmSystem refer)
    {
        RhythmNote retObj;

        if (noteObjectPool.Count > 0)
        {
            retObj = noteObjectPool.Pop ();
        }
        else
        {
            retObj = GameObject.Instantiate<RhythmNote> (rhythmNotePrefab);
        }


        retObj.rhythmSystem_ref = refer;
		retObj.trackedEvent = ev;
        retObj.gameObject.transform.SetParent (rhyhtmNotesParent_ref.transform);
        retObj.gameObject.transform.localRotation = Quaternion.Euler (0, 0, 0);
		retObj.gameObject.SetActive (true);
        retObj.enabled = true;
        //retObj.gameObject.transform.rotation = rhythmTarget_Ref.transform.rotation;

        //rhyhtmNotesParent_ref.transform.rotation = Camera.main.transform.parent.rotation;

        return retObj;
    }

    // Deactivates and returns a Note Object to the pool.
    public void ReturnNoteObjectToPool (RhythmNote obj)
    {
        if (obj != null)
        {
			if(trackedNotes.Contains(obj))
				trackedNotes.Remove(obj);
            obj.enabled = false;
            obj.gameObject.SetActive (false);

            noteObjectPool.Push (obj);
        }
    }

    // Adjusts the scale with a multiplier against the default scale.
    void AdjustScale (float multiplier)
    {
        targetVisuals.transform.localScale = defaultScale * multiplier;
    }

    // Uses the Target position and the current Note Object speed to determine the audio sample
    //  "position" of the spawn location.  This value is relative to the audio sample position at
    //  the Target position (the "now" time).
    int GetSpawnSampleOffset ()
    {
        // Given the current speed, what is the sample offset of our current.
        float spawnDistToTarget = Vector3.Distance (spawnPosition, rhythmTarget_Ref.transform.position);

        // At the current speed, what is the time to the location?
        double spawnSecsToTarget = (double) spawnDistToTarget / (double) noteSpeed;

        // Figure out the samples to the target.
        return (int) (spawnSecsToTarget * SampleRate);
    }

    // Checks if a Note Object is hit.  If one is, it will perform the Hit and remove the object
    //  from the trackedNotes Queue.
    public void CheckNoteHit ()
    {
        // Always check only the first event as we clear out missed entries before.
        if (trackedNotes.Count > 0 && trackedNotes[0].IsNoteHittable ())
        {
            RhythmNote hitNote = trackedNotes[0];

            hitNote.OnHit ();
        }
    }

    public bool WasNoteHit ()
    {
        // Always check only the first event as we clear out missed entries before.
        if (trackedNotes.Count > 0 && trackedNotes[0].IsNoteHittable ())
        {
            RhythmNote hitNote = trackedNotes[0];

            hitNote.OnHit ();

            return true;
        }

        return false;
    }

    // Checks if the next Note Object should be spawned.  If so, it will spawn the Note Object and
    //  add it to the trackedNotes Queue.
    void CheckSpawnNext ()
    {
        int samplesToTarget = GetSpawnSampleOffset ();

        int currentTime = DelayedSampleTime;

        // Spawn for all events within range.
        while (pendingEventIdx < beatEvents.Count &&
            beatEvents[pendingEventIdx].StartSample < currentTime + samplesToTarget)
        {
            KoreographyEvent evt = beatEvents[pendingEventIdx];

            RhythmNote newObj = GetFreshNoteObject (evt,this);
            newObj.Initialize (evt, Color.cyan, this, false);

            //mirror objects

            if (enableMirrorNotes)
            {
                RhythmNote newObj2 = GetFreshNoteObject (evt,this);
                newObj2.Initialize (evt, Color.cyan, gameObject.GetComponent<RhythmSystem> (), true);
                newObj.mirror_ref = newObj2;
            }

            trackedNotes.Add (newObj);

            pendingEventIdx++;
        }
    }

    // Sets the Target scale to the original default scale.
    public void SetScaleDefault ()
    {
        AdjustScale (scaleNormal);
    }

    // Sets the Target scale to the specified "initially pressed" scale.
    public void SetScalePress ()
    {
        AdjustScale (scalePress);
    }

    // Sets the Target scale to the specified "continuously held" scale.
    public void SetScaleHold ()
    {
        AdjustScale (scaleHold);
    }

    // Restarts the game, causing all Lanes and any active Note Objects to reset or otherwise clear.
    public void Restart ()
    {

        pendingEventIdx = 0;

        // Clear out the tracked notes.
        int numToClear = trackedNotes.Count;
        for (int i = 0; i < numToClear; ++i)
        {
            trackedNotes.Clear();
        }

        // Reset the audio.
        audioCom.Stop ();
        audioCom.time = 0f;

        // Flush the queue of delayed event updates.  This effectively resets the Koreography and ensures that
        //  delayed events that haven't been sent yet do not continue to be sent.
        Koreographer.Instance.FlushDelayQueue (playingKoreo);

        // Reset the Koreography time.  This is usually handled by loading the Koreography.  As we're simply
        //  restarting, we need to handle this ourselves.
        playingKoreo.ResetTimings ();

        // Reset all the lanes so that tracking starts over.

        // Reinitialize the lead-in-timing.
        InitializeLeadIn ();
    }

    public void SetupSpawnPositions ()
    {
        /// // Get the vertical bounds of the camera.  Offset by a bit to allow for offscreen spawning/removal.
        float cameraOffsetZ = -Camera.main.transform.position.z;
        spawnPosition = Camera.main.ViewportToWorldPoint (new Vector3 (-0.7f, 1f, cameraOffsetZ));
        spawnPosition.y = rhythmTarget_Ref.transform.position.y;

        mirrorSpawnPosition = Camera.main.ViewportToWorldPoint (new Vector3 (1.3f, 1f, cameraOffsetZ));
        mirrorSpawnPosition.y = rhythmTarget_Ref.transform.position.y;

        despawnPosition = rhythmTarget_Ref.transform.position;
        despawnPosition.x -= 0.1f;

        mirrorDespawnPosition = rhythmTarget_Ref.transform.position;
        mirrorDespawnPosition.x += 0.1f;
    }

    #endregion

}