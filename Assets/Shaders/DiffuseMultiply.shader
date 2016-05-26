Shader "Custom/Mobile/DiffuseMultiply" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_BlendColor("Blend Color", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd

		sampler2D _MainTex;
		fixed4 _BlendColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = _BlendColor.rgb * c.rgb ;
			o.Alpha = c.a;
		}

		ENDCG
	}

	Fallback "Mobile/VertexLit"
	CustomEditor "CustomMaterialEditor"
}
