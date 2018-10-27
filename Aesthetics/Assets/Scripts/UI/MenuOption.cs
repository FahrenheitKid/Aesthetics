using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;

public class MenuOption : MonoBehaviour {


	public TextMeshPro text;

	public Vector3 selectedScale;
	public Color32 selectedColor;
	public Color32 color;

	public Vector3 punchAddScale;

	

	public MenuOption previous;

	public MenuOption next;


	public MenuScreen previousScreen;
	public MenuScreen nextScreen;

	public TextMeshPro leftArrow;
	public TextMeshPro rightArrow;


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

			if(_isSelected)
			{
				transform.parent.GetComponent<MenuScreen>().currentOption = this;
				if(!isMultipleOptions)
				{
				
				
				transform.DOScale(selectedScale, 0.2f);
				text.fontMaterial.SetFloat("_FaceDilate", 0.4f);
				text.fontMaterial.SetColor("_FaceColor", selectedColor);

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

					
					leftArrow.color = selectedColor;
					rightArrow.color = selectedColor;
					
					
				}
				
			}
			else
			{
				if(!isMultipleOptions)
				{

				transform.DOScale(Vector3.one, 0.2f);
				text.fontMaterial.SetFloat("_FaceDilate", 0.1f);
				text.fontMaterial.SetColor("_FaceColor", color);

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
					
					leftArrow.color = color;
					rightArrow.color = color;

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

			if(pulse)
			{
				if(punchScale == null)
				{
					punchScale = transform.DOScale(punchAddScale + Vector3.one,1).SetLoops(-1,LoopType.Yoyo);
				}
				else
				{
					punchScale.Restart();
				}
				
			}
			else
			{
				punchScale.Kill(true);
				punchScale = null;
				transform.localScale = Vector3.one;
			}
        }
    }

	Tween punchScale;
	public bool isMultipleOptions;
	public bool isHorizontal;
	public List <GameObject> optionsList;
	public GameObject currentMultipleOption;
	

	// Use this for initialization
	void Start () {

		if(text)
		text.color = color;

		
		//punchScale = transform.DOPunchScale(punchAddScale,2,0,0).SetLoops(-1);
		if(isSelected && !pulse)
		{
			isSelected = false;
			isSelected = true;
		}

		if(pulse)
		{
			pulse = false;
			pulse = true;
		}
		//punchScale.Pause();
		
	}
	
	// Update is called once per frame
	void Update () {

		

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


	void Select(MenuOption option)
	{
		if(!option) return;

		isSelected = false;
		pulse = false;
		option.isSelected = true;
		option.pulse = true;

	}

	public void pulsateArrows(bool left, bool right)
	{
		if(right && rightArrow)
		rightArrow.transform.DOPunchScale(new Vector3(0.8f,0.8f,0f),0.3f,2,0.8f);
		if(left && leftArrow)
		leftArrow.transform.DOPunchScale(new Vector3(0.8f,0.8f,0f),0.3f,2,0.8f);
	}

	public void GoToMultipleOption(bool next)
	{
		if(optionsList.Any() && optionsList.Count > 1)
		{
			if(currentMultipleOption)
			currentMultipleOption.SetActive(false);
			else currentMultipleOption = optionsList[0];

			//ARRUMAR AQUI

			int i = optionsList.FindIndex(x => x.gameObject.name == currentMultipleOption.name);
			
			print("before " + i);
			if(next) i++; else i--;


			if(i >= optionsList.Count)
			{
				i = 0;
			} else if(i < 0) i = optionsList.Count - 1;

			print(i);
			currentMultipleOption = optionsList[i];

			currentMultipleOption.SetActive(true);
		}
	}

}
