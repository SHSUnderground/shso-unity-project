Shader "Marvel/FX/Vertex Color Alpha" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [unity_SHAr]
Vector 11 [unity_SHAg]
Vector 12 [unity_SHAb]
Vector 13 [unity_SHBr]
Vector 14 [unity_SHBg]
Vector 15 [unity_SHBb]
Vector 16 [unity_SHC]
"!!ARBvp1.0
# 27 ALU
PARAM c[17] = { { 1 },
		state.matrix.mvp,
		program.local[5..16] };
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
ADD result.texcoord[1].xyz, R2, R1;
MOV result.color, vertex.color;
MOV result.texcoord[0].z, R2.w;
MOV result.texcoord[0].y, R3.w;
MOV result.texcoord[0].x, R0;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 27 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
"vs_2_0
; 27 ALU
def c16, 1.00000000, 0, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r1.xyz, v1, c8.w
dp3 r3.w, r1, c5
dp3 r2.w, r1, c6
dp3 r0.x, r1, c4
mov r0.y, r3.w
mov r0.z, r2.w
mul r1, r0.xyzz, r0.yzzx
mov r0.w, c16.x
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
add oT1.xyz, r2, r1
mov oD0, v2
mov oT0.z, r2.w
mov oT0.y, r3.w
mov oT0.x, r0
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
Vector 9 [unity_LightmapST]
"!!ARBvp1.0
# 6 ALU
PARAM c[10] = { program.local[0],
		state.matrix.mvp,
		program.local[5..9] };
MOV result.color, vertex.color;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[9], c[9].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 6 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 8 [unity_LightmapST]
"vs_2_0
; 6 ALU
dcl_position0 v0
dcl_texcoord1 v2
dcl_color0 v3
mov oD0, v3
mad oT1.xy, v2, c8, c8.zwzw
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
"!!ARBvp1.0
# 32 ALU
PARAM c[18] = { { 1, 0.5 },
		state.matrix.mvp,
		program.local[5..17] };
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
ADD result.texcoord[1].xyz, R3, R2;
ADD result.texcoord[2].xy, R1, R1.z;
MOV result.position, R0;
MOV result.color, vertex.color;
MOV result.texcoord[2].zw, R0;
MOV result.texcoord[0].z, R2.w;
MOV result.texcoord[0].y, R3.w;
MOV result.texcoord[0].x, R1.w;
END
# 32 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
"vs_2_0
; 32 ALU
def c18, 1.00000000, 0.50000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r0.xyz, v1, c10.w
dp3 r3.w, r0, c5
dp3 r2.w, r0, c6
dp3 r1.w, r0, c4
mov r1.x, r3.w
mov r1.y, r2.w
mov r1.z, c18.x
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
mul r1.xyz, r0.xyww, c18.y
mul r1.y, r1, c8.x
add oT1.xyz, r3, r2
mad oT2.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mov oD0, v2
mov oT2.zw, r0
mov oT0.z, r2.w
mov oT0.y, r3.w
mov oT0.x, r1.w
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [unity_LightmapST]
"!!ARBvp1.0
# 11 ALU
PARAM c[11] = { { 0.5 },
		state.matrix.mvp,
		program.local[5..10] };
