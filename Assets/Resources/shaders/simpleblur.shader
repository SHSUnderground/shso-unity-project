Shader "SimpleBlur" {
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
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 8 ALU
PARAM c[6] = { { 0.001953125, -0.001953125, 0.00390625, -0.00390625 },
		state.matrix.mvp,
		{ 0.005859375, -0.005859375 } };
ADD result.texcoord[1], vertex.texcoord[0].xyxy, c[0].xxyy;
ADD result.texcoord[2], vertex.texcoord[0].xyxy, c[0].zzww;
ADD result.texcoord[3], vertex.texcoord[0].xyxy, c[5].xxyy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 8 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 8 ALU
def c4, 0.00195313, -0.00195313, 0.00390625, -0.00390625
def c5, 0.00585938, -0.00585938, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
add oT1, v1.xyxy, c4.xxyy
add oT2, v1.xyxy, c4.zzww
add oT3, v1.xyxy, c5.xxyy
mov oT0.xy, v1
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
# 20 ALU, 7 TEX
PARAM c[1] = { { 0.40000001, 0.15000001, 0.1, 0.050000001 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R2, fragment.texcoord[1].zwzw, texture[0], 2D;
TEX R1, fragment.texcoord[1], texture[0], 2D;
TEX R6, fragment.texcoord[3].zwzw, texture[0], 2D;
TEX R5, fragment.texcoord[3], texture[0], 2D;
TEX R4, fragment.texcoord[2].zwzw, texture[0], 2D;
TEX R3, fragment.texcoord[2], texture[0], 2D;
MUL R2, R2, c[0].y;
MUL R1, R1, c[0].y;
MUL R0, R0, c[0].x;
ADD R0, R0, R1;
ADD R0, R0, R2;
MUL R1, R3, c[0].z;
ADD R0, R0, R1;
MUL R2, R4, c[0].z;
ADD R0, R0, R2;
MUL R1, R5, c[0].w;
MUL R2, R6, c[0].w;
ADD R0, R0, R1;
ADD result.color, R0, R2;
END
# 20 instructions, 7 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 22 ALU, 7 TEX
dcl_2d s0
def c0, 0.40000001, 0.15000001, 0.10000000, 0.05000000
dcl t0.xy
dcl t1
dcl t2
dcl t3
texld r3, t2, s0
texld r6, t0, s0
texld r5, t1, s0
mov r0.y, t1.w
mov r0.x, t1.z
mov r4.xy, r0
mov r1.y, t2.w
mov r1.x, t2.z
mov r2.xy, r1
mov r0.y, t3.w
mov r0.x, t3.z
mul r5, r5, c0.y
mul r6, r6, c0.x
add_pp r5, r6, r5
mul r3, r3, c0.z
texld r0, r0, s0
texld r1, t3, s0
texld r2, r2, s0
texld r4, r4, s0
mul r4, r4, c0.y
add_pp r4, r5, r4
mul r2, r2, c0.z
add_pp r3, r4, r3
add_pp r2, r3, r2
mul r1, r1, c0.w
mul r0, r0, c0.w
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