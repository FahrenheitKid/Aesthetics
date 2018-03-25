Shader "Custom/Tint/2TintSquareVertexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MinX ("Min X", Range(-0.5,0.5)) = 0
		_MaxX ("Max X", Range(-0.5,0.5)) = 0
		_MinZ ("Min Z", Range(-0.5,0.5)) = 0
		_MaxZ ("Max Z", Range(-0.5,0.5)) = 0
		
		_TintColor1("Tint Color 1", Color) = (1,1,1,1)
		_Tint1Gamma("Tint 1 Gamma", Range(0,1)) = 1
		_TintColor2("Tint Color 2", Color) = (1,1,1,1)
		_Tint2Gamma("Tint 2 Gamma", Range(0,1)) = 1

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "UnityCG.cginc"
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			 float3 localPos;
			 
		};

void vert (inout appdata_full v, out Input o) {
   UNITY_INITIALIZE_OUTPUT(Input,o);
   o.localPos = v.vertex.xyz;
 }

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		fixed4 _TintColor1;
		

		fixed4 _TintColor2;
		float _Tint1Gamma;
		float _Tint2Gamma;

		float _MinX;
		float _MaxX;
		float _MinZ;
		float _MaxZ;
	
		
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 pureColor = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

			//tint inside square of the grid
			if((IN.localPos.x > _MinX && IN.localPos.x < _MaxX) && (IN.localPos.z > _MinX && IN.localPos.z < _MaxZ))
			{
				c = c + _Color;
					
 				
				c = c * _TintColor1 * _Tint1Gamma;
			}else
			{

				c = c + _Color;
					
 				
				c = c * _TintColor2 * _Tint2Gamma;
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
