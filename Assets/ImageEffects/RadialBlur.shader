Shader "Custom/FX/RadialBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Center("Center Point", Vector) = (0.5, 0.5, 0.0, 0.0)
		_Params("Strength (X) Samples (Y) Sharpness (Z) Darkness (W)", Vector) = (0.1, 10, 0.4, 0.35)
	}
	SubShader
	{

		Tags{ "RenderType" = "Opaque" }
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_ST;

			half2 _Center;
			half4 _Params;

			fixed4 frag(v2f i) : SV_Target
			{
				half2 coord = i.uv - _Center;
				fixed4 color = fixed4(0,0,0,1);
				half scale;
				half factor = _Params.y - 1;

				color += tex2Dlod(_MainTex, fixed4(coord + _Center, 0.0, 0.0));

				scale = 1.0 + _Params.x * factor;
				color += tex2Dlod(_MainTex, fixed4(coord * scale + _Center, 0.0, 0.0));

				scale = 1.0 + _Params.x * factor * 0.5;
				color += tex2Dlod(_MainTex, fixed4(coord * scale + _Center, 0.0, 0.0));

				scale = 1.0 + _Params.x * factor * 0.333;
				color += tex2Dlod(_MainTex, fixed4(coord * scale + _Center, 0.0, 0.0));

				scale = 1.0 + _Params.x * factor * 0.25;
				color += tex2Dlod(_MainTex, fixed4(coord * scale + _Center, 0.0, 0.0));

				return color * 0.20;

			}
			ENDCG
		}
	}
}
