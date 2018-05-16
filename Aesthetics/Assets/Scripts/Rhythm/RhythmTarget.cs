using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;

public class RhythmTarget : MonoBehaviour
{

    public RhythmSystem rhythmSystem_ref;

    [Range (0, 2)]
    [SerializeField]
    private float beatPunchScale = 1.3f;

    [Range (0, 500)]
    [SerializeField]
    private int vibrato = 20;

    [Range (0, 1)]
    [SerializeField]
    private float elasticity = 0.5f;

    //[Range(0,1)]s
    [SerializeField, Candlelight.PropertyBackingField]
    private float _duration = 10f;
    public float duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }

    // Use this for initialization
    void Start ()
    {

        SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
        renderer.sortingOrder = 5;

        float musicBPM = (float) rhythmSystem_ref.currentMusicBPM;

        duration = (float) (60 / musicBPM) / 2;
        Koreographer.Instance.RegisterForEvents (rhythmSystem_ref.eventID, OnMainBeat);

        float cameraOffsetZ = -Camera.main.transform.position.z;
        Vector3 pos = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.09f, cameraOffsetZ));
        pos.z = 0;
        transform.position = pos;
        pos = transform.localPosition;
        pos.z = 5;
        transform.localPosition = pos;
        transform.rotation = Camera.main.transform.parent.rotation;

    }

    void OnMainBeat (KoreographyEvent evt)
    {

        transform.DOPunchScale (transform.localScale * beatPunchScale, duration, vibrato, elasticity);

    }
    // Update is called once per frame
    void Update ()
    {

    }
}