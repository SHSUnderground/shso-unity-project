Shader "Hidden/Depth Of Field" {
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
Vector 2 [_MainTex_TexelSize]
Vector 3 [_DOFParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Noise] 2D
SetTexture 2 [_GlobalDepthTexture] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 59 ALU, 11 TEX
PARAM c[5] = { program.local[0..3],
		{ 4, -1, 1, 0.25 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEMP R8;
TEMP R9;
TEX R0.xy, fragment.texcoord[0].zwzw, texture[1], 2D;
TEX R5, fragment.texcoord[0], texture[0], 2D;
MUL R3.xy, R0, c[3].w;
MUL R3.zw, R3.xyyx, c[4].xyyz;
MAD R0.zw, -R3.xyxy, c[1].xyxy, fragment.texcoord[0].xyxy;
MAD R1.xy, R3.zwzw, c[1], fragment.texcoord[0];
MAD R0.xy, R3, c[1], fragment.texcoord[0];
MAD R2.xy, R3, c[2], fragment.texcoord[0];
MAD R2.zw, R3, c[2].xyxy, fragment.texcoord[0].xyxy;
MAD R1.zw, -R3, c[1].xyxy, fragment.texcoord[0].xyxy;
MAD R3.xy, -R3, c[2], fragment.texcoord[0];
MAD R3.zw, -R3, c[2].xyxy, fragment.texcoord[0].xyxy;
MOV result.color.w, R5;
TEX R8.xyz, R3, texture[0], 2D;
TEX R6.xyz, R2, texture[0], 2D;
TEX R1.x, R1, texture[2], 2D;
TEX R0.x, R0, texture[2], 2D;
TEX R2.x, R1.zwzw, texture[2], 2D;
TEX R4.x, R0.zwzw, texture[2], 2D;
TEX R9.xyz, R3.zwzw, texture[0], 2D;
TEX R7.xyz, R2.zwzw, texture[0], 2D;
TEX R3.x, fragment.texcoord[0], texture[2], 2D;
MAD R0.y, R4.x, c[0].x, c[0];
RCP R0.z, R0.y;
MAD R0.y, R3.x, c[0].x, c[0];
RCP R0.y, R0.y;
MAD R0.w, R2.x, c[0].x, c[0].y;
RCP R0.w, R0.w;
MIN R0.w, R0, R0.y;
MIN R1.y, R0, R0.z;
MAD R0.z, R0.w, c[3].x, c[3].y;
MAD R0.w, R1.y, c[3].x, c[3].y;
ABS R0.z, R0;
ABS R0.w, R0;
ADD_SAT R1.y, R0.z, c[3].z;
ADD_SAT R1.z, R0.w, c[3];
MOV R0.z, R1;
MAD R1.x, R1, c[0], c[0].y;
MAD R1.w, R0.x, c[0].x, c[0].y;
RCP R0.x, R1.x;
MIN R0.x, R0.y, R0;
RCP R1.x, R1.w;
MIN R0.y, R0, R1.x;
MAD R0.x, R0, c[3], c[3].y;
ABS R0.x, R0;
MAD R0.y, R0, c[3].x, c[3];
ADD_SAT R1.x, R0, c[3].z;
ABS R0.y, R0;
ADD_SAT R0.x, R0.y, c[3].z;
MOV R0.y, R1.x;
MOV R0.w, R1.y;
DP4 R0.w, R0, c[4].z;
MUL R0.xyz, R0.x, R6;
ADD R0.w, -R0, c[4].x;
MAD R0.xyz, R5, R0.w, R0;
MAD R0.xyz, R7, R1.x, R0;
MAD R0.xyz, R8, R1.z, R0;
MAD R0.xyz, R9, R1.y, R0;
MUL result.color.xyz, R0, c[4].w;
END
# 59 instructions, 10 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
Vector 1 [_GlobalDepthTexture_TexelSize]
Vector 2 [_MainTex_TexelSize]
Vector 3 [_DOFParams]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Noise] 2D
SetTexture 2 [_GlobalDepthTexture] 2D
"ps_2_0
; 53 ALU, 11 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
def c4, -1.00000000, 1.00000000, 4.00000000, 0.25000000
dcl t0
texld r8, t0, s0
texld r9, t0, s2
mad r9.x, r9, c0, c0.y
rcp r9.x, r9.x
mov r0.y, t0.w
mov r0.x, t0.z
texld r0, r0, s1
mul r0.xy, r0, c3.w
mad r5.xy, -r0, c1, t0
mad r6.xy, r0, c1, t0
mad r7.xy, r0, c2, t0
mov r1.y, r0.x
mov r1.x, r0.y
mul r1.xy, r1, c4
mad r2.xy, -r1, c1, t0
mad r3.xy, r1, c1, t0
mad r4.xy, r1, c2, t0
mad r1.xy, -r1, c2, t0
mad r0.xy, -r0, c2, t0
texld r3, r3, s2
texld r0, r0, s0
texld r1, r1, s0
texld r4, r4, s0
texld r2, r2, s2
texld r7, r7, s0
texld r5, r5, s2
texld r6, r6, s2
mad r2.x, r2, c0, c0.y
rcp r2.x, r2.x
min r2.x, r2, r9
mad r5.x, r5, c0, c0.y
rcp r5.x, r5.x
min r5.x, r9, r5
mad r2.x, r2, c3, c3.y
mad r5.x, r5, c3, c3.y
abs r2.x, r2
add_sat r2.x, r2, c3.z
abs r5.x, r5
add_sat r5.x, r5, c3.z
mad r3.x, r3, c0, c0.y
rcp r3.x, r3.x
min r3.x, r9, r3
mad r3.x, r3, c3, c3.y
abs r3.x, r3
mov_pp r6.z, r5.x
mov_pp r6.w, r2.x
add_sat r3.x, r3, c3.z
mad r6.x, r6, c0, c0.y
rcp r6.x, r6.x
min r6.x, r9, r6
mad r6.x, r6, c3, c3.y
abs r6.x, r6
mov_pp r6.y, r3.x
add_sat r6.x, r6, c3.z
dp4_pp r9.x, r6, c4.y
add_pp r9.x, -r9, c4.z
mul_pp r6.xyz, r6.x, r7
mad_pp r6.xyz, r8, r9.x, r6
mad_pp r3.xyz, r4, r3.x, r6
mad_pp r0.xyz, r0, r5.x, r3
mad_pp r0.xyz, r1, r2.x, r0
mul_pp r0.xyz, r0, c4.w
mov_pp r0.w, r8
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}