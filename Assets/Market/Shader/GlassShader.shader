Shader "Custom/GlassShader" {
	Properties{
		_MainTint("Main Color", Color) = (1, 1, 1, 1)
		_AlphaVal("Alpha", Range(0, 1)) = 0.1
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf BlinnPhong alpha

		fixed4 _MainTint;
	float _AlphaVal;

	struct Input {
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _MainTint.rgb;
		o.Alpha = _AlphaVal;
	}
	ENDCG
	}
		FallBack "Diffuse"
}