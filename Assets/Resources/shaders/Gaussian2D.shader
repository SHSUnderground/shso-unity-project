Shader "Marvel/Gaussian2D" {
Properties {
 _MainTex2D ("Base (RGB)", 2D) = "white" {}
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
Vector 5 [_MainTex2D_TexelSize]
Vector 6 [_Target_TexelSize]
Float 7 [_Horizontal]
Float 8 [_Width]
"!!ARBvp1.0
# 34 ALU
PARAM c[13] = { { 0, 0.5, 1, 4 },
		state.matrix.mvp,
		program.local[5..8],
		{ -0.85714293, 0, -1, -0.5714286 },
		{ -0.71428573, 0, -0.2857143, -0.42857146 },
		{ 0.14285715, 0.2857143, 0, 0.5714286 },
		{ 0.42857146, 0, 0.85714293, 0.71428573 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
DP4 R2.y, vertex.position, c[4];
MOV R0.w, R2.y;
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
DP4 R1.w, R0, c[0].xyxy;
DP4 R1.z, R0, c[0].yxxy;
MOV R2.x, c[0].z;
RCP R1.y, c[6].y;
RCP R1.x, c[6].x;
MUL R1.xy, R1.zwzw, R1;
ADD R1.w, R2.x, -c[7].x;
MOV R1.z, c[7].x;
MUL R1.zw, R1, c[0].w;
MUL R2.zw, R1, c[8].x;
RCP R2.x, R2.y;
MUL R2.xy, R1, R2.x;
MUL R3.xy, R2.zwzw, c[5];
MAD R1, R3.xyxy, c[10].xxzz, R2.xyxy;
MOV result.texcoord[1].xy, R1;
MOV result.texcoord[2].zw, R1;
MAD R1, R3.xyxy, c[12].xxzz, R2.xyxy;
MAD result.texcoord[0], R3.xyxy, c[9].zzxx, R2.xyxy;
MAD result.texcoord[1].zw, R3.xyxy, c[9].w, R2.xyxy;
MAD result.texcoord[2].xy, R3, c[10].w, R2;
MOV result.texcoord[3].zw, R2.xyxy;
MAD result.texcoord[3].xy, -R3, c[11].x, R2;
MAD result.texcoord[4], R3.xyxy, c[11].xxyy, R2.xyxy;
MAD result.texcoord[5].zw, R3.xyxy, c[11].w, R2.xyxy;
MOV result.texcoord[5].xy, R1;
MOV result.texcoord[6].zw, R1;
MAD result.texcoord[6].xy, R3, c[12].w, R2;
MAD result.texcoord[7].xy, R2.zwzw, c[5], R2;
MOV result.position, R0;
END
# 34 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex2D_TexelSize]
Vector 5 [_Target_TexelSize]
Float 6 [_Horizontal]
Float 7 [_Width]
"vs_2_0
; 37 ALU
def c8, 0.50000000, 0.00000000, -0.50000000, 1.00000000
def c9, 4.00000000, -0.85714293, -1.00000000, -0.57142860
def c10, -0.71428573, -0.28571430, -0.42857146, 0.14285715
def c11, 0.28571430, 0.57142860, 0.42857146, 0.85714293
def c12, 0.71428573, 0, 0, 0
dcl_position0 v0
dp4 r2.z, v0, c3
mov r0.w, c5.y
mov r1.w, r2.z
dp4 r1.z, v0, c2
dp4 r1.y, v0, c1
dp4 r1.x, v0, c0
mov r0.xyz, c8.yzyw
mad r0.w, r0, c8.x, c8.x
dp4 r2.y, r1, r0
mov r0.w, c5.x
mov r0.xyz, c8.xyyw
mad r0.w, r0, c8.x, c8.x
dp4 r2.x, r1, r0
rcp r0.z, r2.z
mul r2.xy, r2, r0.z
mov r2.w, c6.x
add r0.y, c8.w, -r2.w
mov r0.x, c6
mul r0.xy, r0, c9.x
mul r0.xy, r0, c7.x
mul r0.zw, r0.xyxy, c4.xyxy
mad oT0.zw, r0, c9.y, r2.xyxy
mad oT0.xy, r0.zwzw, c9.z, r2
mad oT1.zw, r0, c9.w, r2.xyxy
mad oT1.xy, r0.zwzw, c10.x, r2
mad oT2.zw, r0, c10.y, r2.xyxy
mad oT2.xy, r0.zwzw, c10.z, r2
mov oT3.zw, r2.xyxy
mad oT3.xy, -r0.zwzw, c10.w, r2
mad oT4.zw, r0, c11.x, r2.xyxy
mad oT4.xy, r0.zwzw, c10.w, r2
mad oT5.zw, r0, c11.y, r2.xyxy
mad oT5.xy, r0.zwzw, c11.z, r2
mad oT6.zw, r0, c11.w, r2.xyxy
mad oT6.xy, r0.zwzw, c12.x, r2
mad oT7.xy, r0, c4, r2
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
SetTexture 0 [_MainTex2D] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 35 ALU, 12 TEX
PARAM c[4] = { program.local[0..3] };
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
TEX R1, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R0, fragment.texcoord[1], texture[0], 2D;
TEX R2, fragment.texcoord[2], texture[0], 2D;
TEX R11, fragment.texcoord[6].zwzw, texture[0], 2D;
TEX R10, fragment.texcoord[6], texture[0], 2D;
TEX R9, fragment.texcoord[5].zwzw, texture[0], 2D;
TEX R8, fragment.texcoord[5], texture[0], 2D;
TEX R7, fragment.texcoord[4].zwzw, texture[0], 2D;
TEX R6, fragment.texcoord[4], texture[0], 2D;
TEX R5, fragment.texcoord[3].zwzw, texture[0], 2D;
TEX R4, fragment.texcoord[3], texture[0], 2D;
TEX R3, fragment.texcoord[2].zwzw, texture[0], 2D;
MUL R1, R1, c[0].w;
MUL R0, R0, c[0].z;
ADD R0, R0, R1;
MUL R1, R2, c[1].x;
ADD R0, R0, R1;
MUL R2, R3, c[1].y;
ADD R0, R0, R2;
MUL R1, R4, c[1].z;
ADD R0, R0, R1;
MUL R2, R5, c[1].w;
ADD R0, R0, R2;
MUL R1, R6, c[2].x;
ADD R0, R0, R1;
MUL R2, R7, c[2].y;
ADD R0, R0, R2;
MUL R1, R8, c[2].z;
ADD R0, R0, R1;
MUL R2, R9, c[2].w;
ADD R0, R0, R2;
MUL R1, R10, c[3].x;
MUL R2, R11, c[3].y;
ADD R0, R0, R1;
ADD result.color, R0, R2;
END
# 35 instructions, 12 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_Weights0]
Vector 1 [_Weights1]
Vector 2 [_Weights2]
Vector 3 [_Weights3]
SetTexture 0 [_MainTex2D] 2D
"ps_2_0
; 41 ALU, 12 TEX
dcl_2d s0
dcl t1
dcl t2
dcl t3
dcl t4
dcl t5
dcl t6
texld r3, t5, s0
texld r5, t4, s0
texld r7, t3, s0
texld r9, t2, s0
texld r11, t1, s0
mov r0.y, t1.w
mov r0.x, t1.z
mov r10.xy, r0
mov r0.y, t2.w
mov r0.x, t2.z
mov r8.xy, r0
mov r0.y, t3.w
mov r0.x, t3.z
mov r6.xy, r0
mov r0.y, t4.w
mov r0.x, t4.z
mov r4.xy, r0
mov r1.y, t5.w
mov r1.x, t5.z
mov r2.xy, r1
mov r0.y, t6.w
mov r0.x, t6.z
mul r11, r11, c0.z
mul r9, r9, c1.x
mul r7, r7, c1.z
mul r5, r5, c2.x
mul r3, r3, c2.z
texld r0, r0, s0
texld r1, t6, s0
texld r2, r2, s0
texld r4, r4, s0
texld r6, r6, s0
texld r8, r8, s0
texld r10, r10, s0
mul r10, r10, c0.w
add_pp r10, r11, r10
mul r8, r8, c1.y
add_pp r9, r10, r9
add_pp r8, r9, r8
mul r6, r6, c1.w
add_pp r7, r8, r7
add_pp r6, r7, r6
mul r4, r4, c2.y
add_pp r5, r6, r5
add_pp r4, r5, r4
mul r2, r2, c2.w
add_pp r3, r4, r3
add_pp r2, r3, r2
mul r1, r1, c3.x
mul r0, r0, c3.y
add_pp r1, r2, r1
add_pp r0, r1, r0
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}