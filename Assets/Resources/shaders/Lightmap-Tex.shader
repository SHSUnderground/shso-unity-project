//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Marvel/Lightmap/Diffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _LightMap ("Lightmap (RGB) AO (A)", 2D) = "black" {}
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [unity_SHAr]
Vector 11 [unity_SHAg]
Vector 12 [unity_SHAb]
Vector 13 [unity_SHBr]
Vector 14 [unity_SHBg]
Vector 15 [unity_SHBb]
Vector 16 [unity_SHC]
Vector 17 [_MainTex_ST]
Vector 18 [_LightMap_ST]
"!!ARBvp1.0
# 28 ALU
PARAM c[19] = { { 1 },
		state.matrix.mvp,
		program.local[5..18] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MUL R1.xyz, vertex.normal, c[9].w;
DP3 R3.w, R1, c[6];
DP3 R2.w, R1, c[7];
DP3 R0.x, R1, c[5];
MOV R0.y, R3.w;
MOV R0.z, R2.w;
MUL R1, R0.xyzz, R0.yzzx;
MOV R0.w, c[0].x;
DP4 R2.z, R0, c[12];
DP4 R2.y, R0, c[11];
DP4 R2.x, R0, c[10];
MUL R0.y, R3.w, R3.w;
DP4 R3.z, R1, c[15];
DP4 R3.y, R1, c[14];
DP4 R3.x, R1, c[13];
MAD R0.y, R0.x, R0.x, -R0;
MUL R1.xyz, R0.y, c[16];
ADD R2.xyz, R2, R3;
ADD result.texcoord[2].xyz, R2, R1;
MOV result.texcoord[1].z, R2.w;
MOV result.texcoord[1].y, R3.w;
MOV result.texcoord[1].x, R0;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[18].xyxy, c[18];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 28 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [unity_SHAr]
Vector 10 [unity_SHAg]
Vector 11 [unity_SHAb]
Vector 12 [unity_SHBr]
Vector 13 [unity_SHBg]
Vector 14 [unity_SHBb]
Vector 15 [unity_SHC]
Vector 16 [_MainTex_ST]
Vector 17 [_LightMap_ST]
"vs_2_0
; 28 ALU
def c18, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r1.xyz, v1, c8.w
dp3 r3.w, r1, c5
dp3 r2.w, r1, c6
dp3 r0.x, r1, c4
mov r0.y, r3.w
mov r0.z, r2.w
mul r1, r0.xyzz, r0.yzzx
mov r0.w, c18.x
dp4 r2.z, r0, c11
dp4 r2.y, r0, c10
dp4 r2.x, r0, c9
mul r0.y, r3.w, r3.w
dp4 r3.z, r1, c14
dp4 r3.y, r1, c13
dp4 r3.x, r1, c12
mad r0.y, r0.x, r0.x, -r0
mul r1.xyz, r0.y, c15
add r2.xyz, r2, r3
add oT2.xyz, r2, r1
mov oT1.z, r2.w
mov oT1.y, r3.w
mov oT1.x, r0
mad oT0.zw, v3.xyxy, c17.xyxy, c17
mad oT0.xy, v2, c16, c16.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [unity_LightmapST]
Vector 10 [_MainTex_ST]
Vector 11 [_LightMap_ST]
"!!ARBvp1.0
# 7 ALU
PARAM c[12] = { program.local[0],
		state.matrix.mvp,
		program.local[5..11] };
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[11].xyxy, c[11];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[10], c[10].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[9], c[9].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 7 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [unity_LightmapST]
Vector 9 [_MainTex_ST]
Vector 10 [_LightMap_ST]
"vs_2_0
; 7 ALU
dcl_position0 v0
dcl_texcoord0 v2
dcl_texcoord1 v3
mad oT0.zw, v3.xyxy, c10.xyxy, c10
mad oT0.xy, v2, c9, c9.zwzw
mad oT2.xy, v3, c8, c8.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [_ProjectionParams]
Vector 10 [unity_Scale]
Vector 11 [unity_SHAr]
Vector 12 [unity_SHAg]
Vector 13 [unity_SHAb]
Vector 14 [unity_SHBr]
Vector 15 [unity_SHBg]
Vector 16 [unity_SHBb]
Vector 17 [unity_SHC]
Vector 18 [_MainTex_ST]
Vector 19 [_LightMap_ST]
"!!ARBvp1.0
# 33 ALU
PARAM c[20] = { { 1, 0.5 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
MUL R0.xyz, vertex.normal, c[10].w;
DP3 R3.w, R0, c[6];
DP3 R2.w, R0, c[7];
DP3 R1.w, R0, c[5];
MOV R1.x, R3.w;
MOV R1.y, R2.w;
MOV R1.z, c[0].x;
MUL R0, R1.wxyy, R1.xyyw;
DP4 R2.z, R1.wxyz, c[13];
DP4 R2.y, R1.wxyz, c[12];
DP4 R2.x, R1.wxyz, c[11];
DP4 R1.z, R0, c[16];
DP4 R1.y, R0, c[15];
DP4 R1.x, R0, c[14];
MUL R3.x, R3.w, R3.w;
MAD R0.x, R1.w, R1.w, -R3;
ADD R3.xyz, R2, R1;
MUL R2.xyz, R0.x, c[17];
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].y;
MUL R1.y, R1, c[9].x;
ADD result.texcoord[2].xyz, R3, R2;
ADD result.texcoord[3].xy, R1, R1.z;
MOV result.position, R0;
MOV result.texcoord[3].zw, R0;
MOV result.texcoord[1].z, R2.w;
MOV result.texcoord[1].y, R3.w;
MOV result.texcoord[1].x, R1.w;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[19].xyxy, c[19];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[18], c[18].zwzw;
END
# 33 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
Vector 10 [unity_Scale]
Vector 11 [unity_SHAr]
Vector 12 [unity_SHAg]
Vector 13 [unity_SHAb]
Vector 14 [unity_SHBr]
Vector 15 [unity_SHBg]
Vector 16 [unity_SHBb]
Vector 17 [unity_SHC]
Vector 18 [_MainTex_ST]
Vector 19 [_LightMap_ST]
"vs_2_0
; 33 ALU
def c20, 1.00000000, 0.50000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r0.xyz, v1, c10.w
dp3 r3.w, r0, c5
dp3 r2.w, r0, c6
dp3 r1.w, r0, c4
mov r1.x, r3.w
mov r1.y, r2.w
mov r1.z, c20.x
mul r0, r1.wxyy, r1.xyyw
dp4 r2.z, r1.wxyz, c13
dp4 r2.y, r1.wxyz, c12
dp4 r2.x, r1.wxyz, c11
dp4 r1.z, r0, c16
dp4 r1.y, r0, c15
dp4 r1.x, r0, c14
mul r3.x, r3.w, r3.w
mad r0.x, r1.w, r1.w, -r3
add r3.xyz, r2, r1
mul r2.xyz, r0.x, c17
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c20.y
mul r1.y, r1, c8.x
add oT2.xyz, r3, r2
mad oT3.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mov oT3.zw, r0
mov oT1.z, r2.w
mov oT1.y, r3.w
mov oT1.x, r1.w
mad oT0.zw, v3.xyxy, c19.xyxy, c19
mad oT0.xy, v2, c18, c18.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [unity_LightmapST]
Vector 11 [_MainTex_ST]
Vector 12 [_LightMap_ST]
"!!ARBvp1.0
# 12 ALU
PARAM c[13] = { { 0.5 },
		state.matrix.mvp,
		program.local[5..12] };
TEMP R0;
TEMP R1;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[9].x;
ADD result.texcoord[3].xy, R1, R1.z;
MOV result.position, R0;
MOV result.texcoord[3].zw, R0;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[12].xyxy, c[12];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[11], c[11].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[10], c[10].zwzw;
END
# 12 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
Vector 10 [unity_LightmapST]
Vector 11 [_MainTex_ST]
Vector 12 [_LightMap_ST]
"vs_2_0
; 12 ALU
def c13, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v2
dcl_texcoord1 v3
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c13.x
mul r1.y, r1, c8.x
mad oT3.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mov oT3.zw, r0
mad oT0.zw, v3.xyxy, c12.xyxy, c12
mad oT0.xy, v2, c11, c11.zwzw
mad oT2.xy, v3, c10, c10.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
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
Vector 25 [_MainTex_ST]
Vector 26 [_LightMap_ST]
"!!ARBvp1.0
# 58 ALU
PARAM c[27] = { { 1, 0 },
		state.matrix.mvp,
		program.local[5..26] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MUL R3.xyz, vertex.normal, c[9].w;
DP3 R4.x, R3, c[5];
DP3 R3.w, R3, c[6];
DP3 R3.x, R3, c[7];
DP4 R0.x, vertex.position, c[6];
ADD R1, -R0.x, c[11];
MUL R2, R3.w, R1;
DP4 R0.x, vertex.position, c[5];
ADD R0, -R0.x, c[10];
MUL R1, R1, R1;
MOV R4.z, R3.x;
MOV R4.w, c[0].x;
MAD R2, R4.x, R0, R2;
DP4 R4.y, vertex.position, c[7];
MAD R1, R0, R0, R1;
ADD R0, -R4.y, c[12];
MAD R1, R0, R0, R1;
MAD R0, R3.x, R0, R2;
MUL R2, R1, c[13];
MOV R4.y, R3.w;
RSQ R1.x, R1.x;
RSQ R1.y, R1.y;
RSQ R1.w, R1.w;
RSQ R1.z, R1.z;
MUL R0, R0, R1;
ADD R1, R2, c[0].x;
DP4 R2.z, R4, c[20];
DP4 R2.y, R4, c[19];
DP4 R2.x, R4, c[18];
RCP R1.x, R1.x;
RCP R1.y, R1.y;
RCP R1.w, R1.w;
RCP R1.z, R1.z;
MAX R0, R0, c[0].y;
MUL R0, R0, R1;
MUL R1.xyz, R0.y, c[15];
MAD R1.xyz, R0.x, c[14], R1;
MAD R0.xyz, R0.z, c[16], R1;
MAD R1.xyz, R0.w, c[17], R0;
MUL R0, R4.xyzz, R4.yzzx;
MUL R1.w, R3, R3;
DP4 R4.w, R0, c[23];
DP4 R4.z, R0, c[22];
DP4 R4.y, R0, c[21];
MAD R1.w, R4.x, R4.x, -R1;
MUL R0.xyz, R1.w, c[24];
ADD R2.xyz, R2, R4.yzww;
ADD R0.xyz, R2, R0;
ADD result.texcoord[2].xyz, R0, R1;
MOV result.texcoord[1].z, R3.x;
MOV result.texcoord[1].y, R3.w;
MOV result.texcoord[1].x, R4;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[26].xyxy, c[26];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[25], c[25].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 58 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [unity_4LightPosX0]
Vector 10 [unity_4LightPosY0]
Vector 11 [unity_4LightPosZ0]
Vector 12 [unity_4LightAtten0]
Vector 13 [unity_LightColor0]
Vector 14 [unity_LightColor1]
Vector 15 [unity_LightColor2]
Vector 16 [unity_LightColor3]
Vector 17 [unity_SHAr]
Vector 18 [unity_SHAg]
Vector 19 [unity_SHAb]
Vector 20 [unity_SHBr]
Vector 21 [unity_SHBg]
Vector 22 [unity_SHBb]
Vector 23 [unity_SHC]
Vector 24 [_MainTex_ST]
Vector 25 [_LightMap_ST]
"vs_2_0
; 58 ALU
def c26, 1.00000000, 0.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r3.xyz, v1, c8.w
dp3 r4.x, r3, c4
dp3 r3.w, r3, c5
dp3 r3.x, r3, c6
dp4 r0.x, v0, c5
add r1, -r0.x, c10
mul r2, r3.w, r1
dp4 r0.x, v0, c4
add r0, -r0.x, c9
mul r1, r1, r1
mov r4.z, r3.x
mov r4.w, c26.x
mad r2, r4.x, r0, r2
dp4 r4.y, v0, c6
mad r1, r0, r0, r1
add r0, -r4.y, c11
mad r1, r0, r0, r1
mad r0, r3.x, r0, r2
mul r2, r1, c12
mov r4.y, r3.w
rsq r1.x, r1.x
rsq r1.y, r1.y
rsq r1.w, r1.w
rsq r1.z, r1.z
mul r0, r0, r1
add r1, r2, c26.x
dp4 r2.z, r4, c19
dp4 r2.y, r4, c18
dp4 r2.x, r4, c17
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c26.y
mul r0, r0, r1
mul r1.xyz, r0.y, c14
mad r1.xyz, r0.x, c13, r1
mad r0.xyz, r0.z, c15, r1
mad r1.xyz, r0.w, c16, r0
mul r0, r4.xyzz, r4.yzzx
mul r1.w, r3, r3
dp4 r4.w, r0, c22
dp4 r4.z, r0, c21
dp4 r4.y, r0, c20
mad r1.w, r4.x, r4.x, -r1
mul r0.xyz, r1.w, c23
add r2.xyz, r2, r4.yzww
add r0.xyz, r2, r0
add oT2.xyz, r0, r1
mov oT1.z, r3.x
mov oT1.y, r3.w
mov oT1.x, r4
mad oT0.zw, v3.xyxy, c25.xyxy, c25
mad oT0.xy, v2, c24, c24.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [_ProjectionParams]
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
Vector 26 [_MainTex_ST]
Vector 27 [_LightMap_ST]
"!!ARBvp1.0
# 64 ALU
PARAM c[28] = { { 1, 0, 0.5 },
		state.matrix.mvp,
		program.local[5..27] };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
MUL R3.xyz, vertex.normal, c[10].w;
DP3 R4.x, R3, c[5];
DP3 R3.w, R3, c[6];
DP3 R3.x, R3, c[7];
DP4 R0.x, vertex.position, c[6];
ADD R1, -R0.x, c[12];
MUL R2, R3.w, R1;
DP4 R0.x, vertex.position, c[5];
ADD R0, -R0.x, c[11];
MUL R1, R1, R1;
MOV R4.z, R3.x;
MOV R4.w, c[0].x;
MAD R2, R4.x, R0, R2;
DP4 R4.y, vertex.position, c[7];
MAD R1, R0, R0, R1;
ADD R0, -R4.y, c[13];
MAD R1, R0, R0, R1;
MAD R0, R3.x, R0, R2;
MUL R2, R1, c[14];
MOV R4.y, R3.w;
RSQ R1.x, R1.x;
RSQ R1.y, R1.y;
RSQ R1.w, R1.w;
RSQ R1.z, R1.z;
MUL R0, R0, R1;
ADD R1, R2, c[0].x;
DP4 R2.z, R4, c[21];
DP4 R2.y, R4, c[20];
DP4 R2.x, R4, c[19];
RCP R1.x, R1.x;
RCP R1.y, R1.y;
RCP R1.w, R1.w;
RCP R1.z, R1.z;
MAX R0, R0, c[0].y;
MUL R0, R0, R1;
MUL R1.xyz, R0.y, c[16];
MAD R1.xyz, R0.x, c[15], R1;
MAD R0.xyz, R0.z, c[17], R1;
MAD R1.xyz, R0.w, c[18], R0;
MUL R0, R4.xyzz, R4.yzzx;
MUL R1.w, R3, R3;
DP4 R4.w, R0, c[24];
DP4 R4.z, R0, c[23];
DP4 R4.y, R0, c[22];
MAD R1.w, R4.x, R4.x, -R1;
MUL R0.xyz, R1.w, c[25];
ADD R2.xyz, R2, R4.yzww;
ADD R4.yzw, R2.xxyz, R0.xxyz;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R2.xyz, R0.xyww, c[0].z;
ADD result.texcoord[2].xyz, R4.yzww, R1;
MOV R1.x, R2;
MUL R1.y, R2, c[9].x;
ADD result.texcoord[3].xy, R1, R2.z;
MOV result.position, R0;
MOV result.texcoord[3].zw, R0;
MOV result.texcoord[1].z, R3.x;
MOV result.texcoord[1].y, R3.w;
MOV result.texcoord[1].x, R4;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[27].xyxy, c[27];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[26], c[26].zwzw;
END
# 64 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
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
Vector 26 [_MainTex_ST]
Vector 27 [_LightMap_ST]
"vs_2_0
; 64 ALU
def c28, 1.00000000, 0.00000000, 0.50000000, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r3.xyz, v1, c10.w
dp3 r4.x, r3, c4
dp3 r3.w, r3, c5
dp3 r3.x, r3, c6
dp4 r0.x, v0, c5
add r1, -r0.x, c12
mul r2, r3.w, r1
dp4 r0.x, v0, c4
add r0, -r0.x, c11
mul r1, r1, r1
mov r4.z, r3.x
mov r4.w, c28.x
mad r2, r4.x, r0, r2
dp4 r4.y, v0, c6
mad r1, r0, r0, r1
add r0, -r4.y, c13
mad r1, r0, r0, r1
mad r0, r3.x, r0, r2
mul r2, r1, c14
mov r4.y, r3.w
rsq r1.x, r1.x
rsq r1.y, r1.y
rsq r1.w, r1.w
rsq r1.z, r1.z
mul r0, r0, r1
add r1, r2, c28.x
dp4 r2.z, r4, c21
dp4 r2.y, r4, c20
dp4 r2.x, r4, c19
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c28.y
mul r0, r0, r1
mul r1.xyz, r0.y, c16
mad r1.xyz, r0.x, c15, r1
mad r0.xyz, r0.z, c17, r1
mad r1.xyz, r0.w, c18, r0
mul r0, r4.xyzz, r4.yzzx
mul r1.w, r3, r3
dp4 r4.w, r0, c24
dp4 r4.z, r0, c23
dp4 r4.y, r0, c22
mad r1.w, r4.x, r4.x, -r1
mul r0.xyz, r1.w, c25
add r2.xyz, r2, r4.yzww
add r4.yzw, r2.xxyz, r0.xxyz
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r2.xyz, r0.xyww, c28.z
add oT2.xyz, r4.yzww, r1
mov r1.x, r2
mul r1.y, r2, c8.x
mad oT3.xy, r2.z, c9.zwzw, r1
mov oPos, r0
mov oT3.zw, r0
mov oT1.z, r3.x
mov oT1.y, r3.w
mov oT1.x, r4
mad oT0.zw, v3.xyxy, c27.xyxy, c27
mad oT0.xy, v2, c26, c26.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 11 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R1, fragment.texcoord[0], texture[0], 2D;
TEX R0, fragment.texcoord[0].zwzw, texture[1], 2D;
MUL R1.xyz, R1, R0.w;
DP3 R0.w, fragment.texcoord[1], c[0];
MUL R3.xyz, R1, fragment.texcoord[2];
MUL R2.xyz, R1, c[1];
MAX R0.w, R0, c[2].x;
MUL R2.xyz, R0.w, R2;
MAD R2.xyz, R2, c[2].y, R3;
MAD result.color.xyz, R1, R0, R2;
MOV result.color.w, R1;
END
# 11 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
"ps_2_0
; 11 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
mov r0.y, t0.w
mov r0.x, t0.z
texld r1, r0, s1
texld r0, t0, s0
mul_pp r2.xyz, r0, r1.w
dp3_pp r0.x, t1, c0
mul_pp r4.xyz, r2, t2
mul_pp r3.xyz, r2, c1
max_pp r0.x, r0, c2
mul_pp r0.xyz, r0.x, r3
mad_pp r0.xyz, r0, c2.y, r4
mad_pp r0.xyz, r2, r1, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 3 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R2, fragment.texcoord[2], texture[2], 2D;
MUL R0.xyz, R0, R1.w;
MUL R1.xyz, R0, R1;
MUL R2.xyz, R2.w, R2;
MUL R0.xyz, R2, R0;
MAD result.color.xyz, R0, c[0].x, R1;
MOV result.color.w, R0;
END
# 9 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [unity_Lightmap] 2D
"ps_2_0
; 10 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c0, 8.00000000, 0, 0, 0
dcl t0
dcl t2.xy
texld r2, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
texld r0, t2, s2
texld r1, r1, s1
mul_pp r2.xyz, r2, r1.w
mul_pp r0.xyz, r0.w, r0
mul_pp r0.xyz, r0, r2
mul_pp r1.xyz, r2, r1
mov_pp r0.w, r2
mad_pp r0.xyz, r0, c0.x, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 13 ALU, 3 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TXP R2.x, fragment.texcoord[3], texture[2], 2D;
MUL R2.yzw, R0.xxyz, R1.w;
DP3 R1.w, fragment.texcoord[1], c[0];
MAX R1.w, R1, c[2].x;
MUL R3.xyz, R2.yzww, fragment.texcoord[2];
MUL R0.xyz, R2.yzww, c[1];
MUL R1.w, R1, R2.x;
MUL R0.xyz, R1.w, R0;
MAD R0.xyz, R0, c[2].y, R3;
MAD result.color.xyz, R2.yzww, R1, R0;
MOV result.color.w, R0;
END
# 13 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
"ps_2_0
; 12 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c2, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
dcl t3
texldp r5, t3, s2
mov r0.y, t0.w
mov r0.x, t0.z
texld r1, r0, s1
texld r0, t0, s0
mul_pp r2.xyz, r0, r1.w
dp3_pp r0.x, t1, c0
max_pp r0.x, r0, c2
mul_pp r4.xyz, r2, t2
mul_pp r3.xyz, r2, c1
mul_pp r0.x, r0, r5
mul_pp r0.xyz, r0.x, r3
mad_pp r0.xyz, r0, c2.y, r4
mad_pp r0.xyz, r2, r1, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
SetTexture 3 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 4 TEX
PARAM c[1] = { { 8, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R2, fragment.texcoord[2], texture[3], 2D;
TXP R3.x, fragment.texcoord[3], texture[2], 2D;
MUL R0.xyz, R0, R1.w;
MUL R2.xyz, R2.w, R2;
MUL R1.xyz, R0, R1;
MUL R1.w, R3.x, c[0].y;
MUL R2.xyz, R2, c[0].x;
MIN R2.xyz, R2, R1.w;
MAD result.color.xyz, R0, R2, R1;
MOV result.color.w, R0;
END
# 12 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_ShadowMapTexture] 2D
SetTexture 3 [unity_Lightmap] 2D
"ps_2_0
; 12 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c0, 8.00000000, 2.00000000, 0, 0
dcl t0
dcl t2.xy
dcl t3
texldp r3, t3, s2
texld r2, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
mul_pp r3.x, r3, c0.y
texld r0, t2, s3
texld r1, r1, s1
mul_pp r0.xyz, r0.w, r0
mul_pp r2.xyz, r2, r1.w
mul_pp r0.xyz, r0, c0.x
mul_pp r1.xyz, r2, r1
min_pp r0.xyz, r0, r3.x
mov_pp r0.w, r2
mad_pp r0.xyz, r2, r0, r1
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_MainTex_ST]
Vector 16 [_LightMap_ST]
"!!ARBvp1.0
# 18 ALU
PARAM c[17] = { program.local[0],
		state.matrix.mvp,
		program.local[5..16] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[3].z, R0, c[11];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
DP3 result.texcoord[1].z, R1, c[7];
DP3 result.texcoord[1].y, R1, c[6];
DP3 result.texcoord[1].x, R1, c[5];
ADD result.texcoord[2].xyz, -R0, c[14];
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[16].xyxy, c[16];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 18 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_MainTex_ST]
Vector 15 [_LightMap_ST]
"vs_2_0
; 18 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT3.z, r0, c10
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
dp3 oT1.z, r1, c6
dp3 oT1.y, r1, c5
dp3 oT1.x, r1, c4
add oT2.xyz, -r0, c13
mad oT0.zw, v3.xyxy, c15.xyxy, c15
mad oT0.xy, v2, c14, c14.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceLightPos0]
Vector 11 [_MainTex_ST]
Vector 12 [_LightMap_ST]
"!!ARBvp1.0
# 11 ALU
PARAM c[13] = { program.local[0],
		state.matrix.mvp,
		program.local[5..12] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
DP3 result.texcoord[1].z, R0, c[7];
DP3 result.texcoord[1].y, R0, c[6];
DP3 result.texcoord[1].x, R0, c[5];
MOV result.texcoord[2].xyz, c[10];
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[12].xyxy, c[12];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[11], c[11].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 11 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [_WorldSpaceLightPos0]
Vector 10 [_MainTex_ST]
Vector 11 [_LightMap_ST]
"vs_2_0
; 11 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r0.xyz, v1, c8.w
dp3 oT1.z, r0, c6
dp3 oT1.y, r0, c5
dp3 oT1.x, r0, c4
mov oT2.xyz, c9
mad oT0.zw, v3.xyxy, c11.xyxy, c11
mad oT0.xy, v2, c10, c10.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_MainTex_ST]
Vector 16 [_LightMap_ST]
"!!ARBvp1.0
# 19 ALU
PARAM c[17] = { program.local[0],
		state.matrix.mvp,
		program.local[5..16] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[3].w, R0, c[12];
DP4 result.texcoord[3].z, R0, c[11];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
DP3 result.texcoord[1].z, R1, c[7];
DP3 result.texcoord[1].y, R1, c[6];
DP3 result.texcoord[1].x, R1, c[5];
ADD result.texcoord[2].xyz, -R0, c[14];
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[16].xyxy, c[16];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 19 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_MainTex_ST]
Vector 15 [_LightMap_ST]
"vs_2_0
; 19 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT3.w, r0, c11
dp4 oT3.z, r0, c10
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
dp3 oT1.z, r1, c6
dp3 oT1.y, r1, c5
dp3 oT1.x, r1, c4
add oT2.xyz, -r0, c13
mad oT0.zw, v3.xyxy, c15.xyxy, c15
mad oT0.xy, v2, c14, c14.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_MainTex_ST]
Vector 16 [_LightMap_ST]
"!!ARBvp1.0
# 18 ALU
PARAM c[17] = { program.local[0],
		state.matrix.mvp,
		program.local[5..16] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[3].z, R0, c[11];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
DP3 result.texcoord[1].z, R1, c[7];
DP3 result.texcoord[1].y, R1, c[6];
DP3 result.texcoord[1].x, R1, c[5];
ADD result.texcoord[2].xyz, -R0, c[14];
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[16].xyxy, c[16];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 18 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_MainTex_ST]
Vector 15 [_LightMap_ST]
"vs_2_0
; 18 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 oT3.z, r0, c10
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
dp3 oT1.z, r1, c6
dp3 oT1.y, r1, c5
dp3 oT1.x, r1, c4
add oT2.xyz, -r0, c13
mad oT0.zw, v3.xyxy, c15.xyxy, c15
mad oT0.xy, v2, c14, c14.zwzw
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
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_MainTex_ST]
Vector 16 [_LightMap_ST]
"!!ARBvp1.0
# 17 ALU
PARAM c[17] = { program.local[0],
		state.matrix.mvp,
		program.local[5..16] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 result.texcoord[3].y, R0, c[10];
DP4 result.texcoord[3].x, R0, c[9];
DP3 result.texcoord[1].z, R1, c[7];
DP3 result.texcoord[1].y, R1, c[6];
DP3 result.texcoord[1].x, R1, c[5];
MOV result.texcoord[2].xyz, c[14];
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[16].xyxy, c[16];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[15], c[15].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 17 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_MainTex_ST]
Vector 15 [_LightMap_ST]
"vs_2_0
; 17 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_texcoord1 v3
mul r1.xyz, v1, c12.w
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 oT3.y, r0, c9
dp4 oT3.x, r0, c8
dp3 oT1.z, r1, c6
dp3 oT1.y, r1, c5
dp3 oT1.x, r1, c4
mov oT2.xyz, c13
mad oT0.zw, v3.xyxy, c15.xyxy, c15
mad oT0.xy, v2, c14, c14.zwzw
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
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[0].zwzw, texture[1], 2D;
DP3 R0.w, fragment.texcoord[3], fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
MUL R0.xyz, R0, R1.w;
DP3 R1.x, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[1];
MOV result.color.w, c[1].x;
TEX R0.w, R0.w, texture[2], 2D;
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
END
# 15 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
"ps_2_0
; 16 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dp3 r0.x, t3, t3
mov r1.xy, r0.x
mov r0.y, t0.w
mov r0.x, t0.z
texld r2, r1, s2
texld r0, r0, s1
texld r1, t0, s0
dp3_pp r0.x, t2, t2
mul_pp r1.xyz, r1, r0.w
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t2
dp3_pp r0.x, t1, r0
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c1
mul_pp r0.x, r0, r2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c1.y
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.w, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
MUL R0.xyz, R0, R0.w;
MOV R1.xyz, fragment.texcoord[2];
DP3 R0.w, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MAX R0.w, R0, c[1].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
MOV result.color.w, c[1].x;
END
# 10 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
"ps_2_0
; 11 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
texld r1, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mul_pp r1.xyz, r1, r0.w
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
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
SetTexture 3 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 21 ALU, 4 TEX
PARAM c[2] = { program.local[0],
		{ 0, 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R2.w, fragment.texcoord[0].zwzw, texture[1], 2D;
RCP R0.x, fragment.texcoord[3].w;
MAD R1.xy, fragment.texcoord[3], R0.x, c[1].y;
DP3 R1.z, fragment.texcoord[3], fragment.texcoord[3];
MOV result.color.w, c[1].x;
TEX R0.w, R1, texture[2], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, R1.z, texture[3], 2D;
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
DP3 R1.x, fragment.texcoord[1], R1;
SLT R1.y, c[1].x, fragment.texcoord[3].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MUL R0.xyz, R0, R2.w;
MAX R0.w, R1.x, c[1].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].z;
END
# 21 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
SetTexture 3 [_LightTextureB0] 2D
"ps_2_0
; 22 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c1, 0.50000000, 0.00000000, 1.00000000, 2.00000000
dcl t0
dcl t1.xyz
dcl t2.xyz
dcl t3
dp3 r2.x, t3, t3
mov r2.xy, r2.x
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
rcp r0.x, t3.w
mad r0.xy, t3, r0.x, c1.x
texld r3, r2, s3
texld r1, r1, s1
texld r2, t0, s0
texld r0, r0, s2
mul_pp r0.xyz, r2, r1.w
mul_pp r2.xyz, r0, c0
dp3_pp r0.x, t2, t2
rsq_pp r1.x, r0.x
cmp r0.x, -t3.z, c1.y, c1.z
mul_pp r0.x, r0, r0.w
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
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTextureB0] 2D
SetTexture 3 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 17 ALU, 4 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R2.w, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R1.w, fragment.texcoord[3], texture[3], CUBE;
DP3 R0.w, fragment.texcoord[3], fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[2];
MUL R0.xyz, R0, R2.w;
DP3 R1.x, fragment.texcoord[1], R1;
MUL R0.xyz, R0, c[0];
MOV result.color.w, c[1].x;
TEX R0.w, R0.w, texture[2], 2D;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[1].x;
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
END
# 17 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTextureB0] 2D
SetTexture 3 [_LightTexture0] CUBE
"ps_2_0
; 18 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_cube s3
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
texld r3, t0, s0
dp3 r0.x, t3, t3
mov r1.y, t0.w
mov r1.x, t0.z
mov r2.xy, r1
mov r1.xy, r0.x
texld r0, t3, s3
texld r2, r2, s1
texld r1, r1, s2
mul_pp r2.xyz, r3, r2.w
dp3_pp r0.x, t2, t2
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t2
dp3_pp r0.x, t1, r0
mul r1.x, r1, r0.w
max_pp r0.x, r0, c1
mul_pp r2.xyz, r2, c0
mul_pp r0.x, r0, r1
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c1.y
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R0.w, fragment.texcoord[3], texture[2], 2D;
MOV R1.xyz, fragment.texcoord[2];
MUL R0.xyz, R0, R1.w;
DP3 R1.x, fragment.texcoord[1], R1;
MAX R1.x, R1, c[1];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[1].y;
MOV result.color.w, c[1].x;
END
# 12 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightTexture0] 2D
"ps_2_0
; 13 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c1, 0.00000000, 2.00000000, 0, 0
dcl t0
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r2, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
texld r0, t3, s2
texld r1, r1, s1
mov_pp r0.xyz, t2
dp3_pp r0.x, t1, r0
mul_pp r1.xyz, r2, r1.w
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
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
"!!ARBvp1.0
# 8 ALU
PARAM c[10] = { program.local[0],
		state.matrix.mvp,
		program.local[5..9] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 8 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
"vs_2_0
; 8 ALU
dcl_position0 v0
dcl_normal0 v1
mul r0.xyz, v1, c8.w
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
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
Program "vp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 5 [_ProjectionParams]
Vector 6 [_MainTex_ST]
Vector 7 [_LightMap_ST]
"!!ARBvp1.0
# 11 ALU
PARAM c[8] = { { 0.5 },
		state.matrix.mvp,
		program.local[5..7] };
TEMP R0;
TEMP R1;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[5].x;
ADD result.texcoord[1].xy, R1, R1.z;
MOV result.position, R0;
MOV result.texcoord[1].zw, R0;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[7].xyxy, c[7];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[6], c[6].zwzw;
END
# 11 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ProjectionParams]
Vector 5 [_ScreenParams]
Vector 6 [_MainTex_ST]
Vector 7 [_LightMap_ST]
"vs_2_0
; 11 ALU
def c8, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_texcoord1 v2
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c8.x
mul r1.y, r1, c4.x
mad oT1.xy, r1.z, c5.zwzw, r1
mov oPos, r0
mov oT1.zw, r0
mad oT0.zw, v2.xyxy, c7.xyxy, c7
mad oT0.xy, v1, c6, c6.zwzw
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [unity_LightmapST]
Vector 11 [unity_LightmapFade]
Vector 12 [_MainTex_ST]
Vector 13 [_LightMap_ST]
"!!ARBvp1.0
# 14 ALU
PARAM c[14] = { { 0.5 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9..13] };
TEMP R0;
TEMP R1;
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[9].x;
MOV result.position, R0;
DP4 R0.x, vertex.position, c[3];
ADD result.texcoord[1].xy, R1, R1.z;
MOV result.texcoord[1].zw, R0;
MAD result.texcoord[0].zw, vertex.texcoord[1].xyxy, c[13].xyxy, c[13];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[12], c[12].zwzw;
MAD result.texcoord[2].xy, vertex.texcoord[1], c[10], c[10].zwzw;
MAD result.texcoord[2].z, -R0.x, c[11], c[11].w;
END
# 14 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
Vector 10 [unity_LightmapST]
Vector 11 [unity_LightmapFade]
Vector 12 [_MainTex_ST]
Vector 13 [_LightMap_ST]
"vs_2_0
; 14 ALU
def c14, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_texcoord1 v2
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mul r1.xyz, r0.xyww, c14.x
mul r1.y, r1, c8.x
mov oPos, r0
dp4 r0.x, v0, c2
mad oT1.xy, r1.z, c9.zwzw, r1
mov oT1.zw, r0
mad oT0.zw, v2.xyxy, c13.xyxy, c13
mad oT0.xy, v1, c12, c12.zwzw
mad oT2.xy, v2, c10, c10.zwzw
mad oT2.z, -r0.x, c11, c11.w
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightBuffer] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 11 ALU, 3 TEX
PARAM c[1] = { program.local[0] };
TEMP R0;
TEMP R1;
TEMP R2;
TXP R2.xyz, fragment.texcoord[1], texture[2], 2D;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
MUL R0.xyz, R0, R1.w;
MUL R1.xyz, R0, R1;
LG2 R2.x, R2.x;
LG2 R2.z, R2.z;
LG2 R2.y, R2.y;
ADD R2.xyz, -R2, c[0];
MAD result.color.xyz, R0, R2, R1;
MOV result.color.w, R0;
END
# 11 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [unity_Ambient]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightBuffer] 2D
"ps_2_0
; 11 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl t0
dcl t1
texldp r2, t1, s2
texld r1, t0, s0
mov r0.y, t0.w
mov r0.x, t0.z
log_pp r2.x, r2.x
log_pp r2.z, r2.z
log_pp r2.y, r2.y
add_pp r2.xyz, -r2, c0
texld r0, r0, s1
mul_pp r1.xyz, r1, r0.w
mul_pp r0.xyz, r1, r0
mov_pp r0.w, r1
mad_pp r0.xyz, r1, r2, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightBuffer] 2D
SetTexture 3 [unity_Lightmap] 2D
SetTexture 4 [unity_LightmapInd] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 19 ALU, 5 TEX
PARAM c[1] = { { 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R3, fragment.texcoord[2], texture[3], 2D;
TEX R2, fragment.texcoord[2], texture[4], 2D;
TXP R4.xyz, fragment.texcoord[1], texture[2], 2D;
MUL R0.xyz, R0, R1.w;
MUL R2.xyz, R2.w, R2;
MUL R2.xyz, R2, c[0].x;
MUL R3.xyz, R3.w, R3;
MAD R3.xyz, R3, c[0].x, -R2;
MOV_SAT R1.w, fragment.texcoord[2].z;
MAD R2.xyz, R1.w, R3, R2;
MUL R1.xyz, R0, R1;
LG2 R3.x, R4.x;
LG2 R3.y, R4.y;
LG2 R3.z, R4.z;
ADD R2.xyz, -R3, R2;
MAD result.color.xyz, R0, R2, R1;
MOV result.color.w, R0;
END
# 19 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightMap] 2D
SetTexture 2 [_LightBuffer] 2D
SetTexture 3 [unity_Lightmap] 2D
SetTexture 4 [unity_LightmapInd] 2D
"ps_2_0
; 17 ALU, 5 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
dcl_2d s4
def c0, 8.00000000, 0, 0, 0
dcl t0
dcl t1
dcl t2.xyz
texld r3, t0, s0
texldp r4, t1, s2
texld r1, t2, s4
mul_pp r1.xyz, r1.w, r1
mul_pp r1.xyz, r1, c0.x
mov r0.y, t0.w
mov r0.x, t0.z
mov_sat r5.x, t2.z
texld r2, r0, s1
texld r0, t2, s3
mul_pp r0.xyz, r0.w, r0
mad_pp r0.xyz, r0, c0.x, -r1
mad_pp r0.xyz, r5.x, r0, r1
mul_pp r1.xyz, r3, r2.w
log_pp r3.x, r4.x
log_pp r3.y, r4.y
log_pp r3.z, r4.z
add_pp r0.xyz, -r3, r0
mul_pp r2.xyz, r1, r2
mov_pp r0.w, r3
mad_pp r0.xyz, r1, r0, r2
mov_pp oC0, r0
"
}
}
 }
}
Fallback "VertexLit"
}