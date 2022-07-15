Shader "FOV/FieldOfView"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        ColorMask 0
        LOD 100

        Stencil{
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {

        }

        CGINCLUDE
        half4 frag(v2 i) : SV_Target{
            return half4(1,1,1,1);
        }
        ENDCG
    }
}
