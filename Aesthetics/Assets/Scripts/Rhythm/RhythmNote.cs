#define DEBUG

using System.Collections;
using System.Collections.Generic;
using Aesthetics;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class RhythmNote : MonoBehaviour
{

    #region Fields

    [Tooltip ("The visual to use for this Note Object.")]
    public SpriteRenderer visuals;

    // If active, the KoreographyEvent that this Note Object wraps.  Contains the relevant timing information.
    public KoreographyEvent trackedEvent;

    public RhythmSystem rhythmSystem_ref;
    public Vector3 destination;

    public bool isMirror;
    public RhythmNote mirror_ref;

    [SerializeField]
    private float scalingFactorX = 1f;

    [SerializeField]
    private float scalingFactorY = 1f;

    #endregion
    #region Static Methods

    // Unclamped Lerp.  Same as Vector3.Lerp without the [0.0-1.0] clamping.
    static Vector3 Lerp (Vector3 from, Vector3 to, float t)
    {
        return new Vector3 (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
    }

    #endregion
    #region Methods
    private void Start ()
    {

    }
    // Prepares the Note Object for use.
    public void Initialize (KoreographyEvent evt, Color color, RhythmSystem rhythmSys, bool isMirrorNote)
    {
        trackedEvent = evt;
        //visuals.color = color;
        isMirror = isMirrorNote;
        rhythmSystem_ref = rhythmSys;

        SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
        renderer.sortingOrder = 4;

        transform.position = rhythmSystem_ref.spawnPosition;

#if DEBUG
        Assert.IsNotNull (rhythmSystem_ref);
        Assert.IsNotNull (trackedEvent);
#endif
    }

    // Resets the Note Object to its default state.
    void Reset ()
    {
        trackedEvent = null;
        rhythmSystem_ref = null;
    }

    void Update ()
    {

        UpdateWidth ();
        UpdateHeight ();

        UpdatePosition (isMirror);

        CheckDespawn ();
    }

    void CheckDespawn ()
    {

        float overdrive = visuals.sprite.rect.width / visuals.sprite.pixelsPerUnit;
        overdrive /= 6;
        //overdrive*= 0;

        if (!isMirror)
        {
            if (transform.localPosition.x >= rhythmSystem_ref.LocalTargetPosition.x)
            {
                visuals.enabled = false;
                if (transform.localPosition.x >= rhythmSystem_ref.LocalTargetPosition.x + overdrive)
                    ReturnToPool ();
            }
        }
        else
        {
            if (transform.localPosition.x <= rhythmSystem_ref.LocalTargetPosition.x)
            {
                visuals.enabled = false;

                if (transform.localPosition.x <= rhythmSystem_ref.LocalTargetPosition.x - overdrive)
                    ReturnToPool ();
            }

        }

    }

    // Updates the height of the Note Object.  This is relative to the speed at which the notes fall and 
    //  the specified Hit Window range.
    void UpdateHeight ()
    {
        float baseUnitHeight = visuals.sprite.rect.height / visuals.sprite.pixelsPerUnit;
        float targetUnitHeight = rhythmSystem_ref.WindowSizeInUnits * 2f; // Double it for before/after.

        Vector3 scale = transform.localScale;
        scale.y = (targetUnitHeight / baseUnitHeight) * scalingFactorY;
        transform.localScale = scale;
    }

    void UpdateWidth ()
    {
        float baseUnitWidth = visuals.sprite.rect.width / visuals.sprite.pixelsPerUnit;
        float targetUnitWidth = rhythmSystem_ref.WindowSizeInUnits * 2f; // Double it for before/after.

        Vector3 scale = transform.localScale;
        scale.x = (targetUnitWidth / baseUnitWidth) * scalingFactorX;
        transform.localScale = scale;
    }

    // Updates the position of the Note Object along the Lane based on current audio position.
    void UpdatePosition ()
    {
        // Get the number of samples we traverse given the current speed in Units-Per-Second.
        float samplesPerUnit = rhythmSystem_ref.SampleRate / rhythmSystem_ref.noteSpeed;

        // Our position is offset by the distance from the target in world coordinates.  This depends on
        //  the distance from "perfect time" in samples (the time of the Koreography Event!).
        Vector3 pos = rhythmSystem_ref.TargetPosition;
        pos.y -= (rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
        transform.position = pos;
    }

    void UpdatePosition (bool mirror)
    {

#if DEBUG
        Assert.IsNotNull (rhythmSystem_ref);
#endif

        // Get the number of samples we traverse given the current speed in Units-Per-Second.
        float samplesPerUnit = rhythmSystem_ref.SampleRate / rhythmSystem_ref.noteSpeed;

        // Our position is offset by the distance from the target in world coordinates.  This depends on
        //  the distance from "perfect time" in samples (the time of the Koreography Event!).
        Vector3 pos = (rhythmSystem_ref.LocalTargetPosition);

        if (mirror)
        {

            pos.x -= ((rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit);

        }
        else
        {
            pos.x += (rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
        }

        transform.localPosition = pos;
    }

    // Checks to see if the Note Object is currently hittable or not based on current audio sample
    //  position and the configured hit window width in samples (this window used during checks for both
    //  before/after the specific sample time of the Note Object).
    public bool IsNoteHittable ()
    {
#if DEBUG
        Assert.IsNotNull (rhythmSystem_ref);
        Assert.IsNotNull (trackedEvent);
#endif

        int noteTime = trackedEvent.StartSample;
        int curTime = rhythmSystem_ref.DelayedSampleTime;
        int hitWindow = rhythmSystem_ref.HitWindowSampleWidth;

        return (Mathf.Abs (noteTime - curTime) <= hitWindow);
    }

    // Checks to see if the note is no longer hittable based on the configured hit window width in
    //  samples.
    public bool IsNoteMissed ()
    {
        bool bMissed = true;

        if (enabled)
        {
            int noteTime = trackedEvent.StartSample;
            int curTime = rhythmSystem_ref.DelayedSampleTime;
            int hitWindow = rhythmSystem_ref.HitWindowSampleWidth;

            bMissed = (curTime - noteTime > hitWindow);
        }

        return bMissed;
    }

    // Returns this Note Object to the pool which is controlled by the Rhythm Game Controller.  This
    //  helps reduce runtime allocations.
    void ReturnToPool ()
    {
        if (!isMirror && mirror_ref)
            mirror_ref.ReturnToPool ();

        visuals.enabled = true;
        rhythmSystem_ref.ReturnNoteObjectToPool (this);
        //Reset ();
    }

    // Performs actions when the Note Object is hit.
    public void OnHit ()
    {
        //print ("Hit it!");
        // ReturnToPool ();
    }

    // Performs actions when the Note Object is cleared.
    public void OnClear ()
    {
        ReturnToPool ();
    }

    #endregion

}