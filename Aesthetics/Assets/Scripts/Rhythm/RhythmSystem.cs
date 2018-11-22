using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
using UnityEngine.Events;

namespace Aesthetics
{

    public class RhythmSystem : MonoBehaviour
    {

        #region Fields

        [Tooltip ("The Event ID of the track to use for target generation.")]
        [EventID]
        public string mainBeatID;

        [Tooltip ("The Event ID of the track to use for fall events.")]
        [EventID]
        public List<string> fallBeatIDs = new List<string> ();

        [Tooltip ("The Event ID of the track to use for block events.")]
        [EventID]
        public List<string> blockBeatIDs = new List<string> ();

        [Tooltip ("The number of milliseconds (both early and late) within which input will be detected as a Hit.")]
        [Range (8f, 150f)]
        public float hitWindowRangeInMS = 150;

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
        private TheGrid grid_ref;

        [SerializeField]
        private List<Koreography> koreographyList;

        [Tooltip ("The amount of time in seconds to provide before playback of the audio begins.  Changes to this value are not immediately handled during the lead-in phase while playing in the Editor.")]
        public float leadInTime;

        [Tooltip ("The Audio Source through which the Koreographed audio will be played.  Be sure to disable 'Auto Play On Awake' in the Music Player.")]
        public AudioSource audioCom;

        Countdown pitchTimer;
        Tweener pitchTweener;

        public bool isResettingPitch = false;
        [SerializeField]
        private float pitchTweenDuration = 4f;
        [Range (0.1f, 2f)]
        [SerializeField]
        private float initial_pitch = 1f;
        [Range (0.1f, 2f)]
        [SerializeField]
        private float pitch;
        [SerializeField]
        private float pitch_sizeChange = 0.2f;
        [SerializeField]
        private float pitchDuration = 8f;

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

        // list with all fall events
        List<List<KoreographyEvent>> fallEvents = new List<List<KoreographyEvent>> ();

        // list with all fall events
        List<List<KoreographyEvent>> blockEvents = new List<List<KoreographyEvent>> ();

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

        #region Events

        private UnityEvent onNoteReturnedToPool;

        #endregion

        #region Methods

