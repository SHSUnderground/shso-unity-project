Shader "Transparent/Faded Object (Additive)" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+10" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+10" "RenderType"="Transparent" }
  Cull Off
  ColorMask 0
 }
 Pass {
  Tags { "QUEUE"="Transparent+10" "RenderType"="Transparent" }
  Lighting On
  Material {
   Ambient [_Color]
   Diffuse [_Color]
  }
  ZWrite Off
  Cull Off
  Blend One One
  ColorMask RGB
  SetTexture [_MainTex] { combine texture * primary double, texture alpha * primary alpha }
 }
}
Fallback "Diffuse"
}