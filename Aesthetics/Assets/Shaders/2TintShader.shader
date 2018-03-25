Shader "Custom/Tint/2TintShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ColorMask1("Color Mask 1", Color) = (1,1,1,1)
		_ColorProximity1("Tolerance 1", Range(0,1)) = 0.8
		_TintColor1("Tint Color 1", Color) = (1,1,1,1)

		_ColorMask2("Color Mask 2", Color) = (1,1,1,1)
		_ColorProximity2("Tolerance 2", Range(0,1)) = 0.8
		_TintColor2("Tint Color 2", Color) = (1,1,1,1)

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

 		
        
		#include "UnityCG.cginc"
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		fixed4 _TintColor1;
		fixed4 _ColorMask1;
		float  _ColorProximity1;

		fixed4 _TintColor2;
		fixed4 _ColorMask2;
		float  _ColorProximity2;

		
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 pureColor = tex2D (_MainTex, IN.uv_MainTex);
			
			bool once = false;
			//check to see if color are "similar"
			float diffRed = abs( pureColor.r - _ColorMask1.r);
			float diffGreen = abs( pureColor.g - _ColorMask1.g);
			float diffBlue = abs( pureColor.b - _ColorMask1.b);

			float diff = (diffRed +  diffGreen + diffGreen) / 3;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
				if(diff > _ColorProximity1)
				{
 				c = c + _Color;
				c = c * _TintColor1;
				once = true;
				}
			
			diffRed = abs( pureColor.r - _ColorMask2.r);
			 diffGreen = abs( pureColor.g - _ColorMask2.g);
			 diffBlue = abs( pureColor.b - _ColorMask2.b);

			 diff = (diffRed +  diffGreen + diffGreen) / 3;

			 if(diff > _ColorProximity2)
				{
					if(once == false)
					{
						c = c + _Color;
					}
 				
				c = c * _TintColor2;
				
				}


			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
