//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Marvel/FX/Water Surface" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _WaveColor ("Wave Color", Color) = (1,1,1,1)
 _ThresholdValue ("Wave threshold value", Float) = 0.65
 _ThresholdRange ("Wave threshold range", Float) = 0.1
 _WaveTex ("Wave Texture (Private)", 2D) = "white" {}
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Fog {
   Color [_AddFog]
  }
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_ST]
"!!ARBvp1.0
# 6 ALU
PARAM c[6] = { { 1 },
		state.matrix.mvp,
		program.local[5] };
ADD result.texcoord[0].zw, -vertex.texcoord[0].xyxy, c[0].x;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 6 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_ST]
"vs_2_0
; 6 ALU
def c5, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
add oT0.zw, -v1.xyxy, c5.x
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_Color]
Vector 1 [_WaveColor]
Float 2 [_ThresholdValue]
Float 3 [_ThresholdRange]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_WaveTex] 2D
"!!ARBfp1.0
OPTION ARB_fog_exp2;
OPTION ARB_precision_hint_fastest;
# 12 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R1.x, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MAD R1.x, R1, c[4].y, -c[4];
ABS R1.x, R1;
ADD R1.x, R1, -c[2];
MUL R0, R0, c[0];
RCP R1.y, c[3].x;
ABS R1.x, R1;
MUL_SAT R2.x, R1, R1.y;
ADD R1, -R0, c[1];
ADD R2.x, -R2, c[4];
MAD result.color, R2.x, R1, R0;
END
# 12 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_Color]
Vector 1 [_WaveColor]
Float 2 [_ThresholdValue]
Float 3 [_ThresholdRange]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_WaveTex] 2D
"ps_2_0
; 13 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c4, 2.00000000, -1.00000000, 1.00000000, 0
dcl t0
texld r1, t0, s0
mul r1, r1, c0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mad r0.x, r0, c4, c4.y
abs_pp r0.x, r0
add r2.x, r0, -c2
abs r2.x, r2
rcp r0.x, c3.x
mul_sat r0.x, r2, r0
add_pp r2, -r1, c1
add r0.x, -r0, c4.z
mad_pp r0, r0.x, r2, r1
mov_pp oC0, r0
"
}
}
 }
}
}