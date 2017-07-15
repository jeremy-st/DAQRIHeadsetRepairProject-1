// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:2,rfrpo:True,rfrpn:Refraction,ufog:False,aust:False,igpj:True,qofs:-4000,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:1825,x:33791,y:32749,varname:node_1825,prsc:2|emission-4672-OUT,alpha-5407-R,voffset-1266-OUT;n:type:ShaderForge.SFN_VertexColor,id:638,x:32813,y:32823,varname:node_638,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:183,x:32813,y:32964,prsc:2,pt:False;n:type:ShaderForge.SFN_Tex2d,id:7222,x:32806,y:32324,ptovrint:False,ptlb:Noise_Texture,ptin:_Noise_Texture,varname:node_7222,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5046,x:33114,y:32933,varname:node_5046,prsc:2|A-7222-R,B-638-R,C-183-OUT;n:type:ShaderForge.SFN_Slider,id:2672,x:32793,y:33225,ptovrint:False,ptlb:Displacement_Amplitude,ptin:_Displacement_Amplitude,varname:node_2672,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:1266,x:33310,y:33018,varname:node_1266,prsc:2|A-5046-OUT,B-2672-OUT;n:type:ShaderForge.SFN_Lerp,id:5783,x:33536,y:32533,varname:node_5783,prsc:2|A-2386-RGB,B-9154-RGB,T-9872-OUT;n:type:ShaderForge.SFN_Color,id:2386,x:33291,y:32320,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:node_2386,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.3023831,c3:0.6544118,c4:1;n:type:ShaderForge.SFN_Color,id:9154,x:33291,y:32501,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:_node_2386_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:5169,x:33291,y:32684,ptovrint:False,ptlb:ColorC,ptin:_ColorC,varname:_ColorA_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9448277,c2:1,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:4672,x:33536,y:32694,varname:node_4672,prsc:2|A-5783-OUT,B-5169-RGB,T-638-R;n:type:ShaderForge.SFN_Tex2d,id:5407,x:32806,y:32515,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_5407,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9872,x:33077,y:32704,varname:node_9872,prsc:2|A-8381-OUT,B-638-R;n:type:ShaderForge.SFN_RemapRange,id:8381,x:33043,y:32365,varname:node_8381,prsc:2,frmn:0,frmx:1,tomn:-10,tomx:10|IN-7222-R;proporder:7222-5407-2386-9154-5169-2672;pass:END;sub:END;*/

Shader "DAQRI/DC_Shaders/DC_JetEngine_Jetwash_viz" {
    Properties {
        _Noise_Texture ("Noise_Texture", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _ColorA ("ColorA", Color) = (0,0.3023831,0.6544118,1)
        _ColorB ("ColorB", Color) = (1,0,0,1)
        _ColorC ("ColorC", Color) = (0.9448277,1,0,1)
        _Displacement_Amplitude ("Displacement_Amplitude", Range(0, 100)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent-4000"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            #pragma glsl
            uniform sampler2D _Noise_Texture; uniform float4 _Noise_Texture_ST;
            uniform float _Displacement_Amplitude;
            uniform float4 _ColorA;
            uniform float4 _ColorB;
            uniform float4 _ColorC;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 _Noise_Texture_var = tex2Dlod(_Noise_Texture,float4(TRANSFORM_TEX(o.uv0, _Noise_Texture),0.0,0));
                v.vertex.xyz += ((_Noise_Texture_var.r*o.vertexColor.r*v.normal)*_Displacement_Amplitude);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _Noise_Texture_var = tex2D(_Noise_Texture,TRANSFORM_TEX(i.uv0, _Noise_Texture));
                float3 emissive = lerp(lerp(_ColorA.rgb,_ColorB.rgb,((_Noise_Texture_var.r*20.0+-10.0)*i.vertexColor.r)),_ColorC.rgb,i.vertexColor.r);
                float3 finalColor = emissive;
                float4 _Alpha_var = tex2D(_Alpha,TRANSFORM_TEX(i.uv0, _Alpha));
                return fixed4(finalColor,_Alpha_var.r);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 2.0
            #pragma glsl
            uniform sampler2D _Noise_Texture; uniform float4 _Noise_Texture_ST;
            uniform float _Displacement_Amplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 _Noise_Texture_var = tex2Dlod(_Noise_Texture,float4(TRANSFORM_TEX(o.uv0, _Noise_Texture),0.0,0));
                v.vertex.xyz += ((_Noise_Texture_var.r*o.vertexColor.r*v.normal)*_Displacement_Amplitude);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    //CustomEditor "ShaderForgeMaterialInspector"
}
