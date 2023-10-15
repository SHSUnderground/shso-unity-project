Shader "GoldShader" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _RampTex ("Ramp (RGB)", 2D) = "grayscaleRamp" {}
 _Offset ("Offset", Float) = 0
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 8 ALU
PARAM c[9] = { { 0 },
		state.matrix.mvp,
		state.matrix.texture[0] };
TEMP R0;
MOV R0.zw, c[0].x;
MOV R0.xy, vertex.texcoord[0];
DP4 result.texcoord[0].y, R0, c[6];
DP4 result.texcoord[0].x, R0, c[5];
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
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"vs_2_0
; 8 ALU
def c8, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.zw, c8.x
mov r0.xy, v1
dp4 oT0.y, r0, c5
dp4 oT0.x, r0, c4
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Float 0 [_Offset]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_RampTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[3] = { program.local[0],
		{ 1, 0.21264648, 0.71533203, 0.07220459 },
		{ 0.5 } };
TEMP R0;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
DP3 R0.x, R0, c[1].yzww;
MOV R0.y, c[2].x;
ADD R0.x, R0, c[0];
MOV result.color.w, c[1].x;
TEX result.color.xyz, R0, texture[1], 2D;
END
# 6 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Float 0 [_Offset]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_RampTex] 2D
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 0.21264648, 0.71533203, 0.07220459, 0.50000000
def c2, 1.00000000, 0, 0, 0
dcl t0.xy
texld r0, t0, s0
dp3_pp r0.x, r0, c1
mov r0.y, c1.w
add r0.x, r0, c0
texld r0, r0, s1
mov_pp r0.w, c2.x
mov_pp oC0, r0
"
}
}
 }
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 8 ALU
PARAM c[9] = { { 0 },
		state.matrix.mvp,
		state.matrix.texture[0] };
TEMP R0;
MOV R0.zw, c[0].x;
MOV R0.xy, vertex.texcoord[0];
DP4 result.texcoord[0].y, R0, c[6];
DP4 result.texcoord[0].x, R0, c[5];
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
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"vs_2_0
; 8 ALU
def c8, 0.00000000, 0, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
mov r0.zw, c8.x
mov r0.xy, v1
dp4 oT0.y, r0, c5
dp4 oT0.x, r0, c4
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Float 0 [_Offset]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_RampTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 6 ALU, 2 TEX
PARAM c[3] = { program.local[0],
		{ 1, 0.21264648, 0.71533203, 0.07220459 },
		{ 0.5 } };
TEMP R0;
TEX R0.xyz, fragment.texcoord[0], texture[0], 2D;
DP3 R0.x, R0, c[1].yzww;
MOV R0.y, c[2].x;
ADD R0.x, R0, c[0];
MOV result.color.w, c[1].x;
TEX result.color.xyz, R0, texture[1], 2D;
END
# 6 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Float 0 [_Offset]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_RampTex] 2D
"ps_2_0
; 5 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 0.21264648, 0.71533203, 0.07220459, 0.50000000
def c2, 1.00000000, 0, 0, 0
dcl t0.xy
texld r0, t0, s0
dp3_pp r0.x, r0, c1
mov r0.y, c1.w
add r0.x, r0, c0
texld r0, r0, s1
mov_pp r0.w, c2.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}