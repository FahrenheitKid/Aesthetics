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

	public bool isSelected;

	public bool pulse = true;

	Tween punchScale;
	

	// Use this for initialization
	void Start () {
		text.color = color;
		punchScale = transform.DOPunchScale(punchAddScale,2,0,0).SetLoops(-1);
		
	}
	
	// Update is called once per frame
	void Update () {

		

		if(pulse && !punchScale.IsPlaying())
		{
			print(punchScale.IsPlaying());
			punchScale.Restart();
			
		}
	}

}