        private void Awake ()
        {
            if (onNoteReturnedToPool == null)
                onNoteReturnedToPool = new UnityEvent ();

            UnityEngine.Random.InitState (System.Environment.TickCount);

            if (!grid_ref)
                grid_ref = GameObject.FindGameObjectWithTag ("Grid").GetComponent<TheGrid> ();

            int randomidx = UnityEngine.Random.Range (0, koreographyList.Count);
            //LoadKoreography (koreographyList[randomidx], 0, true);

            string songName = null;
            if (grid_ref.menu_ref && grid_ref.menu_ref != null)
                songName = getKoreoNameBySongName (grid_ref.menu_ref.gameSong.songName);

            if (songName == null) songName = "LolaEdKoreo";

            LoadKoreography (koreographyList.Find (element => element.name.ToLower ().Contains (songName.ToLower ())), 0, true);

        }
        void Start ()
        {
            pitchTimer = gameObject.AddComponent<Countdown> ();
            pitch = initial_pitch;

            InitializeLeadIn ();

            string songName = null;
            if (grid_ref.menu_ref && grid_ref.menu_ref != null)
                songName = getKoreoNameBySongName (grid_ref.menu_ref.gameSong.songName);

            if (songName == null) songName = "Pleasure";

            //load random or specific Koreo by name
            LoadKoreography (koreographyList.Find (element => element.name.ToLower ().Contains (songName.ToLower ())), 0, true);

            //int randomidx = UnityEngine.Random.Range(0,koreographyList.Count);
            //LoadKoreography (koreographyList[randomidx], 0, true);

            InitKoreography ();
            audioCom.pitch = pitch;

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

        public void Pause()
        {
              List<Koreography> loaded = new List<Koreography> ();
                Koreographer.Instance.GetAllLoadedKoreography (loaded);

            if(!musicPlayer_ref.IsPlaying && Time.timeScale == 1)
            {
                musicPlayer_ref.Play();
            }
            else
            {

                 musicPlayer_ref.Pause();
            }
        }

        void LoadKoreography (Koreography koreo, int startSample, bool autoPlay)
        {
            print ("total tracks: " + koreo.GetEventIDs ().Length);
            foreach (string s in koreo.GetEventIDs ())
            {
                print ("eventID:" + s);
            }

            if (!musicPlayer_ref.IsPlaying)
            {
                musicPlayer_ref.LoadSong (koreo, startSample, autoPlay);
                //gets main beat track
                mainBeatID = Array.Find (koreo.GetEventIDs (), element => element.ToLower ().Contains ("mainbeat"));
                //mainBeatID = Array.FindLast(koreo.GetEventIDs(),"MainBeat");
                fallBeatIDs = Array.FindAll (koreo.GetEventIDs (), element => element.ToLower ().Contains ("fallbeat")).ToList ();

                blockBeatIDs = Array.FindAll (koreo.GetEventIDs (), element => element.ToLower ().Contains ("blockbeat")).ToList ();

                //musicPlayer_ref.Play();

            }

        }

        string getKoreoNameBySongName (string songName)
        {

            if (songName.ToLower ().Contains ("footsteps"))
            {
                return "MainKoreo";
            }
            else if (songName.ToLower ().Contains ("夏の準備は"))
            {
                return "LolaEdKoreo";
            }
            else if (songName.ToLower ().Contains ("ウォーク·オン·バイ"))
            {
                return "WalkOnByKoreo";
            }
            else if (songName.ToLower ().Contains ("pleasure"))
            {
                return "PleasureKoreo";
            }

            return null;

        }

        void InitKoreography ()
        {
            // finishes koreography loading setup

            UnityEngine.Random.InitState (System.Environment.TickCount);
            if (onNoteReturnedToPool == null)
                onNoteReturnedToPool = new UnityEvent ();

            // Ensure the slider and the readout are properly in sync with the AudioSource on Start!
            //pitch = audioCom.pitch;

            targetVisuals = rhythmTarget_Ref.GetComponent<SpriteRenderer> ();

            // Initialize events.
            playingKoreo = Koreographer.Instance.GetKoreographyAtIndex (0);

            // Grab all the events out of the Koreography.
            KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID (mainBeatID);

            List<KoreographyTrackBase> fallTracks = new List<KoreographyTrackBase> ();

            foreach (string lis in fallBeatIDs)
            {
                fallTracks.Add (playingKoreo.GetTrackByID (lis));
                Koreographer.Instance.RegisterForEvents (lis, OnFallBeat);

            }

            List<KoreographyTrackBase> blockTracks = new List<KoreographyTrackBase> ();

            foreach (string lis in blockBeatIDs)
            {
                blockTracks.Add (playingKoreo.GetTrackByID (lis));
                Koreographer.Instance.RegisterForEvents (lis, OnBlockBeat);

            }

            beatEvents = rhythmTrack.GetAllEvents ();

            for (int i = 0; i < fallTracks.Count; i++)
            {
                //if(fallTracks[i].GetAllEvents().Any())
                fallEvents.Add (fallTracks[i].GetAllEvents ());
            }

            for (int i = 0; i < blockTracks.Count; i++)
            {
                //if(blockTracks[i].GetAllEvents().Any())
                blockEvents.Add (blockTracks[i].GetAllEvents ());
            }

            //Koreographer.Instance.RegisterForEvents (fallBeatID, OnFallBeat);
            //Koreographer.Instance.RegisterForEvents (blockBeatID, OnBlockBeat);
            Koreographer.Instance.RegisterForEvents (mainBeatID, OnMainBeat);
            /*------------------------------------------------------------- */

            SetupSpawnPositions ();

            // Update our visual color.
            //targetVisuals.color = color;

            // Capture the default scale set in the Inspector.
            defaultScale = targetVisuals.transform.localScale;

        }
        void Update ()
        {
            if (musicPlayer_ref)
            {
                List<Koreography> loaded = new List<Koreography> ();
                Koreographer.Instance.GetAllLoadedKoreography (loaded);
                if (!musicPlayer_ref.IsPlaying)
                {
                    //print("nao tocaaaa" + loaded.First().GetLatestSampleTime());
                    if (beatEvents.Last ().EndSample <= loaded.First ().GetLatestSampleTime () && !grid_ref.winnerText.gameObject.activeSelf)
                    {
                        //get the name of highest score player
                        int winnerID = grid_ref.GetPlayerList ().OrderByDescending (item => item.score).First ().ID + 1;
                        
                        print("winner id " + winnerID);
                        PlayerUI winnerUI = grid_ref.GetPlayerUIList ().Find (ui => ui.name.ToLower ().Contains (winnerID.ToString ()));
                        grid_ref.winnerText.text = winnerUI.playerName.text + " is the Winner!!! ";
                        grid_ref.winnerText.color = winnerUI.playerName.color;
                        grid_ref.winnerText.gameObject.SetActive (true);

                        grid_ref.endGameText.text =  "Press R to replay this song" + Environment.NewLine + "Press T to go back to the Main Menu";
                        grid_ref.endGameText.gameObject.SetActive (true);
                    }

                }

            }

            // Clear out invalid entries.
            while (trackedNotes.Count > 0 && trackedNotes[0].IsNoteMissed ())
            {
                trackedNotes.RemoveAt (0);

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
            if (Input.GetKeyDown (KeyCode.K) )
            {
                // CheckNoteHit ();
                //SetScalePress();
                #if UNITY_EDITOR
                musicPlayer_ref.SeekToSample(beatEvents.Last ().EndSample);
                #endif
            }
            else if (Input.GetKey (keyboardButton))
            {
                //SetScaleHold();
            }
            else if (Input.GetKeyUp (keyboardButton))
            {
                //SetScaleDefault();
            }

            PitchTimerUpdate ();

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
                if (trackedNotes.Contains (obj))
                    trackedNotes.Remove (obj);
                obj.enabled = false;
                obj.gameObject.SetActive (false);

                noteObjectPool.Push (obj);
            }

            if (!obj.isMirror)
                onNoteReturnedToPool.Invoke ();
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

                RhythmNote newObj = GetFreshNoteObject (evt, this);
                newObj.Initialize (evt, Color.cyan, this, false);

                //mirror objects

                if (enableMirrorNotes)
                {
                    RhythmNote newObj2 = GetFreshNoteObject (evt, this);
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
                trackedNotes.Clear ();
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

        public void ChangePitch (bool up)
        {
            pitch = audioCom.pitch;
            isResettingPitch = false;
            if (up)
            {
                if ((pitch + pitch_sizeChange) <= initial_pitch + pitch_sizeChange)
                {
                    //tween up;
                    //pitchTweener.Complete();
                    if (pitchTweener == null)
                    {
                        pitchTweener = DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch + pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad);
                    }
                    else
                    if (!pitchTweener.IsActive () && pitchTweener != null)
                    {
                        pitchTweener = DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch + pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad);
                    }
                    else
                    {
                        pitchTweener.ChangeEndValue (DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch + pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad));
                    }

                    pitchTimer.startTimer (pitchDuration + pitchTweenDuration);
                }
                else
                {
                    //already too sped up
                    if (pitchDuration > pitchTimer.timeLeft)
                        pitchTimer.startTimer (pitchDuration);
                }
            }
            else
            {
                if ((pitch - pitch_sizeChange) <= initial_pitch - pitch_sizeChange)
                {
                    //tween down;
                    //pitchTweener.Complete();
                    print ("NewPitch");
                    if (pitchTweener == null)
                    {
                        pitchTweener = DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch - pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad);
                    }
                    else
                    if (!pitchTweener.IsActive () && pitchTweener != null)
                    {
                        pitchTweener = DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch - pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad);
                    }
                    else
                    {
                        pitchTweener.ChangeEndValue (DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch - pitch_sizeChange, pitchTweenDuration).SetEase (Ease.InQuad));
                    }

