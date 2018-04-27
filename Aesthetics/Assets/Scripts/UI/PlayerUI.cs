#define DEBUG
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{

    #region properties

    [SerializeField, Candlelight.PropertyBackingField]
    private TMP_Text _playerName;
    public TMP_Text playerName
    {
        get
        {
            return _playerName;
        }
        set
        {
            _playerName = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private TMP_Text _score;
    public TMP_Text score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private TMP_Text _combo;
    public TMP_Text combo
    {
        get
        {
            return _combo;
        }
        set
        {
            _combo = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private TMP_Text _multiplier;
    public TMP_Text multiplier
    {
        get
        {
            return _multiplier;
        }
        set
        {
            _multiplier = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _totalWidth = 0;
    public float totalWidth
    {
        get
        {
            return _totalWidth;
        }
        set
        {
            _totalWidth = value;
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    private float _totalHeight = 0;
    public float totalHeight
    {
        get
        {
            return _totalHeight;
        }
        set
        {
            _totalHeight = value;
        }
    }

    #endregion

    // Use this for initialization
    void Start ()
    {

#if DEBUG
        Assert.IsNotNull (_playerName);
        Assert.IsNotNull (_combo);
        Assert.IsNotNull (_multiplier);
        Assert.IsNotNull (_score);
#endif

        UpdateTotalSizeValues ();
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void AdjustPosition ()
    {
        if (gameObject.CompareTag ("Player 1 UI"))
        {
            Vector2 pos = Vector2.zero;
            pos.x = _totalWidth / 2;
            pos.y = (_totalHeight * 1.5f) * -1;
            gameObject.GetComponent<RectTransform> ().rect.Set (pos.x, pos.y, 0, 0);
        }
    }

    void UpdateTotalSizeValues ()
    {

        _totalWidth = (_playerName.rectTransform.rect.width > _totalWidth) ? _playerName.rectTransform.rect.width : _totalWidth;
        _totalWidth = (_combo.rectTransform.rect.width > _totalWidth) ? _combo.rectTransform.rect.width : _totalWidth;
        _totalWidth = (_multiplier.rectTransform.rect.width > _totalWidth) ? _multiplier.rectTransform.rect.width : _totalWidth;
        _totalWidth = (_score.rectTransform.rect.width > _totalWidth) ? _score.rectTransform.rect.width : _totalWidth;

        _totalHeight = 0;
        _totalHeight = _playerName.rectTransform.rect.height;
        _totalHeight = _combo.rectTransform.rect.height;
        _totalHeight = _multiplier.rectTransform.rect.height;
        _totalHeight = _score.rectTransform.rect.height;

        //	AdjustPosition();
    }

    public void setScore (int score)
    {
        _score.text = score.ToString ();
    }

    public void setCombo (int combo)
    {
        if (combo > 0)
            _combo.text = "Combo: " + combo.ToString ();
        else
            _combo.text = "";
    }

    public void setMultiplier (int mult)
    {
        if (mult > 1)
            _multiplier.text = "x" + mult.ToString ();
        else
            _multiplier.text = "";
    }

    public void setName (string nam)
    {
        _playerName.text = nam;
    }
}