Shader "Marvel/FX/Flag" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _WaveScale ("WaveScale", Vector) = (1,1,1,0)
 _WavePeriod ("WavePeriod", Vector) = (1,1,1,0)
 _WaveAmp ("WaveAmp", Vector) = (1,1,1,0)
 _XScale ("XScale", Float) = 1
 _XBias ("XBias", Float) = 0.25
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "RenderType"="Opaque" }
  Cull Off
Program "vp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [unity_Scale]
Vector 11 [unity_SHAr]
Vector 12 [unity_SHAg]
Vector 13 [unity_SHAb]
Vector 14 [unity_SHBr]
Vector 15 [unity_SHBg]
Vector 16 [unity_SHBb]
Vector 17 [unity_SHC]
Vector 18 [_WaveScale]
Vector 19 [_WavePeriod]
Vector 20 [_WaveAmp]
Float 21 [_XScale]
Float 22 [_XBias]
Vector 23 [_MainTex_ST]
"!!ARBvp1.0
# 132 ALU
PARAM c[28] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..23],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[19];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[18], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[24];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[25].xyxw;
MAD R2.xyz, R2, R1, c[25].zwzw;
MAD R2.xyz, R2, R1, c[26].xyxw;
MAD R2.xyz, R2, R1, c[26].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[24].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[27].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[24].wzww;
DP3 R1.w, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[24];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R2.w, c[24];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[25].xyxw;
MAD R2.xyz, R2, R1, c[25].zwzw;
MAD R2.xyz, R2, R1, c[26].xyxw;
MAD R2.xyz, R2, R1, c[26].zwzw;
ADD R4.xyz, R4, c[25].xyxw;
MAD R4.xyz, R4, R3, c[25].zwzw;
MAD R4.xyz, R4, R3, c[26].xyxw;
MAD R4.xyz, R4, R3, c[26].zwzw;
MAD R2.xyz, R2, R1, c[24].wzww;
FRC R0.x, R0;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[27].xxyw;
MOV R1.xz, R5;
DP3 R1.y, R5, c[24].wzww;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[24].wzww;
SGE R3.yz, R0.w, c[27].xxyw;
SLT R3.x, R0.w, c[0].y;
MUL R0.w, vertex.position.x, c[21].x;
DP3 R4.y, R3, c[24].wzww;
MOV R4.xz, R3;
DP3 R1.y, R2, -R4;
ADD R2.w, R0, c[22].x;
MUL R1.xyz, -R1.wxyw, c[20];
MAD R1.xyz, R2.w, R1, vertex.normal;
DP3 R0.w, R1, R1;
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, R1;
MUL R2.xyz, R1, c[10].w;
DP3 R3.w, R2, c[6];
DP3 R0.w, R2, c[7];
DP3 R1.w, R2, c[5];
MOV R1.y, R0.w;
MOV R1.x, R3.w;
MUL R4, R1.wxyy, R1.xyyw;
MOV R1.z, c[24];
DP4 R3.z, R4, c[16];
DP4 R3.y, R4, c[15];
DP4 R3.x, R4, c[14];
MUL R4.x, R3.w, R3.w;
MAD R4.x, R1.w, R1.w, -R4;
MUL R4.xyz, R4.x, c[17];
DP4 R2.z, R1.wxyz, c[13];
DP4 R2.y, R1.wxyz, c[12];
DP4 R2.x, R1.wxyz, c[11];
ADD R3.xyz, R2, R3;
ADD R1.xyz, -R0.x, c[24];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[25].xyxw;
MAD R2.xyz, R2, R1, c[25].zwzw;
MAD R2.xyz, R2, R1, c[26].xyxw;
MAD R2.xyz, R2, R1, c[26].zwzw;
MAD R1.xyz, R2, R1, c[24].wzww;
ADD result.texcoord[2].xyz, R3, R4;
MAD R0.z, R0, c[0].x, -c[0].y;
FRC R4.y, R0.z;
SLT R2.x, R0, c[0].y;
SGE R2.yz, R0.x, c[27].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R4.w, R0.x;
ADD R0.xyz, -R4.y, c[24];
SLT R4.x, R4.y, c[0].y;
MUL R0.xyz, R0, R0;
MOV R3.xz, R2;
DP3 R3.y, R2, c[24].wzww;
MOV result.texcoord[1].z, R0.w;
DP3 R0.w, R1, -R3;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R4.w, c[24];
MUL R2.xyz, R1, R1;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[25].xyxw;
MAD R3.xyz, R3, R0, c[25].zwzw;
MAD R3.xyz, R3, R0, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
ADD R1.xyz, R1, c[25].xyxw;
MAD R1.xyz, R1, R2, c[25].zwzw;
MAD R1.xyz, R1, R2, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R1.xyz, R1, R2, c[24].wzww;
MAD R3.xyz, R3, R0, c[24].wzww;
SGE R4.yz, R4.y, c[27].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[24].wzww;
DP3 R0.y, R3, -R0;
SLT R2.x, R4.w, c[0].y;
SGE R2.yz, R4.w, c[27].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[24].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[20];
DP3 R0.x, R0, c[24].z;
MUL R0.xyz, vertex.normal, R0.x;
MOV R0.w, vertex.position;
MAD R0.xyz, R0, R2.w, vertex.position;
MOV result.texcoord[1].y, R3.w;
MOV result.texcoord[1].x, R1.w;
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[23], c[23].zwzw;
END
# 132 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [unity_Scale]
Vector 10 [unity_SHAr]
Vector 11 [unity_SHAg]
Vector 12 [unity_SHAb]
Vector 13 [unity_SHBr]
Vector 14 [unity_SHBg]
Vector 15 [unity_SHBb]
Vector 16 [unity_SHC]
Vector 17 [_WaveScale]
Vector 18 [_WavePeriod]
Vector 19 [_WaveAmp]
Float 20 [_XScale]
Float 21 [_XBias]
Vector 22 [_MainTex_ST]
"vs_2_0
; 87 ALU
def c23, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c24, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c25, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c8.w
mul r0.xyz, c18, r0.x
mad r1.xyz, v0, c17, r0
mad r0.x, r1, c23, c23.y
frc r0.x, r0
mad r1.x, r0, c23.z, c23.w
sincos r0.xy, r1.x, c24.xyzw, c25.xyzw
mov r3.x, r0
mad r0.z, r1.y, c23.x, c23.y
mad r0.x, r1.z, c23, c23.y
frc r0.z, r0
mad r0.z, r0, c23, c23.w
sincos r1.xy, r0.z, c24.xyzw, c25.xyzw
frc r0.x, r0
mad r0.x, r0, c23.z, c23.w
sincos r2.xy, r0.x, c24.xyzw, c25.xyzw
mov r3.w, c25.z
mov r3.y, r1.x
mov r3.z, r2.x
mul r0.x, v0, c20
add r2.x, r0, c21
mul r3.xyz, -r3, c19
mad r3.xyz, r2.x, r3, v2
dp3 r0.x, r3, r3
rsq r0.x, r0.x
mul r3.xyz, r0.x, r3
mul r4.xyz, r3, c9.w
dp3 r2.z, r4, c5
dp3 r2.w, r4, c6
dp3 r3.x, r4, c4
mov r3.y, r2.z
mov r3.z, r2.w
mul r4, r3.xyzz, r3.yzzx
dp4 r0.w, r3, c12
dp4 r0.z, r3, c11
dp4 r0.x, r3, c10
mul r1.x, r2.z, r2.z
mad r3.y, r3.x, r3.x, -r1.x
dp4 r1.w, r4, c15
dp4 r1.z, r4, c14
dp4 r1.x, r4, c13
add r0.xzw, r0, r1
mul r4.xyz, r3.y, c16
add oT2.xyz, r0.xzww, r4
mov r0.x, r0.y
mov r0.w, v0
mov r0.z, r2.y
mov r0.y, r1
mul r0.xyz, r0, c19
dp3 r0.x, r0, c25.z
mul r0.xyz, v2, r0.x
mad r0.xyz, r0, r2.x, v0
mov oT1.z, r2.w
mov oT1.y, r2.z
mov oT1.x, r3
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mad oT0.xy, v3, c22, c22.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_Time]
Vector 10 [_WaveScale]
Vector 11 [_WavePeriod]
Vector 12 [_WaveAmp]
Float 13 [_XScale]
Float 14 [_XBias]
Vector 15 [unity_LightmapST]
Vector 16 [_MainTex_ST]
"!!ARBvp1.0
# 61 ALU
PARAM c[21] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..16],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.xyz, c[11];
MUL R0.xyz, R0, c[9].w;
MAD R3.xyz, vertex.position, c[10], R0;
MAD R0.x, R3, c[0], -c[0].y;
FRC R0.w, R0.x;
ADD R0.xyz, -R0.w, c[17];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[18].xyxw;
MAD R1.xyz, R1, R0, c[18].zwzw;
MAD R1.xyz, R1, R0, c[19].xyxw;
MAD R1.xyz, R1, R0, c[19].zwzw;
MAD R0.xyz, R1, R0, c[17].wzww;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[20].xxyw;
MOV R1.xz, R2;
DP3 R1.y, R2, c[17].wzww;
DP3 R0.w, R0, -R1;
MAD R0.y, R3.z, c[0].x, -c[0];
FRC R2.w, R0.y;
MAD R0.x, R3.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R0.xyz, -R2.w, c[17];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R1.w, c[17];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R3.xyz, R3, c[18].xyxw;
MAD R3.xyz, R3, R0, c[18].zwzw;
MAD R3.xyz, R3, R0, c[19].xyxw;
MAD R3.xyz, R3, R0, c[19].zwzw;
ADD R2.xyz, R2, c[18].xyxw;
MAD R2.xyz, R2, R1, c[18].zwzw;
MAD R2.xyz, R2, R1, c[19].xyxw;
MAD R2.xyz, R2, R1, c[19].zwzw;
MAD R1.xyz, R2, R1, c[17].wzww;
MAD R3.xyz, R3, R0, c[17].wzww;
SLT R4.x, R2.w, c[0].y;
SGE R4.yz, R2.w, c[20].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[17].wzww;
DP3 R0.y, R3, -R0;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[20].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[17].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[12];
MUL R0.w, vertex.position.x, c[13].x;
ADD R1.x, R0.w, c[14];
DP3 R0.x, R0, c[17].z;
MUL R0.xyz, vertex.normal, R0.x;
MOV R0.w, vertex.position;
MAD R0.xyz, R0, R1.x, vertex.position;
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[16], c[16].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[15], c[15].zwzw;
END
# 61 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [_Time]
Vector 9 [_WaveScale]
Vector 10 [_WavePeriod]
Vector 11 [_WaveAmp]
Float 12 [_XScale]
Float 13 [_XBias]
Vector 14 [unity_LightmapST]
Vector 15 [_MainTex_ST]
"vs_2_0
; 57 ALU
def c16, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c17, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c18, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
dcl_texcoord1 v4
mov r0.x, c8.w
mul r0.xyz, c10, r0.x
mad r1.xyz, v0, c9, r0
mad r0.x, r1, c18, c18.y
frc r0.x, r0
mad r1.x, r0, c18.z, c18.w
sincos r0.xy, r1.x, c17.xyzw, c16.xyzw
mov r0.x, r0.y
mad r0.z, r1, c18.x, c18.y
frc r0.y, r0.z
mad r0.x, r1.y, c18, c18.y
mad r0.y, r0, c18.z, c18.w
sincos r1.xy, r0.y, c17.xyzw, c16.xyzw
frc r0.x, r0
mad r1.x, r0, c18.z, c18.w
sincos r0.xy, r1.x, c17.xyzw, c16.xyzw
mov r0.z, r1.y
mul r0.xyz, r0, c11
mul r0.w, v0.x, c12.x
add r1.x, r0.w, c13
dp3 r0.x, r0, c16.z
mul r0.xyz, v2, r0.x
mov r0.w, v0
mad r0.xyz, r0, r1.x, v0
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mad oT0.xy, v3, c15, c15.zwzw
mad oT2.xy, v4, c14, c14.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [_ProjectionParams]
Vector 11 [unity_Scale]
Vector 12 [unity_SHAr]
Vector 13 [unity_SHAg]
Vector 14 [unity_SHAb]
Vector 15 [unity_SHBr]
Vector 16 [unity_SHBg]
Vector 17 [unity_SHBb]
Vector 18 [unity_SHC]
Vector 19 [_WaveScale]
Vector 20 [_WavePeriod]
Vector 21 [_WaveAmp]
Float 22 [_XScale]
Float 23 [_XBias]
Vector 24 [_MainTex_ST]
"!!ARBvp1.0
# 138 ALU
PARAM c[29] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..24],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
MOV R0.xyz, c[20];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[19], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[25];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[26].xyxw;
MAD R2.xyz, R2, R1, c[26].zwzw;
MAD R2.xyz, R2, R1, c[27].xyxw;
MAD R2.xyz, R2, R1, c[27].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[25].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[28].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R1.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[25].wzww;
DP3 R6.x, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[25];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R1.w, c[25];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[26].xyxw;
MAD R2.xyz, R2, R1, c[26].zwzw;
MAD R2.xyz, R2, R1, c[27].xyxw;
MAD R2.xyz, R2, R1, c[27].zwzw;
ADD R4.xyz, R4, c[26].xyxw;
MAD R4.xyz, R4, R3, c[26].zwzw;
MAD R4.xyz, R4, R3, c[27].xyxw;
MAD R4.xyz, R4, R3, c[27].zwzw;
MAD R2.xyz, R2, R1, c[25].wzww;
SLT R5.x, R1.w, c[0].y;
SGE R5.yz, R1.w, c[28].xxyw;
FRC R0.x, R0;
MOV R1.xz, R5;
DP3 R1.y, R5, c[25].wzww;
DP3 R6.y, R2, -R1;
MAD R1.xyz, R4, R3, c[25].wzww;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[28].xxyw;
MUL R0.w, vertex.position.x, c[22].x;
MOV R3.xz, R2;
DP3 R3.y, R2, c[25].wzww;
DP3 R6.z, R1, -R3;
ADD R2.xyz, -R0.x, c[25];
MUL R2.xyz, R2, R2;
ADD R2.w, R0, c[23].x;
MUL R1.xyz, -R6, c[21];
MAD R1.xyz, R2.w, R1, vertex.normal;
DP3 R0.w, R1, R1;
RSQ R0.w, R0.w;
MUL R1.xyz, R0.w, R1;
MUL R1.xyz, R1, c[11].w;
DP3 R1.w, R1, c[5];
DP3 R4.w, R1, c[6];
DP3 R3.w, R1, c[7];
MOV R1.x, R4.w;
MOV R1.y, R3.w;
MUL R5, R1.wxyy, R1.xyyw;
MOV R1.z, c[25];
DP4 R4.z, R5, c[17];
DP4 R4.y, R5, c[16];
DP4 R4.x, R5, c[15];
MAD R0.z, R0, c[0].x, -c[0].y;
FRC R5.y, R0.z;
DP4 R3.z, R1.wxyz, c[14];
DP4 R3.y, R1.wxyz, c[13];
DP4 R3.x, R1.wxyz, c[12];
ADD R4.xyz, R3, R4;
MUL R1.xyz, R2, c[0].zwzw;
ADD R1.xyz, R1, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R1.xyz, R1, R2, c[27].xyxw;
MAD R1.xyz, R1, R2, c[27].zwzw;
MAD R2.xyz, R1, R2, c[25].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[28].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R6.x, R0;
ADD R0.xyz, -R5.y, c[25];
SLT R5.x, R5.y, c[0].y;
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[25].wzww;
DP3 R0.w, R2, -R1;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R6.x, c[25];
MUL R2.xyz, R1, R1;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[26].xyxw;
MAD R3.xyz, R3, R0, c[26].zwzw;
MAD R3.xyz, R3, R0, c[27].xyxw;
MAD R3.xyz, R3, R0, c[27].zwzw;
ADD R1.xyz, R1, c[26].xyxw;
MAD R1.xyz, R1, R2, c[26].zwzw;
MAD R1.xyz, R1, R2, c[27].xyxw;
MAD R1.xyz, R1, R2, c[27].zwzw;
MAD R1.xyz, R1, R2, c[25].wzww;
MAD R3.xyz, R3, R0, c[25].wzww;
SGE R5.yz, R5.y, c[28].xxyw;
SGE R2.yz, R6.x, c[28].xxyw;
MOV R0.xz, R5;
DP3 R0.y, R5, c[25].wzww;
DP3 R0.y, R3, -R0;
SLT R2.x, R6, c[0].y;
DP3 R3.y, R2, c[25].wzww;
MOV R3.xz, R2;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[21];
MUL R5.w, R4, R4;
MAD R0.w, R1, R1, -R5;
MUL R1.xyz, R0.w, c[18];
DP3 R0.x, R0, c[25].z;
MUL R0.xyz, vertex.normal, R0.x;
MAD R0.xyz, R0, R2.w, vertex.position;
MOV R0.w, vertex.position;
DP4 R2.w, R0, c[4];
DP4 R2.z, R0, c[3];
DP4 R2.x, R0, c[1];
DP4 R2.y, R0, c[2];
MUL R3.xyz, R2.xyww, c[25].y;
ADD result.texcoord[2].xyz, R4, R1;
MOV R1.x, R3;
MUL R1.y, R3, c[10].x;
ADD result.texcoord[3].xy, R1, R3.z;
MOV result.position, R2;
MOV result.texcoord[3].zw, R2;
MOV result.texcoord[1].z, R3.w;
MOV result.texcoord[1].y, R4.w;
MOV result.texcoord[1].x, R1.w;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[24], c[24].zwzw;
END
# 138 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [_ProjectionParams]
Vector 10 [_ScreenParams]
Vector 11 [unity_Scale]
Vector 12 [unity_SHAr]
Vector 13 [unity_SHAg]
Vector 14 [unity_SHAb]
Vector 15 [unity_SHBr]
Vector 16 [unity_SHBg]
Vector 17 [unity_SHBb]
Vector 18 [unity_SHC]
Vector 19 [_WaveScale]
Vector 20 [_WavePeriod]
Vector 21 [_WaveAmp]
Float 22 [_XScale]
Float 23 [_XBias]
Vector 24 [_MainTex_ST]
"vs_2_0
; 93 ALU
def c25, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c26, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c27, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c8.w
mul r0.xyz, c20, r0.x
mad r0.xyz, v0, c19, r0
mad r0.x, r0, c25, c25.y
frc r0.x, r0
mad r0.x, r0, c25.z, c25.w
sincos r4.xy, r0.x, c26.xyzw, c27.xyzw
mad r0.y, r0, c25.x, c25
mad r0.z, r0, c25.x, c25.y
frc r0.y, r0
mad r0.y, r0, c25.z, c25.w
sincos r3.xy, r0.y, c26.xyzw, c27.xyzw
frc r0.z, r0
mad r0.z, r0, c25, c25.w
sincos r2.xy, r0.z, c26.xyzw, c27.xyzw
mov r0.x, r4
mov r0.y, r3.x
mov r0.z, r2.x
mul r0.w, v0.x, c22.x
add r3.x, r0.w, c23
mul r0.xyz, -r0, c21
mad r0.xyz, r3.x, r0, v2
dp3 r0.w, r0, r0
rsq r0.w, r0.w
mul r0.xyz, r0.w, r0
mul r1.xyz, r0, c11.w
dp3 r4.w, r1, c5
dp3 r3.w, r1, c6
dp3 r0.x, r1, c4
mov r0.y, r4.w
mov r0.z, r3.w
mov r0.w, c27.z
mul r1, r0.xyzz, r0.yzzx
dp4 r2.w, r0, c14
dp4 r2.z, r0, c13
dp4 r2.x, r0, c12
dp4 r0.w, r1, c17
dp4 r0.y, r1, c15
dp4 r0.z, r1, c16
add r0.yzw, r2.xxzw, r0
mul r1.w, r4, r4
mov r2.w, v0
mov r1.x, r4.y
mad r1.w, r0.x, r0.x, -r1
mul r4.xyz, r1.w, c18
mov r1.z, r2.y
mov r1.y, r3
mul r1.xyz, r1, c21
dp3 r1.x, r1, c27.z
mul r1.xyz, v2, r1.x
mad r2.xyz, r1, r3.x, v0
dp4 r1.w, r2, c3
dp4 r1.z, r2, c2
dp4 r1.x, r2, c0
dp4 r1.y, r2, c1
mul r3.xyz, r1.xyww, c25.y
add oT2.xyz, r0.yzww, r4
mov r0.z, r3.x
mul r0.w, r3.y, c9.x
mad oT3.xy, r3.z, c10.zwzw, r0.zwzw
mov oPos, r1
mov oT3.zw, r1
mov oT1.z, r3.w
mov oT1.y, r4.w
mov oT1.x, r0
mad oT0.xy, v3, c24, c24.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_Time]
Vector 10 [_ProjectionParams]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
Vector 16 [unity_LightmapST]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 66 ALU
PARAM c[22] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..17],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.xyz, c[12];
MUL R0.xyz, R0, c[9].w;
MAD R3.xyz, vertex.position, c[11], R0;
MAD R0.x, R3, c[0], -c[0].y;
FRC R0.w, R0.x;
ADD R0.xyz, -R0.w, c[18];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[19].xyxw;
MAD R1.xyz, R1, R0, c[19].zwzw;
MAD R1.xyz, R1, R0, c[20].xyxw;
MAD R1.xyz, R1, R0, c[20].zwzw;
MAD R0.xyz, R1, R0, c[18].wzww;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[21].xxyw;
MOV R1.xz, R2;
DP3 R1.y, R2, c[18].wzww;
DP3 R0.w, R0, -R1;
MAD R0.y, R3.z, c[0].x, -c[0];
FRC R2.w, R0.y;
MAD R0.x, R3.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[18];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R0.xyz, -R2.w, c[18];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[19].xyxw;
MAD R3.xyz, R3, R0, c[19].zwzw;
MAD R3.xyz, R3, R0, c[20].xyxw;
MAD R3.xyz, R3, R0, c[20].zwzw;
ADD R2.xyz, R2, c[19].xyxw;
MAD R2.xyz, R2, R1, c[19].zwzw;
MAD R2.xyz, R2, R1, c[20].xyxw;
MAD R2.xyz, R2, R1, c[20].zwzw;
MAD R1.xyz, R2, R1, c[18].wzww;
MAD R3.xyz, R3, R0, c[18].wzww;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[21].xxyw;
MOV R1.w, vertex.position;
SLT R4.x, R2.w, c[0].y;
SGE R4.yz, R2.w, c[21].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[18].wzww;
DP3 R0.y, R3, -R0;
MOV R3.xz, R2;
DP3 R3.y, R2, c[18].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[13];
DP3 R0.x, R0, c[18].z;
MUL R0.w, vertex.position.x, c[14].x;
MUL R0.xyz, vertex.normal, R0.x;
ADD R0.w, R0, c[15].x;
MAD R1.xyz, R0, R0.w, vertex.position;
DP4 R0.w, R1, c[4];
DP4 R0.z, R1, c[3];
DP4 R0.x, R1, c[1];
DP4 R0.y, R1, c[2];
MUL R2.xyz, R0.xyww, c[18].y;
MUL R2.y, R2, c[10].x;
ADD result.texcoord[3].xy, R2, R2.z;
MOV result.position, R0;
MOV result.texcoord[3].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[16], c[16].zwzw;
END
# 66 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [_Time]
Vector 9 [_ProjectionParams]
Vector 10 [_ScreenParams]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
Vector 16 [unity_LightmapST]
Vector 17 [_MainTex_ST]
"vs_2_0
; 62 ALU
def c18, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c19, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c20, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
dcl_texcoord1 v4
mov r0.x, c8.w
mul r0.xyz, c12, r0.x
mad r1.xyz, v0, c11, r0
mad r0.x, r1, c20, c20.y
frc r0.x, r0
mad r1.x, r0, c20.z, c20.w
sincos r0.xy, r1.x, c19.xyzw, c18.xyzw
mov r0.x, r0.y
mad r0.z, r1, c20.x, c20.y
frc r0.y, r0.z
mad r0.x, r1.y, c20, c20.y
mad r0.y, r0, c20.z, c20.w
sincos r1.xy, r0.y, c19.xyzw, c18.xyzw
frc r0.x, r0
mad r1.x, r0, c20.z, c20.w
sincos r0.xy, r1.x, c19.xyzw, c18.xyzw
mov r0.z, r1.y
mul r0.xyz, r0, c13
dp3 r0.x, r0, c18.z
mul r0.w, v0.x, c14.x
mul r0.xyz, v2, r0.x
add r0.w, r0, c15.x
mad r1.xyz, r0, r0.w, v0
mov r1.w, v0
dp4 r0.w, r1, c3
dp4 r0.z, r1, c2
dp4 r0.x, r1, c0
dp4 r0.y, r1, c1
mul r2.xyz, r0.xyww, c18.w
mul r2.y, r2, c9.x
mad oT3.xy, r2.z, c10.zwzw, r2
mov oPos, r0
mov oT3.zw, r0
mad oT0.xy, v3, c17, c17.zwzw
mad oT2.xy, v4, c16, c16.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [unity_Scale]
Vector 11 [unity_4LightPosX0]
Vector 12 [unity_4LightPosY0]
Vector 13 [unity_4LightPosZ0]
Vector 14 [unity_4LightAtten0]
Vector 15 [unity_LightColor0]
Vector 16 [unity_LightColor1]
Vector 17 [unity_LightColor2]
Vector 18 [unity_LightColor3]
Vector 19 [unity_SHAr]
Vector 20 [unity_SHAg]
Vector 21 [unity_SHAb]
Vector 22 [unity_SHBr]
Vector 23 [unity_SHBg]
Vector 24 [unity_SHBb]
Vector 25 [unity_SHC]
Vector 26 [_WaveScale]
Vector 27 [_WavePeriod]
Vector 28 [_WaveAmp]
Float 29 [_XScale]
Float 30 [_XBias]
Vector 31 [_MainTex_ST]
"!!ARBvp1.0
# 162 ALU
PARAM c[36] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..31],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[27];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[26], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[32];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[33].xyxw;
MAD R2.xyz, R2, R1, c[33].zwzw;
MAD R2.xyz, R2, R1, c[34].xyxw;
MAD R2.xyz, R2, R1, c[34].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[32].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[35].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[32].wzww;
DP3 R1.w, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[32];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R2.w, c[32];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
MAD R0.z, R0, c[0].x, -c[0].y;
ADD R2.xyz, R2, c[33].xyxw;
MAD R2.xyz, R2, R1, c[33].zwzw;
MAD R2.xyz, R2, R1, c[34].xyxw;
MAD R2.xyz, R2, R1, c[34].zwzw;
ADD R4.xyz, R4, c[33].xyxw;
MAD R4.xyz, R4, R3, c[33].zwzw;
MAD R4.xyz, R4, R3, c[34].xyxw;
MAD R4.xyz, R4, R3, c[34].zwzw;
MAD R2.xyz, R2, R1, c[32].wzww;
FRC R3.w, R0.z;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[35].xxyw;
MOV R1.xz, R5;
DP3 R1.y, R5, c[32].wzww;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[32].wzww;
SGE R5.yz, R3.w, c[35].xxyw;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[35].xxyw;
MUL R0.w, vertex.position.x, c[29].x;
FRC R0.x, R0;
ADD R2.w, R0, c[30].x;
SLT R5.x, R3.w, c[0].y;
MOV R5.w, c[32].z;
DP3 R4.y, R3, c[32].wzww;
MOV R4.xz, R3;
DP3 R1.y, R2, -R4;
ADD R3.xyz, -R0.x, c[32];
MUL R2.xyz, R3, R3;
MUL R3.xyz, -R1.wxyw, c[28];
MAD R3.xyz, R2.w, R3, vertex.normal;
DP3 R0.w, R3, R3;
MUL R1.xyz, R2, c[0].zwzw;
RSQ R0.w, R0.w;
MUL R4.xyz, R0.w, R3;
MUL R4.xyz, R4, c[10].w;
DP3 R4.w, R4, c[6];
ADD R1.xyz, R1, c[33].xyxw;
MAD R1.xyz, R1, R2, c[33].zwzw;
MAD R1.xyz, R1, R2, c[34].xyxw;
MAD R1.xyz, R1, R2, c[34].zwzw;
MAD R2.xyz, R1, R2, c[32].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[35].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R0.xyz, -R3.w, c[32];
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[32].wzww;
DP3 R0.w, R2, -R1;
ADD R1.xyz, -R1.w, c[32];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R0, c[0].zwzw;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[33].xyxw;
MAD R3.xyz, R3, R0, c[33].zwzw;
MAD R3.xyz, R3, R0, c[34].xyxw;
MAD R3.xyz, R3, R0, c[34].zwzw;
MAD R3.xyz, R3, R0, c[32].wzww;
ADD R1.xyz, R1, c[33].xyxw;
MAD R1.xyz, R1, R2, c[33].zwzw;
MAD R1.xyz, R1, R2, c[34].xyxw;
MAD R1.xyz, R1, R2, c[34].zwzw;
MAD R1.xyz, R1, R2, c[32].wzww;
MOV R0.xz, R5;
DP3 R0.y, R5, c[32].wzww;
DP3 R5.x, R4, c[5];
DP3 R4.x, R4, c[7];
MOV R5.z, R4.x;
DP3 R0.y, R3, -R0;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[35].xxyw;
MOV R1.w, vertex.position;
MOV R3.xz, R2;
DP3 R3.y, R2, c[32].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[28];
DP3 R0.x, R0, c[32].z;
MUL R0.xyz, vertex.normal, R0.x;
MAD R1.xyz, R0, R2.w, vertex.position;
DP4 R0.x, R1, c[6];
ADD R2, -R0.x, c[12];
MUL R3, R4.w, R2;
DP4 R0.x, R1, c[5];
ADD R0, -R0.x, c[11];
MUL R2, R2, R2;
MAD R3, R5.x, R0, R3;
DP4 R5.y, R1, c[7];
MAD R2, R0, R0, R2;
ADD R0, -R5.y, c[13];
MAD R2, R0, R0, R2;
MAD R0, R4.x, R0, R3;
MUL R3, R2, c[14];
MOV R5.y, R4.w;
RSQ R2.x, R2.x;
RSQ R2.y, R2.y;
RSQ R2.w, R2.w;
RSQ R2.z, R2.z;
MUL R0, R0, R2;
ADD R2, R3, c[32].z;
DP4 R3.z, R5, c[21];
DP4 R3.y, R5, c[20];
DP4 R3.x, R5, c[19];
RCP R2.x, R2.x;
RCP R2.y, R2.y;
RCP R2.w, R2.w;
RCP R2.z, R2.z;
MAX R0, R0, c[32].x;
MUL R0, R0, R2;
MUL R2.xyz, R0.y, c[16];
MAD R2.xyz, R0.x, c[15], R2;
MAD R0.xyz, R0.z, c[17], R2;
MAD R2.xyz, R0.w, c[18], R0;
MUL R0, R5.xyzz, R5.yzzx;
MUL R2.w, R4, R4;
DP4 R5.w, R0, c[24];
DP4 R5.z, R0, c[23];
DP4 R5.y, R0, c[22];
MAD R2.w, R5.x, R5.x, -R2;
MUL R0.xyz, R2.w, c[25];
ADD R3.xyz, R3, R5.yzww;
ADD R0.xyz, R3, R0;
ADD result.texcoord[2].xyz, R0, R2;
MOV result.texcoord[1].z, R4.x;
MOV result.texcoord[1].y, R4.w;
MOV result.texcoord[1].x, R5;
DP4 result.position.w, R1, c[4];
DP4 result.position.z, R1, c[3];
DP4 result.position.y, R1, c[2];
DP4 result.position.x, R1, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[31], c[31].zwzw;
END
# 162 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [unity_Scale]
Vector 10 [unity_4LightPosX0]
Vector 11 [unity_4LightPosY0]
Vector 12 [unity_4LightPosZ0]
Vector 13 [unity_4LightAtten0]
Vector 14 [unity_LightColor0]
Vector 15 [unity_LightColor1]
Vector 16 [unity_LightColor2]
Vector 17 [unity_LightColor3]
Vector 18 [unity_SHAr]
Vector 19 [unity_SHAg]
Vector 20 [unity_SHAb]
Vector 21 [unity_SHBr]
Vector 22 [unity_SHBg]
Vector 23 [unity_SHBb]
Vector 24 [unity_SHC]
Vector 25 [_WaveScale]
Vector 26 [_WavePeriod]
Vector 27 [_WaveAmp]
Float 28 [_XScale]
Float 29 [_XBias]
Vector 30 [_MainTex_ST]
"vs_2_0
; 116 ALU
def c31, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c32, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c33, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c34, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c8.w
mul r0.xyz, c26, r0.x
mad r0.xyz, v0, c25, r0
mad r0.x, r0, c31, c31.y
frc r0.x, r0
mad r0.x, r0, c31.z, c31.w
sincos r2.xy, r0.x, c32.xyzw, c33.xyzw
mad r0.x, r0.y, c31, c31.y
mad r0.y, r0.z, c31.x, c31
frc r0.x, r0
mad r0.x, r0, c31.z, c31.w
sincos r1.xy, r0.x, c32.xyzw, c33.xyzw
mov r3.y, r1.x
mov r1.w, v0
mov r5.w, c33.z
mov r3.x, r2
frc r0.y, r0
mad r2.x, r0.y, c31.z, c31.w
sincos r0.xy, r2.x, c32.xyzw, c33.xyzw
mov r3.z, r0.x
mul r0.x, v0, c28
add r0.x, r0, c29
mul r3.xyz, -r3, c27
mad r3.xyz, r0.x, r3, v2
dp3 r0.z, r3, r3
rsq r0.z, r0.z
mul r3.xyz, r0.z, r3
mul r4.xyz, r3, c9.w
dp3 r5.x, r4, c4
dp3 r4.w, r4, c5
dp3 r4.x, r4, c6
mov r5.z, r4.x
mov r1.x, r2.y
mov r1.z, r0.y
mul r1.xyz, r1, c27
dp3 r0.y, r1, c33.z
mul r1.xyz, v2, r0.y
mad r1.xyz, r1, r0.x, v0
dp4 r0.x, r1, c5
add r2, -r0.x, c11
mul r3, r4.w, r2
dp4 r0.x, r1, c4
add r0, -r0.x, c10
mul r2, r2, r2
mad r3, r5.x, r0, r3
dp4 r5.y, r1, c6
mad r2, r0, r0, r2
add r0, -r5.y, c12
mad r2, r0, r0, r2
mad r0, r4.x, r0, r3
mul r3, r2, c13
mov r5.y, r4.w
rsq r2.x, r2.x
rsq r2.y, r2.y
rsq r2.w, r2.w
rsq r2.z, r2.z
mul r0, r0, r2
add r2, r3, c33.z
dp4 r3.z, r5, c20
dp4 r3.y, r5, c19
dp4 r3.x, r5, c18
rcp r2.x, r2.x
rcp r2.y, r2.y
rcp r2.w, r2.w
rcp r2.z, r2.z
max r0, r0, c34.x
mul r0, r0, r2
mul r2.xyz, r0.y, c15
mad r2.xyz, r0.x, c14, r2
mad r0.xyz, r0.z, c16, r2
mad r2.xyz, r0.w, c17, r0
mul r0, r5.xyzz, r5.yzzx
mul r2.w, r4, r4
dp4 r5.w, r0, c23
dp4 r5.z, r0, c22
dp4 r5.y, r0, c21
mad r2.w, r5.x, r5.x, -r2
mul r0.xyz, r2.w, c24
add r3.xyz, r3, r5.yzww
add r0.xyz, r3, r0
add oT2.xyz, r0, r2
mov oT1.z, r4.x
mov oT1.y, r4.w
mov oT1.x, r5
dp4 oPos.w, r1, c3
dp4 oPos.z, r1, c2
dp4 oPos.y, r1, c1
dp4 oPos.x, r1, c0
mad oT0.xy, v3, c30, c30.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [_ProjectionParams]
Vector 11 [unity_Scale]
Vector 12 [unity_4LightPosX0]
Vector 13 [unity_4LightPosY0]
Vector 14 [unity_4LightPosZ0]
Vector 15 [unity_4LightAtten0]
Vector 16 [unity_LightColor0]
Vector 17 [unity_LightColor1]
Vector 18 [unity_LightColor2]
Vector 19 [unity_LightColor3]
Vector 20 [unity_SHAr]
Vector 21 [unity_SHAg]
Vector 22 [unity_SHAb]
Vector 23 [unity_SHBr]
Vector 24 [unity_SHBg]
Vector 25 [unity_SHBb]
Vector 26 [unity_SHC]
Vector 27 [_WaveScale]
Vector 28 [_WavePeriod]
Vector 29 [_WaveAmp]
Float 30 [_XScale]
Float 31 [_XBias]
Vector 32 [_MainTex_ST]
"!!ARBvp1.0
# 168 ALU
PARAM c[37] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..32],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[28];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[27], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[33];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[34].xyxw;
MAD R2.xyz, R2, R1, c[34].zwzw;
MAD R2.xyz, R2, R1, c[35].xyxw;
MAD R2.xyz, R2, R1, c[35].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[33].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[36].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[33].wzww;
DP3 R1.w, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[33];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R2.w, c[33];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
MAD R0.z, R0, c[0].x, -c[0].y;
ADD R2.xyz, R2, c[34].xyxw;
MAD R2.xyz, R2, R1, c[34].zwzw;
MAD R2.xyz, R2, R1, c[35].xyxw;
MAD R2.xyz, R2, R1, c[35].zwzw;
ADD R4.xyz, R4, c[34].xyxw;
MAD R4.xyz, R4, R3, c[34].zwzw;
MAD R4.xyz, R4, R3, c[35].xyxw;
MAD R4.xyz, R4, R3, c[35].zwzw;
MAD R2.xyz, R2, R1, c[33].wzww;
FRC R3.w, R0.z;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[36].xxyw;
MOV R1.xz, R5;
DP3 R1.y, R5, c[33].wzww;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[33].wzww;
SGE R5.yz, R3.w, c[36].xxyw;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[36].xxyw;
MUL R0.w, vertex.position.x, c[30].x;
FRC R0.x, R0;
ADD R2.w, R0, c[31].x;
SLT R5.x, R3.w, c[0].y;
MOV R5.w, c[33].z;
DP3 R4.y, R3, c[33].wzww;
MOV R4.xz, R3;
DP3 R1.y, R2, -R4;
ADD R3.xyz, -R0.x, c[33];
MUL R2.xyz, R3, R3;
MUL R3.xyz, -R1.wxyw, c[29];
MAD R3.xyz, R2.w, R3, vertex.normal;
DP3 R0.w, R3, R3;
MUL R1.xyz, R2, c[0].zwzw;
RSQ R0.w, R0.w;
MUL R4.xyz, R0.w, R3;
MUL R4.xyz, R4, c[11].w;
DP3 R4.w, R4, c[6];
ADD R1.xyz, R1, c[34].xyxw;
MAD R1.xyz, R1, R2, c[34].zwzw;
MAD R1.xyz, R1, R2, c[35].xyxw;
MAD R1.xyz, R1, R2, c[35].zwzw;
MAD R2.xyz, R1, R2, c[33].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[36].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R0.xyz, -R3.w, c[33];
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[33].wzww;
DP3 R0.w, R2, -R1;
ADD R1.xyz, -R1.w, c[33];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R0, c[0].zwzw;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[34].xyxw;
MAD R3.xyz, R3, R0, c[34].zwzw;
MAD R3.xyz, R3, R0, c[35].xyxw;
MAD R3.xyz, R3, R0, c[35].zwzw;
MAD R3.xyz, R3, R0, c[33].wzww;
ADD R1.xyz, R1, c[34].xyxw;
MAD R1.xyz, R1, R2, c[34].zwzw;
MAD R1.xyz, R1, R2, c[35].xyxw;
MAD R1.xyz, R1, R2, c[35].zwzw;
MAD R1.xyz, R1, R2, c[33].wzww;
MOV R0.xz, R5;
DP3 R0.y, R5, c[33].wzww;
DP3 R5.x, R4, c[5];
DP3 R4.x, R4, c[7];
MOV R5.z, R4.x;
DP3 R0.y, R3, -R0;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[36].xxyw;
MOV R1.w, vertex.position;
MOV R3.xz, R2;
DP3 R3.y, R2, c[33].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[29];
DP3 R0.x, R0, c[33].z;
MUL R0.xyz, vertex.normal, R0.x;
MAD R1.xyz, R0, R2.w, vertex.position;
DP4 R0.x, R1, c[6];
ADD R2, -R0.x, c[13];
MUL R3, R4.w, R2;
DP4 R0.x, R1, c[5];
ADD R0, -R0.x, c[12];
MUL R2, R2, R2;
MAD R3, R5.x, R0, R3;
DP4 R5.y, R1, c[7];
MAD R2, R0, R0, R2;
ADD R0, -R5.y, c[14];
MAD R2, R0, R0, R2;
MAD R0, R4.x, R0, R3;
MUL R3, R2, c[15];
MOV R5.y, R4.w;
RSQ R2.x, R2.x;
RSQ R2.y, R2.y;
RSQ R2.w, R2.w;
RSQ R2.z, R2.z;
MUL R0, R0, R2;
ADD R2, R3, c[33].z;
DP4 R3.z, R5, c[22];
DP4 R3.y, R5, c[21];
DP4 R3.x, R5, c[20];
RCP R2.x, R2.x;
RCP R2.y, R2.y;
RCP R2.w, R2.w;
RCP R2.z, R2.z;
MAX R0, R0, c[33].x;
MUL R0, R0, R2;
MUL R2.xyz, R0.y, c[17];
MAD R2.xyz, R0.x, c[16], R2;
MAD R0.xyz, R0.z, c[18], R2;
MAD R2.xyz, R0.w, c[19], R0;
MUL R0, R5.xyzz, R5.yzzx;
MUL R2.w, R4, R4;
DP4 R5.w, R0, c[25];
DP4 R5.z, R0, c[24];
DP4 R5.y, R0, c[23];
MAD R2.w, R5.x, R5.x, -R2;
MUL R0.xyz, R2.w, c[26];
ADD R3.xyz, R3, R5.yzww;
ADD R5.yzw, R3.xxyz, R0.xxyz;
DP4 R0.w, R1, c[4];
DP4 R0.z, R1, c[3];
DP4 R0.x, R1, c[1];
DP4 R0.y, R1, c[2];
MUL R3.xyz, R0.xyww, c[33].y;
ADD result.texcoord[2].xyz, R5.yzww, R2;
MOV R2.x, R3;
MUL R2.y, R3, c[10].x;
ADD result.texcoord[3].xy, R2, R3.z;
MOV result.position, R0;
MOV result.texcoord[3].zw, R0;
MOV result.texcoord[1].z, R4.x;
MOV result.texcoord[1].y, R4.w;
MOV result.texcoord[1].x, R5;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[32], c[32].zwzw;
END
# 168 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [_ProjectionParams]
Vector 10 [_ScreenParams]
Vector 11 [unity_Scale]
Vector 12 [unity_4LightPosX0]
Vector 13 [unity_4LightPosY0]
Vector 14 [unity_4LightPosZ0]
Vector 15 [unity_4LightAtten0]
Vector 16 [unity_LightColor0]
Vector 17 [unity_LightColor1]
Vector 18 [unity_LightColor2]
Vector 19 [unity_LightColor3]
Vector 20 [unity_SHAr]
Vector 21 [unity_SHAg]
Vector 22 [unity_SHAb]
Vector 23 [unity_SHBr]
Vector 24 [unity_SHBg]
Vector 25 [unity_SHBb]
Vector 26 [unity_SHC]
Vector 27 [_WaveScale]
Vector 28 [_WavePeriod]
Vector 29 [_WaveAmp]
Float 30 [_XScale]
Float 31 [_XBias]
Vector 32 [_MainTex_ST]
"vs_2_0
; 122 ALU
def c33, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c34, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c35, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c36, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c8.w
mul r0.xyz, c28, r0.x
mad r0.xyz, v0, c27, r0
mad r0.x, r0, c33, c33.y
frc r0.x, r0
mad r0.x, r0, c33.z, c33.w
sincos r2.xy, r0.x, c34.xyzw, c35.xyzw
mad r0.x, r0.y, c33, c33.y
mad r0.y, r0.z, c33.x, c33
frc r0.x, r0
mad r0.x, r0, c33.z, c33.w
sincos r1.xy, r0.x, c34.xyzw, c35.xyzw
mov r3.y, r1.x
mov r1.w, v0
mov r5.w, c35.z
mov r3.x, r2
frc r0.y, r0
mad r2.x, r0.y, c33.z, c33.w
sincos r0.xy, r2.x, c34.xyzw, c35.xyzw
mov r3.z, r0.x
mul r0.x, v0, c30
add r0.x, r0, c31
mul r3.xyz, -r3, c29
mad r3.xyz, r0.x, r3, v2
dp3 r0.z, r3, r3
rsq r0.z, r0.z
mul r3.xyz, r0.z, r3
mul r4.xyz, r3, c11.w
dp3 r5.x, r4, c4
dp3 r4.w, r4, c5
dp3 r4.x, r4, c6
mov r5.z, r4.x
mov r1.x, r2.y
mov r1.z, r0.y
mul r1.xyz, r1, c29
dp3 r0.y, r1, c35.z
mul r1.xyz, v2, r0.y
mad r1.xyz, r1, r0.x, v0
dp4 r0.x, r1, c5
add r2, -r0.x, c13
mul r3, r4.w, r2
dp4 r0.x, r1, c4
add r0, -r0.x, c12
mul r2, r2, r2
mad r3, r5.x, r0, r3
dp4 r5.y, r1, c6
mad r2, r0, r0, r2
add r0, -r5.y, c14
mad r2, r0, r0, r2
mad r0, r4.x, r0, r3
mul r3, r2, c15
mov r5.y, r4.w
rsq r2.x, r2.x
rsq r2.y, r2.y
rsq r2.w, r2.w
rsq r2.z, r2.z
mul r0, r0, r2
add r2, r3, c35.z
dp4 r3.z, r5, c22
dp4 r3.y, r5, c21
dp4 r3.x, r5, c20
rcp r2.x, r2.x
rcp r2.y, r2.y
rcp r2.w, r2.w
rcp r2.z, r2.z
max r0, r0, c36.x
mul r0, r0, r2
mul r2.xyz, r0.y, c17
mad r2.xyz, r0.x, c16, r2
mad r0.xyz, r0.z, c18, r2
mad r2.xyz, r0.w, c19, r0
mul r0, r5.xyzz, r5.yzzx
mul r2.w, r4, r4
dp4 r5.w, r0, c25
dp4 r5.z, r0, c24
dp4 r5.y, r0, c23
mad r2.w, r5.x, r5.x, -r2
mul r0.xyz, r2.w, c26
add r3.xyz, r3, r5.yzww
add r5.yzw, r3.xxyz, r0.xxyz
dp4 r0.w, r1, c3
dp4 r0.z, r1, c2
dp4 r0.x, r1, c0
dp4 r0.y, r1, c1
mul r3.xyz, r0.xyww, c33.y
add oT2.xyz, r5.yzww, r2
mov r2.x, r3
mul r2.y, r3, c9.x
mad oT3.xy, r3.z, c10.zwzw, r2
mov oPos, r0
mov oT3.zw, r0
mov oT1.z, r4.x
mov oT1.y, r4.w
mov oT1.x, r5
mad oT0.xy, v3, c32, c32.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 1 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MUL R1.xyz, R0, fragment.texcoord[2];
DP3 R1.w, fragment.texcoord[1], c[0];
MUL R0.xyz, R0, c[1];
MAX R1.w, R1, c[2].x;
MUL R0.xyz, R1.w, R0;
MAD result.color.xyz, R0, c[2].y, R1;
MOV result.color.w, R0;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 8 ALU, 1 TEX
dcl_2d s0
def c2, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
texld r1, t0, s0
mul_pp r2.xyz, r1, t2
dp3_pp r0.x, t1, c0
mov_pp r0.w, r1
mul_pp r1.xyz, r1, c1
max_pp r0.x, r0, c2
mul_pp r0.xyz, r0.x, r1
mad_pp r0.xyz, r0, c2.y, r2
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[2], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
MUL R0.xyz, R0, R1;
MUL result.color.xyz, R0, c[0].x;
MOV result.color.w, R0;
END
# 6 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 8.00000000, 0, 0, 0
dcl t0.xy
dcl t2.xy
texld r0, t2, s1
texld r1, t0, s0
mul_pp r0.xyz, r0.w, r0
mul_pp r0.xyz, r1, r0
mul_pp r0.xyz, r0, c0.x
mov_pp r0.w, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TXP R2.x, fragment.texcoord[3], texture[1], 2D;
MUL R1.xyz, R0, fragment.texcoord[2];
DP3 R1.w, fragment.texcoord[1], c[0];
MAX R1.w, R1, c[2].x;
MUL R0.xyz, R0, c[1];
MUL R1.w, R1, R2.x;
MUL R0.xyz, R1.w, R0;
MAD result.color.xyz, R0, c[2].y, R1;
MOV result.color.w, R0;
END
# 10 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"ps_2_0
; 9 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3
texld r1, t0, s0
texldp r3, t3, s1
mul_pp r2.xyz, r1, c1
dp3_pp r0.x, t1, c0
max_pp r0.x, r0, c2
mul_pp r0.x, r0, r3
mul_pp r0.xyz, r0.x, r2
mul_pp r1.xyz, r1, t2
mov_pp r0.w, r1
mad_pp r0.xyz, r0, c2.y, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 3 TEX
PARAM c[1] = { { 8, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[2], texture[2], 2D;
TXP R2.x, fragment.texcoord[3], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
MUL R1.w, R2.x, c[0].y;
MUL R1.xyz, R1, c[0].x;
MIN R1.xyz, R1, R1.w;
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 9 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"ps_2_0
; 7 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c0, 8.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t2.xy
dcl t3
texld r2, t0, s0
texld r0, t2, s2
texldp r1, t3, s1
mul_pp r0.xyz, r0.w, r0
mul_pp r1.x, r1, c0.y
mul_pp r0.xyz, r0, c0.x
min_pp r0.xyz, r0, r1.x
mul_pp r0.xyz, r2, r0
mov_pp r0.w, r2
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Cull Off
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
Matrix 9 [_LightMatrix0]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceLightPos0]
Vector 16 [_WaveScale]
Vector 17 [_WavePeriod]
Vector 18 [_WaveAmp]
Float 19 [_XScale]
Float 20 [_XBias]
Vector 21 [_MainTex_ST]
"!!ARBvp1.0
# 121 ALU
PARAM c[26] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
MOV R0.xyz, c[17];
MUL R0.xyz, R0, c[13].w;
MAD R5.xyz, vertex.position, c[16], R0;
MAD R0.x, R5, c[0], -c[0].y;
FRC R1.w, R0.x;
MAD R0.y, R5, c[0].x, -c[0];
FRC R0.w, R0.y;
ADD R1.xyz, -R1.w, c[22];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R2, c[0].zwzw;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R3.xyz, R3, R2, c[24].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
MAD R0.xyz, R1, R0, c[22].wzww;
MAD R3.xyz, R3, R2, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R2.xz, R4;
DP3 R2.y, R4, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MAD R0.w, R5.z, c[0].x, -c[0].y;
FRC R1.w, R0;
DP3 R1.y, R1, c[22].wzww;
DP3 R6.y, R0, -R1;
ADD R0.xyz, -R1.w, c[22];
MUL R0.w, R5.x, c[0].x;
MUL R1.xyz, R0, R0;
FRC R0.w, R0;
DP3 R6.x, R3, -R2;
MUL R2.xyz, R1, c[0].zwzw;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R2.xyz, R2, c[23].xyxw;
MAD R2.xyz, R2, R1, c[23].zwzw;
MAD R2.xyz, R2, R1, c[24].xyxw;
MAD R2.xyz, R2, R1, c[24].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R0, c[23].zwzw;
MAD R3.xyz, R3, R0, c[24].xyxw;
MAD R2.xyz, R2, R1, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R1.xz, R4;
DP3 R1.y, R4, c[22].wzww;
DP3 R6.z, R2, -R1;
MAD R1.xyz, R3, R0, c[24].zwzw;
MAD R0.xyz, R1, R0, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MOV R3.xz, R1;
DP3 R3.y, R1, c[22].wzww;
DP3 R0.w, R0, -R3;
MUL R0.x, R5.y, c[0];
FRC R2.w, R0.x;
MUL R0.y, R5.z, c[0].x;
FRC R1.w, R0.y;
ADD R1.xyz, -R1.w, c[22];
MUL R3.xyz, R1, R1;
MUL R4.xyz, R3, c[0].zwzw;
ADD R0.xyz, -R2.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
ADD R4.xyz, R4, c[23].xyxw;
MAD R4.xyz, R4, R3, c[23].zwzw;
MAD R4.xyz, R4, R3, c[24].xyxw;
MAD R4.xyz, R4, R3, c[24].zwzw;
MAD R1.xyz, R1, R0, c[22].wzww;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[25].xxyw;
MOV R2.w, vertex.position;
MUL R2.xyz, R6, c[18];
MOV R0.xz, R5;
DP3 R0.y, R5, c[22].wzww;
DP3 R0.x, R1, -R0;
MAD R1.xyz, R4, R3, c[22].wzww;
SLT R3.x, R1.w, c[0].y;
SGE R3.yz, R1.w, c[25].xxyw;
MUL R0.z, vertex.position.x, c[19].x;
ADD R1.w, R0.z, c[20].x;
MOV R4.xz, R3;
DP3 R4.y, R3, c[22].wzww;
DP3 R0.y, R1, -R4;
MUL R1.xyz, -R0.wxyw, c[18];
MAD R0.xyz, R1.w, R1, vertex.normal;
DP3 R0.w, R2, c[22].z;
MUL R1.xyz, vertex.normal, R0.w;
MAD R2.xyz, R1, R1.w, vertex.position;
DP3 R3.x, R0, R0;
RSQ R0.w, R3.x;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, c[14].w;
DP4 R1.z, R2, c[7];
DP4 R1.x, R2, c[5];
DP4 R1.y, R2, c[6];
DP4 R1.w, R2, c[8];
DP4 result.texcoord[3].z, R1, c[11];
DP4 result.texcoord[3].y, R1, c[10];
DP4 result.texcoord[3].x, R1, c[9];
ADD result.texcoord[2].xyz, -R1, c[15];
DP3 result.texcoord[1].z, R0, c[7];
DP3 result.texcoord[1].y, R0, c[6];
DP3 result.texcoord[1].x, R0, c[5];
DP4 result.position.w, R2, c[4];
DP4 result.position.z, R2, c[3];
DP4 result.position.y, R2, c[2];
DP4 result.position.x, R2, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[21], c[21].zwzw;
END
# 121 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_WaveScale]
Vector 16 [_WavePeriod]
Vector 17 [_WaveAmp]
Float 18 [_XScale]
Float 19 [_XBias]
Vector 20 [_MainTex_ST]
"vs_2_0
; 75 ALU
def c21, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c12.w
mul r0.xyz, c16, r0.x
mad r0.xyz, v0, c15, r0
mad r0.x, r0, c21, c21.y
frc r0.x, r0
mad r0.x, r0, c21.z, c21.w
sincos r2.xy, r0.x, c22.xyzw, c23.xyzw
mad r0.x, r0.y, c21, c21.y
mad r0.z, r0, c21.x, c21.y
frc r0.y, r0.z
frc r0.x, r0
mad r1.x, r0.y, c21.z, c21.w
mad r2.z, r0.x, c21, c21.w
sincos r0.xy, r1.x, c22.xyzw, c23.xyzw
sincos r1.xy, r2.z, c22.xyzw, c23.xyzw
mov r1.w, v0
mov r2.w, r0.y
mov r2.z, r1.y
mul r0.yzw, r2, c17.xxyz
mov r2.z, r0.x
mov r2.y, r1.x
mul r0.x, v0, c18
add r1.x, r0, c19
dp3 r0.x, r0.yzww, c23.z
mul r2.xyz, -r2, c17
mad r2.xyz, r1.x, r2, v2
mul r0.xyz, v2, r0.x
mad r1.xyz, r0, r1.x, v0
dp3 r2.w, r2, r2
rsq r2.w, r2.w
mul r2.xyz, r2.w, r2
mul r2.xyz, r2, c13.w
dp4 r0.z, r1, c6
dp4 r0.x, r1, c4
dp4 r0.y, r1, c5
dp4 r0.w, r1, c7
dp4 oT3.z, r0, c10
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
add oT2.xyz, -r0, c14
dp3 oT1.z, r2, c6
dp3 oT1.y, r2, c5
dp3 oT1.x, r2, c4
dp4 oPos.w, r1, c3
dp4 oPos.z, r1, c2
dp4 oPos.y, r1, c1
dp4 oPos.x, r1, c0
mad oT0.xy, v3, c20, c20.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [unity_Scale]
Vector 11 [_WorldSpaceLightPos0]
Vector 12 [_WaveScale]
Vector 13 [_WavePeriod]
Vector 14 [_WaveAmp]
Float 15 [_XScale]
Float 16 [_XBias]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 114 ALU
PARAM c[22] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..17],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[13];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[12], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[18];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[19].xyxw;
MAD R2.xyz, R2, R1, c[19].zwzw;
MAD R2.xyz, R2, R1, c[20].xyxw;
MAD R2.xyz, R2, R1, c[20].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[18].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[21].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[18].wzww;
DP3 R1.w, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[18];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R2.w, c[18];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
MAD R0.z, R0, c[0].x, -c[0].y;
ADD R2.xyz, R2, c[19].xyxw;
MAD R2.xyz, R2, R1, c[19].zwzw;
MAD R2.xyz, R2, R1, c[20].xyxw;
MAD R2.xyz, R2, R1, c[20].zwzw;
ADD R4.xyz, R4, c[19].xyxw;
MAD R4.xyz, R4, R3, c[19].zwzw;
MAD R4.xyz, R4, R3, c[20].xyxw;
MAD R4.xyz, R4, R3, c[20].zwzw;
MAD R2.xyz, R2, R1, c[18].wzww;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[21].xxyw;
MOV R1.xz, R5;
DP3 R1.y, R5, c[18].wzww;
FRC R3.w, R0.z;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[18].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[21].xxyw;
DP3 R3.y, R3, c[18].wzww;
DP3 R1.y, R2, -R3;
FRC R0.x, R0;
MUL R2.xyz, -R1.wxyw, c[14];
ADD R1.xyz, -R0.x, c[18];
MUL R0.w, vertex.position.x, c[15].x;
ADD R1.w, R0, c[16].x;
MAD R3.xyz, R1.w, R2, vertex.normal;
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
DP3 R0.w, R3, R3;
RSQ R0.w, R0.w;
MUL R3.xyz, R0.w, R3;
MUL R4.xyz, R3, c[10].w;
ADD R2.xyz, R2, c[19].xyxw;
MAD R2.xyz, R2, R1, c[19].zwzw;
MAD R2.xyz, R2, R1, c[20].xyxw;
MAD R2.xyz, R2, R1, c[20].zwzw;
MAD R2.xyz, R2, R1, c[18].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[21].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R2.w, R0.x;
ADD R0.xyz, -R3.w, c[18];
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[18].wzww;
DP3 R0.w, R2, -R1;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R2.w, c[18];
MUL R2.xyz, R1, R1;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[19].xyxw;
MAD R3.xyz, R3, R0, c[19].zwzw;
MAD R3.xyz, R3, R0, c[20].xyxw;
MAD R3.xyz, R3, R0, c[20].zwzw;
ADD R1.xyz, R1, c[19].xyxw;
MAD R1.xyz, R1, R2, c[19].zwzw;
MAD R1.xyz, R1, R2, c[20].xyxw;
MAD R1.xyz, R1, R2, c[20].zwzw;
MAD R1.xyz, R1, R2, c[18].wzww;
MAD R3.xyz, R3, R0, c[18].wzww;
SLT R5.x, R3.w, c[0].y;
SGE R5.yz, R3.w, c[21].xxyw;
MOV R0.xz, R5;
DP3 R0.y, R5, c[18].wzww;
DP3 R0.y, R3, -R0;
SLT R2.x, R2.w, c[0].y;
SGE R2.yz, R2.w, c[21].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[18].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[14];
DP3 R0.x, R0, c[18].z;
MUL R0.xyz, vertex.normal, R0.x;
MOV R0.w, vertex.position;
MAD R0.xyz, R0, R1.w, vertex.position;
DP3 result.texcoord[1].z, R4, c[7];
DP3 result.texcoord[1].y, R4, c[6];
DP3 result.texcoord[1].x, R4, c[5];
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
MOV result.texcoord[2].xyz, c[11];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
END
# 114 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceLightPos0]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
Vector 16 [_MainTex_ST]
"vs_2_0
; 69 ALU
def c17, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c18, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c19, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c8.w
mul r0.xyz, c12, r0.x
mad r1.xyz, v0, c11, r0
mad r0.x, r1, c17, c17.y
frc r0.x, r0
mad r1.x, r0, c17.z, c17.w
sincos r0.xy, r1.x, c18.xyzw, c19.xyzw
mad r0.w, r1.y, c17.x, c17.y
mad r0.z, r1, c17.x, c17.y
frc r0.w, r0
mad r0.w, r0, c17.z, c17
sincos r1.xy, r0.w, c18.xyzw, c19.xyzw
frc r0.z, r0
mad r0.z, r0, c17, c17.w
sincos r2.xy, r0.z, c18.xyzw, c19.xyzw
mov r0.z, r1.x
mov r0.w, r2.x
mov r2.z, r2.y
mul r1.x, v0, c14
add r1.x, r1, c15
mul r0.xzw, -r0, c13.xyyz
mad r0.xzw, r1.x, r0, v2.xyyz
dp3 r1.z, r0.xzww, r0.xzww
rsq r1.z, r1.z
mul r0.xzw, r1.z, r0
mul r0.xzw, r0, c9.w
dp3 oT1.z, r0.xzww, c6
mov r2.x, r0.y
mov r2.y, r1
mul r2.xyz, r2, c13
dp3 oT1.y, r0.xzww, c5
dp3 oT1.x, r0.xzww, c4
dp3 r0.y, r2, c19.z
mul r0.xyz, v2, r0.y
mov r0.w, v0
mad r0.xyz, r0, r1.x, v0
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mov oT2.xyz, c10
mad oT0.xy, v3, c16, c16.zwzw
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceLightPos0]
Vector 16 [_WaveScale]
Vector 17 [_WavePeriod]
Vector 18 [_WaveAmp]
Float 19 [_XScale]
Float 20 [_XBias]
Vector 21 [_MainTex_ST]
"!!ARBvp1.0
# 122 ALU
PARAM c[26] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
MOV R0.xyz, c[17];
MUL R0.xyz, R0, c[13].w;
MAD R6.xyz, vertex.position, c[16], R0;
MAD R0.x, R6, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[22];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R2, c[0].zwzw;
MAD R0.y, R6, c[0].x, -c[0];
FRC R0.w, R0.y;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R3.xyz, R3, R2, c[24].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
MAD R0.xyz, R1, R0, c[22].wzww;
MAD R3.xyz, R3, R2, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R2.xz, R4;
DP3 R2.y, R4, c[22].wzww;
DP3 R5.x, R3, -R2;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
DP3 R1.y, R1, c[22].wzww;
MAD R0.w, R6.z, c[0].x, -c[0].y;
FRC R1.w, R0;
DP3 R5.y, R0, -R1;
ADD R0.xyz, -R1.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
MUL R0.w, R6.x, c[0].x;
FRC R0.w, R0;
ADD R2.xyz, -R0.w, c[22];
MUL R2.xyz, R2, R2;
MUL R3.xyz, R2, c[0].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R1.xyz, R1, R0, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R5.z, R1, -R0;
MUL R1.xyz, R5, c[18];
DP3 R1.w, R1, c[22].z;
MAD R0.xyz, R3, R2, c[24].xyxw;
MAD R0.xyz, R0, R2, c[24].zwzw;
MAD R0.xyz, R0, R2, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MOV R2.xz, R1;
DP3 R2.y, R1, c[22].wzww;
DP3 R5.x, R0, -R2;
MUL R0.x, R6.y, c[0];
FRC R2.w, R0.x;
MUL R0.y, R6.z, c[0].x;
FRC R0.w, R0.y;
ADD R1.xyz, -R0.w, c[22];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R2, c[0].zwzw;
ADD R0.xyz, -R2.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R1.xyz, R1, R0, c[22].wzww;
MAD R3.xyz, R3, R2, c[24].zwzw;
SLT R4.x, R2.w, c[0].y;
SGE R4.yz, R2.w, c[25].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[22].wzww;
DP3 R5.y, R1, -R0;
MAD R0.xyz, R3, R2, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MOV R2.xz, R1;
DP3 R2.y, R1, c[22].wzww;
DP3 R5.z, R0, -R2;
MUL R0.x, vertex.position, c[19];
ADD R0.w, R0.x, c[20].x;
MUL R0.xyz, vertex.normal, R1.w;
MUL R1.xyz, -R5, c[18];
MAD R2.xyz, R0.w, R1, vertex.normal;
DP3 R2.w, R2, R2;
RSQ R2.w, R2.w;
MUL R2.xyz, R2.w, R2;
MUL R2.xyz, R2, c[14].w;
MAD R1.xyz, R0, R0.w, vertex.position;
MOV R1.w, vertex.position;
DP4 R0.z, R1, c[7];
DP4 R0.x, R1, c[5];
DP4 R0.y, R1, c[6];
DP4 R0.w, R1, c[8];
DP4 result.texcoord[3].w, R0, c[12];
DP4 result.texcoord[3].z, R0, c[11];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
ADD result.texcoord[2].xyz, -R0, c[15];
DP3 result.texcoord[1].z, R2, c[7];
DP3 result.texcoord[1].y, R2, c[6];
DP3 result.texcoord[1].x, R2, c[5];
DP4 result.position.w, R1, c[4];
DP4 result.position.z, R1, c[3];
DP4 result.position.y, R1, c[2];
DP4 result.position.x, R1, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[21], c[21].zwzw;
END
# 122 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_WaveScale]
Vector 16 [_WavePeriod]
Vector 17 [_WaveAmp]
Float 18 [_XScale]
Float 19 [_XBias]
Vector 20 [_MainTex_ST]
"vs_2_0
; 76 ALU
def c21, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c12.w
mul r0.xyz, c16, r0.x
mad r1.xyz, v0, c15, r0
mad r0.x, r1, c21, c21.y
frc r0.x, r0
mad r1.x, r0, c21.z, c21.w
sincos r0.xy, r1.x, c22.xyzw, c23.xyzw
mad r0.z, r1, c21.x, c21.y
mad r0.w, r1.y, c21.x, c21.y
frc r0.z, r0
mad r0.z, r0, c21, c21.w
sincos r1.xy, r0.z, c22.xyzw, c23.xyzw
frc r0.w, r0
mad r0.w, r0, c21.z, c21
sincos r2.xy, r0.w, c22.xyzw, c23.xyzw
mov r0.w, r1.y
mov r0.z, r2.y
mul r0.yzw, r0, c17.xxyz
dp3 r0.y, r0.yzww, c23.z
mov r1.y, r0.x
mov r1.z, r2.x
mul r0.x, v0, c18
add r1.w, r0.x, c19.x
mul r1.xyz, -r1.yzxw, c17
mad r2.xyz, r1.w, r1, v2
dp3 r2.w, r2, r2
mul r0.xyz, v2, r0.y
rsq r2.w, r2.w
mul r2.xyz, r2.w, r2
mul r2.xyz, r2, c13.w
mad r0.xyz, r0, r1.w, v0
mov r0.w, v0
dp4 r1.z, r0, c6
dp4 r1.x, r0, c4
dp4 r1.y, r0, c5
dp4 r1.w, r0, c7
dp4 oT3.w, r1, c11
dp4 oT3.z, r1, c10
dp4 oT3.y, r1, c9
dp4 oT3.x, r1, c8
add oT2.xyz, -r1, c14
dp3 oT1.z, r2, c6
dp3 oT1.y, r2, c5
dp3 oT1.x, r2, c4
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mad oT0.xy, v3, c20, c20.zwzw
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceLightPos0]
Vector 16 [_WaveScale]
Vector 17 [_WavePeriod]
Vector 18 [_WaveAmp]
Float 19 [_XScale]
Float 20 [_XBias]
Vector 21 [_MainTex_ST]
"!!ARBvp1.0
# 121 ALU
PARAM c[26] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
MOV R0.xyz, c[17];
MUL R0.xyz, R0, c[13].w;
MAD R5.xyz, vertex.position, c[16], R0;
MAD R0.x, R5, c[0], -c[0].y;
FRC R1.w, R0.x;
MAD R0.y, R5, c[0].x, -c[0];
FRC R0.w, R0.y;
ADD R1.xyz, -R1.w, c[22];
MUL R2.xyz, R1, R1;
MUL R3.xyz, R2, c[0].zwzw;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R2, c[23].zwzw;
MAD R3.xyz, R3, R2, c[24].xyxw;
MAD R3.xyz, R3, R2, c[24].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
MAD R0.xyz, R1, R0, c[22].wzww;
MAD R3.xyz, R3, R2, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R2.xz, R4;
DP3 R2.y, R4, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MAD R0.w, R5.z, c[0].x, -c[0].y;
FRC R1.w, R0;
DP3 R1.y, R1, c[22].wzww;
DP3 R6.y, R0, -R1;
ADD R0.xyz, -R1.w, c[22];
MUL R0.w, R5.x, c[0].x;
MUL R1.xyz, R0, R0;
FRC R0.w, R0;
DP3 R6.x, R3, -R2;
MUL R2.xyz, R1, c[0].zwzw;
ADD R0.xyz, -R0.w, c[22];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R2.xyz, R2, c[23].xyxw;
MAD R2.xyz, R2, R1, c[23].zwzw;
MAD R2.xyz, R2, R1, c[24].xyxw;
MAD R2.xyz, R2, R1, c[24].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R0, c[23].zwzw;
MAD R3.xyz, R3, R0, c[24].xyxw;
MAD R2.xyz, R2, R1, c[22].wzww;
SLT R4.x, R1.w, c[0].y;
SGE R4.yz, R1.w, c[25].xxyw;
MOV R1.xz, R4;
DP3 R1.y, R4, c[22].wzww;
DP3 R6.z, R2, -R1;
MAD R1.xyz, R3, R0, c[24].zwzw;
MAD R0.xyz, R1, R0, c[22].wzww;
SLT R1.x, R0.w, c[0].y;
SGE R1.yz, R0.w, c[25].xxyw;
MOV R3.xz, R1;
DP3 R3.y, R1, c[22].wzww;
DP3 R0.w, R0, -R3;
MUL R0.x, R5.y, c[0];
FRC R2.w, R0.x;
MUL R0.y, R5.z, c[0].x;
FRC R1.w, R0.y;
ADD R1.xyz, -R1.w, c[22];
MUL R3.xyz, R1, R1;
MUL R4.xyz, R3, c[0].zwzw;
ADD R0.xyz, -R2.w, c[22];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R0, c[23].zwzw;
MAD R1.xyz, R1, R0, c[24].xyxw;
MAD R1.xyz, R1, R0, c[24].zwzw;
ADD R4.xyz, R4, c[23].xyxw;
MAD R4.xyz, R4, R3, c[23].zwzw;
MAD R4.xyz, R4, R3, c[24].xyxw;
MAD R4.xyz, R4, R3, c[24].zwzw;
MAD R1.xyz, R1, R0, c[22].wzww;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[25].xxyw;
MOV R2.w, vertex.position;
MUL R2.xyz, R6, c[18];
MOV R0.xz, R5;
DP3 R0.y, R5, c[22].wzww;
DP3 R0.x, R1, -R0;
MAD R1.xyz, R4, R3, c[22].wzww;
SLT R3.x, R1.w, c[0].y;
SGE R3.yz, R1.w, c[25].xxyw;
MUL R0.z, vertex.position.x, c[19].x;
ADD R1.w, R0.z, c[20].x;
MOV R4.xz, R3;
DP3 R4.y, R3, c[22].wzww;
DP3 R0.y, R1, -R4;
MUL R1.xyz, -R0.wxyw, c[18];
MAD R0.xyz, R1.w, R1, vertex.normal;
DP3 R0.w, R2, c[22].z;
MUL R1.xyz, vertex.normal, R0.w;
MAD R2.xyz, R1, R1.w, vertex.position;
DP3 R3.x, R0, R0;
RSQ R0.w, R3.x;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, c[14].w;
DP4 R1.z, R2, c[7];
DP4 R1.x, R2, c[5];
DP4 R1.y, R2, c[6];
DP4 R1.w, R2, c[8];
DP4 result.texcoord[3].z, R1, c[11];
DP4 result.texcoord[3].y, R1, c[10];
DP4 result.texcoord[3].x, R1, c[9];
ADD result.texcoord[2].xyz, -R1, c[15];
DP3 result.texcoord[1].z, R0, c[7];
DP3 result.texcoord[1].y, R0, c[6];
DP3 result.texcoord[1].x, R0, c[5];
DP4 result.position.w, R2, c[4];
DP4 result.position.z, R2, c[3];
DP4 result.position.y, R2, c[2];
DP4 result.position.x, R2, c[1];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[21], c[21].zwzw;
END
# 121 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_WaveScale]
Vector 16 [_WavePeriod]
Vector 17 [_WaveAmp]
Float 18 [_XScale]
Float 19 [_XBias]
Vector 20 [_MainTex_ST]
"vs_2_0
; 75 ALU
def c21, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c12.w
mul r0.xyz, c16, r0.x
mad r0.xyz, v0, c15, r0
mad r0.x, r0, c21, c21.y
frc r0.x, r0
mad r0.x, r0, c21.z, c21.w
sincos r2.xy, r0.x, c22.xyzw, c23.xyzw
mad r0.x, r0.y, c21, c21.y
mad r0.z, r0, c21.x, c21.y
frc r0.y, r0.z
frc r0.x, r0
mad r1.x, r0.y, c21.z, c21.w
mad r2.z, r0.x, c21, c21.w
sincos r0.xy, r1.x, c22.xyzw, c23.xyzw
sincos r1.xy, r2.z, c22.xyzw, c23.xyzw
mov r1.w, v0
mov r2.w, r0.y
mov r2.z, r1.y
mul r0.yzw, r2, c17.xxyz
mov r2.z, r0.x
mov r2.y, r1.x
mul r0.x, v0, c18
add r1.x, r0, c19
dp3 r0.x, r0.yzww, c23.z
mul r2.xyz, -r2, c17
mad r2.xyz, r1.x, r2, v2
mul r0.xyz, v2, r0.x
mad r1.xyz, r0, r1.x, v0
dp3 r2.w, r2, r2
rsq r2.w, r2.w
mul r2.xyz, r2.w, r2
mul r2.xyz, r2, c13.w
dp4 r0.z, r1, c6
dp4 r0.x, r1, c4
dp4 r0.y, r1, c5
dp4 r0.w, r1, c7
dp4 oT3.z, r0, c10
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
add oT2.xyz, -r0, c14
dp3 oT1.z, r2, c6
dp3 oT1.y, r2, c5
dp3 oT1.x, r2, c4
dp4 oPos.w, r1, c3
dp4 oPos.z, r1, c2
dp4 oPos.y, r1, c1
dp4 oPos.x, r1, c0
mad oT0.xy, v3, c20, c20.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [_Time]
Vector 14 [unity_Scale]
Vector 15 [_WorldSpaceLightPos0]
Vector 16 [_WaveScale]
Vector 17 [_WavePeriod]
Vector 18 [_WaveAmp]
Float 19 [_XScale]
Float 20 [_XBias]
Vector 21 [_MainTex_ST]
"!!ARBvp1.0
# 120 ALU
PARAM c[26] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..21],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[17];
MUL R0.xyz, R0, c[13].w;
MAD R0.xyz, vertex.position, c[16], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[22];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[23].xyxw;
MAD R2.xyz, R2, R1, c[23].zwzw;
MAD R2.xyz, R2, R1, c[24].xyxw;
MAD R2.xyz, R2, R1, c[24].zwzw;
MAD R1.xyz, R2, R1, c[22].wzww;
MAD R0.x, R0, c[0], -c[0].y;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[25].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MUL R0.w, R0.z, c[0].x;
FRC R0.w, R0;
DP3 R2.y, R2, c[22].wzww;
DP3 R1.w, R1, -R2;
ADD R1.xyz, -R2.w, c[22];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R3.xyz, -R0.w, c[22];
MUL R3.xyz, R3, R3;
MUL R4.xyz, R3, c[0].zwzw;
MAD R0.z, R0, c[0].x, -c[0].y;
ADD R2.xyz, R2, c[23].xyxw;
MAD R2.xyz, R2, R1, c[23].zwzw;
MAD R2.xyz, R2, R1, c[24].xyxw;
MAD R2.xyz, R2, R1, c[24].zwzw;
ADD R4.xyz, R4, c[23].xyxw;
MAD R4.xyz, R4, R3, c[23].zwzw;
MAD R2.xyz, R2, R1, c[22].wzww;
SGE R5.yz, R2.w, c[25].xxyw;
SLT R5.x, R2.w, c[0].y;
DP3 R1.y, R5, c[22].wzww;
MOV R1.xz, R5;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[24].xyxw;
MAD R2.xyz, R2, R3, c[24].zwzw;
MAD R4.xyz, R2, R3, c[22].wzww;
FRC R0.x, R0;
ADD R2.xyz, -R0.x, c[22];
MUL R2.xyz, R2, R2;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[25].xxyw;
MOV R5.xz, R3;
DP3 R5.y, R3, c[22].wzww;
DP3 R1.y, R4, -R5;
FRC R4.w, R0.z;
MUL R3.xyz, R2, c[0].zwzw;
ADD R4.xyz, R3, c[23].xyxw;
MUL R3.xyz, -R1.wxyw, c[18];
MAD R1.xyz, R4, R2, c[23].zwzw;
MAD R1.xyz, R1, R2, c[24].xyxw;
MAD R1.xyz, R1, R2, c[24].zwzw;
MUL R0.w, vertex.position.x, c[19].x;
ADD R1.w, R0, c[20].x;
MAD R4.xyz, R1.w, R3, vertex.normal;
MAD R2.xyz, R1, R2, c[22].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[25].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R3.w, R0.x;
ADD R0.xyz, -R4.w, c[22];
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[22].wzww;
DP3 R0.w, R2, -R1;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R3.w, c[22];
MUL R2.xyz, R1, R1;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[23].xyxw;
MAD R3.xyz, R3, R0, c[23].zwzw;
MAD R3.xyz, R3, R0, c[24].xyxw;
MAD R3.xyz, R3, R0, c[24].zwzw;
ADD R1.xyz, R1, c[23].xyxw;
MAD R1.xyz, R1, R2, c[23].zwzw;
MAD R1.xyz, R1, R2, c[24].xyxw;
MAD R1.xyz, R1, R2, c[24].zwzw;
MAD R1.xyz, R1, R2, c[22].wzww;
MAD R3.xyz, R3, R0, c[22].wzww;
SLT R2.x, R3.w, c[0].y;
SGE R2.yz, R3.w, c[25].xxyw;
DP3 R2.w, R4, R4;
SLT R5.x, R4.w, c[0].y;
SGE R5.yz, R4.w, c[25].xxyw;
MOV R0.xz, R5;
DP3 R0.y, R5, c[22].wzww;
DP3 R0.y, R3, -R0;
MOV R3.xz, R2;
DP3 R3.y, R2, c[22].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[18];
RSQ R0.w, R2.w;
MUL R1.xyz, R0.w, R4;
MUL R1.xyz, R1, c[14].w;
DP3 R0.x, R0, c[22].z;
MUL R0.xyz, vertex.normal, R0.x;
MAD R2.xyz, R0, R1.w, vertex.position;
MOV R2.w, vertex.position;
DP4 R0.w, R2, c[8];
DP4 R0.z, R2, c[7];
DP4 R0.x, R2, c[5];
DP4 R0.y, R2, c[6];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
DP3 result.texcoord[1].z, R1, c[7];
DP3 result.texcoord[1].y, R1, c[6];
DP3 result.texcoord[1].x, R1, c[5];
DP4 result.position.w, R2, c[4];
DP4 result.position.z, R2, c[3];
DP4 result.position.y, R2, c[2];
DP4 result.position.x, R2, c[1];
MOV result.texcoord[2].xyz, c[15];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[21], c[21].zwzw;
END
# 120 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [_Time]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_WaveScale]
Vector 16 [_WavePeriod]
Vector 17 [_WaveAmp]
Float 18 [_XScale]
Float 19 [_XBias]
Vector 20 [_MainTex_ST]
"vs_2_0
; 75 ALU
def c21, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c22, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c23, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c12.w
mul r0.xyz, c16, r0.x
mad r1.xyz, v0, c15, r0
mad r0.x, r1, c21, c21.y
frc r0.x, r0
mad r1.x, r0, c21.z, c21.w
sincos r0.xy, r1.x, c22.xyzw, c23.xyzw
mad r0.w, r1.y, c21.x, c21.y
mad r0.z, r1, c21.x, c21.y
frc r0.w, r0
mad r0.w, r0, c21.z, c21
sincos r1.xy, r0.w, c22.xyzw, c23.xyzw
frc r0.z, r0
mad r0.z, r0, c21, c21.w
sincos r2.xy, r0.z, c22.xyzw, c23.xyzw
mov r0.z, r1.x
mov r0.w, r2.x
mov r2.z, r2.y
mul r1.x, v0, c18
add r1.x, r1, c19
mul r0.xzw, -r0, c17.xyyz
mad r0.xzw, r1.x, r0, v2.xyyz
dp3 r1.z, r0.xzww, r0.xzww
mov r2.x, r0.y
mov r2.y, r1
mul r2.xyz, r2, c17
rsq r0.y, r1.z
dp3 r1.y, r2, c23.z
mul r2.xyz, r0.y, r0.xzww
mul r2.xyz, r2, c13.w
mul r0.xyz, v2, r1.y
mad r0.xyz, r0, r1.x, v0
mov r0.w, v0
dp4 r1.w, r0, c7
dp4 r1.z, r0, c6
dp4 r1.x, r0, c4
dp4 r1.y, r0, c5
dp4 oT3.y, r1, c9
dp4 oT3.x, r1, c8
dp3 oT1.z, r2, c6
dp3 oT1.y, r2, c5
dp3 oT1.x, r2, c4
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
mov oT2.xyz, c14
mad oT0.xy, v3, c20, c20.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 13 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
DP3 R0.w, fragment.texcoord[3], fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3 R1.x, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[1];
MOV result.color.w, c[1].x;
TEX R0.w, R0.w, texture[1], 2D;
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
END
# 13 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 13 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r1, t0, s0
dp3 r0.x, t3, t3
mov r0.xy, r0.x
mul_pp r1.xyz, r1, c0
mov_pp r0.w, c1.x
texld r2, r0, s1
dp3_pp r0.x, t2, t2
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t2
dp3_pp r0.x, t1, r0
max_pp r0.x, r0, c1
mul_pp r0.x, r0, r2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c1.y
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 1 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
MOV R1.xyz, fragment.texcoord[2];
DP3 R0.w, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MAX R0.w, R0, c[1].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
MOV result.color.w, c[1].x;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 8 ALU, 1 TEX
dcl_2d s0
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
texld r1, t0, s0
mov_pp r0.xyz, t2
dp3_pp r0.x, t1, r0
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c1
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c1.y
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 19 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 0, 0.5, 2 } };
TEMP R0;
TEMP R1;
RCP R0.x, fragment.texcoord[3].w;
MAD R1.xy, fragment.texcoord[3], R0.x, c[1].y;
DP3 R1.z, fragment.texcoord[3], fragment.texcoord[3];
MOV result.color.w, c[1].x;
TEX R0.w, R1, texture[1], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, R1.z, texture[2], 2D;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3 R1.x, fragment.texcoord[1], R1;
SLT R1.y, c[1].x, fragment.texcoord[3].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[1].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].z;
END
# 19 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 18 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c1, 0.50000000, 0.00000000, 1.00000000, 2.00000000
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3
texld r2, t0, s0
dp3 r1.x, t3, t3
mov r1.xy, r1.x
rcp r0.x, t3.w
mad r0.xy, t3, r0.x, c1.x
mul_pp r2.xyz, r2, c0
texld r3, r1, s2
texld r0, r0, s1
cmp r0.x, -t3.z, c1.y, c1.z
mul_pp r0.x, r0, r0.w
dp3_pp r1.x, t2, t2
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t2
dp3_pp r1.x, t1, r1
mul_pp r0.x, r0, r3
max_pp r1.x, r1, c1.y
mul_pp r0.x, r1, r0
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c1.w
mov_pp r0.w, c1.y
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[3], texture[2], CUBE;
DP3 R0.w, fragment.texcoord[3], fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3 R1.x, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MOV result.color.w, c[1].x;
TEX R0.w, R0.w, texture[1], 2D;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[1].x;
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
END
# 15 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 14 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_cube s2
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r2, t3, s2
texld r1, t0, s0
dp3_pp r2.x, t2, t2
dp3 r0.x, t3, t3
mov r0.xy, r0.x
rsq_pp r2.x, r2.x
mul_pp r2.xyz, r2.x, t2
dp3_pp r2.x, t1, r2
mul_pp r1.xyz, r1, c0
max_pp r2.x, r2, c1
texld r0, r0, s1
mul r0.x, r0, r2.w
mul_pp r0.x, r2, r0
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c1.y
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R0.w, fragment.texcoord[3], texture[1], 2D;
MOV R1.xyz, fragment.texcoord[2];
DP3 R1.x, fragment.texcoord[1], R1;
MAX R1.x, R1, c[1];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
MOV result.color.w, c[1].x;
END
# 10 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 9 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0.xy
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r0, t3, s1
texld r1, t0, s0
mov_pp r0.xyz, t2
dp3_pp r0.x, t1, r0
max_pp r0.x, r0, c1
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1, c0
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c1.y
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 5 [_Object2World]
Vector 9 [_Time]
Vector 10 [unity_Scale]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
"!!ARBvp1.0
# 112 ALU
PARAM c[20] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..15],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
MOV R0.xyz, c[12];
MUL R0.xyz, R0, c[9].w;
MAD R0.xyz, vertex.position, c[11], R0;
MUL R0.w, R0.x, c[0].x;
FRC R0.w, R0;
ADD R1.xyz, -R0.w, c[16];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R2.xyz, R2, c[17].xyxw;
MAD R2.xyz, R2, R1, c[17].zwzw;
MAD R2.xyz, R2, R1, c[18].xyxw;
MAD R2.xyz, R2, R1, c[18].zwzw;
MAD R0.x, R0, c[0], -c[0].y;
MAD R2.xyz, R2, R1, c[16].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[19].xxyw;
MUL R0.w, R0.y, c[0].x;
FRC R2.w, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[16].wzww;
DP3 R1.w, R2, -R1;
MUL R1.x, R0.z, c[0];
FRC R0.w, R1.x;
ADD R2.xyz, -R0.w, c[16];
MUL R3.xyz, R2, R2;
MUL R4.xyz, R3, c[0].zwzw;
ADD R1.xyz, -R2.w, c[16];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
MAD R0.z, R0, c[0].x, -c[0].y;
ADD R2.xyz, R2, c[17].xyxw;
MAD R2.xyz, R2, R1, c[17].zwzw;
MAD R2.xyz, R2, R1, c[18].xyxw;
MAD R2.xyz, R2, R1, c[18].zwzw;
ADD R4.xyz, R4, c[17].xyxw;
MAD R4.xyz, R4, R3, c[17].zwzw;
MAD R4.xyz, R4, R3, c[18].xyxw;
MAD R4.xyz, R4, R3, c[18].zwzw;
MAD R2.xyz, R2, R1, c[16].wzww;
SLT R5.x, R2.w, c[0].y;
SGE R5.yz, R2.w, c[19].xxyw;
MOV R1.xz, R5;
DP3 R1.y, R5, c[16].wzww;
FRC R3.w, R0.z;
DP3 R1.x, R2, -R1;
MAD R2.xyz, R4, R3, c[16].wzww;
SLT R3.x, R0.w, c[0].y;
SGE R3.yz, R0.w, c[19].xxyw;
DP3 R3.y, R3, c[16].wzww;
DP3 R1.y, R2, -R3;
FRC R0.x, R0;
MUL R2.xyz, -R1.wxyw, c[13];
ADD R1.xyz, -R0.x, c[16];
MUL R0.w, vertex.position.x, c[14].x;
ADD R1.w, R0, c[15].x;
MAD R3.xyz, R1.w, R2, vertex.normal;
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
DP3 R0.w, R3, R3;
RSQ R0.w, R0.w;
MUL R3.xyz, R0.w, R3;
MUL R4.xyz, R3, c[10].w;
ADD R2.xyz, R2, c[17].xyxw;
MAD R2.xyz, R2, R1, c[17].zwzw;
MAD R2.xyz, R2, R1, c[18].xyxw;
MAD R2.xyz, R2, R1, c[18].zwzw;
MAD R2.xyz, R2, R1, c[16].wzww;
SLT R3.x, R0, c[0].y;
SGE R3.yz, R0.x, c[19].xxyw;
MAD R0.x, R0.y, c[0], -c[0].y;
FRC R2.w, R0.x;
ADD R0.xyz, -R3.w, c[16];
MUL R0.xyz, R0, R0;
MOV R1.xz, R3;
DP3 R1.y, R3, c[16].wzww;
DP3 R0.w, R2, -R1;
MUL R3.xyz, R0, c[0].zwzw;
ADD R1.xyz, -R2.w, c[16];
MUL R2.xyz, R1, R1;
MUL R1.xyz, R2, c[0].zwzw;
ADD R3.xyz, R3, c[17].xyxw;
MAD R3.xyz, R3, R0, c[17].zwzw;
MAD R3.xyz, R3, R0, c[18].xyxw;
MAD R3.xyz, R3, R0, c[18].zwzw;
ADD R1.xyz, R1, c[17].xyxw;
MAD R1.xyz, R1, R2, c[17].zwzw;
MAD R1.xyz, R1, R2, c[18].xyxw;
MAD R1.xyz, R1, R2, c[18].zwzw;
MAD R1.xyz, R1, R2, c[16].wzww;
MAD R3.xyz, R3, R0, c[16].wzww;
SLT R5.x, R3.w, c[0].y;
SGE R5.yz, R3.w, c[19].xxyw;
MOV R0.xz, R5;
DP3 R0.y, R5, c[16].wzww;
DP3 R0.y, R3, -R0;
SLT R2.x, R2.w, c[0].y;
SGE R2.yz, R2.w, c[19].xxyw;
MOV R3.xz, R2;
DP3 R3.y, R2, c[16].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[13];
DP3 R0.x, R0, c[16].z;
MUL R0.xyz, vertex.normal, R0.x;
MOV R0.w, vertex.position;
MAD R0.xyz, R0, R1.w, vertex.position;
DP3 result.texcoord[0].z, R4, c[7];
DP3 result.texcoord[0].y, R4, c[6];
DP3 result.texcoord[0].x, R4, c[5];
DP4 result.position.w, R0, c[4];
DP4 result.position.z, R0, c[3];
DP4 result.position.y, R0, c[2];
DP4 result.position.x, R0, c[1];
END
# 112 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_Time]
Vector 9 [unity_Scale]
Vector 10 [_WaveScale]
Vector 11 [_WavePeriod]
Vector 12 [_WaveAmp]
Float 13 [_XScale]
Float 14 [_XBias]
"vs_2_0
; 67 ALU
def c15, 0.15915491, 0.50000000, 6.28318501, -3.14159298
def c16, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c17, -0.02083333, -0.12500000, 1.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v2
mov r0.x, c8.w
mul r0.xyz, c11, r0.x
mad r1.xyz, v0, c10, r0
mad r0.x, r1, c15, c15.y
frc r0.x, r0
mad r1.x, r0, c15.z, c15.w
sincos r0.xy, r1.x, c16.xyzw, c17.xyzw
mad r0.w, r1.y, c15.x, c15.y
mad r0.z, r1, c15.x, c15.y
frc r0.w, r0
mad r0.w, r0, c15.z, c15
sincos r1.xy, r0.w, c16.xyzw, c17.xyzw
frc r0.z, r0
mad r0.z, r0, c15, c15.w
sincos r2.xy, r0.z, c16.xyzw, c17.xyzw
mov r0.z, r1.x
mov r0.w, r2.x
mov r2.z, r2.y
mul r1.x, v0, c13
add r1.x, r1, c14
mul r0.xzw, -r0, c12.xyyz
mad r0.xzw, r1.x, r0, v2.xyyz
dp3 r1.z, r0.xzww, r0.xzww
rsq r1.z, r1.z
mul r0.xzw, r1.z, r0
mul r0.xzw, r0, c9.w
dp3 oT0.z, r0.xzww, c6
mov r2.x, r0.y
mov r2.y, r1
mul r2.xyz, r2, c12
dp3 oT0.y, r0.xzww, c5
dp3 oT0.x, r0.xzww, c4
dp3 r0.y, r2, c17.z
mul r0.xyz, v2, r0.y
mov r0.w, v0
mad r0.xyz, r0, r1.x, v0
dp4 oPos.w, r0, c3
dp4 oPos.z, r0, c2
dp4 oPos.y, r0, c1
dp4 oPos.x, r0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 2 ALU, 0 TEX
PARAM c[1] = { { 0, 0.5 } };
MAD result.color.xyz, fragment.texcoord[0], c[0].y, c[0].y;
MOV result.color.w, c[0].x;
END
# 2 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
def c0, 0.50000000, 0.00000000, 0, 0
dcl t0.xyz
mad_pp r0.xyz, t0, c0.x, c0.x
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
  Cull Off
