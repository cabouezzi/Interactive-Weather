Shader "WindShader"
 {
    Properties
    {
        [PerRendererData] _MainTex ("Base (RGB)", 2D) = "white" {}
        _Alpha ("Alpha", Range(0, 1)) = 1
        [MaterialToggle] _Emission ("Emits", Float) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Lighting Off
        Blend One OneMinusSrcAlpha
 
          
        CGPROGRAM
        #pragma surface surf Lambert alpha
  
        sampler2D _MainTex;
        bool _Emission;
        float _Alpha;
        float4 _MainTex_TexelSize;
  
        struct Input 
        {
            float4 color : COLOR;
            float2 uv_MainTex;
            float3 worldPos;
        };

        float2 getPosition2d (float3 position) {

            // arcsin 0 to 180
            float latitude = acos(normalize(position).y) / radians(180);

            // arctan ranges -90 to 90
            float longitude = atan2(position.x, position.z) / radians(360);

            return float2(-longitude, -latitude);

        }
         
        void surf (Input IN, inout SurfaceOutput o) 
        {

            float2 uv = getPosition2d(IN.worldPos);

            float4 color = tex2D(_MainTex, uv);

            o.Albedo = color;
            o.Alpha = _Alpha;
            if (_Emission) {
                o.Emission = color;
            }

        }
        ENDCG
      } 
      FallBack "Diffuse"
 }