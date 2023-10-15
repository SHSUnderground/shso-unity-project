//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Marvel/Characters/SharedFX/WipeFade" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Specular ("Specular", Float) = 4
 _SpecularColor ("Specular Color", Color) = (1,1,1,1)
 _Transition ("Transition", Range(0,1)) = 0
 _BaseY ("Y Fade Start", Float) = 0
 _Height ("Y Fade Height", Float) = 1
}
SubShader { 
 Tags { "QUEUE"="Transparent" "RenderType"="Opaque" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Opaque" }
  ColorMask 0
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
"!!ARBvp1.0
# 4 ALU
PARAM c[5] = { program.local[0],
		state.matrix.mvp };
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 4 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 4 ALU
dcl_position0 v0
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
# 1 ALU, 0 TEX
PARAM c[1] = { { 0 } };
MOV result.color, c[0].x;
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 2 ALU
def c0, 0.00000000, 0, 0, 0
mov_pp r0, c0.x
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Opaque" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [unity_SHAr]
Vector 11 [unity_SHAg]
Vector 12 [unity_SHAb]
Vector 13 [unity_SHBr]
Vector 14 [unity_SHBg]
Vector 15 [unity_SHBb]
Vector 16 [unity_SHC]
Float 17 [_BaseY]
Float 18 [_Height]
Vector 19 [_MainTex_ST]
"!!ARBvp1.0
# 30 ALU
PARAM c[20] = { { 1 },
		state.matrix.mvp,
		program.local[5..19] };
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
MAD R0.y, R0.x, R0.x, -R0;
MOV result.texcoord[2].x, R0;
DP4 R3.z, R1, c[15];
DP4 R3.y, R1, c[14];
DP4 R3.x, R1, c[13];
MUL R1.xyz, R0.y, c[16];
ADD R2.xyz, R2, R3;
RCP R0.x, c[18].x;
ADD R0.y, vertex.position.z, -c[17].x;
ADD result.texcoord[3].xyz, R2, R1;
MOV result.texcoord[2].z, R2.w;
MOV result.texcoord[2].y, R3.w;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[19], c[19].zwzw;
MUL result.texcoord[1].x, R0.y, R0;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 30 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
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
Float 16 [_BaseY]
Float 17 [_Height]
Vector 18 [_MainTex_ST]
"vs_2_0
; 30 ALU
def c19, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c8.w
dp3 r3.w, r1, c5
dp3 r2.w, r1, c6
dp3 r0.x, r1, c4
mov r0.y, r3.w
mov r0.z, r2.w
mul r1, r0.xyzz, r0.yzzx
mov r0.w, c19.x
dp4 r2.z, r0, c11
dp4 r2.y, r0, c10
dp4 r2.x, r0, c9
mul r0.y, r3.w, r3.w
mad r0.y, r0.x, r0.x, -r0
mov oT2.x, r0
dp4 r3.z, r1, c14
dp4 r3.y, r1, c13
dp4 r3.x, r1, c12
mul r1.xyz, r0.y, c15
add r2.xyz, r2, r3
rcp r0.x, c17.x
add r0.y, v0.z, -c16.x
add oT3.xyz, r2, r1
mov oT2.z, r2.w
mov oT2.y, r3.w
mad oT0.xy, v2, c18, c18.zwzw
mul oT1.x, r0.y, r0
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Float 9 [_BaseY]
Float 10 [_Height]
Vector 11 [unity_LightmapST]
Vector 12 [_MainTex_ST]
"!!ARBvp1.0
# 9 ALU
PARAM c[13] = { program.local[0],
		state.matrix.mvp,
		program.local[5..12] };
TEMP R0;
RCP R0.x, c[10].x;
ADD R0.y, vertex.position.z, -c[9].x;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[12], c[12].zwzw;
MUL result.texcoord[1].x, R0.y, R0;
MAD result.texcoord[3].xy, vertex.texcoord[1], c[11], c[11].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 9 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Float 8 [_BaseY]
Float 9 [_Height]
Vector 10 [unity_LightmapST]
Vector 11 [_MainTex_ST]
"vs_2_0
; 9 ALU
dcl_position0 v0
dcl_texcoord0 v2
dcl_texcoord1 v3
rcp r0.x, c9.x
add r0.y, v0.z, -c8.x
mad oT0.xy, v2, c11, c11.zwzw
mul oT1.x, r0.y, r0
mad oT3.xy, v3, c10, c10.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
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
Float 25 [_BaseY]
Float 26 [_Height]
Vector 27 [_MainTex_ST]
"!!ARBvp1.0
# 60 ALU
PARAM c[28] = { { 1, 0 },
		state.matrix.mvp,
		program.local[5..27] };
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
RCP R0.y, c[26].x;
ADD R0.x, vertex.position.z, -c[25];
MOV result.texcoord[2].z, R3.x;
MOV result.texcoord[2].y, R3.w;
MOV result.texcoord[2].x, R4;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[27], c[27].zwzw;
MUL result.texcoord[1].x, R0, R0.y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 60 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
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
Float 24 [_BaseY]
Float 25 [_Height]
Vector 26 [_MainTex_ST]
"vs_2_0
; 60 ALU
def c27, 1.00000000, 0.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
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
mov r4.w, c27.x
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
add r1, r2, c27.x
dp4 r2.z, r4, c19
dp4 r2.y, r4, c18
dp4 r2.x, r4, c17
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c27.y
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
rcp r0.y, c25.x
add r0.x, v0.z, -c24
mov oT2.z, r3.x
mov oT2.y, r3.w
mov oT2.x, r4
mad oT0.xy, v2, c26, c26.zwzw
mul oT1.x, r0, r0.y
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_Transition]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 1 TEX
PARAM c[5] = { program.local[0..3],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
MUL R0.xyz, R0, c[2];
MUL R1.xyz, R0, fragment.texcoord[3];
DP3 R0.w, fragment.texcoord[2], c[0];
MUL R0.xyz, R0, c[1];
MAX R0.w, R0, c[4].x;
MUL R0.xyz, R0.w, R0;
MAD result.color.xyz, R0, c[4].y, R1;
SLT result.color.w, c[3].x, fragment.texcoord[1].x;
END
# 9 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
Float 3 [_Transition]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 10 ALU, 1 TEX
dcl_2d s0
def c4, 0.00000000, 1.00000000, 2.00000000, 0
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
texld r0, t0, s0
mul r1.xyz, r0, c2
mul_pp r2.xyz, r1, c1
dp3_pp r0.x, t2, c0
max_pp r0.x, r0, c4
mul_pp r2.xyz, r0.x, r2
mul_pp r1.xyz, r1, t3
add r0.x, -t1, c3
mad_pp r1.xyz, r2, c4.z, r1
cmp r1.w, r0.x, c4.x, c4.y
mov_pp oC0, r1
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 7 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 8 } };
TEMP R0;
TEMP R1;
TEX R1.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R0, fragment.texcoord[3], texture[1], 2D;
MUL R1.xyz, R1, c[0];
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, R1;
MUL result.color.xyz, R0, c[2].x;
SLT result.color.w, c[1].x, fragment.texcoord[1].x;
END
# 7 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "LIGHTMAP_ON" }
Vector 0 [_Color]
Float 1 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 7 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, 0.00000000, 1.00000000, 8.00000000, 0
dcl t0.xy
dcl t1.x
dcl t3.xy
texld r1, t0, s0
texld r0, t3, s1
mul r1.xyz, r1, c0
mul_pp r0.xyz, r0.w, r0
mul_pp r0.xyz, r0, r1
add r1.x, -t1, c1
mul_pp r0.xyz, r0, c2.z
cmp r0.w, r1.x, c2.x, c2.y
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Float 15 [_BaseY]
Float 16 [_Height]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 20 ALU
PARAM c[18] = { program.local[0],
		state.matrix.mvp,
		program.local[5..17] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.z, vertex.position, c[7];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
ADD result.texcoord[3].xyz, -R0, c[14];
RCP R0.y, c[16].x;
ADD R0.x, vertex.position.z, -c[15];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
MUL result.texcoord[1].x, R0, R0.y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 20 instructions, 2 R-regs
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
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Float 14 [_BaseY]
Float 15 [_Height]
Vector 16 [_MainTex_ST]
"vs_2_0
; 20 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c12.w
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.z, v0, c6
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
add oT3.xyz, -r0, c13
rcp r0.y, c15.x
add r0.x, v0.z, -c14
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT0.xy, v2, c16, c16.zwzw
mul oT1.x, r0, r0.y
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
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceLightPos0]
Float 11 [_BaseY]
Float 12 [_Height]
Vector 13 [_MainTex_ST]
"!!ARBvp1.0
# 13 ALU
PARAM c[14] = { program.local[0],
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
DP3 result.texcoord[2].z, R0, c[7];
DP3 result.texcoord[2].y, R0, c[6];
DP3 result.texcoord[2].x, R0, c[5];
RCP R0.x, c[12].x;
ADD R0.y, vertex.position.z, -c[11].x;
MOV result.texcoord[3].xyz, c[10];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[13], c[13].zwzw;
MUL result.texcoord[1].x, R0.y, R0;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 13 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [_WorldSpaceLightPos0]
Float 10 [_BaseY]
Float 11 [_Height]
Vector 12 [_MainTex_ST]
"vs_2_0
; 13 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r0.xyz, v1, c8.w
dp3 oT2.z, r0, c6
dp3 oT2.y, r0, c5
dp3 oT2.x, r0, c4
rcp r0.x, c11.x
add r0.y, v0.z, -c10.x
mov oT3.xyz, c9
mad oT0.xy, v2, c12, c12.zwzw
mul oT1.x, r0.y, r0
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
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Float 15 [_BaseY]
Float 16 [_Height]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 21 ALU
PARAM c[18] = { program.local[0],
		state.matrix.mvp,
		program.local[5..17] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.z, vertex.position, c[7];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].w, R0, c[12];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
ADD result.texcoord[3].xyz, -R0, c[14];
RCP R0.y, c[16].x;
ADD R0.x, vertex.position.z, -c[15];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
MUL result.texcoord[1].x, R0, R0.y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 21 instructions, 2 R-regs
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
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Float 14 [_BaseY]
Float 15 [_Height]
Vector 16 [_MainTex_ST]
"vs_2_0
; 21 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c12.w
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.z, v0, c6
dp4 r0.w, v0, c7
dp4 oT4.w, r0, c11
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
add oT3.xyz, -r0, c13
rcp r0.y, c15.x
add r0.x, v0.z, -c14
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT0.xy, v2, c16, c16.zwzw
mul oT1.x, r0, r0.y
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
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Float 15 [_BaseY]
Float 16 [_Height]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 20 ALU
PARAM c[18] = { program.local[0],
		state.matrix.mvp,
		program.local[5..17] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.z, vertex.position, c[7];
DP4 R0.w, vertex.position, c[8];
DP4 result.texcoord[4].z, R0, c[11];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
ADD result.texcoord[3].xyz, -R0, c[14];
RCP R0.y, c[16].x;
ADD R0.x, vertex.position.z, -c[15];
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
MUL result.texcoord[1].x, R0, R0.y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 20 instructions, 2 R-regs
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
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Float 14 [_BaseY]
Float 15 [_Height]
Vector 16 [_MainTex_ST]
"vs_2_0
; 20 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c12.w
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.z, v0, c6
dp4 r0.w, v0, c7
dp4 oT4.z, r0, c10
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
add oT3.xyz, -r0, c13
rcp r0.y, c15.x
add r0.x, v0.z, -c14
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mad oT0.xy, v2, c16, c16.zwzw
mul oT1.x, r0, r0.y
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
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
Float 15 [_BaseY]
Float 16 [_Height]
Vector 17 [_MainTex_ST]
"!!ARBvp1.0
# 19 ALU
PARAM c[18] = { program.local[0],
		state.matrix.mvp,
		program.local[5..17] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 result.texcoord[4].y, R0, c[10];
DP4 result.texcoord[4].x, R0, c[9];
RCP R0.x, c[16].x;
ADD R0.y, vertex.position.z, -c[15].x;
DP3 result.texcoord[2].z, R1, c[7];
DP3 result.texcoord[2].y, R1, c[6];
DP3 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[3].xyz, c[14];
MAD result.texcoord[0].xy, vertex.texcoord[0], c[17], c[17].zwzw;
MUL result.texcoord[1].x, R0.y, R0;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 19 instructions, 2 R-regs
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
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
Float 14 [_BaseY]
Float 15 [_Height]
Vector 16 [_MainTex_ST]
"vs_2_0
; 19 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
mul r1.xyz, v1, c12.w
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 oT4.y, r0, c9
dp4 oT4.x, r0, c8
rcp r0.x, c15.x
add r0.y, v0.z, -c14.x
dp3 oT2.z, r1, c6
dp3 oT2.y, r1, c5
dp3 oT2.x, r1, c4
mov oT3.xyz, c13
mad oT0.xy, v2, c16, c16.zwzw
mul oT1.x, r0.y, r0
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
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 14 ALU, 2 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.x, fragment.texcoord[3], fragment.texcoord[3];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[3];
MUL R0.xyz, R0, c[1];
DP3 R1.x, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
MAX R1.x, R1, c[3];
SLT result.color.w, c[2].x, fragment.texcoord[1].x;
TEX R0.w, R0.w, texture[1], 2D;
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[3].y;
END
# 14 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 15 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 2.00000000, 0
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
texld r1, t0, s0
dp3 r0.x, t4, t4
mov r0.xy, r0.x
mul r1.xyz, r1, c1
mul_pp r1.xyz, r1, c0
texld r2, r0, s1
dp3_pp r0.x, t3, t3
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t3
dp3_pp r0.x, t2, r0
max_pp r0.x, r0, c3
mul_pp r0.x, r0, r2
mul_pp r1.xyz, r0.x, r1
add r0.x, -t1, c2
mul_pp r1.xyz, r1, c3.z
cmp r1.w, r0.x, c3.x, c3.y
mov_pp oC0, r1
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 1 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
MOV R1.xyz, fragment.texcoord[3];
MUL R0.xyz, R0, c[1];
DP3 R0.w, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
MAX R0.w, R0, c[3].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[3].y;
SLT result.color.w, c[2].x, fragment.texcoord[1].x;
END
# 9 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 10 ALU, 1 TEX
dcl_2d s0
def c3, 0.00000000, 1.00000000, 2.00000000, 0
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
texld r1, t0, s0
mov_pp r0.xyz, t3
mul r1.xyz, r1, c1
dp3_pp r0.x, t2, r0
max_pp r0.x, r0, c3
mul_pp r1.xyz, r1, c0
mul_pp r1.xyz, r0.x, r1
add r0.x, -t1, c2
mul_pp r1.xyz, r1, c3.z
cmp r1.w, r0.x, c3.x, c3.y
mov_pp oC0, r1
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 20 ALU, 3 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 0.5, 2 } };
TEMP R0;
TEMP R1;
RCP R0.x, fragment.texcoord[4].w;
MAD R1.xy, fragment.texcoord[4], R0.x, c[3].y;
DP3 R1.z, fragment.texcoord[4], fragment.texcoord[4];
SLT result.color.w, c[2].x, fragment.texcoord[1].x;
TEX R0.w, R1, texture[1], 2D;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, R1.z, texture[2], 2D;
DP3 R1.x, fragment.texcoord[3], fragment.texcoord[3];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[3];
DP3 R1.x, fragment.texcoord[2], R1;
SLT R1.y, c[3].x, fragment.texcoord[4].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MUL R0.xyz, R0, c[1];
MAX R0.w, R1.x, c[3].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[3].z;
END
# 20 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
SetTexture 2 [_LightTextureB0] 2D
"ps_2_0
; 20 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c3, 0.00000000, 1.00000000, 0.50000000, 2.00000000
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
dcl t4
texld r2, t0, s0
dp3 r0.x, t4, t4
rcp r1.x, t4.w
mov r0.xy, r0.x
mad r1.xy, t4, r1.x, c3.z
mul r2.xyz, r2, c1
mul_pp r2.xyz, r2, c0
texld r1, r1, s1
texld r0, r0, s2
cmp r1.x, -t4.z, c3, c3.y
mul_pp r3.x, r1, r1.w
dp3_pp r1.x, t3, t3
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t3
dp3_pp r1.x, t2, r1
max_pp r1.x, r1, c3
mul_pp r0.x, r3, r0
mul_pp r0.x, r1, r0
mul_pp r1.xyz, r0.x, r2
add r0.x, -t1, c2
mul_pp r1.xyz, r1, c3.w
cmp r1.w, r0.x, c3.x, c3.y
mov_pp oC0, r1
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 16 ALU, 3 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[4], texture[2], CUBE;
DP3 R0.w, fragment.texcoord[4], fragment.texcoord[4];
DP3 R1.x, fragment.texcoord[3], fragment.texcoord[3];
RSQ R1.x, R1.x;
MUL R1.xyz, R1.x, fragment.texcoord[3];
MUL R0.xyz, R0, c[1];
DP3 R1.x, fragment.texcoord[2], R1;
MUL R0.xyz, R0, c[0];
SLT result.color.w, c[2].x, fragment.texcoord[1].x;
TEX R0.w, R0.w, texture[1], 2D;
MUL R1.y, R0.w, R1.w;
MAX R0.w, R1.x, c[3].x;
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[3].y;
END
# 16 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTextureB0] 2D
SetTexture 2 [_LightTexture0] CUBE
"ps_2_0
; 16 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_cube s2
def c3, 0.00000000, 1.00000000, 2.00000000, 0
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
dcl t4.xyz
texld r2, t4, s2
texld r1, t0, s0
dp3 r0.x, t4, t4
mov r0.xy, r0.x
dp3_pp r2.x, t3, t3
rsq_pp r2.x, r2.x
mul_pp r2.xyz, r2.x, t3
mul r1.xyz, r1, c1
dp3_pp r2.x, t2, r2
mul_pp r1.xyz, r1, c0
max_pp r2.x, r2, c3
texld r0, r0, s1
mul r0.x, r0, r2.w
mul_pp r0.x, r2, r0
mul_pp r0.xyz, r0.x, r1
add r1.x, -t1, c2
mul_pp r0.xyz, r0, c3.z
cmp r0.w, r1.x, c3.x, c3.y
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 11 ALU, 2 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
TEX R0.w, fragment.texcoord[4], texture[1], 2D;
MOV R1.xyz, fragment.texcoord[3];
MUL R0.xyz, R0, c[1];
DP3 R1.x, fragment.texcoord[2], R1;
MAX R1.x, R1, c[3];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[3].y;
SLT result.color.w, c[2].x, fragment.texcoord[1].x;
END
# 11 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
Float 2 [_Transition]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_LightTexture0] 2D
"ps_2_0
; 11 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c3, 0.00000000, 1.00000000, 2.00000000, 0
dcl t0.xy
dcl t1.x
dcl t2.xyz
dcl t3.xyz
dcl t4.xy
texld r1, t0, s0
texld r0, t4, s1
mov_pp r0.xyz, t3
dp3_pp r0.x, t2, r0
mul r1.xyz, r1, c1
max_pp r0.x, r0, c3
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1, c0
mul_pp r1.xyz, r0.x, r1
add r0.x, -t1, c2
mul_pp r1.xyz, r1, c3.z
cmp r1.w, r0.x, c3.x, c3.y
mov_pp oC0, r1
"
}
}
 }
 Pass {
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha One
  AlphaTest Greater 0
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 23 ALU
PARAM c[20] = { { 1 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
TEMP R2;
MOV R0.xyz, c[18];
MOV R0.w, c[0].x;
DP4 R2.z, R0, c[11];
DP4 R2.x, R0, c[9];
DP4 R2.y, R0, c[10];
MOV R1, c[19];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[2].xyz, R0, c[17].w, -vertex.position;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MAD result.texcoord[0].xyz, R2, c[17].w, -vertex.position;
DP4 result.texcoord[3].z, R0, c[15];
DP4 result.texcoord[3].y, R0, c[14];
DP4 result.texcoord[3].x, R0, c[13];
MOV result.texcoord[1].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 23 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
"vs_2_0
; 25 ALU
def c19, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r0.w, c19.x
mov r0.xyz, c17
dp4 r2.z, r0, c10
dp4 r2.x, r0, c8
dp4 r2.y, r0, c9
mov r0, c9
mad oT0.xyz, r2, c16.w, -v0
dp4 r2.y, c18, r0
mov r1, c10
dp4 r2.z, c18, r1
mov r1, c8
dp4 r2.x, c18, r1
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mad oT2.xyz, r2, c16.w, -v0
dp4 oT3.z, r0, c14
dp4 oT3.y, r0, c13
dp4 oT3.x, r0, c12
mov oT1.xyz, v1
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
Matrix 5 [_World2Object]
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceCameraPos]
Vector 11 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 15 ALU
PARAM c[12] = { { 1 },
		state.matrix.mvp,
		program.local[5..11] };
TEMP R0;
TEMP R1;
TEMP R2;
MOV R1, c[11];
MOV R0.w, c[0].x;
MOV R0.xyz, c[10];
DP4 R2.z, R0, c[7];
DP4 R2.x, R0, c[5];
DP4 R2.y, R0, c[6];
MAD result.texcoord[0].xyz, R2, c[9].w, -vertex.position;
DP4 result.texcoord[2].z, R1, c[7];
DP4 result.texcoord[2].y, R1, c[6];
DP4 result.texcoord[2].x, R1, c[5];
MOV result.texcoord[1].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 15 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_World2Object]
Vector 8 [unity_Scale]
Vector 9 [_WorldSpaceCameraPos]
Vector 10 [_WorldSpaceLightPos0]
"vs_2_0
; 17 ALU
def c11, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1, c6
dp4 oT2.z, c10, r1
mov r1, c4
mov r0.w, c11.x
mov r0.xyz, c9
dp4 r2.z, r0, c6
dp4 r2.x, r0, c4
dp4 r2.y, r0, c5
mov r0, c5
mad oT0.xyz, r2, c8.w, -v0
dp4 oT2.y, c10, r0
dp4 oT2.x, c10, r1
mov oT1.xyz, v1
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
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 24 ALU
PARAM c[20] = { { 1 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
TEMP R2;
MOV R0.xyz, c[18];
MOV R0.w, c[0].x;
DP4 R2.z, R0, c[11];
DP4 R2.x, R0, c[9];
DP4 R2.y, R0, c[10];
MOV R1, c[19];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[2].xyz, R0, c[17].w, -vertex.position;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MAD result.texcoord[0].xyz, R2, c[17].w, -vertex.position;
DP4 result.texcoord[3].w, R0, c[16];
DP4 result.texcoord[3].z, R0, c[15];
DP4 result.texcoord[3].y, R0, c[14];
DP4 result.texcoord[3].x, R0, c[13];
MOV result.texcoord[1].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 24 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
"vs_2_0
; 26 ALU
def c19, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r0.w, c19.x
mov r0.xyz, c17
dp4 r2.z, r0, c10
dp4 r2.x, r0, c8
dp4 r2.y, r0, c9
mov r0, c9
mad oT0.xyz, r2, c16.w, -v0
dp4 r2.y, c18, r0
mov r1, c10
dp4 r2.z, c18, r1
mov r1, c8
dp4 r2.x, c18, r1
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mad oT2.xyz, r2, c16.w, -v0
dp4 oT3.w, r0, c15
dp4 oT3.z, r0, c14
dp4 oT3.y, r0, c13
dp4 oT3.x, r0, c12
mov oT1.xyz, v1
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
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 23 ALU
PARAM c[20] = { { 1 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
TEMP R2;
MOV R0.xyz, c[18];
MOV R0.w, c[0].x;
DP4 R2.z, R0, c[11];
DP4 R2.x, R0, c[9];
DP4 R2.y, R0, c[10];
MOV R1, c[19];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, R1, c[11];
DP4 R0.x, R1, c[9];
DP4 R0.y, R1, c[10];
MAD result.texcoord[2].xyz, R0, c[17].w, -vertex.position;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MAD result.texcoord[0].xyz, R2, c[17].w, -vertex.position;
DP4 result.texcoord[3].z, R0, c[15];
DP4 result.texcoord[3].y, R0, c[14];
DP4 result.texcoord[3].x, R0, c[13];
MOV result.texcoord[1].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 23 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
"vs_2_0
; 25 ALU
def c19, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r0.w, c19.x
mov r0.xyz, c17
dp4 r2.z, r0, c10
dp4 r2.x, r0, c8
dp4 r2.y, r0, c9
mov r0, c9
mad oT0.xyz, r2, c16.w, -v0
dp4 r2.y, c18, r0
mov r1, c10
dp4 r2.z, c18, r1
mov r1, c8
dp4 r2.x, c18, r1
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mad oT2.xyz, r2, c16.w, -v0
dp4 oT3.z, r0, c14
dp4 oT3.y, r0, c13
dp4 oT3.x, r0, c12
mov oT1.xyz, v1
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
Matrix 5 [_Object2World]
Matrix 9 [_World2Object]
Matrix 13 [_LightMatrix0]
Vector 17 [unity_Scale]
Vector 18 [_WorldSpaceCameraPos]
Vector 19 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 21 ALU
PARAM c[20] = { { 1 },
		state.matrix.mvp,
		program.local[5..19] };
TEMP R0;
TEMP R1;
TEMP R2;
MOV R1, c[19];
MOV R0.xyz, c[18];
MOV R0.w, c[0].x;
DP4 R2.z, R0, c[11];
DP4 R2.x, R0, c[9];
DP4 R2.y, R0, c[10];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MAD result.texcoord[0].xyz, R2, c[17].w, -vertex.position;
DP4 result.texcoord[2].z, R1, c[11];
DP4 result.texcoord[2].y, R1, c[10];
DP4 result.texcoord[2].x, R1, c[9];
DP4 result.texcoord[3].y, R0, c[14];
DP4 result.texcoord[3].x, R0, c[13];
MOV result.texcoord[1].xyz, vertex.normal;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 21 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_World2Object]
Matrix 12 [_LightMatrix0]
Vector 16 [unity_Scale]
Vector 17 [_WorldSpaceCameraPos]
Vector 18 [_WorldSpaceLightPos0]
"vs_2_0
; 23 ALU
def c19, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
mov r1, c10
dp4 oT2.z, c18, r1
mov r1, c8
mov r0.w, c19.x
mov r0.xyz, c17
dp4 r2.z, r0, c10
dp4 r2.x, r0, c8
dp4 r2.y, r0, c9
mov r0, c9
dp4 oT2.y, c18, r0
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mad oT0.xyz, r2, c16.w, -v0
dp4 oT2.x, c18, r1
dp4 oT3.y, r0, c13
dp4 oT3.x, r0, c12
mov oT1.xyz, v1
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
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
"!!ARBfp1.0
# 31 ALU, 1 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 0, 2, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
DP3 R0.x, fragment.texcoord[3], fragment.texcoord[3];
TEX R0.w, R0.x, texture[0], 2D;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.y, R0.x;
MUL R1.xyz, R0.y, fragment.texcoord[2];
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MAD R2.xyz, R0.x, fragment.texcoord[0], R1;
DP3 R0.z, R2, R2;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R0.y, R0.y;
MUL R1.xyz, R0.y, fragment.texcoord[1];
RSQ R1.w, R0.z;
MUL R0.xyz, R0.x, fragment.texcoord[0];
DP3_SAT R2.w, R0, R1;
MUL R0.xyz, R1.w, R2;
DP3 R0.y, R1, R0;
DP3 R0.x, R1, fragment.texcoord[2];
MAX R0.x, R0, c[4].y;
MUL R0.w, R0.x, R0;
MUL R1.xyz, R0.w, c[0];
POW R0.y, R0.y, c[1].x;
MUL R0.xyz, R0.y, c[2];
MUL R0.xyz, R0, c[4].w;
ADD R2.x, -R2.w, c[4];
MUL R1.xyz, R1, c[4].z;
MOV R1.w, c[4].x;
MOV R0.w, c[4].x;
MUL R1, R1, R0;
SLT R0.x, c[3], R2;
MAD result.color, R0.x, -R1, R1;
END
# 31 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
"ps_2_0
; 36 ALU, 1 TEX
dcl_2d s0
def c4, -1.00000000, 0.00000000, 1.00000000, 2.00000000
def c5, 0.50000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dp3 r0.x, t3, t3
mov r0.xy, r0.x
mov_pp r1.w, c4.z
texld r2, r0, s0
dp3 r0.x, t2, t2
rsq r1.x, r0.x
dp3 r0.x, t0, t0
mul r1.xyz, r1.x, t2
rsq r0.x, r0.x
mad r4.xyz, r0.x, t0, r1
dp3 r1.x, r4, r4
rsq r3.x, r1.x
mul r3.xyz, r3.x, r4
dp3 r1.x, t1, t1
rsq r1.x, r1.x
mul r1.xyz, r1.x, t1
mul r4.xyz, r0.x, t0
dp3 r0.x, r1, r3
dp3_sat r3.x, r4, r1
pow r4.x, r0.x, c1.x
dp3_pp r1.x, r1, t2
add r0.x, r3, c3
add r0.x, r0, c4
mov r3.x, r4.x
max_pp r1.x, r1, c4.y
mul_pp r1.x, r1, r2
mul_pp r2.xyz, r1.x, c0
mul r3.xyz, r3.x, c2
mul r1.xyz, r3, c5.x
mul_pp r2.xyz, r2, c4.w
mov_pp r2.w, c4.z
mul_pp r1, r2, r1
cmp r0.x, r0, c4.y, c4.z
mad_pp r0, r0.x, -r1, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
"!!ARBfp1.0
# 28 ALU, 0 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 0, 2, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.y, R0.x;
MUL R1.xyz, R0.y, fragment.texcoord[2];
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MAD R2.xyz, R0.x, fragment.texcoord[0], R1;
DP3 R0.z, R2, R2;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R0.y, R0.y;
MUL R1.xyz, R0.y, fragment.texcoord[1];
RSQ R0.w, R0.z;
MUL R0.xyz, R0.x, fragment.texcoord[0];
DP3_SAT R1.w, R0, R1;
MUL R0.xyz, R0.w, R2;
DP3 R0.y, R1, R0;
DP3 R0.x, R1, fragment.texcoord[2];
MAX R0.w, R0.x, c[4].y;
MUL R1.xyz, R0.w, c[0];
ADD R2.x, -R1.w, c[4];
POW R0.y, R0.y, c[1].x;
MUL R0.xyz, R0.y, c[2];
MUL R0.xyz, R0, c[4].w;
MUL R1.xyz, R1, c[4].z;
MOV R1.w, c[4].x;
MOV R0.w, c[4].x;
MUL R1, R1, R0;
SLT R0.x, c[3], R2;
MAD result.color, R0.x, -R1, R1;
END
# 28 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
"ps_2_0
; 33 ALU
def c4, -1.00000000, 0.00000000, 1.00000000, 2.00000000
def c5, 0.50000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dp3 r0.x, t2, t2
rsq r1.x, r0.x
dp3 r0.x, t0, t0
mul r1.xyz, r1.x, t2
rsq r0.x, r0.x
mad r3.xyz, r0.x, t0, r1
dp3 r2.x, r3, r3
rsq r2.x, r2.x
dp3 r1.x, t1, t1
mul r0.xyz, r0.x, t0
mul r3.xyz, r2.x, r3
rsq r1.x, r1.x
mul r2.xyz, r1.x, t1
dp3_sat r1.x, r0, r2
dp3 r0.x, r2, r3
pow r3.x, r0.x, c1.x
add r1.x, r1, c3
add r0.x, r1, c4
dp3_pp r1.x, r2, t2
mov r2.x, r3.x
max_pp r1.x, r1, c4.y
mul r3.xyz, r2.x, c2
mul_pp r2.xyz, r1.x, c0
mul r1.xyz, r3, c5.x
mul_pp r2.xyz, r2, c4.w
mov_pp r2.w, c4.z
mov_pp r1.w, c4.z
mul_pp r1, r2, r1
cmp r0.x, r0, c4.y, c4.z
mad_pp r0, r0.x, -r1, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
SetTexture 1 [_LightTextureB0] 2D
"!!ARBfp1.0
# 37 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 0, 0.5, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
DP3 R0.z, fragment.texcoord[3], fragment.texcoord[3];
RCP R0.x, fragment.texcoord[3].w;
MAD R0.xy, fragment.texcoord[3], R0.x, c[4].z;
TEX R0.w, R0, texture[0], 2D;
TEX R1.w, R0.z, texture[1], 2D;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.y, R0.x;
MUL R1.xyz, R0.y, fragment.texcoord[2];
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MAD R1.xyz, R0.x, fragment.texcoord[0], R1;
DP3 R0.z, R1, R1;
RSQ R0.z, R0.z;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R0.z, R1;
RSQ R0.y, R0.y;
MUL R1.xyz, R0.y, fragment.texcoord[1];
MUL R0.xyz, R0.x, fragment.texcoord[0];
DP3_SAT R0.x, R0, R1;
DP3 R0.y, R1, R2;
ADD R2.x, -R0, c[4];
POW R0.x, R0.y, c[1].x;
DP3 R0.y, R1, fragment.texcoord[2];
SLT R0.z, c[4].y, fragment.texcoord[3];
MUL R0.z, R0, R0.w;
MUL R0.z, R0, R1.w;
MAX R0.y, R0, c[4];
MUL R0.w, R0.y, R0.z;
MUL R1.xyz, R0.w, c[0];
MUL R0.xyz, R0.x, c[2];
MUL R0.xyz, R0, c[4].z;
MUL R1.xyz, R1, c[4].w;
MOV R1.w, c[4].x;
MOV R0.w, c[4].x;
MUL R1, R1, R0;
SLT R0.x, c[3], R2;
MAD result.color, R0.x, -R1, R1;
END
# 37 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
SetTexture 1 [_LightTextureB0] 2D
"ps_2_0
; 41 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c4, -1.00000000, 0.00000000, 1.00000000, 0.50000000
def c5, 2.00000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3
dp3 r1.x, t3, t3
mov r1.xy, r1.x
rcp r0.x, t3.w
mad r0.xy, t3, r0.x, c4.w
mov_pp r1.w, c4.z
texld r2, r1, s1
texld r0, r0, s0
dp3 r0.x, t2, t2
rsq r1.x, r0.x
dp3 r0.x, t0, t0
mul r1.xyz, r1.x, t2
rsq r0.x, r0.x
mad r4.xyz, r0.x, t0, r1
dp3 r1.x, r4, r4
rsq r3.x, r1.x
mul r3.xyz, r3.x, r4
dp3 r1.x, t1, t1
rsq r1.x, r1.x
mul r1.xyz, r1.x, t1
mul r4.xyz, r0.x, t0
dp3 r0.x, r1, r3
dp3_sat r3.x, r4, r1
pow r4.x, r0.x, c1.x
dp3_pp r1.x, r1, t2
add r0.x, r3, c3
mov r3.x, r4.x
add r0.x, r0, c4
cmp r4.x, -t3.z, c4.y, c4.z
mul_pp r4.x, r4, r0.w
mul_pp r2.x, r4, r2
max_pp r1.x, r1, c4.y
mul_pp r1.x, r1, r2
mul_pp r2.xyz, r1.x, c0
mul r3.xyz, r3.x, c2
mul r1.xyz, r3, c4.w
mul_pp r2.xyz, r2, c5.x
mov_pp r2.w, c4.z
mul_pp r1, r2, r1
cmp r0.x, r0, c4.y, c4.z
mad_pp r0, r0.x, -r1, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTextureB0] 2D
SetTexture 1 [_LightTexture0] CUBE
"!!ARBfp1.0
# 33 ALU, 2 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 0, 2, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R1.w, fragment.texcoord[3], texture[1], CUBE;
DP3 R0.x, fragment.texcoord[3], fragment.texcoord[3];
TEX R0.w, R0.x, texture[0], 2D;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.y, R0.x;
MUL R1.xyz, R0.y, fragment.texcoord[2];
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MAD R1.xyz, R0.x, fragment.texcoord[0], R1;
DP3 R0.z, R1, R1;
RSQ R0.z, R0.z;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
MUL R2.xyz, R0.z, R1;
RSQ R0.y, R0.y;
MUL R1.xyz, R0.y, fragment.texcoord[1];
MUL R0.xyz, R0.x, fragment.texcoord[0];
DP3_SAT R0.x, R0, R1;
DP3 R0.y, R1, R2;
ADD R2.x, -R0, c[4];
POW R0.z, R0.y, c[1].x;
MUL R0.y, R0.w, R1.w;
DP3 R0.x, R1, fragment.texcoord[2];
MAX R0.x, R0, c[4].y;
MUL R0.w, R0.x, R0.y;
MUL R1.xyz, R0.w, c[0];
MUL R0.xyz, R0.z, c[2];
MUL R0.xyz, R0, c[4].w;
MUL R1.xyz, R1, c[4].z;
MOV R1.w, c[4].x;
MOV R0.w, c[4].x;
MUL R1, R1, R0;
SLT R0.x, c[3], R2;
MAD result.color, R0.x, -R1, R1;
END
# 33 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTextureB0] 2D
SetTexture 1 [_LightTexture0] CUBE
"ps_2_0
; 37 ALU, 2 TEX
dcl_2d s0
dcl_cube s1
def c4, -1.00000000, 0.00000000, 1.00000000, 2.00000000
def c5, 0.50000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xyz
dp3 r0.x, t3, t3
mov r1.xy, r0.x
mov_pp r1.w, c4.z
texld r2, r1, s0
texld r0, t3, s1
dp3 r0.x, t2, t2
rsq r1.x, r0.x
dp3 r0.x, t0, t0
mul r1.xyz, r1.x, t2
rsq r0.x, r0.x
mad r4.xyz, r0.x, t0, r1
dp3 r1.x, r4, r4
rsq r3.x, r1.x
mul r3.xyz, r3.x, r4
dp3 r1.x, t1, t1
rsq r1.x, r1.x
mul r1.xyz, r1.x, t1
mul r4.xyz, r0.x, t0
dp3 r0.x, r1, r3
dp3_sat r3.x, r4, r1
pow r4.x, r0.x, c1.x
dp3_pp r1.x, r1, t2
add r0.x, r3, c3
add r0.x, r0, c4
mov r3.x, r4.x
mul r2.x, r2, r0.w
max_pp r1.x, r1, c4.y
mul_pp r1.x, r1, r2
mul_pp r2.xyz, r1.x, c0
mul r3.xyz, r3.x, c2
mul r1.xyz, r3, c5.x
mul_pp r2.xyz, r2, c4.w
mov_pp r2.w, c4.z
mul_pp r1, r2, r1
cmp r0.x, r0, c4.y, c4.z
mad_pp r0, r0.x, -r1, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
"!!ARBfp1.0
# 30 ALU, 1 TEX
PARAM c[5] = { program.local[0..3],
		{ 1, 0, 2, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0.w, fragment.texcoord[3], texture[0], 2D;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
RSQ R0.y, R0.x;
MUL R1.xyz, R0.y, fragment.texcoord[2];
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MAD R2.xyz, R0.x, fragment.texcoord[0], R1;
DP3 R0.z, R2, R2;
DP3 R0.y, fragment.texcoord[1], fragment.texcoord[1];
RSQ R0.y, R0.y;
MUL R1.xyz, R0.y, fragment.texcoord[1];
RSQ R1.w, R0.z;
MUL R0.xyz, R0.x, fragment.texcoord[0];
DP3_SAT R2.w, R0, R1;
MUL R0.xyz, R1.w, R2;
DP3 R0.y, R1, R0;
DP3 R0.x, R1, fragment.texcoord[2];
MAX R0.x, R0, c[4].y;
MUL R0.w, R0.x, R0;
MUL R1.xyz, R0.w, c[0];
POW R0.y, R0.y, c[1].x;
MUL R0.xyz, R0.y, c[2];
MUL R0.xyz, R0, c[4].w;
ADD R2.x, -R2.w, c[4];
MUL R1.xyz, R1, c[4].z;
MOV R1.w, c[4].x;
MOV R0.w, c[4].x;
MUL R1, R1, R0;
SLT R0.x, c[3], R2;
MAD result.color, R0.x, -R1, R1;
END
# 30 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Float 1 [_Specular]
Vector 2 [_SpecularColor]
Float 3 [_Transition]
SetTexture 0 [_LightTexture0] 2D
"ps_2_0
; 34 ALU, 1 TEX
dcl_2d s0
def c4, -1.00000000, 0.00000000, 1.00000000, 2.00000000
def c5, 0.50000000, 0, 0, 0
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dcl t3.xy
texld r0, t3, s0
dp3 r0.x, t2, t2
rsq r1.x, r0.x
dp3 r0.x, t0, t0
mul r1.xyz, r1.x, t2
rsq r0.x, r0.x
mad r3.xyz, r0.x, t0, r1
dp3 r1.x, r3, r3
rsq r2.x, r1.x
mul r2.xyz, r2.x, r3
dp3 r1.x, t1, t1
rsq r1.x, r1.x
mul r1.xyz, r1.x, t1
mul r3.xyz, r0.x, t0
dp3 r0.x, r1, r2
dp3_sat r2.x, r3, r1
pow r3.x, r0.x, c1.x
dp3_pp r1.x, r1, t2
add r0.x, r2, c3
mov r2.x, r3.x
add r0.x, r0, c4
max_pp r1.x, r1, c4.y
mul_pp r1.x, r1, r0.w
mul r3.xyz, r2.x, c2
mul_pp r2.xyz, r1.x, c0
mul r1.xyz, r3, c5.x
mul_pp r2.xyz, r2, c4.w
mov_pp r2.w, c4.z
mov_pp r1.w, c4.z
mul_pp r1, r2, r1
cmp r0.x, r0, c4.y, c4.z
mad_pp r0, r0.x, -r1, r1
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}