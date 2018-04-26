#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.UI;
using UnityEngine.Assertions;



public class RhythmNote : MonoBehaviour {

	
		#region Fields

		[Tooltip("The visual to use for this Note Object.")]
		public SpriteRenderer visuals;

		// If active, the KoreographyEvent that this Note Object wraps.  Contains the relevant timing information.
		KoreographyEvent trackedEvent;

		public RhythmSystem rhythmSystem_ref;
		public Vector3 destination;

		public bool isMirror;

		private Vector3 foward;
		private Vector3 right;

		#endregion
		#region Static Methods
		
		// Unclamped Lerp.  Same as Vector3.Lerp without the [0.0-1.0] clamping.
		static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			return new Vector3 (from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		#endregion
		#region Methods

		// Prepares the Note Object for use.
		public void Initialize(KoreographyEvent evt, Color color, RhythmSystem rhythmSys, bool isMirrorNote)
		{
			trackedEvent = evt;
			//visuals.color = color;
			isMirror = isMirrorNote;
			rhythmSystem_ref = rhythmSys;

				SpriteRenderer renderer = GetComponent<SpriteRenderer>();
				renderer.sortingOrder = 4;

			transform.position = rhythmSystem_ref.spawnPosition;

			foward = Camera.main.transform.forward;
			foward.y = 0;
			foward = Vector3.Normalize(foward);

			right = Quaternion.Euler(new Vector3(0,90,0)) * foward;

		}

		// Resets the Note Object to its default state.
		void Reset()
		{
			trackedEvent = null;
			rhythmSystem_ref = null;
		}

		void Update()
		{

			UpdateWidth();
			UpdateHeight();

			UpdatePosition(isMirror);

			CheckDespawn();
		}

		void CheckDespawn()
		{

			if(!isMirror)
			{
				if (transform.localPosition.x >= rhythmSystem_ref.LocalTargetPosition.x)
			{
				rhythmSystem_ref.ReturnNoteObjectToPool(this);
				Reset();
			}
			}
			else
			{
				if (transform.localPosition.x <= rhythmSystem_ref.LocalTargetPosition.x)
			{
				rhythmSystem_ref.ReturnNoteObjectToPool(this);
				Reset();
			}

			}

		}

		// Updates the height of the Note Object.  This is relative to the speed at which the notes fall and 
		//  the specified Hit Window range.
		void UpdateHeight()
		{
			float baseUnitHeight = visuals.sprite.rect.height / visuals.sprite.pixelsPerUnit;
			float targetUnitHeight = rhythmSystem_ref.WindowSizeInUnits * 2f;	// Double it for before/after.

			Vector3 scale = transform.localScale;
			scale.y = targetUnitHeight / baseUnitHeight;	
			transform.localScale = scale;
		}

			void UpdateWidth()
		{
			float baseUnitWidth = visuals.sprite.rect.width / visuals.sprite.pixelsPerUnit;
			float targetUnitWidth = rhythmSystem_ref.WindowSizeInUnits * 2f;	// Double it for before/after.

			Vector3 scale = transform.localScale;
			scale.x = targetUnitWidth / baseUnitWidth;	
			transform.localScale = scale;
		}

		// Updates the position of the Note Object along the Lane based on current audio position.
		void UpdatePosition()
		{
			// Get the number of samples we traverse given the current speed in Units-Per-Second.
			float samplesPerUnit = rhythmSystem_ref.SampleRate / rhythmSystem_ref.noteSpeed;

			// Our position is offset by the distance from the target in world coordinates.  This depends on
			//  the distance from "perfect time" in samples (the time of the Koreography Event!).
			Vector3 pos = rhythmSystem_ref.TargetPosition;
			pos.y -= (rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
			transform.position = pos;
		}

		void UpdatePosition(bool mirror)
		{
			
			#if DEBUG
			Assert.IsNotNull(rhythmSystem_ref);
			#endif

			// Get the number of samples we traverse given the current speed in Units-Per-Second.
			float samplesPerUnit = rhythmSystem_ref.SampleRate / rhythmSystem_ref.noteSpeed;


			/*
				GameObject a = new GameObject();
				GameObject b = new GameObject();
				a.transform.position = Vector3.zero;
				b.transform.position = Vector3.zero;
				Vector3 hold = b.transform.position;
				hold.x = 1;
				b.transform.position = hold;

				float dis = Vector3.Distance(Camera.main.transform.InverseTransformPoint(a.transform.position), Camera.main.transform.InverseTransformPoint(b.transform.position));
				print("A = "  + Camera.main.transform.InverseTransformPoint(a.transform.position));
				print("B = "  + Camera.main.transform.InverseTransformPoint(b.transform.position));
				print("new dis  = " + dis);
			 */
			

			// Our position is offset by the distance from the target in world coordinates.  This depends on
			//  the distance from "perfect time" in samples (the time of the Koreography Event!).
			Vector3 pos = (rhythmSystem_ref.LocalTargetPosition);
			

			if(mirror)
			{
				

					pos.x -= ((rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit);
					
			}
			else
			{
				pos.x += (rhythmSystem_ref.DelayedSampleTime - trackedEvent.StartSample) / samplesPerUnit;
			}
			
			
			//pos = (Quaternion.Euler(-transform.rotation.eulerAngles.x ,transform.rotation.eulerAngles.y,0) * pos); 
			//pos = (Quaternion.Euler(rhythmSystem_ref.angleX, rhythmSystem_ref.angleY,0) * pos);
			//pos.y = rhythmSystem_ref.rhythmTarget_Ref.transform.position.y;
			transform.localPosition = pos;
		}

		// Checks to see if the Note Object is currently hittable or not based on current audio sample
		//  position and the configured hit window width in samples (this window used during checks for both
		//  before/after the specific sample time of the Note Object).
		public bool IsNoteHittable()
		{
			int noteTime = trackedEvent.StartSample;
			int curTime = rhythmSystem_ref.DelayedSampleTime;
			int hitWindow = rhythmSystem_ref.HitWindowSampleWidth;

			return (Mathf.Abs(noteTime - curTime) <= hitWindow);
		}

		// Checks to see if the note is no longer hittable based on the configured hit window width in
		//  samples.
		public bool IsNoteMissed()
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
		void ReturnToPool()
		{
			rhythmSystem_ref.ReturnNoteObjectToPool(this);
			Reset();
		}

		// Performs actions when the Note Object is hit.
		public void OnHit()
		{
			ReturnToPool();
		}

		// Performs actions when the Note Object is cleared.
		public void OnClear()
		{
			ReturnToPool();
		}

		#endregion
	
}