TEMP R0;
TEMP R1;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[9].x;
ADD result.texcoord[2].xy, R1, R1.z;
MOV result.position, R0;
MOV result.color, vertex.color;
MOV result.texcoord[2].zw, R0;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[10], c[10].zwzw;
END
# 11 instructions, 2 R-regs
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
Vector 10 [unity_LightmapST]
"vs_2_0
; 11 ALU
def c11, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord1 v2
dcl_color0 v3
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c11.x
mul r1.y, r1, c8.x
mad oT2.xy, r1.z, c9.zwzw, r1
mov oPos, r0
mov oD0, v3
mov oT2.zw, r0
mad oT1.xy, v2, c10, c10.zwzw
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
"!!ARBvp1.0
# 57 ALU
PARAM c[25] = { { 1, 0 },
		state.matrix.mvp,
		program.local[5..24] };
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
ADD result.texcoord[1].xyz, R0, R1;
MOV result.color, vertex.color;
MOV result.texcoord[0].z, R3.x;
MOV result.texcoord[0].y, R3.w;
MOV result.texcoord[0].x, R4;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 57 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
"vs_2_0
; 57 ALU
def c24, 1.00000000, 0.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
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
mov r4.w, c24.x
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
add r1, r2, c24.x
dp4 r2.z, r4, c19
dp4 r2.y, r4, c18
dp4 r2.x, r4, c17
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c24.y
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
add oT1.xyz, r0, r1
mov oD0, v2
mov oT0.z, r3.x
mov oT0.y, r3.w
mov oT0.x, r4
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
"!!ARBvp1.0
# 63 ALU
PARAM c[26] = { { 1, 0, 0.5 },
		state.matrix.mvp,
		program.local[5..25] };
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
ADD result.texcoord[1].xyz, R4.yzww, R1;
MOV R1.x, R2;
MUL R1.y, R2, c[9].x;
ADD result.texcoord[2].xy, R1, R2.z;
MOV result.position, R0;
MOV result.color, vertex.color;
MOV result.texcoord[2].zw, R0;
MOV result.texcoord[0].z, R3.x;
MOV result.texcoord[0].y, R3.w;
MOV result.texcoord[0].x, R4;
END
# 63 instructions, 5 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "VERTEXLIGHT_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
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
"vs_2_0
; 63 ALU
def c26, 1.00000000, 0.00000000, 0.50000000, 0
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
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
mov r4.w, c26.x
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
add r1, r2, c26.x
dp4 r2.z, r4, c21
dp4 r2.y, r4, c20
dp4 r2.x, r4, c19
rcp r1.x, r1.x
rcp r1.y, r1.y
rcp r1.w, r1.w
rcp r1.z, r1.z
max r0, r0, c26.y
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
mul r2.xyz, r0.xyww, c26.z
add oT1.xyz, r4.yzww, r1
mov r1.x, r2
mul r1.y, r2, c8.x
mad oT2.xy, r2.z, c9.zwzw, r1
mov oPos, r0
mov oD0, v2
mov oT2.zw, r0
mov oT0.z, r3.x
mov oT0.y, r3.w
mov oT0.x, r4
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 0 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
MUL R0, fragment.color.primary, c[2];
MUL R1.xyz, R0, fragment.texcoord[1];
DP3 R1.w, fragment.texcoord[0], c[0];
MUL R0.xyz, R0, c[1];
MAX R1.w, R1, c[3].x;
MUL R0.xyz, R1.w, R0;
MAD result.color.xyz, R0, c[3].y, R1;
MOV result.color.w, R0;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
"ps_2_0
; 9 ALU
def c3, 0.00000000, 2.00000000, 0, 0
dcl v0
dcl t0.xyz
dcl t1.xyz
mul r1, v0, c2
mul_pp r2.xyz, r1, t1
dp3_pp r0.x, t0, c0
mov_pp r0.w, r1
mul_pp r1.xyz, r1, c1
max_pp r0.x, r0, c3
mul_pp r0.xyz, r0.x, r1
mad_pp r0.xyz, r0, c3.y, r2
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 1 TEX
PARAM c[2] = { program.local[0],
		{ 8 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[1], texture[0], 2D;
MUL R1.xyz, R0.w, R0;
MUL R0, fragment.color.primary, c[0];
MUL R0.xyz, R0, R1;
MUL result.color.xyz, R0, c[1].x;
MOV result.color.w, R0;
END
# 6 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [unity_Lightmap] 2D
"ps_2_0
; 5 ALU, 1 TEX
dcl_2d s0
def c1, 8.00000000, 0, 0, 0
dcl v0
dcl t1.xy
texld r0, t1, s0
mul_pp r1.xyz, r0.w, r0
mul r0, v0, c0
mul_pp r0.xyz, r0, r1
mul_pp r0.xyz, r0, c1.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
SetTexture 0 [_ShadowMapTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 1 TEX
PARAM c[4] = { program.local[0..2],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEMP R2;
TXP R2.x, fragment.texcoord[2], texture[0], 2D;
MUL R0, fragment.color.primary, c[2];
MUL R1.xyz, R0, fragment.texcoord[1];
DP3 R1.w, fragment.texcoord[0], c[0];
MAX R1.w, R1, c[3].x;
MUL R0.xyz, R0, c[1];
MUL R1.w, R1, R2.x;
MUL R0.xyz, R1.w, R0;
MAD result.color.xyz, R0, c[3].y, R1;
MOV result.color.w, R0;
END
# 10 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" }
Vector 0 [_WorldSpaceLightPos0]
Vector 1 [_LightColor0]
Vector 2 [_Color]
SetTexture 0 [_ShadowMapTexture] 2D
"ps_2_0
; 10 ALU, 1 TEX
dcl_2d s0
def c3, 0.00000000, 2.00000000, 0, 0
dcl v0
dcl t0.xyz
dcl t1.xyz
dcl t2
texldp r3, t2, s0
mul r1, v0, c2
mul_pp r2.xyz, r1, c1
dp3_pp r0.x, t0, c0
max_pp r0.x, r0, c3
mul_pp r0.x, r0, r3
mul_pp r0.xyz, r0.x, r2
mul_pp r1.xyz, r1, t1
mov_pp r0.w, r1
mad_pp r0.xyz, r0, c3.y, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [_ShadowMapTexture] 2D
SetTexture 1 [unity_Lightmap] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 9 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 8, 2 } };
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[1], texture[1], 2D;
TXP R1.x, fragment.texcoord[2], texture[0], 2D;
MUL R0.xyz, R0.w, R0;
MUL R0.w, R1.x, c[1].y;
MUL R0.xyz, R0, c[1].x;
MIN R1.xyz, R0, R0.w;
MUL R0, fragment.color.primary, c[0];
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 9 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [_ShadowMapTexture] 2D
SetTexture 1 [unity_Lightmap] 2D
"ps_2_0
; 7 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 8.00000000, 2.00000000, 0, 0
dcl v0
dcl t1.xy
dcl t2
texldp r1, t2, s0
texld r0, t1, s1
mul_pp r0.xyz, r0.w, r0
mul_pp r0.xyz, r0, c1.x
mul_pp r1.x, r1, c1.y
min_pp r1.xyz, r0, r1.x
mul r0, v0, c0
mul_pp r0.xyz, r0, r1
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
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
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 17 ALU
PARAM c[15] = { program.local[0],
		state.matrix.mvp,
		program.local[5..14] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
MOV result.color, vertex.color;
DP4 result.texcoord[2].z, R0, c[11];
DP4 result.texcoord[2].y, R0, c[10];
DP4 result.texcoord[2].x, R0, c[9];
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
ADD result.texcoord[1].xyz, -R0, c[14];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 17 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
"vs_2_0
; 17 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mov oD0, v2
dp4 oT2.z, r0, c10
dp4 oT2.y, r0, c9
dp4 oT2.x, r0, c8
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
add oT1.xyz, -r0, c13
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
Matrix 5 [_Object2World]
Vector 9 [unity_Scale]
Vector 10 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 10 ALU
PARAM c[11] = { program.local[0],
		state.matrix.mvp,
		program.local[5..10] };
TEMP R0;
MUL R0.xyz, vertex.normal, c[9].w;
MOV result.color, vertex.color;
DP3 result.texcoord[0].z, R0, c[7];
DP3 result.texcoord[0].y, R0, c[6];
DP3 result.texcoord[0].x, R0, c[5];
MOV result.texcoord[1].xyz, c[10];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [unity_Scale]
Vector 9 [_WorldSpaceLightPos0]
"vs_2_0
; 10 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r0.xyz, v1, c8.w
mov oD0, v2
dp3 oT0.z, r0, c6
dp3 oT0.y, r0, c5
dp3 oT0.x, r0, c4
mov oT1.xyz, c9
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
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 18 ALU
PARAM c[15] = { program.local[0],
		state.matrix.mvp,
		program.local[5..14] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
MOV result.color, vertex.color;
DP4 result.texcoord[2].w, R0, c[12];
DP4 result.texcoord[2].z, R0, c[11];
DP4 result.texcoord[2].y, R0, c[10];
DP4 result.texcoord[2].x, R0, c[9];
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
ADD result.texcoord[1].xyz, -R0, c[14];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 18 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
"vs_2_0
; 18 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mov oD0, v2
dp4 oT2.w, r0, c11
dp4 oT2.z, r0, c10
dp4 oT2.y, r0, c9
dp4 oT2.x, r0, c8
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
add oT1.xyz, -r0, c13
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
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 17 ALU
PARAM c[15] = { program.local[0],
		state.matrix.mvp,
		program.local[5..14] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
MOV result.color, vertex.color;
DP4 result.texcoord[2].z, R0, c[11];
DP4 result.texcoord[2].y, R0, c[10];
DP4 result.texcoord[2].x, R0, c[9];
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
ADD result.texcoord[1].xyz, -R0, c[14];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 17 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
"vs_2_0
; 17 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r1.xyz, v1, c12.w
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
mov oD0, v2
dp4 oT2.z, r0, c10
dp4 oT2.y, r0, c9
dp4 oT2.x, r0, c8
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
add oT1.xyz, -r0, c13
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
Matrix 5 [_Object2World]
Matrix 9 [_LightMatrix0]
Vector 13 [unity_Scale]
Vector 14 [_WorldSpaceLightPos0]
"!!ARBvp1.0
# 16 ALU
PARAM c[15] = { program.local[0],
		state.matrix.mvp,
		program.local[5..14] };
TEMP R0;
TEMP R1;
MUL R1.xyz, vertex.normal, c[13].w;
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
MOV result.color, vertex.color;
DP4 result.texcoord[2].y, R0, c[10];
DP4 result.texcoord[2].x, R0, c[9];
DP3 result.texcoord[0].z, R1, c[7];
DP3 result.texcoord[0].y, R1, c[6];
DP3 result.texcoord[0].x, R1, c[5];
MOV result.texcoord[1].xyz, c[14];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 16 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Bind "vertex" Vertex
Bind "color" Color
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Matrix 8 [_LightMatrix0]
Vector 12 [unity_Scale]
Vector 13 [_WorldSpaceLightPos0]
"vs_2_0
; 16 ALU
dcl_position0 v0
dcl_normal0 v1
dcl_color0 v2
mul r1.xyz, v1, c12.w
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mov oD0, v2
dp4 oT2.y, r0, c9
dp4 oT2.x, r0, c8
dp3 oT0.z, r1, c6
dp3 oT0.y, r1, c5
dp3 oT0.x, r1, c4
mov oT1.xyz, c13
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
SetTexture 0 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 13 ALU, 1 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
MOV result.color.w, c[2].x;
TEX R0.w, R0.x, texture[0], 2D;
DP3 R0.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R0.x, R0.x;
MUL R1.xyz, R0.x, fragment.texcoord[1];
MUL R0.xyz, fragment.color.primary, c[1];
DP3 R1.x, fragment.texcoord[0], R1;
MAX R1.x, R1, c[2];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].y;
END
# 13 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTexture0] 2D
"ps_2_0
; 14 ALU, 1 TEX
dcl_2d s0
def c2, 0.00000000, 2.00000000, 0, 0
dcl v0.xyz
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dp3 r0.x, t2, t2
mov r0.xy, r0.x
mul r1.xyz, v0, c1
mul_pp r1.xyz, r1, c0
mov_pp r0.w, c2.x
texld r2, r0, s0
dp3_pp r0.x, t1, t1
rsq_pp r0.x, r0.x
mul_pp r0.xyz, r0.x, t1
dp3_pp r0.x, t0, r0
max_pp r0.x, r0, c2
mul_pp r0.x, r0, r2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.y
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 0 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
MOV R1.xyz, fragment.texcoord[1];
MUL R0.xyz, fragment.color.primary, c[1];
DP3 R0.w, fragment.texcoord[0], R1;
MUL R0.xyz, R0, c[0];
MAX R0.w, R0, c[2].x;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].y;
MOV result.color.w, c[2].x;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
"ps_2_0
; 9 ALU
def c2, 0.00000000, 2.00000000, 0, 0
dcl v0.xyz
dcl t0.xyz
dcl t1.xyz
mov_pp r0.xyz, t1
dp3_pp r0.x, t0, r0
mul r1.xyz, v0, c1
mul_pp r1.xyz, r1, c0
max_pp r0.x, r0, c2
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.y
mov_pp r0.w, c2.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTexture0] 2D
SetTexture 1 [_LightTextureB0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 19 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 0.5, 2 } };
TEMP R0;
TEMP R1;
DP3 R0.z, fragment.texcoord[2], fragment.texcoord[2];
RCP R0.x, fragment.texcoord[2].w;
MAD R0.xy, fragment.texcoord[2], R0.x, c[2].y;
MOV result.color.w, c[2].x;
TEX R0.w, R0, texture[0], 2D;
TEX R1.w, R0.z, texture[1], 2D;
DP3 R0.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.x, R0.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, fragment.texcoord[0], R1;
SLT R1.y, c[2].x, fragment.texcoord[2].z;
MUL R0.w, R1.y, R0;
MUL R1.y, R0.w, R1.w;
MUL R0.xyz, fragment.color.primary, c[1];
MAX R0.w, R1.x, c[2].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].z;
END
# 19 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "SPOT" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTexture0] 2D
SetTexture 1 [_LightTextureB0] 2D
"ps_2_0
; 19 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c2, 0.50000000, 0.00000000, 1.00000000, 2.00000000
dcl v0.xyz
dcl t0.xyz
dcl t1.xyz
dcl t2
rcp r1.x, t2.w
mad r2.xy, t2, r1.x, c2.x
dp3 r0.x, t2, t2
mov r1.xy, r0.x
texld r0, r2, s0
texld r2, r1, s1
cmp r0.x, -t2.z, c2.y, c2.z
mul_pp r0.x, r0, r0.w
mul_pp r0.x, r0, r2
dp3_pp r1.x, t1, t1
rsq_pp r1.x, r1.x
mul_pp r1.xyz, r1.x, t1
mul r2.xyz, v0, c1
dp3_pp r1.x, t0, r1
max_pp r1.x, r1, c2.y
mul_pp r2.xyz, r2, c0
mul_pp r0.x, r1, r0
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c2.w
mov_pp r0.w, c2.y
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTextureB0] 2D
SetTexture 1 [_LightTexture0] CUBE
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 2 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R1.w, fragment.texcoord[2], texture[1], CUBE;
DP3 R0.x, fragment.texcoord[2], fragment.texcoord[2];
MOV result.color.w, c[2].x;
TEX R0.w, R0.x, texture[0], 2D;
DP3 R0.x, fragment.texcoord[1], fragment.texcoord[1];
RSQ R1.x, R0.x;
MUL R1.xyz, R1.x, fragment.texcoord[1];
DP3 R1.x, fragment.texcoord[0], R1;
MUL R1.y, R0.w, R1.w;
MUL R0.xyz, fragment.color.primary, c[1];
MAX R0.w, R1.x, c[2].x;
MUL R0.xyz, R0, c[0];
MUL R0.w, R0, R1.y;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].y;
END
# 15 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "POINT_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTextureB0] 2D
SetTexture 1 [_LightTexture0] CUBE
"ps_2_0
; 15 ALU, 2 TEX
dcl_2d s0
dcl_cube s1
def c2, 0.00000000, 2.00000000, 0, 0
dcl v0.xyz
dcl t0.xyz
dcl t1.xyz
dcl t2.xyz
dp3 r0.x, t2, t2
mov r0.xy, r0.x
texld r2, r0, s0
texld r0, t2, s1
dp3_pp r0.x, t1, t1
rsq_pp r1.x, r0.x
mul_pp r1.xyz, r1.x, t1
dp3_pp r1.x, t0, r1
mul r0.x, r2, r0.w
mul r2.xyz, v0, c1
max_pp r1.x, r1, c2
mul_pp r2.xyz, r2, c0
mul_pp r0.x, r1, r0
mul_pp r0.xyz, r0.x, r2
mul_pp r0.xyz, r0, c2.y
mov_pp r0.w, c2.x
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTexture0] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 10 ALU, 1 TEX
PARAM c[3] = { program.local[0..1],
		{ 0, 2 } };
