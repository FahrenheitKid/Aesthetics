using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {


	 	[SerializeField]
        Color32 [] colorPrim = new Color32[2];
        [SerializeField]
        Color32 [] colorSec = new Color32[2];
        [SerializeField]
        Color32 [] colorTert = new Color32[2];

		public Renderer renderer_Ref;
                public Aesthetics.Player.Character character;

		
	// Use this for initialization
	void Start () {
		 setShaderColors(colorPrim[0],colorPrim[1],colorSec[0], colorSec[1], colorTert[0], colorTert[1]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void setShaderColors(Color32 colorPrim1, Color32 colorPrim2, Color32 colorSec1,Color32 colorSec2, Color32 colorTert1, Color32 colorTert2)
        {
            if(!renderer_Ref) return;

            Shader shader_ref = renderer_Ref.material.shader;

                    renderer_Ref.material.SetColor ("_ColorPrim1", colorPrim1);
                    renderer_Ref.material.SetColor ("_ColorPrim2", colorPrim2);
                    
                    renderer_Ref.material.SetColor ("_ColorSec1", colorSec1);
                    renderer_Ref.material.SetColor ("_ColorSec2", colorSec2);

                    renderer_Ref.material.SetColor ("_ColorTert1", colorTert1);
                    renderer_Ref.material.SetColor ("_ColorTert2", colorTert2);
        }
}