                    pitchTimer.startTimer (pitchDuration + pitchTweenDuration);

                }
                else
                {
                    //already too sped up
                    if (pitchDuration > pitchTimer.timeLeft)
                        pitchTimer.startTimer (pitchDuration);
                }

            }

            pitch = audioCom.pitch;

        }

        void PitchTimerUpdate ()
        {
            pitch = audioCom.pitch;
            if (pitchTimer.stop && !isResettingPitch)
            {
                if (pitch != initial_pitch)
                {
                    print ("resetei");
                    //pitchTweener.Complete();
                    pitchTweener = DOTween.To (() => audioCom.pitch, x => audioCom.pitch = x, initial_pitch, pitchTweenDuration).SetEase (Ease.InQuad);
                    isResettingPitch = true;
                }
            }

            if (isResettingPitch)
            {
                if (pitch == initial_pitch) isResettingPitch = false;
            }

            pitch = audioCom.pitch;

        }

        public UnityEvent getRhythmNoteToPoolEvent ()
        {
            return onNoteReturnedToPool;
        }

        int getEventCountdown (KoreographyEvent evt)
        {

            int pattern = 0;
            int countdown = 0;
            int duration = 0;

            string[] result;
            string[] stringSeparators = new string[] { "," };
            if (evt.Payload is TextPayload)
            {
                TextPayload tp = evt.Payload as TextPayload;
                result = tp.TextVal.Split (stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length == 3)
                {
                    pattern = Convert.ToInt32 (result[0]);
                    if (pattern > Enum.GetNames (typeof (GridBlock.gridBlockPattern)).Length)
                        pattern = 0;
                    countdown = Convert.ToInt32 (result[1]);
                    duration = Convert.ToInt32 (result[2]);
                }

                GridBlock.gridBlockPattern pat = (GridBlock.gridBlockPattern) pattern;
            }

            return countdown;
        }

        KoreographyEvent getEquivalentEvent (List<KoreographyEvent> list, KoreographyEvent match)
        {

            return list.Find (evt => evt.StartSample == match.StartSample);
        }

        int getEquivalentEventIndex (List<KoreographyEvent> list, KoreographyEvent match)
        {

            return list.FindIndex (evt => evt.StartSample == match.StartSample);
        }

        bool hasSameStartSample (KoreographyEvent a, KoreographyEvent b)
        {
            return a.StartSample == b.StartSample;
        }

        void OnMainBeat (KoreographyEvent evt)
        {
            for (int j = 0; j < fallEvents.Count; j++)
            {
                for (int i = 0; i < fallEvents[j].Count; i++)
                {
                    int idx = getEquivalentEventIndex (beatEvents, fallEvents[j][i]);
                    idx -= getEventCountdown (fallEvents[j][i]);
                    if (!(idx > 0 && idx < beatEvents.Count) || getEventCountdown (fallEvents[j][i]) < 0) continue;

                    if (hasSameStartSample (evt, beatEvents[idx]))
                    {
                        //print("fall event id preEQ= " + getEquivalentEventIndex(beatEvents,fallEvents[i]) + " | beat Event id = " +  getEquivalentEventIndex(beatEvents,evt) + " | idx = " + idx);
                        OnFallBeat (fallEvents[j][i], true);

                    }

                }
            }

            for (int j = 0; j < blockEvents.Count; j++)
            {
                for (int i = 0; i < blockEvents[j].Count; i++)
                {
                    int idx = getEquivalentEventIndex (beatEvents, blockEvents[j][i]);
                    idx -= getEventCountdown (blockEvents[j][i]);
                    if (!(idx > 0 && idx < beatEvents.Count) || getEventCountdown (blockEvents[j][i]) < 0) continue;

                    if (hasSameStartSample (evt, beatEvents[idx]))
                    {
                        //print("fall event id preEQ= " + getEquivalentEventIndex(beatEvents,fallEvents[i]) + " | beat Event id = " +  getEquivalentEventIndex(beatEvents,evt) + " | idx = " + idx);
                        OnBlockBeat (blockEvents[j][i], true);

                    }

                }
            }
        }

        //must only run with countdown negative (negative just to sinalize it is to start the "old" way not the predict one)
        void OnFallBeat (KoreographyEvent evt)
        {

            int pattern = 0;
            int countdown = 0;
            int duration = 0;

            string[] result;
            string[] stringSeparators = new string[] { "," };
            if (evt.Payload is TextPayload)
            {
                TextPayload tp = evt.Payload as TextPayload;
                result = tp.TextVal.Split (stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length == 3)
                {
                    pattern = Convert.ToInt32 (result[0]);
                    if (pattern > Enum.GetNames (typeof (GridBlock.gridBlockPattern)).Length)
                        pattern = 0;
                    countdown = Convert.ToInt32 (result[1]);
                    duration = Convert.ToInt32 (result[2]);
                }

                if (countdown < 0)
                {
                    countdown *= -1;
                }
                else
                {
                    return;
                }

                GridBlock.gridBlockPattern pat = (GridBlock.gridBlockPattern) pattern;
                GridBlock.GridBlockStatus status = new GridBlock.GridBlockStatus (null, null, false, false, false, false, false);

                List<GridBlock> patternBlocks = null;
                int safecount = 0;

                switch (pat)
                {
                    case GridBlock.gridBlockPattern.Cross: // 5
                        RetryToFindCrossFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindCrossFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_V: // 4

                        RetryToFindTriple_VFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_VFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_H: // 3

                        RetryToFindTriple_HFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_HFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_V: // 2

                        RetryToFindDouble_VFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_VFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_H: // 1

                        RetryToFindDouble_HFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_HFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Single: // 0
                    default:
                        DefaultFallPattern:
                            GridBlock gb = grid_ref.GetRandomGridBlock (0, status);
                        if (gb)
                            gb.Fall (pat, countdown, duration);
                        break;
                }

            }

        }

        //must only run with countdown positive, it uses the predict one
        void OnFallBeat (KoreographyEvent evt, bool void_)
        {
            int pattern = 0;
            int countdown = 0;
            int duration = 0;

            string[] result;
            string[] stringSeparators = new string[] { "," };
            if (evt.Payload is TextPayload)
            {
                TextPayload tp = evt.Payload as TextPayload;
                result = tp.TextVal.Split (stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length == 3)
                {
                    pattern = Convert.ToInt32 (result[0]);
                    if (pattern > Enum.GetNames (typeof (GridBlock.gridBlockPattern)).Length)
                        pattern = 0;
                    countdown = Convert.ToInt32 (result[1]);
                    duration = Convert.ToInt32 (result[2]);
                }

                GridBlock.gridBlockPattern pat = (GridBlock.gridBlockPattern) pattern;
                GridBlock.GridBlockStatus status = new GridBlock.GridBlockStatus (null, null, false, false, false, false, false);

                List<GridBlock> patternBlocks = null;
                int safecount = 0;

                switch (pat)
                {
                    case GridBlock.gridBlockPattern.Cross: // 5
                        RetryToFindCrossFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindCrossFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_V: // 4

                        RetryToFindTriple_VFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_VFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_H: // 3

                        RetryToFindTriple_HFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_HFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_V: // 2

                        RetryToFindDouble_VFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_VFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_H: // 1

                        RetryToFindDouble_HFall:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Fall (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_HFall;
                            }
                            else
                            {
                                goto DefaultFallPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Single: // 0
                    default:
                        DefaultFallPattern:
                            GridBlock gb = grid_ref.GetRandomGridBlock (0, status);
                        if (gb)
                            gb.Fall (pat, countdown, duration);
                        break;
                }

            }

        }

        //must only run with countdown negative (negative just to sinalize it is to start the "old" way not the predict one)
        void OnBlockBeat (KoreographyEvent evt)
        {
            int pattern = 0;
            int countdown = 0;
            int duration = 0;

            string[] result;
            string[] stringSeparators = new string[] { "," };
            if (evt.Payload is TextPayload)
            {
                TextPayload tp = evt.Payload as TextPayload;
                result = tp.TextVal.Split (stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length == 3)
                {
                    pattern = Convert.ToInt32 (result[0]);
                    if (pattern > Enum.GetNames (typeof (GridBlock.gridBlockPattern)).Length)
                        pattern = 0;
                    countdown = Convert.ToInt32 (result[1]);
                    duration = Convert.ToInt32 (result[2]);
                }

                if (countdown < 0)
                {
                    countdown *= -1;
                }
                else
                {
                    return;
                }

                GridBlock.gridBlockPattern pat = (GridBlock.gridBlockPattern) pattern;
                GridBlock.GridBlockStatus status = new GridBlock.GridBlockStatus (false, false, false, false, false, false, false);

                List<GridBlock> patternBlocks = null;
                int safecount = 0;

                switch (pat)
                {

                    case GridBlock.gridBlockPattern.Cross: // 5
                        RetryToFindCross:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindCross;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_V: // 4

                        RetryToFindTriple_V:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_V;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_H: // 3

                        RetryToFindTriple_H:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_H;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_V: // 2

                        RetryToFindDouble_V:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_V;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_H: // 1

                        RetryToFindDouble_H:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_H;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Single: // 0
                    default:
                        DefaultBlockPattern:
                            GridBlock gb = grid_ref.GetRandomGridBlock (0, status);
                        if (gb)
                            gb.Block (pat, countdown, duration);
                        break;
                }

            }

        }

        //must only run with countdown positive, it uses the predict one
        void OnBlockBeat (KoreographyEvent evt, bool void_)
        {
            int pattern = 0;
            int countdown = 0;
            int duration = 0;

            string[] result;
            string[] stringSeparators = new string[] { "," };
            if (evt.Payload is TextPayload)
            {
                TextPayload tp = evt.Payload as TextPayload;
                result = tp.TextVal.Split (stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (result.Length == 3)
                {
                    pattern = Convert.ToInt32 (result[0]);
                    if (pattern > Enum.GetNames (typeof (GridBlock.gridBlockPattern)).Length)
                        pattern = 0;
                    countdown = Convert.ToInt32 (result[1]);
                    duration = Convert.ToInt32 (result[2]);
                }

                GridBlock.gridBlockPattern pat = (GridBlock.gridBlockPattern) pattern;
                GridBlock.GridBlockStatus status = new GridBlock.GridBlockStatus (false, false, false, false, false, false, false);

                List<GridBlock> patternBlocks = null;
                int safecount = 0;

                switch (pat)
                {

                    case GridBlock.gridBlockPattern.Cross: // 5
                        RetryToFindCross:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindCross;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_V: // 4

                        RetryToFindTriple_V:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_V;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Triple_H: // 3

                        RetryToFindTriple_H:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindTriple_H;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_V: // 2

                        RetryToFindDouble_V:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_V;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;

                    case GridBlock.gridBlockPattern.Double_H: // 1

                        RetryToFindDouble_H:
                            patternBlocks = grid_ref.GetRandomPatternGridBlocks (pat, 0, status);

                        if (patternBlocks != null)
                        {

                            foreach (GridBlock tri in patternBlocks)
                            {
                                if (tri) tri.Block (pat, countdown, duration);
                            }
                        }
                        else
                        {

                            if (safecount < 100)
                            {
                                safecount++;
                                goto RetryToFindDouble_H;
                            }
                            else
                            {
                                goto DefaultBlockPattern;
                            }

                            // print("explodiu");

                        }

                        break;
                    case GridBlock.gridBlockPattern.Single: // 0
                    default:
                        DefaultBlockPattern:
                            GridBlock gb = grid_ref.GetRandomGridBlock (0, status);
                        if (gb)
                            gb.Block (pat, countdown, duration);
                        break;
                }

            }

        }

        #endregion

    }
}