Shader "GreyscaleShader" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
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
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 3 ALU, 1 TEX
PARAM c[1] = { { 0.21264648, 0.71533203, 0.07220459 } };
TEMP R0;
TEX R0, fragment.texcoord[0], texture[0], 2D;
MOV result.color.w, R0;
DP3 result.color.xyz, R0, c[0];
END
# 3 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 2 ALU, 1 TEX
dcl_2d s0
def c0, 0.21264648, 0.71533203, 0.07220459, 0
dcl t0.xy
texld r0, t0, s0
dp3 r0.xyz, r0, c0
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}