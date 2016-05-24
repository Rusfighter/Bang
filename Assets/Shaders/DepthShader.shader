Shader "Custom/OverdrawDebugReplacement"
{
	SubShader
	{
		Tags{ "RenderType" = "Background" }
		LOD 100

		Pass
		{
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			half4 vert(half4 vertex : POSITION) : SV_POSITION
			{
				return mul(UNITY_MATRIX_MVP, vertex);
			}

			fixed4 frag(half4 vertex : SV_POSITION) : COLOR
			{
				return fixed4(0.15, 0, 0, 1.0);
			}
			ENDCG
		}
	}
}