//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Marvel/Characters/Human Torch" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _NoiseScale ("Noise Scale", Vector) = (1,1,1,1)
 _NoiseSpeed ("Noise Speed", Vector) = (1,1,1,1)
 _NoiseBias ("Noise Bias", Float) = 1.3
 _NoiseContrast ("Noise Contrast", Float) = 0.05
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
Program "vp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceCameraPos]
Vector 16 [unity_SHAr]
Vector 17 [unity_SHAg]
Vector 18 [unity_SHAb]
Vector 19 [unity_SHBr]
Vector 20 [unity_SHBg]
Vector 21 [unity_SHBb]
Vector 22 [unity_SHC]
Vector 23 [_NoiseScale]
Vector 24 [_NoiseSpeed]
Vector 25 [_MainTex_ST]
"!!ARBvp1.0
# 86 ALU
PARAM c[30] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..25],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MOV R0.xyz, c[24];
MUL R0.xyz, R0, c[13].yyww;
MUL R1.xyz, vertex.position.xzyw, c[23];
MAD R0.x, R0, c[0], R1;
MUL R0.x, R0, c[26].w;
ADD R0.x, R0, -c[27];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[26];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[27].yzyw;
MAD R3.xyz, R3, R2, c[28].xyxw;
MAD R3.xyz, R3, R2, c[28].zwzw;
MAD R3.xyz, R3, R2, c[29].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
SLT R3.x, R0, c[27];
SGE R3.yz, R0.x, c[29].xzww;
MUL R0.xw, R0.zyzy, c[0].xyzy;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.y, R2, -R3;
MAD R0.x, R1.z, R0.y, R0;
MUL R0.x, R0, c[26].w;
FRC R1.w, R0.x;
ADD R0.xyz, -R1.w, c[26];
MUL R0.xyz, R0, R0;
ADD R0.w, R1.y, R0;
MUL R2.xyz, R0, c[0].zwzw;
ADD R1.xyz, R2, c[27].yzyw;
MAD R1.xyz, R1, R0, c[28].xyxw;
MAD R1.xyz, R1, R0, c[28].zwzw;
MAD R1.xyz, R1, R0, c[29].xyxw;
MAD R1.xyz, R1, R0, c[0].yxyw;
MUL R0.w, R0, c[26];
FRC R0.w, R0;
ADD R2.xyz, -R0.w, c[26];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[27].yzyw;
MAD R3.xyz, R3, R2, c[28].xyxw;
MAD R3.xyz, R3, R2, c[28].zwzw;
MAD R3.xyz, R3, R2, c[29].xyxw;
SGE R0.yz, R1.w, c[29].xzww;
SLT R0.x, R1.w, c[27];
DP3 R0.y, R0, c[0].yxyw;
DP3 R1.w, R1, -R0;
MAD R1.xyz, R3, R2, c[0].yxyw;
SGE R0.yz, R0.w, c[29].xzww;
SLT R0.x, R0.w, c[27];
MOV R2.xz, R0;
DP3 R2.y, R0, c[0].yxyw;
DP3 R0.x, R1, -R2;
MUL R1.xyz, vertex.normal, c[14].w;
DP3 R3.w, R1, c[6];
DP3 R2.w, R1, c[7];
ADD result.texcoord[3].x, R1.w, R0;
DP3 R0.x, R1, c[5];
MOV R0.y, R3.w;
MOV R0.z, R2.w;
MUL R1, R0.xyzz, R0.yzzx;
MOV R0.w, c[0].x;
DP4 R3.z, R1, c[21];
DP4 R3.y, R1, c[20];
DP4 R3.x, R1, c[19];
MOV R1.w, c[0].x;
DP4 R2.z, R0, c[18];
DP4 R2.y, R0, c[17];
DP4 R2.x, R0, c[16];
MUL R0.y, R3.w, R3.w;
MAD R0.y, R0.x, R0.x, -R0;
ADD R2.xyz, R2, R3;
MUL R1.xyz, R0.y, c[22];
ADD result.texcoord[5].xyz, R2, R1;
MOV R1.xyz, c[15];
DP4 R2.z, R1, c[11];
DP4 R2.x, R1, c[9];
DP4 R2.y, R1, c[10];
MAD result.texcoord[1].xyz, R2, c[14].w, -vertex.position;
MOV result.texcoord[2].xyz, vertex.normal;
MOV result.texcoord[4].z, R2.w;
MOV result.texcoord[4].y, R3.w;
MOV result.texcoord[4].x, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[25], c[25].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 86 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [unity_SHAr]
Vector 16 [unity_SHAg]
Vector 17 [unity_SHAb]
Vector 18 [unity_SHBr]
Vector 19 [unity_SHBg]
Vector 20 [unity_SHBb]
Vector 21 [unity_SHC]
Vector 22 [_NoiseScale]
Vector 23 [_NoiseSpeed]
Vector 24 [_MainTex_ST]
"vs_2_0
; 81 ALU
def c25, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c26, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c27, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c28, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c13.w
dp3 r3.w, r1, c5
dp3 r1.w, r1, c6
dp3 r0.w, r1, c4
mov r0.x, r3.w
mov r0.y, r1.w
mov r0.z, c25
mul r2, r0.wxyy, r0.xyyw
dp4 r1.z, r0.wxyz, c17
dp4 r1.y, r0.wxyz, c16
dp4 r1.x, r0.wxyz, c15
dp4 r0.z, r2, c20
dp4 r0.x, r2, c18
dp4 r0.y, r2, c19
add r2.xyz, r1, r0
mov r0.xy, c12.ywzw
mul r0.xyz, c23, r0.xxyw
mul r1.xyz, v0.xzyw, c22
mad r0.x, r0, c25.z, r1
mul r2.w, r3, r3
mad r1.x, r0.w, r0.w, -r2.w
mul r3.xyz, r1.x, c21
add oT5.xyz, r2, r3
mad r0.x, r0, c27.z, c27.w
frc r0.x, r0
mov r2.w, c25.z
mov r2.xyz, c14
mad r0.x, r0, c28, c28.y
dp4 r3.z, r2, c10
dp4 r3.x, r2, c8
dp4 r3.y, r2, c9
sincos r2.xy, r0.x, c26.xyzw, c25.xyzw
mul r0.xy, r0.yzzw, c27
add r0.z, r1.y, r0.x
mad r0.x, r1.z, r2.y, r0.y
mad r0.y, r0.z, c27.z, c27.w
mad r0.x, r0, c27.z, c27.w
frc r0.y, r0
frc r0.x, r0
mad r0.y, r0, c28.x, c28
mad r0.x, r0, c28, c28.y
sincos r2.xy, r0.y, c26.xyzw, c25.xyzw
sincos r4.xy, r0.x, c26.xyzw, c25.xyzw
mad oT1.xyz, r3, c13.w, -v0
add oT3.x, r4, r2
mov oT2.xyz, v1
mov oT4.z, r1.w
mov oT4.y, r3.w
mov oT4.x, r0.w
mad oT0.xy, v2, c24, c24.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceCameraPos]
Vector 16 [_NoiseScale]
Vector 17 [_NoiseSpeed]
Vector 18 [unity_LightmapST]
Vector 19 [_MainTex_ST]
"!!ARBvp1.0
# 65 ALU
PARAM c[24] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..19],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[17];
MUL R0.xyz, vertex.position.xzyw, c[16];
MUL R1.xyz, R1, c[13].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[20].w;
ADD R0.x, R0, -c[21];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[20];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[21].yzyw;
MAD R3.xyz, R3, R2, c[22].xyxw;
MAD R3.xyz, R3, R2, c[22].zwzw;
MAD R3.xyz, R3, R2, c[23].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[20];
FRC R0.w, R0;
SLT R3.x, R0, c[21];
SGE R3.yz, R0.x, c[23].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[20].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[20];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[21].yzyw;
MAD R1.xyz, R0, R2, c[22].xyxw;
MAD R1.xyz, R1, R2, c[22].zwzw;
MAD R3.xyz, R1, R2, c[23].xyxw;
ADD R0.xyz, -R0.w, c[20];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[21].yzyw;
MAD R3.xyz, R3, R0, c[22].xyxw;
MAD R3.xyz, R3, R0, c[22].zwzw;
MAD R3.xyz, R3, R0, c[23].xyxw;
SGE R1.yz, R1.w, c[23].xzww;
SLT R1.x, R1.w, c[21];
MAD R3.xyz, R3, R0, c[0].yxyw;
MOV R1.w, c[0].x;
DP3 R1.y, R1, c[0].yxyw;
SLT R4.x, R0.w, c[21];
SGE R4.yz, R0.w, c[23].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[15];
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[14].w, -vertex.position;
MOV result.texcoord[2].xyz, vertex.normal;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[19], c[19].zwzw;
MAD result.texcoord[5].xy, vertex.texcoord[1], c[18], c[18].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 65 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_NoiseScale]
Vector 16 [_NoiseSpeed]
Vector 17 [unity_LightmapST]
Vector 18 [_MainTex_ST]
"vs_2_0
; 60 ALU
def c19, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c20, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c21, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c22, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mov r0.xy, c12.ywzw
mul r1.xyz, v0.xzyw, c15
mul r2.xyz, c16, r0.xxyw
mad r0.x, r2, c19.z, r1
mad r0.x, r0, c21.z, c21.w
frc r0.x, r0
mad r1.x, r0, c22, c22.y
mov r0.w, c19.z
mov r0.xyz, c14
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
mul r0.zw, r2.xyyz, c21.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c21.z, c21.w
frc r0.y, r0
mad r0.x, r0, c21.z, c21.w
frc r0.x, r0
mad r1.x, r0.y, c22, c22.y
mad r2.x, r0, c22, c22.y
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
sincos r1.xy, r2.x, c20.xyzw, c19.xyzw
mad oT1.xyz, r3, c13.w, -v0
add oT3.x, r1, r0
mov oT2.xyz, v1
mad oT0.xy, v2, c18, c18.zwzw
mad oT5.xy, v3, c17, c17.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [_ProjectionParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [unity_SHAr]
Vector 18 [unity_SHAg]
Vector 19 [unity_SHAb]
Vector 20 [unity_SHBr]
Vector 21 [unity_SHBg]
Vector 22 [unity_SHBb]
Vector 23 [unity_SHC]
Vector 24 [_NoiseScale]
Vector 25 [_NoiseSpeed]
Vector 26 [_MainTex_ST]
"!!ARBvp1.0
# 92 ALU
PARAM c[31] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..26],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.xyz, c[25];
MUL R0.xyz, R0, c[13].yyww;
MUL R1.xyz, vertex.position.xzyw, c[24];
MUL R4.xy, R0.yzzw, c[0].yxzw;
MAD R0.x, R0, c[0], R1;
ADD R0.w, R1.y, R4.x;
MUL R0.x, R0, c[27].w;
ADD R0.x, R0, -c[28];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[27];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[28].yzyw;
MAD R3.xyz, R3, R2, c[29].xyxw;
MAD R3.xyz, R3, R2, c[29].zwzw;
MAD R3.xyz, R3, R2, c[30].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[27];
SLT R3.x, R0, c[28];
SGE R3.yz, R0.x, c[30].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R1.z, R0, R4.y;
MUL R0.x, R0, c[27].w;
FRC R1.w, R0.x;
ADD R0.xyz, -R1.w, c[27];
MUL R0.xyz, R0, R0;
MUL R2.xyz, R0, c[0].zwzw;
ADD R1.xyz, R2, c[28].yzyw;
MAD R1.xyz, R1, R0, c[29].xyxw;
FRC R0.w, R0;
MAD R1.xyz, R1, R0, c[29].zwzw;
MAD R1.xyz, R1, R0, c[30].xyxw;
MAD R1.xyz, R1, R0, c[0].yxyw;
ADD R2.xyz, -R0.w, c[27];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[28].yzyw;
MAD R3.xyz, R3, R2, c[29].xyxw;
MAD R3.xyz, R3, R2, c[29].zwzw;
MAD R3.xyz, R3, R2, c[30].xyxw;
SGE R0.yz, R1.w, c[30].xzww;
SLT R0.x, R1.w, c[28];
DP3 R0.y, R0, c[0].yxyw;
DP3 R1.w, R1, -R0;
MAD R1.xyz, R3, R2, c[0].yxyw;
SLT R0.x, R0.w, c[28];
SGE R0.yz, R0.w, c[30].xzww;
MOV R2.xz, R0;
DP3 R2.y, R0, c[0].yxyw;
DP3 R0.x, R1, -R2;
MUL R1.xyz, vertex.normal, c[15].w;
DP3 R4.x, R1, c[6];
DP3 R3.w, R1, c[7];
DP3 R2.w, R1, c[5];
ADD result.texcoord[3].x, R1.w, R0;
MUL R1.w, R4.x, R4.x;
MOV R2.x, R4;
MOV R2.y, R3.w;
MOV R2.z, c[0].x;
MUL R0, R2.wxyy, R2.xyyw;
DP4 R1.z, R2.wxyz, c[19];
DP4 R1.y, R2.wxyz, c[18];
DP4 R1.x, R2.wxyz, c[17];
DP4 R2.z, R0, c[22];
DP4 R2.y, R0, c[21];
DP4 R2.x, R0, c[20];
MAD R1.w, R2, R2, -R1;
MUL R0.xyz, R1.w, c[23];
ADD R1.xyz, R1, R2;
ADD result.texcoord[5].xyz, R1, R0;
MOV R1.xyz, c[16];
MOV R1.w, c[0].x;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R3.z, R1, c[11];
DP4 R3.x, R1, c[9];
DP4 R3.y, R1, c[10];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R2.xyz, R0.xyww, c[27].y;
MOV R1.x, R2;
MUL R1.y, R2, c[14].x;
MAD result.texcoord[1].xyz, R3, c[15].w, -vertex.position;
ADD result.texcoord[6].xy, R1, R2.z;
MOV result.position, R0;
MOV result.texcoord[6].zw, R0;
MOV result.texcoord[2].xyz, vertex.normal;
MOV result.texcoord[4].z, R3.w;
MOV result.texcoord[4].y, R4.x;
MOV result.texcoord[4].x, R2.w;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[26], c[26].zwzw;
END
# 92 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [_ProjectionParams]
Vector 14 [_ScreenParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [unity_SHAr]
Vector 18 [unity_SHAg]
Vector 19 [unity_SHAb]
Vector 20 [unity_SHBr]
Vector 21 [unity_SHBg]
Vector 22 [unity_SHBb]
Vector 23 [unity_SHC]
Vector 24 [_NoiseScale]
Vector 25 [_NoiseSpeed]
Vector 26 [_MainTex_ST]
"vs_2_0
; 86 ALU
def c27, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c28, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c29, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c30, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r0.xyz, v1, c15.w
dp3 r4.x, r0, c5
dp3 r4.y, r0, c6
dp3 r3.x, r0, c4
mov r3.y, r4.x
mov r3.z, r4.y
mul r0, r3.xyzz, r3.yzzx
mov r3.w, c27.z
dp4 r2.z, r0, c22
dp4 r2.y, r0, c21
dp4 r2.x, r0, c20
mul r1.w, r4.x, r4.x
mad r0.x, r3, r3, -r1.w
dp4 r1.z, r3, c19
dp4 r1.y, r3, c18
dp4 r1.x, r3, c17
add r1.xyz, r1, r2
mul r0.xyz, r0.x, c23
add oT5.xyz, r1, r0
mov r2.xy, c12.ywzw
mov r0.xyz, c16
mul r3.yzw, v0.xxzy, c24.xxyz
mul r2.xyz, c25, r2.xxyw
mad r0.w, r2.x, c27.z, r3.y
mad r1.w, r0, c29.z, c29
mov r0.w, c27.z
dp4 r1.z, r0, c10
dp4 r1.x, r0, c8
dp4 r1.y, r0, c9
mad oT1.xyz, r1, c15.w, -v0
frc r0.w, r1
dp4 r1.w, v0, c3
mad r1.z, r0.w, c30.x, c30.y
dp4 r1.x, v0, c0
dp4 r1.y, v0, c1
mul r0.xyz, r1.xyww, c27.w
mul r0.y, r0, c13.x
mad oT6.xy, r0.z, c14.zwzw, r0
sincos r0.xy, r1.z, c28.xyzw, c27.xyzw
mul r0.zw, r2.xyyz, c29.xyxy
mad r0.x, r3.w, r0.y, r0.w
add r0.y, r3.z, r0.z
mad r0.x, r0, c29.z, c29.w
mad r0.y, r0, c29.z, c29.w
frc r0.y, r0
frc r0.x, r0
mad r0.x, r0, c30, c30.y
sincos r2.xy, r0.x, c28.xyzw, c27.xyzw
mad r1.z, r0.y, c30.x, c30.y
sincos r0.xy, r1.z, c28.xyzw, c27.xyzw
dp4 r1.z, v0, c2
add oT3.x, r2, r0
mov oPos, r1
mov oT6.zw, r1
mov oT2.xyz, v1
mov oT4.z, r4.y
mov oT4.y, r4.x
mov oT4.x, r3
mad oT0.xy, v2, c26, c26.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [_ProjectionParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [_NoiseScale]
Vector 18 [_NoiseSpeed]
Vector 19 [unity_LightmapST]
Vector 20 [_MainTex_ST]
"!!ARBvp1.0
# 71 ALU
PARAM c[25] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..20],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[18];
MUL R0.xyz, vertex.position.xzyw, c[17];
MUL R1.xyz, R1, c[13].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[21].w;
ADD R0.x, R0, -c[22];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[21];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[22].yzyw;
MAD R3.xyz, R3, R2, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[21];
FRC R0.w, R0;
SLT R3.x, R0, c[22];
SGE R3.yz, R0.x, c[24].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[21].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[21];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[22].yzyw;
MAD R1.xyz, R0, R2, c[23].xyxw;
MAD R1.xyz, R1, R2, c[23].zwzw;
MAD R3.xyz, R1, R2, c[24].xyxw;
ADD R0.xyz, -R0.w, c[21];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[22].yzyw;
MAD R3.xyz, R3, R0, c[23].xyxw;
MAD R3.xyz, R3, R0, c[23].zwzw;
MAD R3.xyz, R3, R0, c[24].xyxw;
MAD R3.xyz, R3, R0, c[0].yxyw;
SGE R1.yz, R1.w, c[24].xzww;
SLT R1.x, R1.w, c[22];
SLT R4.x, R0.w, c[22];
SGE R4.yz, R0.w, c[24].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
ADD result.texcoord[3].x, R0, R0.y;
MOV R1.xyz, c[16];
MOV R1.w, c[0].x;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R3.z, R1, c[11];
DP4 R3.x, R1, c[9];
DP4 R3.y, R1, c[10];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R2.xyz, R0.xyww, c[21].y;
MOV R1.x, R2;
MUL R1.y, R2, c[14].x;
MAD result.texcoord[1].xyz, R3, c[15].w, -vertex.position;
ADD result.texcoord[6].xy, R1, R2.z;
MOV result.position, R0;
MOV result.texcoord[6].zw, R0;
MOV result.texcoord[2].xyz, vertex.normal;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[20], c[20].zwzw;
MAD result.texcoord[5].xy, vertex.texcoord[1], c[19], c[19].zwzw;
END
# 71 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [_ProjectionParams]
Vector 14 [_ScreenParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [_NoiseScale]
Vector 18 [_NoiseSpeed]
Vector 19 [unity_LightmapST]
Vector 20 [_MainTex_ST]
"vs_2_0
; 65 ALU
def c21, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c24, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mov r0.xy, c12.ywzw
mul r2.xyz, v0.xzyw, c17
mul r3.xyz, c18, r0.xxyw
mad r0.x, r3, c21.z, r2
mad r1.w, r0.x, c23.z, c23
mov r0.w, c21.z
mov r0.xyz, c16
dp4 r1.z, r0, c10
dp4 r1.x, r0, c8
dp4 r1.y, r0, c9
mad oT1.xyz, r1, c15.w, -v0
frc r0.w, r1
dp4 r1.w, v0, c3
mad r1.z, r0.w, c24.x, c24.y
dp4 r1.x, v0, c0
dp4 r1.y, v0, c1
mul r0.xyz, r1.xyww, c21.w
mul r0.y, r0, c13.x
mad oT6.xy, r0.z, c14.zwzw, r0
sincos r0.xy, r1.z, c22.xyzw, c21.xyzw
mul r0.zw, r3.xyyz, c23.xyxy
mad r0.x, r2.z, r0.y, r0.w
add r0.y, r2, r0.z
mad r0.x, r0, c23.z, c23.w
mad r0.y, r0, c23.z, c23.w
frc r0.y, r0
frc r0.x, r0
mad r0.x, r0, c24, c24.y
sincos r2.xy, r0.x, c22.xyzw, c21.xyzw
mad r1.z, r0.y, c24.x, c24.y
sincos r0.xy, r1.z, c22.xyzw, c21.xyzw
dp4 r1.z, v0, c2
add oT3.x, r2, r0
mov oPos, r1
mov oT6.zw, r1
mov oT2.xyz, v1
mad oT0.xy, v2, c20, c20.zwzw
mad oT5.xy, v3, c19, c19.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceCameraPos]
Vector 16 [unity_4LightPosX0]
Vector 17 [unity_4LightPosY0]
Vector 18 [unity_4LightPosZ0]
Vector 19 [unity_4LightAtten0]
Vector 20 [unity_LightColor0]
Vector 21 [unity_LightColor1]
Vector 22 [unity_LightColor2]
Vector 23 [unity_LightColor3]
Vector 24 [unity_SHAr]
Vector 25 [unity_SHAg]
Vector 26 [unity_SHAb]
Vector 27 [unity_SHBr]
Vector 28 [unity_SHBg]
Vector 29 [unity_SHBb]
Vector 30 [unity_SHC]
Vector 31 [_NoiseScale]
Vector 32 [_NoiseSpeed]
Vector 33 [_MainTex_ST]
"!!ARBvp1.0
# 113 ALU
PARAM c[38] = { { 1, -1, 0.15915491, 0.25 },
		state.matrix.mvp,
		program.local[5..33],
		{ 0, 0.5, 1 },
		{ 24.980801, -24.980801, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
MUL R1.xyz, vertex.normal, c[14].w;
DP3 R1.w, R1, c[6];
DP4 R0.x, vertex.position, c[6];
ADD R4, -R0.x, c[17];
DP4 R0.x, vertex.position, c[5];
ADD R2, -R0.x, c[16];
DP3 R0.x, R1, c[5];
MUL R3, R1.w, R4;
DP4 R0.y, vertex.position, c[7];
MOV R0.w, c[0].x;
MAD R3, R0.x, R2, R3;
MUL R4, R4, R4;
MAD R2, R2, R2, R4;
ADD R5, -R0.y, c[18];
MAD R4, R5, R5, R2;
DP3 R2.w, R1, c[7];
MAD R5, R2.w, R5, R3;
MUL R3, R4, c[19];
MOV R1.xyz, c[32];
MUL R1.xyz, R1, c[13].yyww;
MUL R2.xyz, vertex.position.xzyw, c[31];
MAD R0.y, R1.x, c[0].x, R2.x;
MAD R0.y, R0, c[0].z, -c[0].w;
FRC R1.x, R0.y;
MOV R0.y, R1.w;
MOV R0.z, R2.w;
DP4 R7.z, R0, c[26];
DP4 R7.y, R0, c[25];
DP4 R7.x, R0, c[24];
RSQ R4.x, R4.x;
RSQ R4.y, R4.y;
RSQ R4.w, R4.w;
RSQ R4.z, R4.z;
MUL R5, R5, R4;
ADD R4, R3, c[0].x;
MAX R3, R5, c[34].x;
ADD R5.xyz, -R1.x, c[34];
RCP R4.x, R4.x;
RCP R4.y, R4.y;
RCP R4.z, R4.z;
RCP R4.w, R4.w;
MUL R3, R3, R4;
MUL R4.xyz, R5, R5;
MUL R6.xyz, R3.y, c[21];
MAD R6.xyz, R3.x, c[20], R6;
MAD R3.xyz, R3.z, c[22], R6;
MAD R3.xyz, R3.w, c[23], R3;
MAD R5.xyz, R4, c[35].xyxw, c[35].zwzw;
MAD R5.xyz, R5, R4, c[36].xyxw;
MAD R5.xyz, R5, R4, c[36].zwzw;
MAD R6.xyz, R5, R4, c[37].xyxw;
MUL R5, R0.xyzz, R0.yzzx;
MAD R4.xyz, R6, R4, c[0].yxyw;
DP4 R0.y, R5, c[27];
DP4 R0.z, R5, c[28];
DP4 R0.w, R5, c[29];
ADD R5.xyz, R7, R0.yzww;
MUL R0.y, R1.w, R1.w;
SLT R6.x, R1, c[0].w;
SGE R6.yz, R1.x, c[37].xzww;
MOV R3.w, c[0].x;
MAD R0.y, R0.x, R0.x, -R0;
MOV R7.xz, R6;
DP3 R7.y, R6, c[0].yxyw;
DP3 R0.z, R4, -R7;
MUL R4.xy, R1.yzzw, c[0].yxzw;
MUL R1.xyz, R0.y, c[30];
MAD R0.z, R2, R0, R4.y;
ADD R1.xyz, R5, R1;
ADD R0.y, R2, R4.x;
MUL R0.z, R0, c[0];
FRC R0.z, R0;
MUL R0.y, R0, c[0].z;
ADD R5.xyz, -R0.z, c[34];
ADD result.texcoord[5].xyz, R1, R3;
MUL R1.xyz, R5, R5;
MAD R2.xyz, R1, c[35].xyxw, c[35].zwzw;
MAD R2.xyz, R2, R1, c[36].xyxw;
MAD R3.xyz, R2, R1, c[36].zwzw;
FRC R0.y, R0;
MAD R4.xyz, R3, R1, c[37].xyxw;
ADD R2.xyz, -R0.y, c[34];
MUL R3.xyz, R2, R2;
MAD R2.xyz, R4, R1, c[0].yxyw;
MAD R4.xyz, R3, c[35].xyxw, c[35].zwzw;
MAD R4.xyz, R4, R3, c[36].xyxw;
MAD R4.xyz, R4, R3, c[36].zwzw;
MAD R4.xyz, R4, R3, c[37].xyxw;
MAD R3.xyz, R4, R3, c[0].yxyw;
SGE R1.yz, R0.z, c[37].xzww;
SLT R1.x, R0.z, c[0].w;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.z, R2, -R1;
SLT R4.x, R0.y, c[0].w;
SGE R4.yz, R0.y, c[37].xzww;
MOV R5.xz, R4;
DP3 R5.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R5;
MOV R3.xyz, c[15];
DP4 R1.z, R3, c[11];
DP4 R1.x, R3, c[9];
DP4 R1.y, R3, c[10];
ADD result.texcoord[3].x, R0.z, R0.y;
MAD result.texcoord[1].xyz, R1, c[14].w, -vertex.position;
MOV result.texcoord[2].xyz, vertex.normal;
MOV result.texcoord[4].z, R2.w;
MOV result.texcoord[4].y, R1.w;
MOV result.texcoord[4].x, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[33], c[33].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 113 instructions, 8 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [unity_4LightPosX0]
Vector 16 [unity_4LightPosY0]
Vector 17 [unity_4LightPosZ0]
Vector 18 [unity_4LightAtten0]
Vector 19 [unity_LightColor0]
Vector 20 [unity_LightColor1]
Vector 21 [unity_LightColor2]
Vector 22 [unity_LightColor3]
Vector 23 [unity_SHAr]
Vector 24 [unity_SHAg]
Vector 25 [unity_SHAb]
Vector 26 [unity_SHBr]
Vector 27 [unity_SHBg]
Vector 28 [unity_SHBb]
Vector 29 [unity_SHC]
Vector 30 [_NoiseScale]
Vector 31 [_NoiseSpeed]
Vector 32 [_MainTex_ST]
"vs_2_0
; 111 ALU
def c33, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c34, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c35, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c36, -1.00000000, 1.00000000, 0.00000000, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r3.xyz, v1, c13.w
dp3 r4.x, r3, c4
dp3 r3.w, r3, c5
dp3 r3.x, r3, c6
dp4 r0.x, v0, c5
add r1, -r0.x, c16
mul r2, r3.w, r1
dp4 r0.x, v0, c4
add r0, -r0.x, c15
mul r1, r1, r1
mov r4.z, r3.x
mov r4.w, c33.z
mad r2, r4.x, r0, r2
dp4 r4.y, v0, c6
mad r1, r0, r0, r1
add r0, -r4.y, c17
mad r1, r0, r0, r1
mad r0, r3.x, r0, r2
mul r2, r1, c18
mov r4.y, r3.w
rsq r1.x, r1.x
rsq r1.y, r1.y
rsq r1.w, r1.w
rsq r1.z, r1.z
mul r0, r0, r1
add r1, r2, c33.z
dp4 r2.z, r4, c25
dp4 r2.y, r4, c24
dp4 r2.x, r4, c23
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c36.z
mul r0, r0, r1
mul r1.xyz, r0.y, c20
mad r1.xyz, r0.x, c19, r1
mad r0.xyz, r0.z, c21, r1
mul r1, r4.xyzz, r4.yzzx
mad r0.xyz, r0.w, c22, r0
dp4 r5.z, r1, c28
dp4 r5.y, r1, c27
dp4 r5.x, r1, c26
add r4.yzw, r2.xxyz, r5.xxyz
mul r0.w, r3, r3
mad r1.w, r4.x, r4.x, -r0
mul r5.xyz, r1.w, c29
add r5.xyz, r4.yzww, r5
add oT5.xyz, r5, r0
mov r2.xy, c12.ywzw
mul r1.xyz, v0.xzyw, c30
mul r2.xyz, c31, r2.xxyw
mad r0.w, r2.x, c33.z, r1.x
mad r0.w, r0, c35.x, c35.y
frc r0.w, r0
mad r1.x, r0.w, c35.z, c35.w
mov r0.w, c33.z
mov r0.xyz, c14
dp4 r5.z, r0, c10
dp4 r5.x, r0, c8
dp4 r5.y, r0, c9
sincos r0.xy, r1.x, c34.xyzw, c33.xyzw
mul r0.zw, r2.xyyz, c36.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c35.x, c35
frc r0.y, r0
mad r0.x, r0, c35, c35.y
frc r0.x, r0
mad r1.x, r0.y, c35.z, c35.w
mad r2.x, r0, c35.z, c35.w
sincos r0.xy, r1.x, c34.xyzw, c33.xyzw
sincos r1.xy, r2.x, c34.xyzw, c33.xyzw
mad oT1.xyz, r5, c13.w, -v0
add oT3.x, r1, r0
mov oT2.xyz, v1
mov oT4.z, r3.x
mov oT4.y, r3.w
mov oT4.x, r4
mad oT0.xy, v2, c32, c32.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [_ProjectionParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [unity_4LightPosX0]
Vector 18 [unity_4LightPosY0]
Vector 19 [unity_4LightPosZ0]
Vector 20 [unity_4LightAtten0]
Vector 21 [unity_LightColor0]
Vector 22 [unity_LightColor1]
Vector 23 [unity_LightColor2]
Vector 24 [unity_LightColor3]
Vector 25 [unity_SHAr]
Vector 26 [unity_SHAg]
Vector 27 [unity_SHAb]
Vector 28 [unity_SHBr]
Vector 29 [unity_SHBg]
Vector 30 [unity_SHBb]
Vector 31 [unity_SHC]
Vector 32 [_NoiseScale]
Vector 33 [_NoiseSpeed]
Vector 34 [_MainTex_ST]
"!!ARBvp1.0
# 119 ALU
PARAM c[39] = { { 1, -1, 0.15915491, 0.25 },
		state.matrix.mvp,
		program.local[5..34],
		{ 0, 0.5, 1 },
		{ 24.980801, -24.980801, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
MUL R1.xyz, vertex.normal, c[15].w;
DP3 R1.w, R1, c[6];
DP4 R0.x, vertex.position, c[6];
ADD R4, -R0.x, c[18];
DP4 R0.x, vertex.position, c[5];
ADD R2, -R0.x, c[17];
DP3 R0.x, R1, c[5];
MUL R3, R1.w, R4;
DP4 R0.y, vertex.position, c[7];
MOV R0.w, c[0].x;
MAD R3, R0.x, R2, R3;
MUL R4, R4, R4;
MAD R2, R2, R2, R4;
ADD R5, -R0.y, c[19];
MAD R4, R5, R5, R2;
DP3 R2.w, R1, c[7];
MAD R5, R2.w, R5, R3;
MUL R3, R4, c[20];
MOV R1.xyz, c[33];
MUL R1.xyz, R1, c[13].yyww;
MUL R2.xyz, vertex.position.xzyw, c[32];
MAD R0.y, R1.x, c[0].x, R2.x;
MAD R0.y, R0, c[0].z, -c[0].w;
FRC R1.x, R0.y;
MOV R0.y, R1.w;
MOV R0.z, R2.w;
DP4 R7.z, R0, c[27];
DP4 R7.y, R0, c[26];
DP4 R7.x, R0, c[25];
RSQ R4.x, R4.x;
RSQ R4.y, R4.y;
RSQ R4.w, R4.w;
RSQ R4.z, R4.z;
MUL R5, R5, R4;
ADD R4, R3, c[0].x;
MAX R3, R5, c[35].x;
ADD R5.xyz, -R1.x, c[35];
RCP R4.x, R4.x;
RCP R4.y, R4.y;
RCP R4.z, R4.z;
RCP R4.w, R4.w;
MUL R3, R3, R4;
MUL R4.xyz, R5, R5;
MUL R6.xyz, R3.y, c[22];
MAD R6.xyz, R3.x, c[21], R6;
MAD R3.xyz, R3.z, c[23], R6;
MAD R3.xyz, R3.w, c[24], R3;
MAD R5.xyz, R4, c[36].xyxw, c[36].zwzw;
MAD R5.xyz, R5, R4, c[37].xyxw;
MAD R5.xyz, R5, R4, c[37].zwzw;
MAD R6.xyz, R5, R4, c[38].xyxw;
MUL R5, R0.xyzz, R0.yzzx;
MAD R4.xyz, R6, R4, c[0].yxyw;
DP4 R0.w, R5, c[30];
DP4 R0.y, R5, c[28];
DP4 R0.z, R5, c[29];
ADD R5.xyz, R7, R0.yzww;
MUL R0.y, R1.w, R1.w;
SLT R6.x, R1, c[0].w;
SGE R6.yz, R1.x, c[38].xzww;
DP4 R3.w, vertex.position, c[4];
MOV R4.w, c[0].x;
MAD R0.y, R0.x, R0.x, -R0;
MOV R7.xz, R6;
DP3 R7.y, R6, c[0].yxyw;
DP3 R0.z, R4, -R7;
MUL R4.xy, R1.yzzw, c[0].yxzw;
MUL R1.xyz, R0.y, c[31];
MAD R0.z, R2, R0, R4.y;
ADD R1.xyz, R5, R1;
ADD R0.y, R2, R4.x;
MUL R0.z, R0, c[0];
FRC R0.z, R0;
MUL R0.y, R0, c[0].z;
ADD R5.xyz, -R0.z, c[35];
ADD result.texcoord[5].xyz, R1, R3;
MUL R1.xyz, R5, R5;
MAD R2.xyz, R1, c[36].xyxw, c[36].zwzw;
MAD R2.xyz, R2, R1, c[37].xyxw;
MAD R3.xyz, R2, R1, c[37].zwzw;
FRC R0.y, R0;
MAD R4.xyz, R3, R1, c[38].xyxw;
ADD R2.xyz, -R0.y, c[35];
MUL R3.xyz, R2, R2;
MAD R2.xyz, R4, R1, c[0].yxyw;
MAD R4.xyz, R3, c[36].xyxw, c[36].zwzw;
MAD R4.xyz, R4, R3, c[37].xyxw;
MAD R4.xyz, R4, R3, c[37].zwzw;
MAD R4.xyz, R4, R3, c[38].xyxw;
MAD R3.xyz, R4, R3, c[0].yxyw;
SGE R1.yz, R0.z, c[38].xzww;
SLT R1.x, R0.z, c[0].w;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.z, R2, -R1;
SLT R4.x, R0.y, c[0].w;
SGE R4.yz, R0.y, c[38].xzww;
MOV R5.xz, R4;
DP3 R5.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R5;
MOV R4.xyz, c[16];
DP4 R3.z, vertex.position, c[3];
DP4 R3.x, vertex.position, c[1];
DP4 R3.y, vertex.position, c[2];
MUL R1.xyz, R3.xyww, c[35].y;
ADD result.texcoord[3].x, R0.z, R0.y;
DP4 R2.z, R4, c[11];
DP4 R2.x, R4, c[9];
DP4 R2.y, R4, c[10];
MOV R0.z, R1.x;
MUL R0.w, R1.y, c[14].x;
MAD result.texcoord[1].xyz, R2, c[15].w, -vertex.position;
ADD result.texcoord[6].xy, R0.zwzw, R1.z;
MOV result.position, R3;
MOV result.texcoord[6].zw, R3;
MOV result.texcoord[2].xyz, vertex.normal;
MOV result.texcoord[4].z, R2.w;
MOV result.texcoord[4].y, R1.w;
MOV result.texcoord[4].x, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[34], c[34].zwzw;
END
# 119 instructions, 8 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [_ProjectionParams]
Vector 14 [_ScreenParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [unity_4LightPosX0]
Vector 18 [unity_4LightPosY0]
Vector 19 [unity_4LightPosZ0]
Vector 20 [unity_4LightAtten0]
Vector 21 [unity_LightColor0]
Vector 22 [unity_LightColor1]
Vector 23 [unity_LightColor2]
Vector 24 [unity_LightColor3]
Vector 25 [unity_SHAr]
Vector 26 [unity_SHAg]
Vector 27 [unity_SHAb]
Vector 28 [unity_SHBr]
Vector 29 [unity_SHBg]
Vector 30 [unity_SHBb]
Vector 31 [unity_SHC]
Vector 32 [_NoiseScale]
Vector 33 [_NoiseSpeed]
Vector 34 [_MainTex_ST]
"vs_2_0
; 116 ALU
def c35, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c36, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c37, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c38, -1.00000000, 1.00000000, 0.00000000, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r3.yzw, v1.xxyz, c15.w
dp3 r3.x, r3.yzww, c4
dp3 r4.y, r3.yzww, c5
dp4 r0.x, v0, c5
add r0, -r0.x, c18
mul r2, r4.y, r0
dp4 r1.x, v0, c4
add r1, -r1.x, c17
mad r2, r3.x, r1, r2
mul r0, r0, r0
mad r1, r1, r1, r0
dp4 r4.x, v0, c6
add r0, -r4.x, c19
dp3 r4.x, r3.yzww, c6
mad r1, r0, r0, r1
mad r2, r4.x, r0, r2
mul r0, r1, c20
add r0, r0, c35.z
mov r3.y, r4
mov r3.z, r4.x
mov r3.w, c35.z
rsq r1.x, r1.x
rsq r1.y, r1.y
rsq r1.w, r1.w
rsq r1.z, r1.z
mul r1, r2, r1
mul r2, r3.xyzz, r3.yzzx
max r1, r1, c38.z
rcp r0.x, r0.x
rcp r0.y, r0.y
rcp r0.w, r0.w
rcp r0.z, r0.z
mul r0, r1, r0
mul r1.xyz, r0.y, c22
mad r1.xyz, r0.x, c21, r1
mad r0.xyz, r0.z, c23, r1
mad r1.xyz, r0.w, c24, r0
mul r0.w, r4.y, r4.y
dp4 r0.z, r3, c27
dp4 r0.y, r3, c26
dp4 r0.x, r3, c25
dp4 r3.w, r2, c30
dp4 r3.z, r2, c29
dp4 r3.y, r2, c28
add r0.xyz, r0, r3.yzww
mad r0.w, r3.x, r3.x, -r0
mul r2.xyz, r0.w, c31
add r2.xyz, r0, r2
add oT5.xyz, r2, r1
mov r3.zw, c12.xyyw
mul r0.xyz, v0.xzyw, c32
mul r3.yzw, c33.xxyz, r3.xzzw
mad r0.x, r3.y, c35.z, r0
mad r0.x, r0, c37, c37.y
frc r0.x, r0
mov r2.w, c35.z
mov r2.xyz, c16
dp4 r1.z, r2, c10
dp4 r1.x, r2, c8
dp4 r1.y, r2, c9
mad oT1.xyz, r1, c15.w, -v0
dp4 r1.w, v0, c3
dp4 r1.x, v0, c0
dp4 r1.y, v0, c1
mul r2.xyz, r1.xyww, c35.w
mul r2.y, r2, c13.x
mad r0.x, r0, c37.z, c37.w
mad oT6.xy, r2.z, c14.zwzw, r2
sincos r2.xy, r0.x, c36.xyzw, c35.xyzw
mul r2.zw, r3, c38.xyxy
add r0.y, r0, r2.z
mad r0.x, r0.z, r2.y, r2.w
mad r0.x, r0, c37, c37.y
mad r0.y, r0, c37.x, c37
frc r0.y, r0
frc r0.x, r0
mad r0.x, r0, c37.z, c37.w
sincos r2.xy, r0.x, c36.xyzw, c35.xyzw
mad r1.z, r0.y, c37, c37.w
sincos r0.xy, r1.z, c36.xyzw, c35.xyzw
dp4 r1.z, v0, c2
add oT3.x, r2, r0
mov oPos, r1
mov oT6.zw, r1
mov oT2.xyz, v1
mov oT4.z, r4.x
mov oT4.y, r4
mov oT4.x, r3
mad oT0.xy, v2, c34, c34.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_NoiseBias]
Float 4 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 24 ALU, 1 TEX
PARAM c[6] = { program.local[0..4],
		{ 1, 2, 0 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
DP3 R1.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.y;
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3_SAT R1.x, R1, R2;
MUL R1.w, fragment.texcoord[3].x, c[4].x;
ADD R1.y, R1.w, c[5].x;
MUL R1.x, R1, c[3];
MAD R2.x, -R1, R1.y, c[5].y;
MUL R1, R0, c[2];
MUL R0, R1, R2.x;
MOV_SAT R0.w, R0;
MAD R1.xyz, R0.w, -R1, R1;
MUL R2.xyz, R1, fragment.texcoord[5];
DP3 R1.w, fragment.texcoord[4], c[0];
MUL R1.xyz, R1, c[1];
MAX R1.w, R1, c[5].z;
MUL R1.xyz, R1.w, R1;
MAD R1.xyz, R1, c[5].y, R2;
MAD result.color.xyz, R0.w, R0, R1;
MOV result.color.w, c[5].z;
END
# 24 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_NoiseBias]
Float 4 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 24 ALU, 1 TEX
dcl_2d s0
def c5, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
texld r1, t0, s0
dp3 r2.x, t1, t1
dp3 r0.x, t2, t2
rsq r2.x, r2.x
rsq r0.x, r0.x
mul r1, r1, c2
mul r2.xyz, r2.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r2
mul r2.x, t3, c4
add r2.x, r2, c5
mul r0.x, r0, c3
mad r0.x, -r0, r2, c5.y
mul r2, r1, r0.x
mov_pp_sat r0.x, r2.w
mad_pp r3.xyz, r0.x, -r1, r1
mul_pp r4.xyz, r3, t5
dp3_pp r1.x, t4, c0
mul_pp r3.xyz, r3, c1
max_pp r1.x, r1, c5.z
mul_pp r1.xyz, r1.x, r3
mad_pp r1.xyz, r1, c5.y, r4
mov_pp r0.w, c5.z
mad_pp r0.xyz, r0.x, r2, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 22 ALU, 2 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2, 1, 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R1, fragment.texcoord[5], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
DP3 R2.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.w, R2.y;
MUL R3.xyz, R2.w, fragment.texcoord[1];
DP3 R2.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R2.x, R2.x;
MUL R2.xyz, R2.x, fragment.texcoord[2];
DP3_SAT R2.x, R2, R3;
MUL R2.w, fragment.texcoord[3].x, c[2].x;
MUL R0, R0, c[0];
ADD R2.y, R2.w, c[3].z;
MUL R2.x, R2, c[1];
MAD R2.x, -R2, R2.y, c[3].y;
MUL R2, R0, R2.x;
MOV_SAT R0.w, R2;
MUL R2.xyz, R0.w, R2;
MAD R0.xyz, R0.w, -R0, R0;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R1, R0;
MAD result.color.xyz, R0, c[3].w, R2;
MOV result.color.w, c[3].x;
END
# 22 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 21 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c3, 1.00000000, 2.00000000, 8.00000000, 0.00000000
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t5.xy
texld r2, t5, s1
texld r3, t0, s0
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c2
add r1.x, r1, c3
mul r0.x, r0, c1
mad r0.x, -r0, r1, c3.y
mul r1, r3, c0
mul r3, r1, r0.x
mov_pp_sat r0.x, r3.w
mad_pp r1.xyz, r0.x, -r1, r1
mul_pp r2.xyz, r2.w, r2
mul_pp r1.xyz, r2, r1
mul_pp r0.xyz, r0.x, r3
mov_pp r0.w, c3
mad_pp r0.xyz, r1, c3.z, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_NoiseBias]
Float 4 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 26 ALU, 2 TEX
PARAM c[6] = { program.local[0..4],
		{ 1, 2, 0 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TXP R2.x, fragment.texcoord[6], texture[1], 2D;
DP3 R1.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.x;
MUL R2.yzw, R1.w, fragment.texcoord[1].xxyz;
DP3 R1.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.y, R1.y;
MUL R1.xyz, R1.y, fragment.texcoord[2];
DP3_SAT R1.y, R1, R2.yzww;
MUL R1.w, fragment.texcoord[3].x, c[4].x;
ADD R1.x, R1.w, c[5];
MUL R1.y, R1, c[3].x;
MAD R2.y, -R1, R1.x, c[5];
MUL R1, R0, c[2];
MUL R0, R1, R2.y;
MOV_SAT R0.w, R0;
MAD R2.yzw, R0.w, -R1.xxyz, R1.xxyz;
MUL R1.xyz, R2.yzww, fragment.texcoord[5];
DP3 R1.w, fragment.texcoord[4], c[0];
MAX R1.w, R1, c[5].z;
MUL R2.yzw, R2, c[1].xxyz;
MUL R1.w, R1, R2.x;
MUL R2.xyz, R1.w, R2.yzww;
MAD R1.xyz, R2, c[5].y, R1;
MAD result.color.xyz, R0.w, R0, R1;
MOV result.color.w, c[5].z;
END
# 26 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_NoiseBias]
Float 4 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"ps_2_0
; 25 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c5, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
dcl t6
texldp r5, t6, s1
texld r2, t0, s0
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c4
add r1.x, r1, c5
mul r0.x, r0, c3
mad r0.x, -r0, r1, c5.y
mul r1, r2, c2
mul r2, r1, r0.x
mov_pp_sat r0.x, r2.w
mad_pp r1.xyz, r0.x, -r1, r1
mul_pp r4.xyz, r1, t5
mul_pp r3.xyz, r1, c1
dp3_pp r1.x, t4, c0
max_pp r1.x, r1, c5.z
mul_pp r1.x, r1, r5
mul_pp r1.xyz, r1.x, r3
mad_pp r1.xyz, r1, c5.y, r4
mov_pp r0.w, c5.z
mad_pp r0.xyz, r0.x, r2, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 25 ALU, 3 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2, 1, 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[5], texture[2], 2D;
TEX R1, fragment.texcoord[0], texture[0], 2D;
TXP R3.x, fragment.texcoord[6], texture[1], 2D;
MUL R0.xyz, R0.w, R0;
DP3 R2.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.w, R2.x;
MUL R3.yzw, R2.w, fragment.texcoord[1].xxyz;
DP3 R2.y, fragment.texcoord[2], fragment.texcoord[2];
RSQ R2.y, R2.y;
MUL R2.xyz, R2.y, fragment.texcoord[2];
DP3_SAT R2.y, R2, R3.yzww;
MUL R2.w, fragment.texcoord[3].x, c[2].x;
ADD R2.x, R2.w, c[3].z;
MUL R2.y, R2, c[1].x;
MAD R3.y, -R2, R2.x, c[3];
MUL R2, R1, c[0];
MUL R1, R2, R3.y;
MOV_SAT R1.w, R1;
MUL R1.xyz, R1.w, R1;
MAD R2.xyz, R1.w, -R2, R2;
MUL R0.w, R3.x, c[3].y;
MUL R0.xyz, R0, c[3].w;
MIN R0.xyz, R0, R0.w;
MAD result.color.xyz, R2, R0, R1;
MOV result.color.w, c[3].x;
END
# 25 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"ps_2_0
; 23 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c3, 1.00000000, 2.00000000, 8.00000000, 0.00000000
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t5.xy
dcl t6
texldp r4, t6, s1
texld r2, t5, s2
texld r3, t0, s0
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul_pp r2.xyz, r2.w, r2
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c2
add r1.x, r1, c3
mul r0.x, r0, c1
mad r0.x, -r0, r1, c3.y
mul r1, r3, c0
mul r3, r1, r0.x
mov_pp_sat r0.x, r3.w
mul_pp r3.xyz, r0.x, r3
mad_pp r1.xyz, r0.x, -r1, r1
mul_pp r0.x, r4, c3.y
mul_pp r2.xyz, r2, c3.z
min_pp r0.xyz, r2, r0.x
mov_pp r0.w, c3
mad_pp r0.xyz, r1, r0, r3
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
Program "vp" {
SubProgram "opengl " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [_Time]
Vector 18 [unity_Scale]
Vector 19 [_WorldSpaceCameraPos]
Vector 20 [_WorldSpaceLightPos0]
Vector 21 [_NoiseScale]
Vector 22 [_NoiseSpeed]
Vector 23 [_MainTex_ST]
"!!ARBvp1.0
# 76 ALU
PARAM c[28] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..23],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[22];
MUL R0.xyz, vertex.position.xzyw, c[21];
MUL R1.xyz, R1, c[17].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[24].w;
ADD R0.x, R0, -c[25];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[24];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[25].yzyw;
MAD R3.xyz, R3, R2, c[26].xyxw;
MAD R3.xyz, R3, R2, c[26].zwzw;
MAD R3.xyz, R3, R2, c[27].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[24];
FRC R0.w, R0;
SLT R3.x, R0, c[25];
SGE R3.yz, R0.x, c[27].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[24].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[24];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[25].yzyw;
MAD R1.xyz, R0, R2, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R3.xyz, R1, R2, c[27].xyxw;
ADD R0.xyz, -R0.w, c[24];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[25].yzyw;
MAD R3.xyz, R3, R0, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
MAD R3.xyz, R3, R0, c[27].xyxw;
SGE R1.yz, R1.w, c[27].xzww;
SLT R1.x, R1.w, c[25];
MAD R3.xyz, R3, R0, c[0].yxyw;
SLT R4.x, R0.w, c[25];
SGE R4.yz, R0.w, c[27].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[19];
MOV R1.w, c[0].x;
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[18].w, -vertex.position;
MUL R1.xyz, vertex.normal, c[18].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[6].z, R0, c[15];
DP4 result.texcoord[6].y, R0, c[14];
DP4 result.texcoord[6].x, R0, c[13];
MOV result.texcoord[2].xyz, vertex.normal;
DP3 result.texcoord[4].z, R1, c[7];
DP3 result.texcoord[4].y, R1, c[6];
DP3 result.texcoord[4].x, R1, c[5];
ADD result.texcoord[5].xyz, -R0, c[20];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[23], c[23].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 76 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [_Time]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Vector 20 [_NoiseScale]
Vector 21 [_NoiseSpeed]
Vector 22 [_MainTex_ST]
"vs_2_0
; 71 ALU
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c24, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c25, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c26, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c16.ywzw
mul r1.xyz, v0.xzyw, c20
mul r2.xyz, c21, r0.xxyw
mad r0.x, r2, c23.z, r1
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0, c26, c26.y
mov r0.w, c23.z
mov r0.xyz, c18
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
mul r0.zw, r2.xyyz, c25.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c25.z, c25.w
frc r0.y, r0
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0.y, c26, c26.y
mad r2.x, r0, c26, c26.y
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
sincos r1.xy, r2.x, c24.xyzw, c23.xyzw
add oT3.x, r1, r0
mul r1.xyz, v1, c17.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mad oT1.xyz, r3, c17.w, -v0
dp4 oT6.z, r0, c14
dp4 oT6.y, r0, c13
dp4 oT6.x, r0, c12
mov oT2.xyz, v1
dp3 oT4.z, r1, c6
dp3 oT4.y, r1, c5
dp3 oT4.x, r1, c4
add oT5.xyz, -r0, c19
mad oT0.xy, v2, c22, c22.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceCameraPos]
Vector 16 [_WorldSpaceLightPos0]
Vector 17 [_NoiseScale]
Vector 18 [_NoiseSpeed]
Vector 19 [_MainTex_ST]
"!!ARBvp1.0
# 69 ALU
PARAM c[24] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..19],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[18];
MUL R0.xyz, vertex.position.xzyw, c[17];
MUL R1.xyz, R1, c[13].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[20].w;
ADD R0.x, R0, -c[21];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[20];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[21].yzyw;
MAD R3.xyz, R3, R2, c[22].xyxw;
MAD R3.xyz, R3, R2, c[22].zwzw;
MAD R3.xyz, R3, R2, c[23].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[20];
FRC R0.w, R0;
SLT R3.x, R0, c[21];
SGE R3.yz, R0.x, c[23].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[20].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[20];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[21].yzyw;
MAD R1.xyz, R0, R2, c[22].xyxw;
MAD R1.xyz, R1, R2, c[22].zwzw;
MAD R3.xyz, R1, R2, c[23].xyxw;
ADD R0.xyz, -R0.w, c[20];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[21].yzyw;
MAD R3.xyz, R3, R0, c[22].xyxw;
MAD R3.xyz, R3, R0, c[22].zwzw;
MAD R3.xyz, R3, R0, c[23].xyxw;
SGE R1.yz, R1.w, c[23].xzww;
SLT R1.x, R1.w, c[21];
MAD R3.xyz, R3, R0, c[0].yxyw;
MOV R1.w, c[0].x;
DP3 R1.y, R1, c[0].yxyw;
SLT R4.x, R0.w, c[21];
SGE R4.yz, R0.w, c[23].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[15];
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[14].w, -vertex.position;
MUL R0.xyz, vertex.normal, c[14].w;
MOV result.texcoord[2].xyz, vertex.normal;
DP3 result.texcoord[4].z, R0, c[7];
DP3 result.texcoord[4].y, R0, c[6];
DP3 result.texcoord[4].x, R0, c[5];
MOV result.texcoord[5].xyz, c[16];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[19], c[19].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 69 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_WorldSpaceLightPos0]
Vector 16 [_NoiseScale]
Vector 17 [_NoiseSpeed]
Vector 18 [_MainTex_ST]
"vs_2_0
; 64 ALU
def c19, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c20, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c21, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c22, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c12.ywzw
mul r1.xyz, v0.xzyw, c16
mul r2.xyz, c17, r0.xxyw
mad r0.x, r2, c19.z, r1
mad r0.x, r0, c21.z, c21.w
frc r0.x, r0
mad r1.x, r0, c22, c22.y
mov r0.w, c19.z
mov r0.xyz, c14
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
mul r0.zw, r2.xyyz, c21.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c21.z, c21.w
frc r0.y, r0
mad r0.x, r0, c21.z, c21.w
frc r0.x, r0
mad r1.x, r0.y, c22, c22.y
mad r2.x, r0, c22, c22.y
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
sincos r1.xy, r2.x, c20.xyzw, c19.xyzw
add oT3.x, r1, r0
mul r0.xyz, v1, c13.w
mad oT1.xyz, r3, c13.w, -v0
mov oT2.xyz, v1
dp3 oT4.z, r0, c6
dp3 oT4.y, r0, c5
dp3 oT4.x, r0, c4
mov oT5.xyz, c15
mad oT0.xy, v2, c18, c18.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [_Time]
Vector 18 [unity_Scale]
Vector 19 [_WorldSpaceCameraPos]
Vector 20 [_WorldSpaceLightPos0]
Vector 21 [_NoiseScale]
Vector 22 [_NoiseSpeed]
Vector 23 [_MainTex_ST]
"!!ARBvp1.0
# 77 ALU
PARAM c[28] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..23],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[22];
MUL R0.xyz, vertex.position.xzyw, c[21];
MUL R1.xyz, R1, c[17].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[24].w;
ADD R0.x, R0, -c[25];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[24];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[25].yzyw;
MAD R3.xyz, R3, R2, c[26].xyxw;
MAD R3.xyz, R3, R2, c[26].zwzw;
MAD R3.xyz, R3, R2, c[27].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[24];
FRC R0.w, R0;
SLT R3.x, R0, c[25];
SGE R3.yz, R0.x, c[27].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[24].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[24];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[25].yzyw;
MAD R1.xyz, R0, R2, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R3.xyz, R1, R2, c[27].xyxw;
ADD R0.xyz, -R0.w, c[24];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[25].yzyw;
MAD R3.xyz, R3, R0, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
MAD R3.xyz, R3, R0, c[27].xyxw;
SGE R1.yz, R1.w, c[27].xzww;
SLT R1.x, R1.w, c[25];
MAD R3.xyz, R3, R0, c[0].yxyw;
SLT R4.x, R0.w, c[25];
SGE R4.yz, R0.w, c[27].xzww;
DP4 R0.w, vertex.position, c[8];
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[19];
MOV R1.w, c[0].x;
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[18].w, -vertex.position;
MUL R1.xyz, vertex.normal, c[18].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 result.texcoord[6].w, R0, c[16];
DP4 result.texcoord[6].z, R0, c[15];
DP4 result.texcoord[6].y, R0, c[14];
DP4 result.texcoord[6].x, R0, c[13];
MOV result.texcoord[2].xyz, vertex.normal;
DP3 result.texcoord[4].z, R1, c[7];
DP3 result.texcoord[4].y, R1, c[6];
DP3 result.texcoord[4].x, R1, c[5];
ADD result.texcoord[5].xyz, -R0, c[20];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[23], c[23].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 77 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [_Time]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Vector 20 [_NoiseScale]
Vector 21 [_NoiseSpeed]
Vector 22 [_MainTex_ST]
"vs_2_0
; 72 ALU
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c24, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c25, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c26, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c16.ywzw
mul r1.xyz, v0.xzyw, c20
mul r2.xyz, c21, r0.xxyw
mad r0.x, r2, c23.z, r1
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0, c26, c26.y
mov r0.w, c23.z
mov r0.xyz, c18
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
mul r0.zw, r2.xyyz, c25.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c25.z, c25.w
frc r0.y, r0
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0.y, c26, c26.y
mad r2.x, r0, c26, c26.y
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
sincos r1.xy, r2.x, c24.xyzw, c23.xyzw
add oT3.x, r1, r0
mul r1.xyz, v1, c17.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mad oT1.xyz, r3, c17.w, -v0
dp4 oT6.w, r0, c15
dp4 oT6.z, r0, c14
dp4 oT6.y, r0, c13
dp4 oT6.x, r0, c12
mov oT2.xyz, v1
dp3 oT4.z, r1, c6
dp3 oT4.y, r1, c5
dp3 oT4.x, r1, c4
add oT5.xyz, -r0, c19
mad oT0.xy, v2, c22, c22.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [_Time]
Vector 18 [unity_Scale]
Vector 19 [_WorldSpaceCameraPos]
Vector 20 [_WorldSpaceLightPos0]
Vector 21 [_NoiseScale]
Vector 22 [_NoiseSpeed]
Vector 23 [_MainTex_ST]
"!!ARBvp1.0
# 76 ALU
PARAM c[28] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..23],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[22];
MUL R0.xyz, vertex.position.xzyw, c[21];
MUL R1.xyz, R1, c[17].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[24].w;
ADD R0.x, R0, -c[25];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[24];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[25].yzyw;
MAD R3.xyz, R3, R2, c[26].xyxw;
MAD R3.xyz, R3, R2, c[26].zwzw;
MAD R3.xyz, R3, R2, c[27].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[24];
FRC R0.w, R0;
SLT R3.x, R0, c[25];
SGE R3.yz, R0.x, c[27].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[24].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[24];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[25].yzyw;
MAD R1.xyz, R0, R2, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R3.xyz, R1, R2, c[27].xyxw;
ADD R0.xyz, -R0.w, c[24];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[25].yzyw;
MAD R3.xyz, R3, R0, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
MAD R3.xyz, R3, R0, c[27].xyxw;
SGE R1.yz, R1.w, c[27].xzww;
SLT R1.x, R1.w, c[25];
MAD R3.xyz, R3, R0, c[0].yxyw;
SLT R4.x, R0.w, c[25];
SGE R4.yz, R0.w, c[27].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[19];
MOV R1.w, c[0].x;
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[18].w, -vertex.position;
MUL R1.xyz, vertex.normal, c[18].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[6].z, R0, c[15];
DP4 result.texcoord[6].y, R0, c[14];
DP4 result.texcoord[6].x, R0, c[13];
MOV result.texcoord[2].xyz, vertex.normal;
DP3 result.texcoord[4].z, R1, c[7];
DP3 result.texcoord[4].y, R1, c[6];
DP3 result.texcoord[4].x, R1, c[5];
ADD result.texcoord[5].xyz, -R0, c[20];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[23], c[23].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 76 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [_Time]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Vector 20 [_NoiseScale]
Vector 21 [_NoiseSpeed]
Vector 22 [_MainTex_ST]
"vs_2_0
; 71 ALU
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c24, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c25, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c26, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c16.ywzw
mul r1.xyz, v0.xzyw, c20
mul r2.xyz, c21, r0.xxyw
mad r0.x, r2, c23.z, r1
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0, c26, c26.y
mov r0.w, c23.z
mov r0.xyz, c18
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
mul r0.zw, r2.xyyz, c25.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c25.z, c25.w
frc r0.y, r0
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0.y, c26, c26.y
mad r2.x, r0, c26, c26.y
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
sincos r1.xy, r2.x, c24.xyzw, c23.xyzw
add oT3.x, r1, r0
mul r1.xyz, v1, c17.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mad oT1.xyz, r3, c17.w, -v0
dp4 oT6.z, r0, c14
dp4 oT6.y, r0, c13
dp4 oT6.x, r0, c12
mov oT2.xyz, v1
dp3 oT4.z, r1, c6
dp3 oT4.y, r1, c5
dp3 oT4.x, r1, c4
add oT5.xyz, -r0, c19
mad oT0.xy, v2, c22, c22.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [_Time]
Vector 18 [unity_Scale]
Vector 19 [_WorldSpaceCameraPos]
Vector 20 [_WorldSpaceLightPos0]
Vector 21 [_NoiseScale]
Vector 22 [_NoiseSpeed]
Vector 23 [_MainTex_ST]
"!!ARBvp1.0
# 75 ALU
PARAM c[28] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..23],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[22];
MUL R0.xyz, vertex.position.xzyw, c[21];
MUL R1.xyz, R1, c[17].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[24].w;
ADD R0.x, R0, -c[25];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[24];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[25].yzyw;
MAD R3.xyz, R3, R2, c[26].xyxw;
MAD R3.xyz, R3, R2, c[26].zwzw;
MAD R3.xyz, R3, R2, c[27].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[24];
FRC R0.w, R0;
SLT R3.x, R0, c[25];
SGE R3.yz, R0.x, c[27].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[24].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[24];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[25].yzyw;
MAD R1.xyz, R0, R2, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R3.xyz, R1, R2, c[27].xyxw;
ADD R0.xyz, -R0.w, c[24];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[25].yzyw;
MAD R3.xyz, R3, R0, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
MAD R3.xyz, R3, R0, c[27].xyxw;
SGE R1.yz, R1.w, c[27].xzww;
SLT R1.x, R1.w, c[25];
MAD R3.xyz, R3, R0, c[0].yxyw;
SLT R4.x, R0.w, c[25];
SGE R4.yz, R0.w, c[27].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
MOV R1.w, c[0].x;
MOV R1.xyz, c[19];
ADD result.texcoord[3].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[1].xyz, R0, c[18].w, -vertex.position;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[6].y, R0, c[14];
DP4 result.texcoord[6].x, R0, c[13];
MUL R0.xyz, vertex.normal, c[18].w;
MOV result.texcoord[2].xyz, vertex.normal;
DP3 result.texcoord[4].z, R0, c[7];
DP3 result.texcoord[4].y, R0, c[6];
DP3 result.texcoord[4].x, R0, c[5];
MOV result.texcoord[5].xyz, c[20];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[23], c[23].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 75 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [_Time]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
Vector 20 [_NoiseScale]
Vector 21 [_NoiseSpeed]
Vector 22 [_MainTex_ST]
"vs_2_0
; 70 ALU
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c24, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c25, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c26, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c16.ywzw
mul r1.xyz, v0.xzyw, c20
mul r2.xyz, c21, r0.xxyw
mad r0.x, r2, c23.z, r1
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0, c26, c26.y
mov r0.w, c23.z
mov r0.xyz, c18
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
mul r0.zw, r2.xyyz, c25.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c25.z, c25.w
frc r0.y, r0
mad r0.x, r0, c25.z, c25.w
frc r0.x, r0
mad r1.x, r0.y, c26, c26.y
mad r2.x, r0, c26, c26.y
sincos r0.xy, r1.x, c24.xyzw, c23.xyzw
sincos r1.xy, r2.x, c24.xyzw, c23.xyzw
add oT3.x, r1, r0
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT6.y, r0, c13
dp4 oT6.x, r0, c12
mul r0.xyz, v1, c17.w
mad oT1.xyz, r3, c17.w, -v0
mov oT2.xyz, v1
dp3 oT4.z, r0, c6
dp3 oT4.y, r0, c5
dp3 oT4.x, r0, c4
mov oT5.xyz, c19
mad oT0.xy, v2, c22, c22.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 27 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
DP3 R1.x, fragment.texcoord[6], fragment.texcoord[6];
DP3 R1.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.x, R1.y;
MUL R0, R0, c[1];
MUL R2.xyz, R2.x, fragment.texcoord[1];
MUL R2.w, fragment.texcoord[3].x, c[3].x;
MOV result.color.w, c[4].x;
TEX R1.w, R1.x, texture[1], 2D;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3_SAT R1.x, R1, R2;
ADD R1.y, R2.w, c[4].z;
MUL R1.x, R1, c[2];
MAD R1.x, -R1, R1.y, c[4].y;
MUL_SAT R0.w, R0, R1.x;
MAD R0.xyz, R0.w, -R0, R0;
DP3 R1.y, fragment.texcoord[5], fragment.texcoord[5];
RSQ R1.y, R1.y;
MUL R1.xyz, R1.y, fragment.texcoord[5];
DP3 R0.w, fragment.texcoord[4], R1;
MAX R0.w, R0, c[4].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[4].y;
END
# 27 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 27 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c4, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
dcl t6.xyz
texld r2, t0, s0
dp3 r0.x, t6, t6
mov r0.xy, r0.x
dp3 r1.x, t1, t1
rsq r1.x, r1.x
mul r1.xyz, r1.x, t1
mul r2, r2, c1
texld r3, r0, s1
dp3 r0.x, t2, t2
rsq r0.x, r0.x
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c3
add r1.x, r1, c4
mul r0.x, r0, c2
mad r0.x, -r0, r1, c4.y
mul_sat r0.w, r2, r0.x
dp3_pp r1.x, t5, t5
rsq_pp r0.x, r1.x
mad_pp r1.xyz, r0.w, -r2, r2
mul_pp r0.xyz, r0.x, t5
dp3_pp r0.x, t4, r0
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c4.z
mul_pp r0.x, r0, r3
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c4.y
mov_pp r0.w, c4.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 22 ALU, 1 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
DP3 R1.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.w, R1.y;
MUL R2.xyz, R1.w, fragment.texcoord[1];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3_SAT R1.x, R1, R2;
MUL R1.w, fragment.texcoord[3].x, c[3].x;
ADD R1.y, R1.w, c[4].z;
MUL R1.x, R1, c[2];
MAD R2.x, -R1, R1.y, c[4].y;
MUL R1, R0, c[1];
MUL_SAT R0.w, R1, R2.x;
MOV R0.xyz, fragment.texcoord[5];
MAD R1.xyz, R0.w, -R1, R1;
DP3 R0.w, fragment.texcoord[4], R0;
MUL R0.xyz, R1, c[0];
MAX R0.w, R0, c[4].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[4].y;
MOV result.color.w, c[4].x;
END
# 22 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 22 ALU, 1 TEX
dcl_2d s0
def c4, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
texld r1, t0, s0
dp3 r2.x, t1, t1
dp3 r0.x, t2, t2
rsq r2.x, r2.x
rsq r0.x, r0.x
mul r0.xyz, r0.x, t2
mul r2.xyz, r2.x, t1
dp3_sat r0.x, r0, r2
mul r2.x, t3, c3
mul r1, r1, c1
mul r0.x, r0, c2
add r2.x, r2, c4
mad r0.x, -r0, r2, c4.y
mul_sat r0.w, r1, r0.x
mad_pp r1.xyz, r0.w, -r1, r1
mov_pp r0.xyz, t5
dp3_pp r0.x, t4, r0
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c4.z
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c4.y
mov_pp r0.w, c4.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 33 ALU, 3 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R2, fragment.texcoord[0], texture[0], 2D;
DP3 R0.z, fragment.texcoord[6], fragment.texcoord[6];
RCP R0.x, fragment.texcoord[6].w;
MAD R0.xy, fragment.texcoord[6], R0.x, c[4].w;
MUL R2, R2, c[1];
MUL R3.x, fragment.texcoord[3], c[3];
MOV result.color.w, c[4].x;
TEX R0.w, R0, texture[1], 2D;
TEX R1.w, R0.z, texture[2], 2D;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.x, R0.y;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.x, R0.x;
MUL R0.xyz, R0.x, fragment.texcoord[2];
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3_SAT R0.x, R0, R1;
ADD R0.y, R3.x, c[4].z;
MUL R0.x, R0, c[2];
MAD R0.x, -R0, R0.y, c[4].y;
MUL_SAT R0.x, R2.w, R0;
MAD R1.xyz, R0.x, -R2, R2;
DP3 R0.y, fragment.texcoord[5], fragment.texcoord[5];
RSQ R0.y, R0.y;
MUL R0.xyz, R0.y, fragment.texcoord[5];
DP3 R0.x, fragment.texcoord[4], R0;
SLT R0.y, c[4].x, fragment.texcoord[6].z;
MUL R0.y, R0, R0.w;
MUL R0.y, R0, R1.w;
MAX R0.x, R0, c[4];
MUL R1.xyz, R1, c[0];
MUL R0.x, R0, R0.y;
MUL R0.xyz, R0.x, R1;
MUL result.color.xyz, R0, c[4].y;
END
# 33 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 32 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c4, 1.00000000, 2.00000000, 0.50000000, 0.00000000
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
dcl t6
texld r2, t0, s0
dp3 r1.x, t6, t6
mov r1.xy, r1.x
rcp r0.x, t6.w
mad r0.xy, t6, r0.x, c4.z
texld r3, r1, s2
texld r0, r0, s1
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c3
add r1.x, r1, c4
mul r0.x, r0, c2
mad r0.x, -r0, r1, c4.y
mul r1, r2, c1
mul_sat r1.w, r1, r0.x
mad_pp r0.xyz, r1.w, -r1, r1
mul_pp r2.xyz, r0, c0
dp3_pp r0.x, t5, t5
rsq_pp r1.x, r0.x
cmp r0.x, -t6.z, c4.w, c4
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1.x, t5
dp3_pp r1.x, t4, r1
mul_pp r0.x, r0, r3
max_pp r1.x, r1, c4.w
mul_pp r0.x, r1, r0
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c4.y
mov_pp r0.w, c4
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 29 ALU, 3 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R2, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[6], texture[2], CUBE;
DP3 R0.x, fragment.texcoord[6], fragment.texcoord[6];
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.x, R0.y;
MUL R1.xyz, R1.x, fragment.texcoord[1];
MUL R2, R2, c[1];
MUL R3.x, fragment.texcoord[3], c[3];
MOV result.color.w, c[4].x;
TEX R0.w, R0.x, texture[1], 2D;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.x, R0.x;
MUL R0.xyz, R0.x, fragment.texcoord[2];
DP3_SAT R0.x, R0, R1;
ADD R0.y, R3.x, c[4].z;
MUL R0.x, R0, c[2];
MAD R0.x, -R0, R0.y, c[4].y;
MUL_SAT R0.x, R2.w, R0;
MAD R1.xyz, R0.x, -R2, R2;
DP3 R0.y, fragment.texcoord[5], fragment.texcoord[5];
RSQ R0.y, R0.y;
MUL R0.xyz, R0.y, fragment.texcoord[5];
DP3 R0.x, fragment.texcoord[4], R0;
MUL R0.y, R0.w, R1.w;
MAX R0.x, R0, c[4];
MUL R1.xyz, R1, c[0];
MUL R0.x, R0, R0.y;
MUL R0.xyz, R0.x, R1;
MUL result.color.xyz, R0, c[4].y;
END
# 29 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 28 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_cube s2
def c4, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
dcl t6.xyz
texld r2, t0, s0
dp3 r0.x, t6, t6
mov r1.xy, r0.x
texld r3, r1, s1
texld r0, t6, s2
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c3
add r1.x, r1, c4
mul r0.x, r0, c2
mad r0.x, -r0, r1, c4.y
mul r1, r2, c1
mul_sat r1.w, r1, r0.x
mad_pp r1.xyz, r1.w, -r1, r1
dp3_pp r0.x, t5, t5
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t5
dp3_pp r0.x, t4, r0
mul r2.x, r3, r0.w
max_pp r0.x, r0, c4.z
mul_pp r1.xyz, r1, c0
mul_pp r0.x, r0, r2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c4.y
mov_pp r0.w, c4.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 24 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[6], texture[1], 2D;
DP3 R1.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.x, R1.y;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
MUL R2.xyz, R2.x, fragment.texcoord[1];
DP3_SAT R1.x, R1, R2;
MUL R2.w, fragment.texcoord[3].x, c[3].x;
MUL R0, R0, c[1];
ADD R1.y, R2.w, c[4].z;
MUL R1.x, R1, c[2];
MAD R1.x, -R1, R1.y, c[4].y;
MUL_SAT R0.w, R0, R1.x;
MAD R0.xyz, R0.w, -R0, R0;
MOV R1.xyz, fragment.texcoord[5];
DP3 R0.w, fragment.texcoord[4], R1;
MAX R0.w, R0, c[4].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[4].y;
MOV result.color.w, c[4].x;
END
# 24 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_NoiseBias]
Float 3 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 23 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c4, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4.xyz
dcl t5.xyz
dcl t6.xy
texld r0, t6, s1
texld r2, t0, s0
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c3
add r1.x, r1, c4
mul r0.x, r0, c2
mad r0.x, -r0, r1, c4.y
mul r1, r2, c1
mul_sat r1.w, r1, r0.x
mov_pp r0.xyz, t5
dp3_pp r0.x, t4, r0
mad_pp r1.xyz, r1.w, -r1, r1
max_pp r0.x, r0, c4.z
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1, c0
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c4.y
mov_pp r0.w, c4.z
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceCameraPos]
Vector 16 [_NoiseScale]
Vector 17 [_NoiseSpeed]
"!!ARBvp1.0
# 67 ALU
PARAM c[22] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..17],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[17];
MUL R0.xyz, vertex.position.xzyw, c[16];
MUL R1.xyz, R1, c[13].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[18].w;
ADD R0.x, R0, -c[19];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[18];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[19].yzyw;
MAD R3.xyz, R3, R2, c[20].xyxw;
MAD R3.xyz, R3, R2, c[20].zwzw;
MAD R3.xyz, R3, R2, c[21].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[18];
FRC R0.w, R0;
SLT R3.x, R0, c[19];
SGE R3.yz, R0.x, c[21].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[18].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[18];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[19].yzyw;
MAD R1.xyz, R0, R2, c[20].xyxw;
MAD R1.xyz, R1, R2, c[20].zwzw;
MAD R3.xyz, R1, R2, c[21].xyxw;
ADD R0.xyz, -R0.w, c[18];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[19].yzyw;
MAD R3.xyz, R3, R0, c[20].xyxw;
MAD R3.xyz, R3, R0, c[20].zwzw;
MAD R3.xyz, R3, R0, c[21].xyxw;
SGE R1.yz, R1.w, c[21].xzww;
SLT R1.x, R1.w, c[19];
MAD R3.xyz, R3, R0, c[0].yxyw;
MOV R1.w, c[0].x;
DP3 R1.y, R1, c[0].yxyw;
SLT R4.x, R0.w, c[19];
SGE R4.yz, R0.w, c[21].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R0.x, R2, -R1;
MOV R1.xyz, c[15];
ADD result.texcoord[2].x, R0, R0.y;
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[0].xyz, R0, c[14].w, -vertex.position;
MUL R0.xyz, vertex.normal, c[14].w;
MOV result.texcoord[1].xyz, vertex.normal;
DP3 result.texcoord[3].z, R0, c[7];
DP3 result.texcoord[3].y, R0, c[6];
DP3 result.texcoord[3].x, R0, c[5];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 67 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceCameraPos]
Vector 15 [_NoiseScale]
Vector 16 [_NoiseSpeed]
"vs_2_0
; 62 ALU
def c17, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c18, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c19, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c20, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r0.xy, c12.ywzw
mul r1.xyz, v0.xzyw, c15
mul r2.xyz, c16, r0.xxyw
mad r0.x, r2, c17.z, r1
mad r0.x, r0, c19.z, c19.w
frc r0.x, r0
mad r1.x, r0, c20, c20.y
mov r0.w, c17.z
mov r0.xyz, c14
dp4 r3.z, r0, c10
dp4 r3.x, r0, c8
dp4 r3.y, r0, c9
sincos r0.xy, r1.x, c18.xyzw, c17.xyzw
mul r0.zw, r2.xyyz, c19.xyxy
mad r0.x, r1.z, r0.y, r0.w
add r0.z, r1.y, r0
mad r0.y, r0.z, c19.z, c19.w
frc r0.y, r0
mad r0.x, r0, c19.z, c19.w
frc r0.x, r0
mad r1.x, r0.y, c20, c20.y
mad r2.x, r0, c20, c20.y
sincos r0.xy, r1.x, c18.xyzw, c17.xyzw
sincos r1.xy, r2.x, c18.xyzw, c17.xyzw
add oT2.x, r1, r0
mul r0.xyz, v1, c13.w
mad oT0.xyz, r3, c13.w, -v0
mov oT1.xyz, v1
dp3 oT3.z, r0, c6
dp3 oT3.y, r0, c5
dp3 oT3.x, r0, c4
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 2 ALU, 0 TEX
PARAM c[1] = { { 0, 0.5 } };
MAD result.color.xyz, fragment.texcoord[3], c[0].y, c[0].y;
MOV result.color.w, c[0].x;
END
# 2 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
dcl_2d s0
def c0, 0.50000000, 0.00000000, 0, 0
dcl t3.xyz
mad_pp r0.xyz, t3, c0.x, c0.x
mov_pp r0.w, c0.y
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "RenderType"="Opaque" }
  ZWrite Off
