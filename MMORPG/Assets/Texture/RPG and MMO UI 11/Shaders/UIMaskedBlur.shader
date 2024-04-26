// Author: vinipc
// Forum post: https://forum.unity3d.com/threads/simple-optimized-blur-shader.185327/#post-3038561
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Based on cician's shader from: https://forum.unity3d.com/threads/simple-optimized-blur-shader.185327/#post-1267642

Shader "Custom/MaskedUIBlur"
{
	Properties
	{
		_Size("Blur", Range(0, 30)) = 1
		[HideInInspector] _MainTex("Tint Color (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		// Horizontal blur
		GrabPass
		{
			"_HBlur"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 uvmain : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif

				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;

				o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			sampler2D _HBlur;
			float4 _HBlur_TexelSize;
			float _Size;

			half4 frag(v2f i) : COLOR
			{
				float alpha = tex2D(_MainTex, i.uvmain).a;
				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernelx) tex2Dproj( _HBlur, UNITY_PROJ_COORD(float4(i.uvgrab.x + _HBlur_TexelSize.x * kernelx * _Size * alpha, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18,  0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);

				return sum;
			}
			ENDCG
		}

		// Vertical blur
		GrabPass
		{
			"_VBlur"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 uvmain : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_t v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif

				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;

				o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);

				return o;
			}

			sampler2D _VBlur;
			float4 _VBlur_TexelSize;
			float _Size;

			half4 frag(v2f i) : COLOR
			{
				float alpha = tex2D(_MainTex, i.uvmain).a;
				half4 sum = half4(0,0,0,0);

				#define GRABPIXEL(weight,kernely) tex2Dproj( _VBlur, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _VBlur_TexelSize.y * kernely * _Size * alpha, i.uvgrab.z, i.uvgrab.w))) * weight

				sum += GRABPIXEL(0.05, -4.0);
				sum += GRABPIXEL(0.09, -3.0);
				sum += GRABPIXEL(0.12, -2.0);
				sum += GRABPIXEL(0.15, -1.0);
				sum += GRABPIXEL(0.18,  0.0);
				sum += GRABPIXEL(0.15, +1.0);
				sum += GRABPIXEL(0.12, +2.0);
				sum += GRABPIXEL(0.09, +3.0);
				sum += GRABPIXEL(0.05, +4.0);

				return sum;
			}
			ENDCG
		}
	}
}
