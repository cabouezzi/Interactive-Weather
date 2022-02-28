Shader "EarthShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry" }
        ZWrite On
        LOD 100

        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half4 color : COLOR;
                float4 vertex : SV_POSITION;
                float3 pos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float2 getPosition2d (float3 position) {

                // arcsin 0 to 180
                float latitude = acos(normalize(position).y) / radians(180);

                // arctan ranges -90 to 90
                float longitude = atan2(position.x, position.z) / radians(360);

                return float2(-longitude, -latitude);

            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = v.vertex;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = getPosition2d(i.pos);
                fixed4 col = tex2D(_MainTex, pos);

                return col;
            }
            ENDCG
        }
    }
}
