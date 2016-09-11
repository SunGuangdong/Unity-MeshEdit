Shader "long/ghost" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_pow("pow",float)=0.1
		_Color("Color",color)=(1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		/*
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			float4 _Color;
			float _pow;
			sampler2D _MainTex;			
			struct v2f{
			float4 pos:POSITION;
			float2 texcoord:TEXCOORD0;
			};
			v2f vert(appdata_full i){
			v2f v;
				i.vertex.x-=_pow*3;
			v.pos=mul(UNITY_MATRIX_MVP,i.vertex);
			v.texcoord=i.texcoord;
			return v;
			
			}
			float4 frag(v2f i):SV_Target{
			half4 tmpColor = tex2D(_MainTex, i.texcoord);
			if(tmpColor.a>0.1)
			tmpColor.a=0.25;
			return tmpColor*_Color;	
			}
			ENDCG
		}
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			float _pow;
			float4 _Color;
			sampler2D _MainTex;			
			struct v2f{
			float4 pos:POSITION;
			float2 texcoord:TEXCOORD0;
			};
			v2f vert(appdata_full i){
			v2f v;
				i.vertex.x-=_pow*2;
			v.pos=mul(UNITY_MATRIX_MVP,i.vertex);
			v.texcoord=i.texcoord;
			return v;
			
			}
			float4 frag(v2f i):SV_Target{
			half4 tmpColor = tex2D(_MainTex, i.texcoord);
			if(tmpColor.a>0.1)
			tmpColor.a=0.45;
			return tmpColor*_Color;	
			}
			ENDCG
		}
		
		*/
		Pass 
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			float _pow;
			float4 _Color;
			sampler2D _MainTex;			
			struct v2f{
			float4 pos:POSITION;
			float2 texcoord:TEXCOORD0;
			};
			v2f vert(appdata_full i){
			v2f v;
				i.vertex.x-=_pow*1;
			v.pos=mul(UNITY_MATRIX_MVP,i.vertex);
			v.texcoord=i.texcoord;
			return v;
			
			}
			float4 frag(v2f i):SV_Target{
			half4 tmpColor = tex2D(_MainTex, i.texcoord);
			if(tmpColor.a>0.1)
			tmpColor.a=0.7;
			return tmpColor*_Color;	
			}
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			struct v2f{
			float4 pos:POSITION;
			float2 texcoord:TEXCOORD0;
			};
			v2f vert(appdata_full i){
			v2f v;
			v.pos=mul(UNITY_MATRIX_MVP,i.vertex);
			v.texcoord=i.texcoord;
			return v;
			
			}
			float4 frag(v2f i):SV_Target{
			half4 tmpColor = tex2D(_MainTex, i.texcoord);
			if(tmpColor.a>0.1)
			tmpColor.a=1;
			return tmpColor;	
			}
			ENDCG
		}
		
	}
 
	
}
