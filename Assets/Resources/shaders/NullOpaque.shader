Shader "Special/Null Opaque" {
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  ZWrite Off
  Fog { Mode Off }
  Blend Zero One
 }
}
Fallback Off
}