Shader "Custom/Mask" {
	 Properties
     {
         _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
         _Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
     }
  
     SubShader {  
         Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
          Offset 0, -1
         ColorMask 0
         ZWrite On
         Pass
         {
              SetTexture[_MainTex] {
            Combine texture
        }
        SetTexture[_Alpha] {
            Combine previous, texture
        }
         }
     }
 }