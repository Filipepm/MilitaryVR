Shader "SVF/Lit Emissive Mix"
{
    Properties
    {
        _MainTex("Video Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
		_EmissiveMix("Emissive Mix", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
		fixed _EmissiveMix;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * (1 - _EmissiveMix);
            o.Emission = c.rgb * _EmissiveMix;
        }
        ENDCG
    }

    FallBack "Diffuse"
}