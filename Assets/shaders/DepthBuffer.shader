Shader "Personal/DepthBuffer"
{
    Properties
    {
        _MainTex ("Noise Map", 2D) = "white" {}
		PanSpeed("Panning Speed", Float) = 1
		Threshold("Threshold", Float) = 1
		FoamAmount("Foam Amount", Float) = 1
		FoamColor("Foam Color", Color) = (1,1,1,1)
		DepthColorShallow("Gradient Shallow", Color) = (0,0,0,1)
		DepthColorDeep("Gradient Deep", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			uniform sampler2D _CameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
			float Threshold;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

			fixed4 DepthColorShallow;
			fixed4 DepthColorDeep;
			fixed FoamAmount;
			fixed4 FoamColor;
			float PanSpeed;

            fixed4 frag (v2f i) : SV_Target
            {
				float noise = sin(_Time[3]);
				i.screenPosition.x += noise * i.screenPosition.y;

				float existingDepth = tex2Dproj(_CameraDepthTexture,  UNITY_PROJ_COORD(i.screenPosition));
				float existingDepthLinear = LinearEyeDepth(existingDepth);
				
				float depthDifference = existingDepthLinear - i.screenPosition.w;
				float waterDepthDifference = saturate(depthDifference / Threshold);
				

				float4 waterColor = lerp(DepthColorShallow, DepthColorDeep, waterDepthDifference);
				
				waterColor = waterDepthDifference < FoamAmount ? FoamColor : waterColor;
				
				return waterColor;
            }
            ENDCG
        }
    }
}
