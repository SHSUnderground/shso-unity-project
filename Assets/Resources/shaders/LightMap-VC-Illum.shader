//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Marvel/Lightmap/Vertex Color-Self-Illuminated" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _LightMap ("Lightmap (RGB) AO (A)", 2D) = "black" {}
 _Falloff ("Falloff", Float) = 0.5
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
Bind "color" Color
Bind "normal" Normal
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
Vector 17 [_Color]
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
ADD result.texcoord[3].xyz, R2, R1;
MUL result.texcoord[1], vertex.color, c[17];
MOV result.texcoord[2].z, R2.w;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R0;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[18], c[18].zwzw;
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
Bind "color" Color
Bind "normal" Normal
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
Vector 16 [_Color]
Vector 17 [_LightMap_ST]
"vs_2_0
; 28 ALU
def c18, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
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
add oT3.xyz, r2, r1
mul oT1, v3, c16
mov oT2.z, r2.w
mov oT2.y, r3.w
mov oT2.x, r0
mad oT0.xy, v2, c17, c17.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 9 [_Color]
Vector 10 [unity_LightmapST]
Vector 11 [_LightMap_ST]
"!!ARBvp1.0
# 7 ALU
PARAM c[12] = { program.local[0],
		state.matrix.mvp,
		program.local[5..11] };
