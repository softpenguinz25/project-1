Shader "FOV/Inmask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Stencil{
            Ref 1
            Comp Equal
        }

        Pass
        {

        }
    }
}
