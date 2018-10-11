using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuOption : MonoBehaviour {


	public TextMeshPro text;

	public Vector3 selectedScale;
	public Color32 selectedColor;

	public Vector3 punchAddScale;

	public Color32 color;

	public MenuOption previous;

	public MenuOption next;

	public MenuScreen nextScreen;


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
				pulse = true;
			}
			else
			{
				pulse = false;
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

			if(pulse && punchScale.IsPlaying())
			{
				punchScale = transform.DOScale(punchAddScale + Vector3.one,1).SetLoops(-1,LoopType.Yoyo);
			}
			else
			{
				punchScale.Kill(true);
			}
        }
    }

	Tween punchScale;
	

	// Use this for initialization
	void Start () {
		text.color = color;

		
		//punchScale = transform.DOPunchScale(punchAddScale,2,0,0).SetLoops(-1);
		if(pulse)
		punchScale = transform.DOScale(punchAddScale + Vector3.one,1).SetLoops(-1,LoopType.Yoyo);
		//punchScale.Pause();
		
	}
	
	// Update is called once per frame
	void Update () {

		

		if(pulse && !punchScale.IsPlaying())
		{
			//print(punchScale.IsPlaying());
			punchScale.Restart();
			
		}

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

		
		
	}


	void Select(MenuOption option)
	{
		if(!option) return;

		isSelected = false;
		pulse = false;
		option.isSelected = true;
		option.pulse = true;

	}

	

}
