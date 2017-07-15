Shader "DC_Shaders/FX/EdgeGradientEmissive" {
	Properties {
		_MainTex	( "Base (RGB)", 2D ) = "white" {}
		_Color		( "Tint Outer Color", Color ) = ( 1, 1, 1, 1 )
		_InnerColor	( "Tint Inner Color", Color ) = ( 0.5,0.5,0.5,0.5 )
		_Bias		( "Bias", Range(-1, 1) ) = 0
		_Tension	( "Tension", Range(0.5, 0) ) = 0
		_AlphaShift	( "Alpha Shift", Range(0, 1) ) = 0
		_Fader		( "Fader", Range( 0, 1 ) ) = 0.5
	}
	
	SubShader {
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		Cull Back

		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		half4 _Color;
		half4 _InnerColor;
		fixed _Bias;
		fixed _Tension;
		fixed _AlphaShift;
		fixed _Fader;

		void surf( Input IN, inout SurfaceOutput o )
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed rim = dot(normalize(IN.viewDir), o.Normal);

			half remap = smoothstep(rim - _Tension, rim + _Tension, _AlphaShift);
			half aBias = ((remap * 2 - 1) * _Bias + 1);

			o.Emission = c.rgb * lerp(_Color, _InnerColor, rim);
			o.Alpha = c.a * _Fader * aBias;
		}
		ENDCG
	}	
	Fallback "VertexLit"
}