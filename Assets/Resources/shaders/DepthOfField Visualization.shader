Shader "Hidden/Depth Of Field Visualization" {
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
Vector 5 [_NoiseSize]
"!!ARBvp1.0
# 6 ALU
PARAM c[6] = { program.local[0],
		state.matrix.mvp,
		program.local[5] };
MUL result.texcoord[0].zw, vertex.texcoord[0].xyxy, c[5].xyxy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 6 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_NoiseSize]
"vs_2_0
; 6 ALU
dcl_position0 v0
dcl_texcoord0 v1
mul oT0.zw, v1.xyxy, c4.xyxy
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
Vector 0 [_ZBufferParams]
Vector 1 [_GlobalDepthTexture_TexelSize]
Vector 2 [_DOFParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Noise] 2D
SetTexture 2 [_GlobalDepthTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 42 ALU, 7 TEX
PARAM c[4] = { program.local[0..2],
		{ 0.125, -1, 1, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEX R0.xy, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R5, fragment.texcoord[0], texture[0], 2D;
TEX R3.x, fragment.texcoord[0], texture[2], 2D;
MUL R1.xy, R0, c[2].w;
MAD R0.zw, -R1.xyxy, c[1].xyxy, fragment.texcoord[0].xyxy;
MUL R0.xy, R1.yxzw, c[3].yzzw;
MAD R1.zw, -R0.xyxy, c[1].xyxy, fragment.texcoord[0].xyxy;
MAD R1.xy, R1, c[1], fragment.texcoord[0];
MAD R0.xy, R0, c[1], fragment.texcoord[0];
MOV result.color.w, R5;
TEX R4.x, R0.zwzw, texture[2], 2D;
TEX R0.x, R0, texture[2], 2D;
TEX R2.x, R1.zwzw, texture[2], 2D;
TEX R1.x, R1, texture[2], 2D;
MAD R0.w, R3.x, c[0].x, c[0].y;
MAD R0.y, R4.x, c[0].x, c[0];
MAD R0.z, R2.x, c[0].x, c[0].y;
MAD R0.x, R0, c[0], c[0].y;
RCP R1.y, R0.w;
RCP R0.y, R0.y;
RCP R0.z, R0.z;
RCP R0.x, R0.x;
MIN R0.x, R1.y, R0;
MIN R0.w, R1.y, R0.y;
MIN R0.z, R0, R1.y;
MAD R0.y, R0.z, c[2].x, c[2];
MAD R0.z, R0.w, c[2].x, c[2].y;
ABS R0.y, R0;
ADD_SAT R0.w, R0.y, c[2].z;
MAD R0.y, R1.x, c[0].x, c[0];
ABS R0.z, R0;
RCP R0.y, R0.y;
MIN R0.y, R1, R0;
MAD R0.y, R0, c[2].x, c[2];
MAD R0.x, R0, c[2], c[2].y;
ABS R1.x, R0.y;
ABS R0.y, R0.x;
ADD_SAT R0.z, R0, c[2];
ADD_SAT R0.x, R1, c[2].z;
ADD_SAT R0.y, R0, c[2].z;
DP4 R0.x, R0, c[3].x;
MAD result.color.xyz, R5, c[3].w, R0.x;
END
# 42 instructions, 6 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
Vector 1 [_GlobalDepthTexture_TexelSize]
Vector 2 [_DOFParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Noise] 2D
SetTexture 2 [_GlobalDepthTexture] 2D
"ps_2_0
; 40 ALU, 7 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c3, -1.00000000, 1.00000000, 0.12500000, 0.50000000
dcl t0
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mul r2.xy, r0, c2.w
mov r0.y, r2.x
mov r0.x, r2.y
mul r0.xy, r0, c3
mad r3.xy, r0, c1, t0
mad r1.xy, -r0, c1, t0
mad r0.xy, -r2, c1, t0
mad r2.xy, r2, c1, t0
texld r5, r3, s2
texld r4, r2, s2
texld r2, r0, s2
texld r0, t0, s2
texld r1, r1, s2
texld r3, t0, s0
mad r1.x, r1, c0, c0.y
mad r0.x, r0, c0, c0.y
mad r2.x, r2, c0, c0.y
rcp r0.x, r0.x
rcp r1.x, r1.x
min r1.x, r1, r0
rcp r2.x, r2.x
min r2.x, r0, r2
mad r1.x, r1, c2, c2.y
abs r1.x, r1
mad r2.x, r2, c2, c2.y
abs r2.x, r2
add_sat r0.w, r1.x, c2.z
mad r1.x, r4, c0, c0.y
rcp r1.x, r1.x
min r1.x, r0, r1
mad r1.x, r1, c2, c2.y
add_sat r0.z, r2.x, c2
mad r2.x, r5, c0, c0.y
rcp r2.x, r2.x
min r0.x, r0, r2
mad r0.x, r0, c2, c2.y
abs r2.x, r0
abs r1.x, r1
add_sat r0.y, r2.x, c2.z
add_sat r0.x, r1, c2.z
dp4_pp r0.x, r0, c3.z
mad_pp r0.xyz, r3, c3.w, r0.x
mov_pp r0.w, r3
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}