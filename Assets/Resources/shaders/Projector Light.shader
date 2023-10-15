//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Projector/Light" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear }
 _FalloffTex ("FallOff", 2D) = "" { TexGen ObjectLinear }
}
SubShader { 
 Pass {
  Color [_Color]
  ZWrite Off
  Fog {
   Color (0,0,0,1)
  }
  Blend DstColor One
  ColorMask RGB
  Offset -1, -1
  SetTexture [_ShadowTex] { Matrix [_Projector] combine texture * primary, one-texture alpha }
  SetTexture [_FalloffTex] { Matrix [_ProjectorClip] ConstantColor (0,0,0,0) combine previous lerp(texture) constant }
 }
}
}