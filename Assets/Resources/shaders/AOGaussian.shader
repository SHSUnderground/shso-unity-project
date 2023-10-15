Shader "Hidden/AO Gaussian" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Horizontal ("Horizontal (Private)", Float) = 1
 _Weights0 ("Weights0 (Private)", Vector) = (0,0,0,0)
 _Weights1 ("Weights1 (Private)", Vector) = (0,0,0,0)
 _Weights2 ("Weights2 (Private)", Vector) = (0,0,0,0)
 _Weights3 ("Weights3 (Private)", Vector) = (0,0,0,0)
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
Vector 5 [_MainTex_Size]
Float 6 [_Horizontal]
Float 7 [_Width]
"!!ARBvp1.0
# 28 ALU
PARAM c[12] = { { -1, 0, -0.85714293, -0.71428573 },
		state.matrix.mvp,
		program.local[5..7],
		{ -0.5714286, 0, -0.42857146, -0.2857143 },
		{ 0.2857143, 0, 0.42857146, 0.5714286 },
		{ 0.71428573, 0, 0.85714293, 0.14285715 },
		{ 0, 0.5, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
DP4 R1.z, vertex.position, c[4];
MOV R0.w, R1.z;
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
DP4 R1.x, R0, c[11].yxxy;
DP4 R1.y, R0, c[11].xyxy;
MUL R2.xy, R1, c[5];
RCP R1.y, R1.z;
MUL R2.zw, R2.xyxy, R1.y;
MOV R1.x, c[11].z;
ADD R2.y, R1.x, -c[6].x;
MOV R2.x, c[6];
MUL R3.xy, R2, c[7].x;
MAD R1, R3.xyxy, c[10].wwzz, R2.zwzw;
MAD result.texcoord[0], R3.xyxy, c[0].xxzz, R2.zwzw;
MAD result.texcoord[1].zw, R3.xyxy, c[8].x, R2;
MAD result.texcoord[1].xy, R3, c[0].w, R2.zwzw;
MAD result.texcoord[2], R3.xyxy, c[8].zzww, R2.zwzw;
MOV result.texcoord[3].zw, R2;
MAD result.texcoord[3].xy, -R3, c[10].w, R2.zwzw;
MAD result.texcoord[4].zw, R3.xyxy, c[9].x, R2;
MOV result.texcoord[4].xy, R1;
MAD result.texcoord[5], R3.xyxy, c[9].zzww, R2.zwzw;
MOV result.texcoord[6].zw, R1;
MAD result.texcoord[6].xy, R3, c[10].x, R2.zwzw;
MAD result.texcoord[7].xy, R2, c[7].x, R2.zwzw;
MOV result.position, R0;
END
# 28 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_Size]
Vector 5 [_MainTex_TexelSize]
Float 6 [_Horizontal]
Float 7 [_Width]
"vs_2_0
; 36 ALU
def c8, 0.50000000, 0.00000000, -0.50000000, 1.00000000
def c9, -0.85714293, -1.00000000, -0.57142860, -0.71428573
def c10, -0.28571430, -0.42857146, 0.14285715, 0.28571430
def c11, 0.57142860, 0.42857146, 0.85714293, 0.71428573
dcl_position0 v0
dp4 r2.z, v0, c3
rcp r0.w, c4.y
mov r1.w, r2.z
dp4 r1.z, v0, c2
dp4 r1.y, v0, c1
dp4 r1.x, v0, c0
mov r0.xyz, c8.yzyw
mad r0.w, r0, c8.x, c8.x
dp4 r2.y, r1, r0
rcp r2.x, c4.x
mov r0.xyz, c8.xyyw
mad r0.w, r2.x, c8.x, c8.x
dp4 r2.x, r1, r0
mov r0.x, c6
rcp r0.z, r2.z
mul r2.xy, r2, r0.z
add r0.y, c8.w, -r0.x
mov r0.x, c6
mul r0.xy, r0, c7.x
mul r0.zw, r0.xyxy, c5.xyxy
mad oT0.zw, r0, c9.x, r2.xyxy
mad oT0.xy, r0.zwzw, c9.y, r2
mad oT1.zw, r0, c9.z, r2.xyxy
mad oT1.xy, r0.zwzw, c9.w, r2
mad oT2.zw, r0, c10.x, r2.xyxy
mad oT2.xy, r0.zwzw, c10.y, r2
mov oT3.zw, r2.xyxy
mad oT3.xy, -r0.zwzw, c10.z, r2
mad oT4.zw, r0, c10.w, r2.xyxy
mad oT4.xy, r0.zwzw, c10.z, r2
mad oT5.zw, r0, c11.x, r2.xyxy
mad oT5.xy, r0.zwzw, c11.y, r2
mad oT6.zw, r0, c11.z, r2.xyxy
mad oT6.xy, r0.zwzw, c11.w, r2
mad oT7.xy, r0, c5, r2
mov oPos, r1
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_Weights0]
Vector 1 [_Weights1]
Vector 2 [_Weights2]
Vector 3 [_Weights3]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 36 ALU, 12 TEX
PARAM c[5] = { program.local[0..3],
		{ 0 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEMP R8;
TEMP R9;
TEMP R10;
TEMP R11;
TEX R8.x, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R11.x, fragment.texcoord[1], texture[0], 2D;
TEX R10.x, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R0.x, fragment.texcoord[6].zwzw, texture[0], 2D;
TEX R1.x, fragment.texcoord[6], texture[0], 2D;
TEX R2.x, fragment.texcoord[5].zwzw, texture[0], 2D;
TEX R3.x, fragment.texcoord[5], texture[0], 2D;
TEX R4.x, fragment.texcoord[4].zwzw, texture[0], 2D;
TEX R5.x, fragment.texcoord[4], texture[0], 2D;
TEX R6.x, fragment.texcoord[3].zwzw, texture[0], 2D;
TEX R7.x, fragment.texcoord[3], texture[0], 2D;
TEX R9.x, fragment.texcoord[2], texture[0], 2D;
MUL R0.y, R11.x, c[0].z;
MUL R0.z, R10.x, c[0].w;
MAX R0.y, R0, c[4].x;
MAX R0.y, R0, R0.z;
MUL R0.z, R9.x, c[1].x;
MAX R0.y, R0, R0.z;
MUL R0.w, R8.x, c[1].y;
MAX R0.y, R0, R0.w;
MUL R0.z, R7.x, c[1];
MAX R0.y, R0, R0.z;
MUL R0.w, R6.x, c[1];
MAX R0.y, R0, R0.w;
MUL R0.z, R5.x, c[2].x;
MAX R0.y, R0, R0.z;
MUL R0.w, R4.x, c[2].y;
MAX R0.y, R0, R0.w;
MUL R0.z, R3.x, c[2];
MAX R0.y, R0, R0.z;
MUL R0.w, R2.x, c[2];
MAX R0.y, R0, R0.w;
MUL R0.w, R0.x, c[3].y;
MUL R0.z, R1.x, c[3].x;
MAX R0.x, R0.y, R0.z;
MAX result.color, R0.x, R0.w;
END
# 36 instructions, 12 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_Weights0]
Vector 1 [_Weights1]
Vector 2 [_Weights2]
Vector 3 [_Weights3]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 43 ALU, 12 TEX
dcl_2d s0
def c4, 0.00000000, 0, 0, 0
dcl t1
dcl t2
dcl t3
dcl t4
dcl t5
dcl t6
texld r6, t5, s0
texld r8, t4, s0
texld r9, t3, s0
texld r10, t2, s0
texld r11, t1, s0
mul r11.x, r11, c0.z
mov r0.y, t1.w
mov r0.x, t1.z
mov r1.xy, r0
mov r0.y, t2.w
mov r0.x, t2.z
mov r3.xy, r0
mov r0.y, t3.w
mov r0.x, t3.z
mov r5.xy, r0
mov r0.y, t4.w
mov r0.x, t4.z
mov r7.xy, r0
mov r2.y, t5.w
mov r2.x, t5.z
mov r4.xy, r2
mov r0.y, t6.w
mov r0.x, t6.z
max_pp r11.x, r11, c4
mul r10.x, r10, c1
texld r0, r0, s0
texld r2, t6, s0
texld r4, r4, s0
texld r7, r7, s0
texld r5, r5, s0
texld r3, r3, s0
texld r1, r1, s0
mul r1.x, r1, c0.w
max_pp r1.x, r11, r1
mul r3.x, r3, c1.y
max_pp r1.x, r1, r10
max_pp r1.x, r1, r3
mul r3.x, r9, c1.z
max_pp r1.x, r1, r3
mul r5.x, r5, c1.w
max_pp r1.x, r1, r5
mul r3.x, r8, c2
max_pp r1.x, r1, r3
mul r5.x, r7, c2.y
max_pp r1.x, r1, r5
mul r3.x, r6, c2.z
mul r4.x, r4, c2.w
max_pp r1.x, r1, r3
max_pp r1.x, r1, r4
mul r2.x, r2, c3
mul r0.x, r0, c3.y
max_pp r1.x, r1, r2
max_pp r0.x, r1, r0
mov_pp r0, r0.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}