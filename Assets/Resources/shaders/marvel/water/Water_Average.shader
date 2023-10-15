Shader "Marvel/FX/Water/Average" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Horizontal ("Horizontal (Private)", Float) = 1
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_TexelSize]
Float 6 [_Horizontal]
"!!ARBvp1.0
# 16 ALU
PARAM c[9] = { { -6.5, -4.5, -2.5, -0.5 },
		state.matrix.mvp,
		program.local[5..6],
		{ 1.5, 3.5, 5.5, 7 },
		{ 0, 1, -2, -1 } };
TEMP R0;
TEMP R1;
MOV R0.zw, c[8].xyyw;
ADD R0.y, R0.z, -c[6].x;
MOV R0.x, c[6];
MUL R1.xy, R0, c[5];
DP4 R0.x, vertex.position, c[2];
DP4 R0.y, vertex.position, c[1];
SLT R1.z, R0.x, c[8].x;
SLT R0.x, R0.y, c[8];
MAD R0.y, R1.z, c[8].z, c[8];
MAD R0.x, R0, c[8].z, c[8].y;
MAD result.position.xy, R0.wzzw, c[5], R0;
MAD result.texcoord[0], R1.xyxy, c[0].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[1], R1.xyxy, c[0].zzww, vertex.texcoord[0].xyxy;
MAD result.texcoord[2], R1.xyxy, c[7].xxyy, vertex.texcoord[0].xyxy;
MAD result.texcoord[3], R1.xyxy, c[7].zzww, vertex.texcoord[0].xyxy;
MOV result.position.zw, c[8].xyxy;
END
# 16 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Float 5 [_Horizontal]
"vs_2_0
; 23 ALU
def c6, 0.00000000, -1.00000000, 1.00000000, 0
def c7, -6.50000000, -4.50000000, -2.50000000, -0.50000000
def c8, 1.50000000, 3.50000000, 5.50000000, 7.00000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.x, c5
add r0.y, c6.z, -r0.x
mov r0.x, c5
mul r1.xy, r0, c4
dp4 r0.x, v0, c1
dp4 r0.y, v0, c0
slt r1.z, r0.x, c6.x
slt r0.x, r0.y, c6
max r0.y, -r1.z, r1.z
max r0.x, -r0, r0
slt r0.y, c6.x, r0
slt r0.x, c6, r0
add r0.y, -r0, -r0
add r0.x, -r0, -r0
mov r0.zw, c4.xyxy
add r0.y, r0, c6.z
add r0.x, r0, c6.z
mad oPos.xy, c6.yzzw, r0.zwzw, r0
mad oT0, r1.xyxy, c7.xxyy, v1.xyxy
mad oT1, r1.xyxy, c7.zzww, v1.xyxy
mad oT2, r1.xyxy, c8.xxyy, v1.xyxy
mad oT3, r1.xyxy, c8.zzww, v1.xyxy
mov oPos.zw, c6.xyxz
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 17 ALU, 8 TEX
PARAM c[1] = { { 0.5, 0.13330078 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEX R1.x, fragment.texcoord[3].zwzw, texture[0], 2D;
TEX R0.x, fragment.texcoord[3], texture[0], 2D;
TEX R2.x, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R3.x, fragment.texcoord[2], texture[0], 2D;
TEX R4.x, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R5.x, fragment.texcoord[1], texture[0], 2D;
TEX R7.x, fragment.texcoord[0].zwzw, texture[0], 2D;
TEX R6.x, fragment.texcoord[0], texture[0], 2D;
ADD R0.y, R6.x, R7.x;
ADD R0.y, R0, R5.x;
ADD R0.y, R0, R4.x;
ADD R0.y, R0, R3.x;
ADD R0.y, R0, R2.x;
MUL R0.z, R1.x, c[0].x;
ADD R0.x, R0.y, R0;
ADD R0.x, R0, R0.z;
MUL result.color, R0.x, c[0].y;
END
# 17 instructions, 8 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 22 ALU, 8 TEX
dcl_2d s0
def c0, 0.50000000, 0.13330078, 0, 0
dcl t0
dcl t1
dcl t2
dcl t3
texld r2, t3, s0
texld r5, t2, s0
texld r6, t1, s0
texld r7, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
mov r0.y, t1.w
mov r0.x, t1.z
mov r3.xy, r0
mov r0.y, t2.w
mov r0.x, t2.z
mov r4.xy, r0
mov r0.y, t3.w
mov r0.x, t3.z
texld r0, r0, s0
texld r4, r4, s0
texld r3, r3, s0
texld r1, r1, s0
add_pp r1.x, r7, r1
add_pp r1.x, r1, r6
add_pp r1.x, r1, r3
add_pp r1.x, r1, r5
add_pp r1.x, r1, r4
mul r0.x, r0, c0
add_pp r1.x, r1, r2
add_pp r0.x, r1, r0
mul_pp r0.x, r0, c0.y
mov_pp r0, r0.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}