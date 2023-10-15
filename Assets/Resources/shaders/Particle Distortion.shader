Shader "Particles/Distortion" {
Properties {
 _MainTex ("Distortion Texture", 2D) = "bump" {}
 _Scale ("Distortion Scale", Float) = 0.1
}
SubShader { 
 Tags { "QUEUE"="Transparent+10" }
 GrabPass {
 }
 Pass {
  Tags { "QUEUE"="Transparent+10" }
  ZTest Less
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Vector 5 [_ProjectionParams]
Vector 6 [_MainTex_ST]
Float 7 [_Scale]
Float 8 [_GrabPassBugFix]
"!!ARBvp1.0
# 19 ALU
PARAM c[9] = { { 0.5, 0 },
		state.matrix.mvp,
		program.local[5..8] };
TEMP R0;
TEMP R1;
DP4 R0.z, vertex.position, c[4];
MOV R0.w, R0.z;
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[5].x;
ADD R1.xy, R1, R1.z;
ABS R1.z, c[8].x;
ADD R1.w, R0.z, -R1.y;
SGE R0.z, c[0].y, R1;
ADD R1.z, R1.w, -R1.y;
MAD result.texcoord[2].y, R1.z, R0.z, R1;
DP4 R0.z, vertex.position, c[3];
MOV result.texcoord[2].x, R1;
MOV result.position, R0;
MOV result.texcoord[2].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[6], c[6].zwzw;
MUL result.texcoord[1].xy, vertex.color, c[7].x;
MOV result.texcoord[1].zw, vertex.color;
END
# 19 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ProjectionParams]
Vector 5 [_ScreenParams]
Vector 6 [_MainTex_ST]
Float 7 [_Scale]
Float 8 [_GrabPassBugFix]
"vs_2_0
; 25 ALU
def c9, 0.00000000, 0.50000000, 1.00000000, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
dp4 r0.z, v0, c3
mov r0.y, c8.x
mov r0.x, c9
mov r0.w, r0.z
sge r0.y, c9.x, r0
sge r0.x, c8, r0
mul r1.w, r0.x, r0.y
max r1.w, -r1, r1
slt r1.w, c9.x, r1
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c9.y
mul r1.y, r1, c4.x
mad r1.xy, r1.z, c5.zwzw, r1
add r2.x, -r1.w, c9.z
add r0.z, r0, -r1.y
mul r1.z, r1.y, r2.x
mad oT2.y, r1.w, r0.z, r1.z
dp4 r0.z, v0, c2
mov oT2.x, r1
mov oPos, r0
mov oT2.zw, r0
mad oT0.xy, v1, c6, c6.zwzw
mul oT1.xy, v2, c7.x
mov oT1.zw, v2
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_GrabTexture] 2D
"!!ARBfp1.0
# 7 ALU, 2 TEX
PARAM c[1] = { { 2, 1 } };
TEMP R0;
TEX R0.xyw, fragment.texcoord[0], texture[0], 2D;
MAD R0.xy, R0, c[0].x, -c[0].y;
MUL R0.xy, fragment.texcoord[1], R0;
RCP R0.z, fragment.texcoord[2].w;
MAD R0.xy, fragment.texcoord[2], R0.z, R0;
MUL result.color.w, fragment.texcoord[1], R0;
TEX result.color.xyz, R0, texture[1], 2D;
END
# 7 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_GrabTexture] 2D
"ps_2_0
; 6 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 2.00000000, -1.00000000, 0, 0
dcl t0.xy
dcl t1.xyzw
dcl t2.xyzw
texld r1, t0, s0
mad_pp r0.xy, r1, c0.x, c0.y
mul r0.xy, t1, r0
rcp r1.x, t2.w
mad r0.xy, t2, r1.x, r0
texld r0, r0, s1
mul r0.w, t1, r1
mov_pp oC0, r0
"
}
}
 }
}
}