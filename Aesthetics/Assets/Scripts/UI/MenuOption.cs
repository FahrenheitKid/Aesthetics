using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MenuOption : MonoBehaviour
{

    public TextMeshPro text;
    public TextMeshPro labelText;

    public Vector3 selectedScale;
    public Color32 selectedColor;
    public Color32 color;

    public Vector3 punchAddScale;

    public int player_idx;
    public MenuOption previous;

    public MenuOption next;

    public MenuScreen previousScreen;
    public MenuScreen nextScreen;

    public TextMeshPro leftArrow;
    public TextMeshPro rightArrow;

    public Tween rightArrowTween;
    public Tween leftArrowTween;

    [SerializeField, Candlelight.PropertyBackingField]
    protected bool _isSelected;
    public bool isSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;

            if (_isSelected)
            {
                transform.parent.GetComponent<MenuScreen> ().currentOptions[player_idx] = this;
                if (currentMultipleOption) currentMultipleOption.SetActive (true);
                if (!isMultipleOptions && !isToggleOption)
                {

                    transform.DOScale (selectedScale, 0.2f);
                    //text.fontMaterial.SetFloat ("_FaceDilate", 0.4f);
                    //text.fontMaterial.SetColor ("_FaceColor", selectedColor);
                    text.color = selectedColor;

                }
                else if (isToggleOption)
                {
                    if (text)
                        text.color = selectedColor;
                    if (labelText)
                        labelText.color = selectedColor;
                }
                else
                {
                    //if(true) return ;
                    /*
                    leftArrow.fontMaterial.SetFloat("_FaceDilate", 0.4f);
                    leftArrow.fontMaterial.SetColor("_FaceColor", selectedColor);

                    rightArrow.fontMaterial.SetFloat("_FaceDilate", 0.4f);
                    rightArrow.fontMaterial.SetColor("_FaceColor", selectedColor);
                     */

                    // if(!leftArrow && !rightArrow)
                    if (text)
                        text.color = selectedColor;

                    if (leftArrow)
                        leftArrow.color = selectedColor;
                    if (rightArrow)
                        rightArrow.color = selectedColor;

                }

                if (pulseSelect)
                {
                    if (!pulse)
                        pulse = true;
                }

            }
            else
            {
                if (!isMultipleOptions && !isToggleOption)
                {

                    transform.DOScale (Vector3.one, 0.2f);
                    //text.fontMaterial.SetFloat ("_FaceDilate", 0.1f);
                    //text.fontMaterial.SetColor ("_FaceColor", color);
                    text.color = color;

                }
                else if (isToggleOption)
                {
                    if (text)
                        text.color = color;
                    if (labelText)
                        labelText.color = color;
                }
                else
                {
                    //if(true) return ;

                    /*
                    	leftArrow.fontMaterial.SetFloat("_FaceDilate", 0.1f);
                    leftArrow.fontMaterial.SetColor("_FaceColor", color);

                    rightArrow.fontMaterial.SetFloat("_FaceDilate", 0.1f);
                    rightArrow.fontMaterial.SetColor("_FaceColor", color);
                     */
                    //if(!leftArrow && !rightArrow)
                    if (text)
                        text.color = color;

                    if (leftArrow)
                        leftArrow.color = color;
                    if (rightArrow)
                        rightArrow.color = color;

                }

                if (pulseSelect)
                {
                    if (pulse)
                        pulse = false;
                }
            }
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected bool _pulse;
    public bool pulse
    {
        get
        {
            return _pulse;
        }
        set
        {
            _pulse = value;

            if (pulse)
            {
                if (punchScale == null)
                {
                    punchScale = transform.DOScale (punchAddScale + Vector3.one, 1).SetLoops (-1, LoopType.Yoyo);
                }
                else
                {
                    punchScale.Restart ();
                }

            }
            else
            {
                punchScale.Kill (true);
                punchScale = null;
                transform.localScale = Vector3.one;
            }
        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected bool _pulseSelect = false;
    public bool pulseSelect
    {
        get
        {
            return _pulseSelect;
        }
        set
        {
            _pulseSelect = value;

        }
    }

    [SerializeField, Candlelight.PropertyBackingField]
    protected bool _isToggleOn;
    public bool isToggleOn
    {
        get
        {
            return _isToggleOn;
        }
        set
        {
            _isToggleOn = value;

            if (_isToggleOn)
            {
                foreach (GameObject go in togglableOptionsOn)
                {
                    go.SetActive (true);
                }

                foreach (GameObject go in togglableOptionsOff)
                {
                    go.SetActive (false);
                }

            }
            else
            {

                foreach (GameObject go in togglableOptionsOn)
                {
                    go.SetActive (false);
                }

                foreach (GameObject go in togglableOptionsOff)
                {
                    go.SetActive (true);
                }
            }
        }
    }

    Tween punchScale;

    public bool isToggleOption;
    public bool isMultipleOptions;
    public bool isHorizontal;
    public List<GameObject> optionsList;

    public List<GameObject> togglableOptionsOn;
    public List<GameObject> togglableOptionsOff;
    public GameObject currentMultipleOption;

    // Use this for initialization
    void Start ()
    {

        if (text)
            text.color = color;

        //punchScale = transform.DOPunchScale(punchAddScale,2,0,0).SetLoops(-1);
        if (isSelected && !pulse)
        {
            isSelected = false;
            isSelected = true;
        }

        if (pulse && pulseSelect == false)
        {
            print ("FOI: pulse: " + pulse + " select: " + pulseSelect);
            pulse = false;
            pulse = true;
        }

        //punchScale.Pause();

    }

    // Update is called once per frame
    void Update ()
    {

        //if(pulse && !punchScale.IsPlaying())
        //{
        //	//print(punchScale.IsPlaying());
        //	punchScale.Restart();

        //}

        /*
        	
        	for(int i = 1; i <=4; i++)
        	{
        		string axis = "Vertical" + i;

        		if(Input.GetAxis(axis) > 0)
        		{
        			Select(previous);
        		}
        		else if(Input.GetAxis(axis) < 0)
        		{
        			Select(next);
        		}
        		
        	}
	
         */

    }

    void Select (MenuOption option)
    {
        if (!option) return;

        isSelected = false;
        pulse = false;
        option.isSelected = true;
        if (pulseSelect)
            option.pulse = true;

    }

    public void pulsateArrows (bool left, bool right)
    {
        if (right && rightArrow)
        {
            if (rightArrowTween != null)
            {
                rightArrowTween.Complete ();
                rightArrowTween.Kill ();
                rightArrowTween = null;
            }
            rightArrowTween = rightArrow.transform.DOPunchScale (new Vector3 (0.8f, 0.8f, 0f), 0.3f, 2, 0.8f);

        }

        if (left && leftArrow)
        {
            if (leftArrowTween != null)
            {
                leftArrowTween.Complete ();
                leftArrowTween.Kill ();
                leftArrowTween = null;
            }
            leftArrowTween = leftArrow.transform.DOPunchScale (new Vector3 (0.8f, 0.8f, 0f), 0.3f, 2, 0.8f);

        }

    }

    public void GoToMultipleOption (bool next)
    {
        if (optionsList.Any () && optionsList.Count > 1)
        {
            if (currentMultipleOption)
                currentMultipleOption.SetActive (false);
            else currentMultipleOption = optionsList[0];

            int i = optionsList.FindIndex (x => x.gameObject.name == currentMultipleOption.name);

            // print ("before " + i);
            if (next) i++;
            else i--;

            if (i >= optionsList.Count)
            {
                i = 0;
            }
            else if (i < 0) i = optionsList.Count - 1;

            //            print (i);
            currentMultipleOption = optionsList[i];

            currentMultipleOption.SetActive (true);
        }
    }

}