MUL result.texcoord[1], vertex.color, c[9];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[11], c[11].zwzw;
MAD result.texcoord[3].xy, vertex.texcoord[1], c[10], c[10].zwzw;
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
Bind "color" Color
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [_Color]
Vector 9 [unity_LightmapST]
Vector 10 [_LightMap_ST]
"vs_2_0
; 7 ALU
dcl_position0 v0
dcl_texcoord1 v2
dcl_color0 v3
mul oT1, v3, c8
mad oT0.xy, v2, c10, c10.zwzw
mad oT3.xy, v2, c9, c9.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
Vector 18 [_Color]
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
ADD result.texcoord[3].xyz, R3, R2;
ADD result.texcoord[4].xy, R1, R1.z;
MOV result.position, R0;
MUL result.texcoord[1], vertex.color, c[18];
MOV result.texcoord[4].zw, R0;
MOV result.texcoord[2].z, R2.w;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R1.w;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[19], c[19].zwzw;
END
# 33 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
Vector 18 [_Color]
Vector 19 [_LightMap_ST]
"vs_2_0
; 33 ALU
def c20, 1.00000000, 0.50000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
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
add oT3.xyz, r3, r2
mad oT4.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mul oT1, v3, c18
mov oT4.zw, r0
mov oT2.z, r2.w
mov oT2.y, r3.w
mov oT2.x, r1.w
mad oT0.xy, v2, c19, c19.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [_Color]
Vector 11 [unity_LightmapST]
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
ADD result.texcoord[4].xy, R1, R1.z;
MOV result.position, R0;
MUL result.texcoord[1], vertex.color, c[10];
MOV result.texcoord[4].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[12], c[12].zwzw;
MAD result.texcoord[3].xy, vertex.texcoord[1], c[11], c[11].zwzw;
END
# 12 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
Vector 10 [_Color]
Vector 11 [unity_LightmapST]
Vector 12 [_LightMap_ST]
"vs_2_0
; 12 ALU
def c13, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord1 v2
dcl_color0 v3
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c13.x
mul r1.y, r1, c8.x
mad oT4.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mul oT1, v3, c10
mov oT4.zw, r0
mad oT0.xy, v2, c12, c12.zwzw
mad oT3.xy, v2, c11, c11.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
Vector 25 [_Color]
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
ADD result.texcoord[3].xyz, R0, R1;
MUL result.texcoord[1], vertex.color, c[25];
MOV result.texcoord[2].z, R3.x;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R4;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[26], c[26].zwzw;
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
Bind "color" Color
Bind "normal" Normal
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
Vector 24 [_Color]
Vector 25 [_LightMap_ST]
"vs_2_0
; 58 ALU
def c26, 1.00000000, 0.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
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
add oT3.xyz, r0, r1
mul oT1, v3, c24
mov oT2.z, r3.x
mov oT2.y, r3.w
mov oT2.x, r4
mad oT0.xy, v2, c25, c25.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
Vector 26 [_Color]
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
ADD result.texcoord[3].xyz, R4.yzww, R1;
MOV R1.x, R2;
MUL R1.y, R2, c[9].x;
ADD result.texcoord[4].xy, R1, R2.z;
MOV result.position, R0;
MUL result.texcoord[1], vertex.color, c[26];
MOV result.texcoord[4].zw, R0;
MOV result.texcoord[2].z, R3.x;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R4;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[27], c[27].zwzw;
END
# 64 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
Vector 26 [_Color]
Vector 27 [_LightMap_ST]
"vs_2_0
; 64 ALU
def c28, 1.00000000, 0.00000000, 0.50000000, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
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
add oT3.xyz, r4.yzww, r1
mov r1.x, r2
mul r1.y, r2, c8.x
mad oT4.xy, r2.z, c9.zwzw, r1
mov oPos, r0
mul oT1, v3, c26
mov oT4.zw, r0
mov oT2.z, r3.x
mov oT2.y, r3.w
mov oT2.x, r4
mad oT0.xy, v2, c27, c27.zwzw
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Float 2 [_Falloff]
SetTexture 0 [_LightMap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 16 ALU, 1 TEX
PARAM c[4] = { program.local[0..2],
		{ 1, 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R1.x, R0.w, -c[3];
POW_SAT R0.w, fragment.texcoord[1].w, c[2].x;
MAD R1.x, R0.w, R1, c[3];
ADD R0.xyz, -fragment.texcoord[1], R0;
MAD R0.xyz, R0.w, R0, fragment.texcoord[1];
MUL R1.xyz, fragment.texcoord[1], R1.x;
MUL R2.xyz, R1, R0;
MUL R0.xyz, R1, fragment.texcoord[3];
DP3 R0.w, fragment.texcoord[2], c[0];
MUL R1.xyz, R1, c[1];
MAX R0.w, R0, c[3].y;
MUL R1.xyz, R0.w, R1;
MAD R0.xyz, R1, c[3].z, R0;
ADD result.color.xyz, R0, R2;
MOV result.color.w, c[3].x;
END
# 16 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Float 2 [_Falloff]
SetTexture 0 [_LightMap] 2D
"ps_2_0
; 18 ALU, 1 TEX
dcl_2d s0
def c3, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
texld r2, t0, s0
pow_sat r0.x, t1.w, c2.x
add r2.xyz, -t1, r2
add_pp r1.x, r2.w, c3
mad_pp r1.x, r0.x, r1, c3.y
mul r1.xyz, t1, r1.x
mad r4.xyz, r0.x, r2, t1
dp3_pp r0.x, t2, c0
mul_pp r3.xyz, r1, t3
mul_pp r2.xyz, r1, c1
max_pp r0.x, r0, c3.z
mul_pp r0.xyz, r0.x, r2
mad_pp r0.xyz, r0, c3.w, r3
mul r1.xyz, r1, r4
mov_pp r0.w, c3.y
add_pp r0.xyz, r0, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 13 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 1, 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[3], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
ADD R1.w, R0, -c[1].x;
POW_SAT R0.w, fragment.texcoord[1].w, c[0].x;
ADD R0.xyz, -fragment.texcoord[1], R0;
MAD R2.xyz, R0.w, R0, fragment.texcoord[1];
MAD R1.w, R0, R1, c[1].x;
MUL R0.xyz, fragment.texcoord[1], R1.w;
MUL R2.xyz, R0, R2;
MUL R0.xyz, R1, R0;
MAD result.color.xyz, R0, c[1].y, R2;
MOV result.color.w, c[1].x;
END
# 13 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 14 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, -1.00000000, 1.00000000, 8.00000000, 0
dcl t0.xy
dcl t1
dcl t3.xy
texld r1, t3, s1
texld r2, t0, s0
pow_sat r0.x, t1.w, c0.x
add_pp r3.x, r2.w, c1
mad_pp r3.x, r0.x, r3, c1.y
add r2.xyz, -t1, r2
mul r3.xyz, t1, r3.x
mad r0.xyz, r0.x, r2, t1
mul_pp r1.xyz, r1.w, r1
mul r0.xyz, r3, r0
mul_pp r1.xyz, r1, r3
mov_pp r0.w, c1.y
mad_pp r0.xyz, r1, c1.z, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Float 2 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 18 ALU, 2 TEX
PARAM c[4] = { program.local[0..2],
		{ 1, 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TXP R1.x, fragment.texcoord[4], texture[1], 2D;
ADD R1.y, R0.w, -c[3].x;
POW_SAT R0.w, fragment.texcoord[1].w, c[2].x;
ADD R0.xyz, -fragment.texcoord[1], R0;
MAD R1.y, R0.w, R1, c[3].x;
MAD R2.xyz, R0.w, R0, fragment.texcoord[1];
MUL R0.xyz, fragment.texcoord[1], R1.y;
DP3 R0.w, fragment.texcoord[2], c[0];
MAX R0.w, R0, c[3].y;
MUL R2.xyz, R0, R2;
MUL R1.yzw, R0.xxyz, fragment.texcoord[3].xxyz;
MUL R0.xyz, R0, c[1];
MUL R0.w, R0, R1.x;
MUL R0.xyz, R0.w, R0;
MAD R0.xyz, R0, c[3].z, R1.yzww;
ADD result.color.xyz, R0, R2;
MOV result.color.w, c[3].x;
END
# 18 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Float 2 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_ShadowMapTexture] 2D
"ps_2_0
; 19 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c3, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
dcl t4
texldp r5, t4, s1
texld r2, t0, s0
pow_sat r0.x, t1.w, c2.x
add_pp r1.x, r2.w, c3
mad_pp r1.x, r0.x, r1, c3.y
mul r3.xyz, t1, r1.x
dp3_pp r1.x, t2, c0
mul_pp r4.xyz, r3, c1
max_pp r1.x, r1, c3.z
mul_pp r1.x, r1, r5
mul_pp r1.xyz, r1.x, r4
add r4.xyz, -t1, r2
mul_pp r2.xyz, r3, t3
mad r0.xyz, r0.x, r4, t1
mad_pp r1.xyz, r1, c3.w, r2
mul r0.xyz, r3, r0
mov_pp r0.w, c3.y
add_pp r0.xyz, r1, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 16 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 1, 8, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[3], texture[2], 2D;
TXP R2.x, fragment.texcoord[4], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
POW_SAT R2.y, fragment.texcoord[1].w, c[0].x;
ADD R0.xyz, -fragment.texcoord[1], R0;
ADD R0.w, R0, -c[1].x;
MAD R0.w, R2.y, R0, c[1].x;
MAD R0.xyz, R2.y, R0, fragment.texcoord[1];
MUL R2.yzw, fragment.texcoord[1].xxyz, R0.w;
MUL R0.xyz, R2.yzww, R0;
MUL R0.w, R2.x, c[1].z;
MUL R1.xyz, R1, c[1].y;
MIN R1.xyz, R1, R0.w;
MAD result.color.xyz, R2.yzww, R1, R0;
MOV result.color.w, c[1].x;
END
# 16 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_ShadowMapTexture] 2D
SetTexture 2 [unity_Lightmap] 2D
"ps_2_0
; 16 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c1, -1.00000000, 1.00000000, 8.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t3.xy
dcl t4
texldp r4, t4, s1
texld r0, t0, s0
texld r1, t3, s2
add r3.xyz, -t1, r0
pow_sat r2.x, t1.w, c0.x
add_pp r0.x, r0.w, c1
mul_pp r1.xyz, r1.w, r1
mad_pp r0.x, r2.x, r0, c1.y
mad r3.xyz, r2.x, r3, t1
mul r2.xyz, t1, r0.x
mul r3.xyz, r2, r3
mul_pp r0.x, r4, c1.w
mul_pp r1.xyz, r1, c1.z
min_pp r0.xyz, r1, r0.x
mov_pp r0.w, c1.y
mad_pp r0.xyz, r2, r0, r3
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_Color]
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
MUL result.texcoord[1], vertex.color, c[15];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[14];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[16], c[16].zwzw;
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_Color]
Vector 15 [_LightMap_ST]
"vs_2_0
; 18 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mul oT1, v3, c14
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c13
mad oT0.xy, v2, c15, c15.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceLightPos0]
Vector 11 [_Color]
Vector 12 [_LightMap_ST]
"!!ARBvp1.0
# 11 ALU
PARAM c[13] = { program.local[0],
		state.matrix.mvp,
		program.local[5..12] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
MUL result.texcoord[1], vertex.color, c[11];
DP3 result.texcoord[2].z, R0, c[7];
DP3 result.texcoord[2].y, R0, c[6];
DP3 result.texcoord[2].x, R0, c[5];
MOV result.texcoord[3].xyz, c[10];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[12], c[12].zwzw;
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [_WorldSpaceLightPos0]
Vector 10 [_Color]
Vector 11 [_LightMap_ST]
"vs_2_0
; 11 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
mul r0.xyz, v1, c8.w
mul oT1, v3, c10
dp3 oT2.z, r0, c6
dp3 oT2.y, r0, c5
dp3 oT2.x, r0, c4
mov oT3.xyz, c9
mad oT0.xy, v2, c11, c11.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_Color]
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
MUL result.texcoord[1], vertex.color, c[15];
DP4 result.texcoord[4].w, R0, c[12];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[14];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[16], c[16].zwzw;
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_Color]
Vector 15 [_LightMap_ST]
"vs_2_0
; 19 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mul oT1, v3, c14
dp4 oT4.w, r0, c11
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c13
mad oT0.xy, v2, c15, c15.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_Color]
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
MUL result.texcoord[1], vertex.color, c[15];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
ADD result.texcoord[3].xyz, -R0, c[14];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[16], c[16].zwzw;
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_Color]
Vector 15 [_LightMap_ST]
"vs_2_0
; 18 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mul oT1, v3, c14
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
add oT3.xyz, -r0, c13
mad oT0.xy, v2, c15, c15.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Vector 15 [_Color]
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
MUL result.texcoord[1], vertex.color, c[15];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[14];
MAD result.texcoord[0].xy, vertex.texcoord[1], c[16], c[16].zwzw;
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
Bind "color" Color
Bind "normal" Normal
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Vector 14 [_Color]
Vector 15 [_LightMap_ST]
"vs_2_0
; 17 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord1 v2
dcl_color0 v3
mul r1.xyz, v1, c12.w
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mul oT1, v3, c14
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c13
mad oT0.xy, v2, c15, c15.zwzw
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
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 17 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 1, 2 } };
TEMP R0;
TEMP R1;
TEX R1.w, fragment.texcoord[0], texture[0], 2D;
DP3 R0.x, fragment.texcoord[4], fragment.texcoord[4];
ADD R0.y, R1.w, -c[2];
MOV result.color.w, c[2].x;
TEX R0.w, R0.x, texture[1], 2D;
DP3 R0.x, fragment.texcoord[3], fragment.texcoord[3];
RSQ R0.z, R0.x;
MUL R1.xyz, R0.z, fragment.texcoord[3];
POW_SAT R0.x, fragment.texcoord[1].w, c[1].x;
MAD R0.x, R0, R0.y, c[2].y;
MUL R0.xyz, fragment.texcoord[1], R0.x;
DP3 R1.x, fragment.texcoord[2], R1;
MAX R1.x, R1, c[2];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].z;
END
# 17 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 19 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
pow_sat r1.x, t1.w, c1.x
dp3 r0.x, t4, t4
mov r0.xy, r0.x
texld r2, r0, s1
texld r0, t0, s0
add_pp r3.x, r0.w, c2
dp3_pp r0.x, t3, t3
mad_pp r1.x, r1.x, r3, c2.y
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t3
dp3_pp r0.x, t2, r0
mul r1.xyz, t1, r1.x
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c2.z
mul_pp r0.x, r0, r2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.w
mov_pp r0.w, c2.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 12 ALU, 1 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 1, 2 } };
TEMP R0;
TEMP R1;
TEX R0.w, fragment.texcoord[0], texture[0], 2D;
ADD R0.y, R0.w, -c[2];
POW_SAT R0.x, fragment.texcoord[1].w, c[1].x;
MAD R0.x, R0, R0.y, c[2].y;
MOV R1.xyz, fragment.texcoord[3];
MUL R0.xyz, fragment.texcoord[1], R0.x;
DP3 R0.w, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
MAX R0.w, R0, c[2].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].z;
MOV result.color.w, c[2].x;
END
# 12 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
"ps_2_0
; 14 ALU, 1 TEX
dcl_2d s0
def c2, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
texld r0, t0, s0
add_pp r0.x, r0.w, c2
pow_sat r1.x, t1.w, c1.x
mad_pp r1.x, r1.x, r0, c2.y
mov_pp r0.xyz, t3
dp3_pp r0.x, t2, r0
mul r1.xyz, t1, r1.x
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c2.z
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.w
mov_pp r0.w, c2.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 23 ALU, 3 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 1, 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R2.w, fragment.texcoord[0], texture[0], 2D;
DP3 R0.z, fragment.texcoord[4], fragment.texcoord[4];
RCP R0.x, fragment.texcoord[4].w;
MAD R0.xy, fragment.texcoord[4], R0.x, c[2].z;
MOV result.color.w, c[2].x;
TEX R0.w, R0, texture[1], 2D;
TEX R1.w, R0.z, texture[2], 2D;
DP3 R0.z, fragment.texcoord[3], fragment.texcoord[3];
RSQ R1.x, R0.z;
MUL R1.xyz, R1.x, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
SLT R1.y, c[2].x, fragment.texcoord[4].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[2].x;
ADD R0.y, R2.w, -c[2];
POW_SAT R0.x, fragment.texcoord[1].w, c[1].x;
MAD R0.x, R0, R0.y, c[2].y;
MUL R0.xyz, fragment.texcoord[1], R0.x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].w;
END
# 23 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 25 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c2, -1.00000000, 1.00000000, 0.50000000, 0.00000000
def c3, 2.00000000, 0, 0, 0
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
dcl t4
pow_sat r2.x, t1.w, c1.x
dp3 r1.x, t4, t4
mov r1.xy, r1.x
rcp r0.x, t4.w
mad r0.xy, t4, r0.x, c2.z
texld r3, r1, s2
texld r1, t0, s0
texld r0, r0, s1
mov r1.x, r2.x
add_pp r0.x, r1.w, c2
mad_pp r0.x, r1, r0, c2.y
mul r0.xyz, t1, r0.x
mul_pp r2.xyz, r0, c0
dp3_pp r0.x, t3, t3
rsq_pp r1.x, r0.x
cmp r0.x, -t4.z, c2.w, c2.y
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
mul_pp r0.x, r0, r3
max_pp r1.x, r1, c2.w
mul_pp r0.x, r1, r0
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c3.x
mov_pp r0.w, c2
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 19 ALU, 3 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 1, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R2.w, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[4], texture[2], CUBE;
DP3 R0.x, fragment.texcoord[4], fragment.texcoord[4];
DP3 R0.z, fragment.texcoord[3], fragment.texcoord[3];
RSQ R1.x, R0.z;
MUL R1.xyz, R1.x, fragment.texcoord[3];
ADD R0.y, R2.w, -c[2];
DP3 R1.x, fragment.texcoord[2], R1;
MOV result.color.w, c[2].x;
TEX R0.w, R0.x, texture[1], 2D;
MUL R1.y, R0.w, R1.w;
POW_SAT R0.x, fragment.texcoord[1].w, c[1].x;
MAD R0.x, R0, R0.y, c[2].y;
MUL R0.xyz, fragment.texcoord[1], R0.x;
MAX R0.w, R1.x, c[2].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].z;
END
# 19 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 21 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_cube s2
def c2, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
texld r2, t0, s0
dp3 r0.x, t4, t4
mov r1.xy, r0.x
pow_sat r3.x, t1.w, c1.x
mov r2.x, r3.x
texld r0, t4, s2
texld r1, r1, s1
add_pp r0.x, r2.w, c2
mad_pp r0.x, r2, r0, c2.y
mul r2.xyz, t1, r0.x
dp3_pp r0.x, t3, t3
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t3
dp3_pp r0.x, t2, r0
mul r1.x, r1, r0.w
max_pp r0.x, r0, c2.z
mul_pp r2.xyz, r2, c0
mul_pp r0.x, r0, r1
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c2.w
mov_pp r0.w, c2.z
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 14 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 1, 2 } };
TEMP R0;
TEMP R1;
TEX R1.w, fragment.texcoord[0], texture[0], 2D;
TEX R0.w, fragment.texcoord[4], texture[1], 2D;
MOV R1.xyz, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
MAX R1.x, R1, c[2];
ADD R0.y, R1.w, -c[2];
POW_SAT R0.x, fragment.texcoord[1].w, c[1].x;
MAD R0.x, R0, R0.y, c[2].y;
MUL R0.xyz, fragment.texcoord[1], R0.x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].z;
MOV result.color.w, c[2].x;
END
# 14 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 16 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, -1.00000000, 1.00000000, 0.00000000, 2.00000000
dcl t0.xy
dcl t1
dcl t2.xyz
dcl t3.xyz
dcl t4.xy
texld r1, t0, s0
texld r0, t4, s1
pow_sat r2.x, t1.w, c1.x
mov r1.x, r2.x
add_pp r0.x, r1.w, c2
mad_pp r0.x, r1, r0, c2.y
mul r1.xyz, t1, r0.x
mov_pp r0.xyz, t3
dp3_pp r0.x, t2, r0
max_pp r0.x, r0, c2.z
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1, c0
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.w
mov_pp r0.w, c2.z
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
Bind "color" Color
Bind "normal" Normal
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [_Color]
"!!ARBvp1.0
# 9 ALU
PARAM c[11] = { program.local[0],
		state.matrix.mvp,
		program.local[5..10] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
MUL result.texcoord[0], vertex.color, c[10];
DP3 result.texcoord[1].z, R0, c[7];
DP3 result.texcoord[1].y, R0, c[6];
DP3 result.texcoord[1].x, R0, c[5];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 9 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [_Color]
"vs_2_0
; 9 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r0.xyz, v1, c8.w
mul oT0, v2, c9
dp3 oT1.z, r0, c6
dp3 oT1.y, r0, c5
dp3 oT1.x, r0, c4
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
PARAM c[2] = { program.local[0],
		{ 0, 0.5 } };
MAD result.color.xyz, fragment.texcoord[1], c[1].y, c[1].y;
MOV result.color.w, c[1].x;
END
# 2 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 3 ALU
dcl_2d s0
def c0, 0.50000000, 0.00000000, 0, 0
dcl t1.xyz
mad_pp r0.xyz, t1, c0.x, c0.x
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
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 5 [_ProjectionParams]
Vector 6 [_Color]
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
ADD result.texcoord[2].xy, R1, R1.z;
MOV result.position, R0;
MUL result.texcoord[1], vertex.color, c[6];
MOV result.texcoord[2].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[7], c[7].zwzw;
END
# 11 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ProjectionParams]
Vector 5 [_ScreenParams]
Vector 6 [_Color]
Vector 7 [_LightMap_ST]
"vs_2_0
; 11 ALU
def c8, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord1 v1
dcl_color0 v2
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c8.x
mul r1.y, r1, c4.x
mad oT2.xy, r1.z, c5.zwzw, r1
mov oPos, r0
mul oT1, v2, c6
mov oT2.zw, r0
mad oT0.xy, v1, c7, c7.zwzw
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [_Color]
Vector 11 [unity_LightmapST]
Vector 12 [unity_LightmapFade]
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
ADD result.texcoord[2].xy, R1, R1.z;
MUL result.texcoord[1], vertex.color, c[10];
MOV result.texcoord[2].zw, R0;
MAD result.texcoord[0].xy, vertex.texcoord[1], c[13], c[13].zwzw;
MAD result.texcoord[3].xy, vertex.texcoord[1], c[11], c[11].zwzw;
MAD result.texcoord[3].z, -R0.x, c[12], c[12].w;
END
# 14 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_modelview0]
Matrix 4 [glstate_matrix_mvp]
Vector 8 [_ProjectionParams]
Vector 9 [_ScreenParams]
Vector 10 [_Color]
Vector 11 [unity_LightmapST]
Vector 12 [unity_LightmapFade]
Vector 13 [_LightMap_ST]
"vs_2_0
; 14 ALU
def c14, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord1 v1
dcl_color0 v2
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mul r1.xyz, r0.xyww, c14.x
mul r1.y, r1, c8.x
mov oPos, r0
dp4 r0.x, v0, c2
mad oT2.xy, r1.z, c9.zwzw, r1
mul oT1, v2, c10
mov oT2.zw, r0
mad oT0.xy, v1, c13, c13.zwzw
mad oT3.xy, v1, c11, c11.zwzw
mad oT3.z, -r0.x, c12, c12.w
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Float 0 [_Falloff]
Vector 1 [unity_Ambient]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightBuffer] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 1 } };
TEMP R0;
TEMP R1;
TEMP R2;
TXP R0.xyz, fragment.texcoord[2], texture[1], 2D;
TEX R1, fragment.texcoord[0], texture[0], 2D;
ADD R1.xyz, -fragment.texcoord[1], R1;
POW_SAT R0.w, fragment.texcoord[1].w, c[0].x;
ADD R1.w, R1, -c[2].x;
MAD R2.xyz, R0.w, R1, fragment.texcoord[1];
MAD R1.w, R0, R1, c[2].x;
MUL R1.xyz, fragment.texcoord[1], R1.w;
MUL R2.xyz, R1, R2;
LG2 R0.x, R0.x;
LG2 R0.z, R0.z;
LG2 R0.y, R0.y;
ADD R0.xyz, -R0, c[1];
MAD result.color.xyz, R1, R0, R2;
MOV result.color.w, c[2].x;
END
# 15 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Float 0 [_Falloff]
Vector 1 [unity_Ambient]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightBuffer] 2D
"ps_2_0
; 16 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, -1.00000000, 1.00000000, 0, 0
dcl t0.xy
dcl t1
dcl t2
texldp r2, t2, s1
texld r3, t0, s0
pow_sat r0.x, t1.w, c0.x
add_pp r1.x, r3.w, c2
mad_pp r1.x, r0.x, r1, c2.y
add r3.xyz, -t1, r3
mul r1.xyz, t1, r1.x
mad r0.xyz, r0.x, r3, t1
mul r0.xyz, r1, r0
log_pp r2.x, r2.x
log_pp r2.z, r2.z
log_pp r2.y, r2.y
add_pp r2.xyz, -r2, c1
mov_pp r0.w, c2.y
mad_pp r0.xyz, r1, r2, r0
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 23 ALU, 4 TEX
PARAM c[2] = { program.local[0],
		{ 1, 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1, fragment.texcoord[3], texture[3], 2D;
TEX R2, fragment.texcoord[3], texture[2], 2D;
TXP R3.xyz, fragment.texcoord[2], texture[1], 2D;
MUL R1.xyz, R1.w, R1;
ADD R0.xyz, -fragment.texcoord[1], R0;
POW_SAT R1.w, fragment.texcoord[1].w, c[0].x;
ADD R0.w, R0, -c[1].x;
MAD R0.w, R1, R0, c[1].x;
MAD R4.xyz, R1.w, R0, fragment.texcoord[1];
MUL R0.xyz, fragment.texcoord[1], R0.w;
MUL R1.xyz, R1, c[1].y;
MUL R2.xyz, R2.w, R2;
MAD R2.xyz, R2, c[1].y, -R1;
MOV_SAT R0.w, fragment.texcoord[3].z;
MAD R1.xyz, R0.w, R2, R1;
MUL R4.xyz, R0, R4;
LG2 R2.x, R3.x;
LG2 R2.y, R3.y;
LG2 R2.z, R3.z;
ADD R1.xyz, -R2, R1;
MAD result.color.xyz, R0, R1, R4;
MOV result.color.w, c[1].x;
END
# 23 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Float 0 [_Falloff]
SetTexture 0 [_LightMap] 2D
SetTexture 1 [_LightBuffer] 2D
SetTexture 2 [unity_Lightmap] 2D
SetTexture 3 [unity_LightmapInd] 2D
"ps_2_0
; 23 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c1, -1.00000000, 1.00000000, 8.00000000, 0
dcl t0.xy
dcl t1
dcl t2
dcl t3.xyz
texld r2, t0, s0
texldp r3, t2, s1
texld r0, t3, s3
texld r1, t3, s2
mul_pp r0.xyz, r0.w, r0
mul_pp r4.xyz, r0, c1.z
mul_pp r0.xyz, r1.w, r1
mad_pp r1.xyz, r0, c1.z, -r4
mov_sat r0.x, t3.z
mad_pp r0.xyz, r0.x, r1, r4
log_pp r1.x, r3.x
add r4.xyz, -t1, r2
log_pp r1.y, r3.y
log_pp r1.z, r3.z
add_pp r1.xyz, -r1, r0
pow_sat r0.x, t1.w, c0.x
mov r2.x, r0.x
add_pp r3.x, r2.w, c1
mad_pp r0.x, r2, r3, c1.y
mul r0.xyz, t1, r0.x
mad r2.xyz, r2.x, r4, t1
mul r2.xyz, r0, r2
mov_pp r0.w, c1.y
mad_pp r0.xyz, r0, r1, r2
mov_pp oC0, r0
"
}
}
 }
}
Fallback "VertexLit"
}