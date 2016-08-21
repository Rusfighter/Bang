Shader "Custom/PostFX/Unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Fog{ Mode off }
		ZTest Always Cull Off ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = float4(v.vertex.xy * 2 - float2(1, 1), 0, 1);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				//return i.color;
				fixed4 col = tex2D(_MainTex, i.uv);
				//color += tex2D(_MainTex, i.uv);
				//color += tex2D(_MainTex, i.uv2);
				//color += tex2D(_MainTex, i.uv3);
				//color *= 0.333;
				return col;
			}
			ENDCG
		}
	}
}