Program "vp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Vector 5 [_Time]
Vector 6 [_ProjectionParams]
Vector 7 [_WaveScale]
Vector 8 [_WavePeriod]
Vector 9 [_WaveAmp]
Float 10 [_XScale]
Float 11 [_XBias]
Vector 12 [_MainTex_ST]
"!!ARBvp1.0
# 65 ALU
PARAM c[17] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.mvp,
		program.local[5..12],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.xyz, c[8];
MUL R0.xyz, R0, c[5].w;
MAD R3.xyz, vertex.position, c[7], R0;
MAD R0.x, R3, c[0], -c[0].y;
FRC R0.w, R0.x;
ADD R0.xyz, -R0.w, c[13];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[14].xyxw;
MAD R1.xyz, R1, R0, c[14].zwzw;
MAD R1.xyz, R1, R0, c[15].xyxw;
MAD R1.xyz, R1, R0, c[15].zwzw;
MAD R0.xyz, R1, R0, c[13].wzww;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[16].xxyw;
MOV R1.xz, R2;
DP3 R1.y, R2, c[13].wzww;
DP3 R0.w, R0, -R1;
MAD R0.y, R3.z, c[0].x, -c[0];
FRC R2.w, R0.y;
MAD R0.x, R3.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[13];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R0.xyz, -R2.w, c[13];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[14].xyxw;
MAD R3.xyz, R3, R0, c[14].zwzw;
MAD R3.xyz, R3, R0, c[15].xyxw;
MAD R3.xyz, R3, R0, c[15].zwzw;
ADD R2.xyz, R2, c[14].xyxw;
MAD R2.xyz, R2, R1, c[14].zwzw;
MAD R2.xyz, R2, R1, c[15].xyxw;
MAD R2.xyz, R2, R1, c[15].zwzw;
MAD R1.xyz, R2, R1, c[13].wzww;
MAD R3.xyz, R3, R0, c[13].wzww;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[16].xxyw;
MOV R1.w, vertex.position;
SLT R4.x, R2.w, c[0].y;
SGE R4.yz, R2.w, c[16].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[13].wzww;
DP3 R0.y, R3, -R0;
MOV R3.xz, R2;
DP3 R3.y, R2, c[13].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[9];
DP3 R0.x, R0, c[13].z;
MUL R0.w, vertex.position.x, c[10].x;
MUL R0.xyz, vertex.normal, R0.x;
ADD R0.w, R0, c[11].x;
MAD R1.xyz, R0, R0.w, vertex.position;
DP4 R0.w, R1, c[4];
DP4 R0.z, R1, c[3];
DP4 R0.x, R1, c[1];
DP4 R0.y, R1, c[2];
MUL R2.xyz, R0.xyww, c[13].y;
MUL R2.y, R2, c[6].x;
ADD result.texcoord[1].xy, R2, R2.z;
MOV result.position, R0;
MOV result.texcoord[1].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[12], c[12].zwzw;
END
# 65 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Time]
Vector 5 [_ProjectionParams]
Vector 6 [_ScreenParams]
Vector 7 [_WaveScale]
Vector 8 [_WavePeriod]
Vector 9 [_WaveAmp]
Float 10 [_XScale]
Float 11 [_XBias]
Vector 12 [_MainTex_ST]
"vs_2_0
; 61 ALU
def c13, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c14, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c15, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
mov r0.x, c4.w
mul r0.xyz, c8, r0.x
mad r1.xyz, v0, c7, r0
mad r0.x, r1, c15, c15.y
frc r0.x, r0
mad r1.x, r0, c15.z, c15.w
sincos r0.xy, r1.x, c14.xyzw, c13.xyzw
mov r0.x, r0.y
mad r0.z, r1, c15.x, c15.y
frc r0.y, r0.z
mad r0.x, r1.y, c15, c15.y
mad r0.y, r0, c15.z, c15.w
sincos r1.xy, r0.y, c14.xyzw, c13.xyzw
frc r0.x, r0
mad r1.x, r0, c15.z, c15.w
sincos r0.xy, r1.x, c14.xyzw, c13.xyzw
mov r0.z, r1.y
mul r0.xyz, r0, c9
dp3 r0.x, r0, c13.z
mul r0.w, v0.x, c10.x
mul r0.xyz, v2, r0.x
add r0.w, r0, c11.x
mad r1.xyz, r0, r0.w, v0
mov r1.w, v0
dp4 r0.w, r1, c3
dp4 r0.z, r1, c2
dp4 r0.x, r1, c0
dp4 r0.y, r1, c1
mul r2.xyz, r0.xyww, c13.w
mul r2.y, r2, c5.x
mad oT1.xy, r2.z, c6.zwzw, r2
mov oPos, r0
mov oT1.zw, r0
mad oT0.xy, v3, c12, c12.zwzw
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_Time]
Vector 10 [_ProjectionParams]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
Vector 16 [unity_LightmapST]
Vector 17 [unity_LightmapFade]
Vector 18 [_MainTex_ST]
"!!ARBvp1.0
# 68 ALU
PARAM c[23] = { { 0.15915491, 0.25, 24.980801, -24.980801 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9..18],
		{ 0, 0.5, 1, -1 },
		{ -60.145809, 60.145809, 85.453789, -85.453789 },
		{ -64.939346, 64.939346, 19.73921, -19.73921 },
		{ -9, 0.75 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MOV R0.xyz, c[12];
MUL R0.xyz, R0, c[9].w;
MAD R3.xyz, vertex.position, c[11], R0;
MAD R0.x, R3, c[0], -c[0].y;
FRC R0.w, R0.x;
ADD R0.xyz, -R0.w, c[19];
MUL R0.xyz, R0, R0;
MUL R1.xyz, R0, c[0].zwzw;
ADD R1.xyz, R1, c[20].xyxw;
MAD R1.xyz, R1, R0, c[20].zwzw;
MAD R1.xyz, R1, R0, c[21].xyxw;
MAD R1.xyz, R1, R0, c[21].zwzw;
MAD R0.xyz, R1, R0, c[19].wzww;
SLT R2.x, R0.w, c[0].y;
SGE R2.yz, R0.w, c[22].xxyw;
MOV R1.xz, R2;
DP3 R1.y, R2, c[19].wzww;
DP3 R0.w, R0, -R1;
MAD R0.y, R3.z, c[0].x, -c[0];
FRC R2.w, R0.y;
MAD R0.x, R3.y, c[0], -c[0].y;
FRC R1.w, R0.x;
ADD R1.xyz, -R1.w, c[19];
MUL R1.xyz, R1, R1;
MUL R2.xyz, R1, c[0].zwzw;
ADD R0.xyz, -R2.w, c[19];
MUL R0.xyz, R0, R0;
MUL R3.xyz, R0, c[0].zwzw;
ADD R3.xyz, R3, c[20].xyxw;
MAD R3.xyz, R3, R0, c[20].zwzw;
MAD R3.xyz, R3, R0, c[21].xyxw;
MAD R3.xyz, R3, R0, c[21].zwzw;
ADD R2.xyz, R2, c[20].xyxw;
MAD R2.xyz, R2, R1, c[20].zwzw;
MAD R2.xyz, R2, R1, c[21].xyxw;
MAD R2.xyz, R2, R1, c[21].zwzw;
MAD R1.xyz, R2, R1, c[19].wzww;
MAD R3.xyz, R3, R0, c[19].wzww;
SLT R2.x, R1.w, c[0].y;
SGE R2.yz, R1.w, c[22].xxyw;
MOV R1.w, vertex.position;
SLT R4.x, R2.w, c[0].y;
SGE R4.yz, R2.w, c[22].xxyw;
MOV R0.xz, R4;
DP3 R0.y, R4, c[19].wzww;
DP3 R0.y, R3, -R0;
MOV R3.xz, R2;
DP3 R3.y, R2, c[19].wzww;
DP3 R0.x, R1, -R3;
MUL R0.xyz, R0.wxyw, c[13];
DP3 R0.x, R0, c[19].z;
MUL R0.w, vertex.position.x, c[14].x;
MUL R0.xyz, vertex.normal, R0.x;
ADD R0.w, R0, c[15].x;
MAD R1.xyz, R0, R0.w, vertex.position;
DP4 R0.w, R1, c[8];
DP4 R0.z, R1, c[7];
DP4 R0.x, R1, c[5];
DP4 R0.y, R1, c[6];
MUL R2.xyz, R0.xyww, c[19].y;
MUL R2.y, R2, c[10].x;
MOV result.position, R0;
DP4 R0.x, R1, c[3];
ADD result.texcoord[1].xy, R2, R2.z;
MOV result.texcoord[1].zw, R0;
MAD result.texcoord[2].z, -R0.x, c[17], c[17].w;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[18], c[18].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[16], c[16].zwzw;
END
# 68 instructions, 5 R-regs
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
Vector 8 [_Time]
Vector 9 [_ProjectionParams]
Vector 10 [_ScreenParams]
Vector 11 [_WaveScale]
Vector 12 [_WavePeriod]
Vector 13 [_WaveAmp]
Float 14 [_XScale]
Float 15 [_XBias]
Vector 16 [unity_LightmapST]
Vector 17 [unity_LightmapFade]
Vector 18 [_MainTex_ST]
"vs_2_0
; 64 ALU
def c19, -0.02083333, -0.12500000, 1.00000000, 0.50000000
def c20, -0.00000155, -0.00002170, 0.00260417, 0.00026042
def c21, 0.15915491, 0.50000000, 6.28318501, -3.14159298
dcl_position0 v0
dcl_normal0 v2
dcl_texcoord0 v3
dcl_texcoord1 v4
mov r0.x, c8.w
mul r0.xyz, c12, r0.x
mad r1.xyz, v0, c11, r0
mad r0.x, r1, c21, c21.y
frc r0.x, r0
mad r1.x, r0, c21.z, c21.w
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
mov r0.x, r0.y
mad r0.z, r1, c21.x, c21.y
frc r0.y, r0.z
mad r0.x, r1.y, c21, c21.y
mad r0.y, r0, c21.z, c21.w
sincos r1.xy, r0.y, c20.xyzw, c19.xyzw
frc r0.x, r0
mad r1.x, r0, c21.z, c21.w
sincos r0.xy, r1.x, c20.xyzw, c19.xyzw
mov r0.z, r1.y
mul r0.xyz, r0, c13
dp3 r0.x, r0, c19.z
mul r0.w, v0.x, c14.x
mul r0.xyz, v2, r0.x
add r0.w, r0, c15.x
mad r1.xyz, r0, r0.w, v0
mov r1.w, v0
dp4 r0.w, r1, c7
dp4 r0.z, r1, c6
dp4 r0.x, r1, c4
dp4 r0.y, r1, c5
mul r2.xyz, r0.xyww, c19.w
mul r2.y, r2, c9.x
mov oPos, r0
dp4 r0.x, r1, c2
mad oT1.xy, r2.z, c10.zwzw, r2
mov oT1.zw, r0
mad oT2.z, -r0.x, c17, c17.w
mad oT0.xy, v3, c18, c18.zwzw
mad oT2.xy, v4, c16, c16.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 2 TEX
PARAM c[1] = { program.local[0] };
TEMP R0;
TEMP R1;
TXP R1.xyz, fragment.texcoord[1], texture[1], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
LG2 R1.x, R1.x;
LG2 R1.z, R1.z;
LG2 R1.y, R1.y;
ADD R1.xyz, -R1, c[0];
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
"ps_2_0
; 7 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
dcl t0.xy
dcl t1
texldp r0, t1, s1
texld r1, t0, s0
log_pp r0.x, r0.x
log_pp r0.z, r0.z
log_pp r0.y, r0.y
add_pp r0.xyz, -r0, c0
mul_pp r0.xyz, r1, r0
mov_pp r0.w, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 16 ALU, 4 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[2], texture[3], 2D;
TEX R2, fragment.texcoord[2], texture[2], 2D;
TXP R3.xyz, fragment.texcoord[1], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
MUL R1.xyz, R1, c[0].x;
MUL R2.xyz, R2.w, R2;
MAD R2.xyz, R2, c[0].x, -R1;
MOV_SAT R1.w, fragment.texcoord[2].z;
MAD R1.xyz, R1.w, R2, R1;
LG2 R2.x, R3.x;
LG2 R2.y, R3.y;
LG2 R2.z, R3.z;
ADD R1.xyz, -R2, R1;
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 16 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"ps_2_0
; 13 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c0, 8.00000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2.xyz
texld r2, t0, s0
texldp r3, t1, s1
texld r1, t2, s3
texld r0, t2, s2
mul_pp r1.xyz, r1.w, r1
mul_pp r1.xyz, r1, c0.x
mul_pp r0.xyz, r0.w, r0
mad_pp r0.xyz, r0, c0.x, -r1
mov_sat r4.x, t2.z
mad_pp r0.xyz, r4.x, r0, r1
log_pp r1.x, r3.x
log_pp r1.y, r3.y
log_pp r1.z, r3.z
add_pp r0.xyz, -r1, r0
mul_pp r0.xyz, r2, r0
mov_pp r0.w, r2
mov_pp oC0, r0
"
}
}
 }
}
Fallback "Diffuse"
}