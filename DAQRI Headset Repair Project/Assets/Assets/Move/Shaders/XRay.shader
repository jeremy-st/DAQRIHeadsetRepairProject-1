Shader "DAQRI/Core/XRay" {
	Properties {
		_MainTex 		 ( "Base (RGB)", 2D ) = "white" {}
		_TintColor 		 ( "Tint Color", Color ) = ( 1, 1, 1, 1 )
		_RimColor 		 ( "Rim Color", Color ) = ( 0.5,0.5,0.5,0.5 )
		_InnerColor 	 ( "Inner Color", Color ) = ( 0.5,0.5,0.5,0.5 )
		_InnerColorPower ( "Inner Color Power", Range( 0.0,1.0 ) ) = 0.5
		_RimPower 		 ( "Rim Power", Range( 1.0,5.0 ) ) = 2.5
		_AlphaPower 	 ( "Alpha Rim Power", Range( 0.0,8.0 ) ) = 4.0
		_Fader			 ( "Fader", Range( 0, 1 ) ) = 1.0
	}
	
	SubShader {
		Tags { "Queue" = "Transparent" }
		
		Cull Back
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		
		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		float4 	  _TintColor;
		float4 	  _RimColor;
		float4    _InnerColor;
		float 	  _InnerColorPower;
		float 	  _RimPower;
		float 	  _AlphaPower;
		fixed	  _Fader;

		void surf( Input IN, inout SurfaceOutput o ) {
			half4 c  = tex2D( _MainTex, IN.uv_MainTex ) * _TintColor;
			half rim = saturate( dot( normalize( IN.viewDir ), o.Normal ) );
			
			rim = 1.0 - rim;
			
			o.Emission = ( c.rgb * _RimColor.rgb * pow( rim, _RimPower ) + ( _InnerColor.rgb * 2 * _InnerColorPower ) ) * _Fader;
			o.Alpha    = c.a * ( pow( rim, _AlphaPower ) ) * _RimPower * _Fader;
		}
		
		ENDCG
	}
	
	Fallback "VertexLit"
}