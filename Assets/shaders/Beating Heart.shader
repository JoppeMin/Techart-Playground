Shader "Joppe/BeatingHeart"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				//float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex += v.normal * (v.color * sin(_Time[2]));
				UNITY_TRANSFER_FOG(o,o.vertex);
				
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
			// apply fog
			UNITY_APPLY_FOG(i.fogCoord, col);
			return float4 (1,1,1,1);
		}
		ENDCG
	}
	}
}
