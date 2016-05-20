Shader "Custom/ImageEffect/MulAdd"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _ColorBuffer;
	fixed intensity;

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		o.uv = v.texcoord;

		return o;
	}

	fixed4 frag(v2f i) : SV_Target{
		return tex2D(_MainTex, i.uv) * intensity;
	}

	ENDCG

	SubShader {
		ZTest Always
		Cull Off
		ZWrite Off
		Fog{ Mode off }

		Pass{
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	FallBack off
}