Program "vp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_World2Object]
Vector 9 [_Time]
Vector 10 [_ProjectionParams]
Vector 11 [unity_Scale]
Vector 12 [_WorldSpaceCameraPos]
Vector 13 [_NoiseScale]
Vector 14 [_NoiseSpeed]
Vector 15 [_MainTex_ST]
"!!ARBvp1.0
# 70 ALU
PARAM c[20] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..15],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[14];
MUL R0.xyz, vertex.position.xzyw, c[13];
MUL R1.xyz, R1, c[9].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[16].w;
ADD R0.x, R0, -c[17];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[16];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[17].yzyw;
MAD R3.xyz, R3, R2, c[18].xyxw;
MAD R3.xyz, R3, R2, c[18].zwzw;
MAD R3.xyz, R3, R2, c[19].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[16];
FRC R0.w, R0;
SLT R3.x, R0, c[17];
SGE R3.yz, R0.x, c[19].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[16].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[16];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[17].yzyw;
MAD R1.xyz, R0, R2, c[18].xyxw;
MAD R1.xyz, R1, R2, c[18].zwzw;
MAD R3.xyz, R1, R2, c[19].xyxw;
ADD R0.xyz, -R0.w, c[16];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[17].yzyw;
MAD R3.xyz, R3, R0, c[18].xyxw;
MAD R3.xyz, R3, R0, c[18].zwzw;
MAD R3.xyz, R3, R0, c[19].xyxw;
MAD R3.xyz, R3, R0, c[0].yxyw;
SGE R1.yz, R1.w, c[19].xzww;
SLT R1.x, R1.w, c[17];
SLT R4.x, R0.w, c[17];
SGE R4.yz, R0.w, c[19].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
ADD result.texcoord[3].x, R0, R0.y;
MOV R1.xyz, c[12];
MOV R1.w, c[0].x;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R3.z, R1, c[7];
DP4 R3.x, R1, c[5];
DP4 R3.y, R1, c[6];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R2.xyz, R0.xyww, c[16].y;
MOV R1.x, R2;
MUL R1.y, R2, c[10].x;
MAD result.texcoord[1].xyz, R3, c[11].w, -vertex.position;
ADD result.texcoord[4].xy, R1, R2.z;
MOV result.position, R0;
MOV result.texcoord[4].zw, R0;
MOV result.texcoord[2].xyz, vertex.normal;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
END
# 70 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_World2Object]
Vector 8 [_Time]
Vector 9 [_ProjectionParams]
Vector 10 [_ScreenParams]
Vector 11 [unity_Scale]
Vector 12 [_WorldSpaceCameraPos]
Vector 13 [_NoiseScale]
Vector 14 [_NoiseSpeed]
Vector 15 [_MainTex_ST]
"vs_2_0
; 64 ALU
def c16, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c17, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c18, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c19, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mov r0.xy, c8.ywzw
mul r2.xyz, v0.xzyw, c13
mul r3.xyz, c14, r0.xxyw
mad r0.x, r3, c16.z, r2
mad r1.w, r0.x, c18.z, c18
mov r0.w, c16.z
mov r0.xyz, c12
dp4 r1.z, r0, c6
dp4 r1.x, r0, c4
dp4 r1.y, r0, c5
mad oT1.xyz, r1, c11.w, -v0
frc r0.w, r1
dp4 r1.w, v0, c3
mad r1.z, r0.w, c19.x, c19.y
dp4 r1.x, v0, c0
dp4 r1.y, v0, c1
mul r0.xyz, r1.xyww, c16.w
mul r0.y, r0, c9.x
mad oT4.xy, r0.z, c10.zwzw, r0
sincos r0.xy, r1.z, c17.xyzw, c16.xyzw
mul r0.zw, r3.xyyz, c18.xyxy
mad r0.x, r2.z, r0.y, r0.w
add r0.y, r2, r0.z
mad r0.x, r0, c18.z, c18.w
mad r0.y, r0, c18.z, c18.w
frc r0.y, r0
frc r0.x, r0
mad r0.x, r0, c19, c19.y
sincos r2.xy, r0.x, c17.xyzw, c16.xyzw
mad r1.z, r0.y, c19.x, c19.y
sincos r0.xy, r1.z, c17.xyzw, c16.xyzw
dp4 r1.z, v0, c2
add oT3.x, r2, r0
mov oPos, r1
mov oT4.zw, r1
mov oT2.xyz, v1
mad oT0.xy, v2, c15, c15.zwzw
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 9 [_World2Object]
Vector 13 [_Time]
Vector 14 [_ProjectionParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [_NoiseScale]
Vector 18 [_NoiseSpeed]
Vector 19 [unity_LightmapST]
Vector 20 [unity_LightmapFade]
Vector 21 [_MainTex_ST]
"!!ARBvp1.0
# 73 ALU
PARAM c[26] = { { 1, -1, 24.980801, -24.980801 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9..21],
		{ 0, 0.5, 1, 0.15915491 },
		{ 0.25, -60.145809, 60.145809 },
		{ 85.453789, -85.453789, -64.939346, 64.939346 },
		{ 19.73921, -19.73921, -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R1.xyz, c[18];
MUL R0.xyz, vertex.position.xzyw, c[17];
MUL R1.xyz, R1, c[13].yyww;
MAD R0.x, R1, c[0], R0;
MUL R1.xy, R1.yzzw, c[0].yxzw;
ADD R0.w, R0.y, R1.x;
MUL R0.x, R0, c[22].w;
ADD R0.x, R0, -c[23];
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[22];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[23].yzyw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R3.xyz, R3, R2, c[24].zwzw;
MAD R3.xyz, R3, R2, c[25].xyxw;
MAD R2.xyz, R3, R2, c[0].yxyw;
MUL R0.w, R0, c[22];
FRC R0.w, R0;
SLT R3.x, R0, c[23];
SGE R3.yz, R0.x, c[25].xzww;
DP3 R3.y, R3, c[0].yxyw;
DP3 R0.x, R2, -R3;
MAD R0.x, R0.z, R0, R1.y;
MUL R0.x, R0, c[22].w;
FRC R1.w, R0.x;
ADD R2.xyz, -R1.w, c[22];
MUL R2.xyz, R2, R2;
MUL R0.xyz, R2, c[0].zwzw;
ADD R0.xyz, R0, c[23].yzyw;
MAD R1.xyz, R0, R2, c[24].xyxw;
MAD R1.xyz, R1, R2, c[24].zwzw;
MAD R3.xyz, R1, R2, c[25].xyxw;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MAD R2.xyz, R3, R2, c[0].yxyw;
ADD R3.xyz, R1, c[23].yzyw;
MAD R3.xyz, R3, R0, c[24].xyxw;
MAD R3.xyz, R3, R0, c[24].zwzw;
MAD R3.xyz, R3, R0, c[25].xyxw;
MAD R3.xyz, R3, R0, c[0].yxyw;
SGE R1.yz, R1.w, c[25].xzww;
SLT R1.x, R1.w, c[23];
SLT R4.x, R0.w, c[23];
SGE R4.yz, R0.w, c[25].xzww;
MOV R0.xz, R4;
DP3 R0.y, R4, c[0].yxyw;
DP3 R0.y, R3, -R0;
DP3 R1.y, R1, c[0].yxyw;
DP3 R0.x, R2, -R1;
ADD result.texcoord[3].x, R0, R0.y;
MOV R1.xyz, c[16];
MOV R1.w, c[0].x;
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MUL R2.xyz, R0.xyww, c[22].y;
MOV result.position, R0;
DP4 R0.x, vertex.position, c[3];
DP4 R3.z, R1, c[11];
DP4 R3.x, R1, c[9];
DP4 R3.y, R1, c[10];
MOV R1.x, R2;
MUL R1.y, R2, c[14].x;
MAD result.texcoord[1].xyz, R3, c[15].w, -vertex.position;
ADD result.texcoord[4].xy, R1, R2.z;
MOV result.texcoord[4].zw, R0;
MOV result.texcoord[2].xyz, vertex.normal;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[21], c[21].zwzw;
MAD result.texcoord[5].xy, vertex.texcoord[1], c[19], c[19].zwzw;
MAD result.texcoord[5].z, -R0.x, c[20], c[20].w;
END
# 73 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Matrix 8 [_World2Object]
Vector 12 [_Time]
Vector 13 [_ProjectionParams]
Vector 14 [_ScreenParams]
Vector 15 [unity_Scale]
Vector 16 [_WorldSpaceCameraPos]
Vector 17 [_NoiseScale]
Vector 18 [_NoiseSpeed]
Vector 19 [unity_LightmapST]
Vector 20 [unity_LightmapFade]
Vector 21 [_MainTex_ST]
"vs_2_0
; 67 ALU
def c22, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c23, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c24, -1.00000000, 1.00000000, 0.15915491, 0.50000000
def c25, 6.28318501, -3.14159298, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mov r0.xy, c12.ywzw
mul r2.xyz, v0.xzyw, c17
mul r3.xyz, c18, r0.xxyw
mad r0.x, r3, c22.z, r2
mad r1.w, r0.x, c24.z, c24
mov r0.w, c22.z
mov r0.xyz, c16
dp4 r1.z, r0, c10
dp4 r1.x, r0, c8
dp4 r1.y, r0, c9
mad oT1.xyz, r1, c15.w, -v0
frc r0.w, r1
dp4 r1.w, v0, c7
mad r1.z, r0.w, c25.x, c25.y
dp4 r1.x, v0, c4
dp4 r1.y, v0, c5
mul r0.xyz, r1.xyww, c22.w
mul r0.y, r0, c13.x
mad oT4.xy, r0.z, c14.zwzw, r0
sincos r0.xy, r1.z, c23.xyzw, c22.xyzw
mul r0.zw, r3.xyyz, c24.xyxy
mad r0.x, r2.z, r0.y, r0.w
add r0.y, r2, r0.z
mad r0.x, r0, c24.z, c24.w
mad r0.y, r0, c24.z, c24.w
frc r0.y, r0
frc r0.x, r0
mad r0.x, r0, c25, c25.y
sincos r2.xy, r0.x, c23.xyzw, c22.xyzw
mad r1.z, r0.y, c25.x, c25.y
sincos r0.xy, r1.z, c23.xyzw, c22.xyzw
dp4 r1.z, v0, c6
add oT3.x, r2, r0
dp4 r0.x, v0, c2
mov oPos, r1
mov oT4.zw, r1
mov oT2.xyz, v1
mad oT0.xy, v2, c21, c21.zwzw
mad oT5.xy, v3, c19, c19.zwzw
mad oT5.z, -r0.x, c20, c20.w
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
Vector 3 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 24 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2, 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TXP R1.xyz, fragment.texcoord[4], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
DP3 R2.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.w, R2.x;
MUL R3.xyz, R2.w, fragment.texcoord[1];
DP3 R1.w, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.w, R1.w;
MUL R2.xyz, R1.w, fragment.texcoord[2];
DP3_SAT R1.w, R2, R3;
MUL R2.w, fragment.texcoord[3].x, c[2].x;
MUL R0, R0, c[0];
ADD R2.x, R2.w, c[4].z;
MUL R1.w, R1, c[1].x;
MAD R1.w, -R1, R2.x, c[4].y;
MUL R2, R0, R1.w;
MOV_SAT R0.w, R2;
MUL R2.xyz, R0.w, R2;
MAD R0.xyz, R0.w, -R0, R0;
LG2 R1.x, R1.x;
LG2 R1.z, R1.z;
LG2 R1.y, R1.y;
ADD R1.xyz, -R1, c[3];
MAD result.color.xyz, R0, R1, R2;
MOV result.color.w, c[4].x;
END
# 24 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
Vector 3 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
"ps_2_0
; 23 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c4, 1.00000000, 2.00000000, 0.00000000, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4
texldp r3, t4, s1
texld r2, t0, s0
dp3 r1.x, t1, t1
dp3 r0.x, t2, t2
rsq r1.x, r1.x
rsq r0.x, r0.x
mul r1.xyz, r1.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r1
mul r1.x, t3, c2
add r1.x, r1, c4
mul r0.x, r0, c1
mad r0.x, -r0, r1, c4.y
mul r1, r2, c0
mul r2, r1, r0.x
mov_pp_sat r0.x, r2.w
mul_pp r2.xyz, r0.x, r2
mad_pp r0.xyz, r0.x, -r1, r1
log_pp r1.x, r3.x
log_pp r1.z, r3.z
log_pp r1.y, r3.y
add_pp r1.xyz, -r1, c3
mov_pp r0.w, c4.z
mad_pp r0.xyz, r0, r1, r2
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 32 ALU, 4 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2, 1, 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TXP R1.xyz, fragment.texcoord[4], texture[1], 2D;
TEX R2, fragment.texcoord[5], texture[2], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R3, fragment.texcoord[5], texture[3], 2D;
MUL R3.xyz, R3.w, R3;
DP3 R1.w, fragment.texcoord[2], fragment.texcoord[2];
MUL R2.xyz, R2.w, R2;
MUL R4.xyz, R3, c[3].w;
MAD R5.xyz, R2, c[3].w, -R4;
DP3 R2.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R2.w, R2.x;
RSQ R1.w, R1.w;
MUL R2.xyz, R1.w, fragment.texcoord[2];
MUL R3.xyz, R2.w, fragment.texcoord[1];
DP3_SAT R2.x, R2, R3;
MUL R1.w, fragment.texcoord[3].x, c[2].x;
ADD R2.y, R1.w, c[3].z;
MUL R1.w, R2.x, c[1].x;
MUL R0, R0, c[0];
MAD R1.w, -R1, R2.y, c[3].y;
MUL R2, R0, R1.w;
MOV_SAT R0.w, fragment.texcoord[5].z;
MAD R3.xyz, R0.w, R5, R4;
MOV_SAT R0.w, R2;
LG2 R1.x, R1.x;
LG2 R1.y, R1.y;
LG2 R1.z, R1.z;
ADD R1.xyz, -R1, R3;
MUL R2.xyz, R0.w, R2;
MAD R0.xyz, R0.w, -R0, R0;
MAD result.color.xyz, R0, R1, R2;
MOV result.color.w, c[3].x;
END
# 32 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_NoiseBias]
Float 2 [_NoiseContrast]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"ps_2_0
; 29 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c3, 1.00000000, 2.00000000, 8.00000000, 0.00000000
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.x
dcl t4
dcl t5.xyz
texldp r1, t4, s1
texld r2, t0, s0
texld r3, t5, s3
texld r0, t5, s2
mul_pp r0.xyz, r0.w, r0
mul_pp r3.xyz, r3.w, r3
mul_pp r3.xyz, r3, c3.z
mad_pp r4.xyz, r0, c3.z, -r3
mov_sat r0.x, t5.z
mad_pp r4.xyz, r0.x, r4, r3
dp3 r3.x, t1, t1
dp3 r0.x, t2, t2
rsq r3.x, r3.x
rsq r0.x, r0.x
mul r3.xyz, r3.x, t1
mul r0.xyz, r0.x, t2
dp3_sat r0.x, r0, r3
mul r3.x, t3, c2
add r3.x, r3, c3
mul r0.x, r0, c1
mad r0.x, -r0, r3, c3.y
mul r2, r2, c0
mul r3, r2, r0.x
mov_pp_sat r0.x, r3.w
mul_pp r3.xyz, r0.x, r3
log_pp r1.x, r1.x
log_pp r1.y, r1.y
log_pp r1.z, r1.z
add_pp r1.xyz, -r1, r4
mad_pp r0.xyz, r0.x, -r2, r2
mov_pp r0.w, c3
mad_pp r0.xyz, r0, r1, r3
mov_pp oC0, r0
"
}
}
 }
}
Fallback "VertexLit"
}