TEMP R0;
TEMP R1;
TEX R0.w, fragment.texcoord[2], texture[0], 2D;
MOV R1.xyz, fragment.texcoord[1];
MUL R0.xyz, fragment.color.primary, c[1];
DP3 R1.x, fragment.texcoord[0], R1;
MAX R1.x, R1, c[2];
MUL R0.xyz, R0, c[0];
MUL R0.w, R1.x, R0;
MUL R0.xyz, R0.w, R0;
MUL result.color.xyz, R0, c[2].y;
MOV result.color.w, c[2].x;
END
# 10 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "DIRECTIONAL_COOKIE" }
Vector 0 [_LightColor0]
Vector 1 [_Color]
SetTexture 0 [_LightTexture0] 2D
"ps_2_0
; 10 ALU, 1 TEX
dcl_2d s0
def c2, 0.00000000, 2.00000000, 0, 0
dcl v0.xyz
dcl t0.xyz
dcl t1.xyz
dcl t2.xy
texld r0, t2, s0
mov_pp r0.xyz, t1
dp3_pp r0.x, t0, r0
mul r1.xyz, v0, c1
max_pp r0.x, r0, c2
mul_pp r0.x, r0, r0.w
mul_pp r1.xyz, r1, c0
mul_pp r0.xyz, r0.x, r1
mul_pp r0.xyz, r0, c2.y
mov_pp r0.w, c2.x
mov_pp oC0, r0
"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
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
  Tags { "LIGHTMODE"="PrePassFinal" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Vector 5 [_ProjectionParams]
"!!ARBvp1.0
# 10 ALU
PARAM c[6] = { { 0.5 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
TEMP R1;
DP4 R0.w, vertex.position, c[4];
DP4 R0.z, vertex.position, c[3];
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].x;
MUL R1.y, R1, c[5].x;
ADD result.texcoord[0].xy, R1, R1.z;
MOV result.position, R0;
MOV result.color, vertex.color;
MOV result.texcoord[0].zw, R0;
END
# 10 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Bind "vertex" Vertex
Bind "color" Color
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ProjectionParams]
Vector 5 [_ScreenParams]
"vs_2_0
; 10 ALU
def c6, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_color0 v1
dp4 r0.w, v0, c3
dp4 r0.z, v0, c2
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c6.x
mul r1.y, r1, c4.x
mad oT0.xy, r1.z, c5.zwzw, r1
mov oPos, r0
mov oD0, v1
mov oT0.zw, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord1" TexCoord1
Vector 9 [_ProjectionParams]
Vector 10 [unity_LightmapST]
Vector 11 [unity_LightmapFade]
"!!ARBvp1.0
# 13 ALU
PARAM c[12] = { { 0.5 },
		state.matrix.modelview[0],
		state.matrix.mvp,
		program.local[9..11] };
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
ADD result.texcoord[0].xy, R1, R1.z;
MOV result.color, vertex.color;
MOV result.texcoord[0].zw, R0;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[10], c[10].zwzw;
MAD result.texcoord[1].z, -R0.x, c[11], c[11].w;
END
# 13 instructions, 2 R-regs
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
Vector 10 [unity_LightmapST]
Vector 11 [unity_LightmapFade]
"vs_2_0
; 13 ALU
def c12, 0.50000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord1 v1
dcl_color0 v2
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
mul r1.xyz, r0.xyww, c12.x
mul r1.y, r1, c8.x
mov oPos, r0
dp4 r0.x, v0, c2
mad oT0.xy, r1.z, c9.zwzw, r1
mov oD0, v2
mov oT0.zw, r0
mad oT1.xy, v1, c10, c10.zwzw
mad oT1.z, -r0.x, c11, c11.w
"
}
}
Program "fp" {
SubProgram "opengl " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [unity_Ambient]
SetTexture 0 [_LightBuffer] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 8 ALU, 1 TEX
PARAM c[2] = { program.local[0..1] };
TEMP R0;
TEMP R1;
TXP R0.xyz, fragment.texcoord[0], texture[0], 2D;
LG2 R0.x, R0.x;
LG2 R0.z, R0.z;
LG2 R0.y, R0.y;
ADD R1.xyz, -R0, c[1];
MUL R0, fragment.color.primary, c[0];
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 8 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_OFF" }
Vector 0 [_Color]
Vector 1 [unity_Ambient]
SetTexture 0 [_LightBuffer] 2D
"ps_2_0
; 7 ALU, 1 TEX
dcl_2d s0
dcl v0
dcl t0
texldp r0, t0, s0
log_pp r0.x, r0.x
log_pp r0.z, r0.z
log_pp r0.y, r0.y
add_pp r1.xyz, -r0, c1
mul r0, v0, c0
mul_pp r0.xyz, r0, r1
mov_pp oC0, r0
"
}
SubProgram "opengl " {
Keywords { "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [_LightBuffer] 2D
SetTexture 1 [unity_Lightmap] 2D
SetTexture 2 [unity_LightmapInd] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 16 ALU, 3 TEX
PARAM c[2] = { program.local[0],
		{ 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEX R0, fragment.texcoord[1], texture[2], 2D;
TEX R1, fragment.texcoord[1], texture[1], 2D;
TXP R2.xyz, fragment.texcoord[0], texture[0], 2D;
MUL R0.xyz, R0.w, R0;
MUL R0.xyz, R0, c[1].x;
MUL R1.xyz, R1.w, R1;
MAD R1.xyz, R1, c[1].x, -R0;
MOV_SAT R0.w, fragment.texcoord[1].z;
MAD R0.xyz, R0.w, R1, R0;
LG2 R1.x, R2.x;
LG2 R1.y, R2.y;
LG2 R1.z, R2.z;
ADD R1.xyz, -R1, R0;
MUL R0, fragment.color.primary, c[0];
MUL result.color.xyz, R0, R1;
MOV result.color.w, R0;
END
# 16 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Keywords { "LIGHTMAP_ON" }
Vector 0 [_Color]
SetTexture 0 [_LightBuffer] 2D
SetTexture 1 [unity_Lightmap] 2D
SetTexture 2 [unity_LightmapInd] 2D
"ps_2_0
; 13 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c1, 8.00000000, 0, 0, 0
dcl v0
dcl t0
dcl t1.xyz
texldp r2, t0, s0
texld r1, t1, s2
texld r0, t1, s1
mul_pp r1.xyz, r1.w, r1
mul_pp r1.xyz, r1, c1.x
mul_pp r0.xyz, r0.w, r0
mad_pp r0.xyz, r0, c1.x, -r1
mov_sat r3.x, t1.z
mad_pp r0.xyz, r3.x, r0, r1
log_pp r1.x, r2.x
log_pp r1.y, r2.y
log_pp r1.z, r2.z
add_pp r1.xyz, -r1, r0
mul r0, v0, c0
mul_pp r0.xyz, r0, r1
mov_pp oC0, r0
"
}
}
 }
}
Fallback "Transparent/VertexLit"
}