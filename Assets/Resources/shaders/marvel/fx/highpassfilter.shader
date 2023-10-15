Shader "HighPassFilter" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Sharpness ("Treshold", Float) = 8
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
Vector 5 [_MainTex_TexelSize]
"!!ARBvp1.0
# 13 ALU
PARAM c[6] = { { 1, -1, 2, -2 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
TEMP R1;
MOV R0, c[0];
MAD R1, R0.yxyx, c[5].xyyx, vertex.texcoord[0].xyyx;
MOV result.texcoord[1].zw, R1;
MOV result.texcoord[2].xy, R1;
MAD result.texcoord[3], R0.zzwz, c[5].xyyx, vertex.texcoord[0].xyyx;
MAD result.texcoord[4], R0.wzww, c[5].xyyx, vertex.texcoord[0].xyyx;
MOV result.texcoord[0].xy, vertex.texcoord[0];
ADD result.texcoord[1].xy, vertex.texcoord[0], c[5];
ADD result.texcoord[2].zw, vertex.texcoord[0].xyyx, -c[5].xyyx;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 13 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 19 ALU
def c5, -1.00000000, 1.00000000, -2.00000000, 2.00000000
dcl_position0 v0
dcl_texcoord0 v1
mov r0.xy, c4
mad r1, c5.xyxy, r0.xyyx, v1.xyyx
mov r0.xy, c4
mad r0, c5.zwzw, r0.xyyx, v1.xyyx
mov oT3.zw, r0
mov oT4.xy, r0
mov r0.xy, c4
mov r0.zw, c4.xyxy
mov oT1.zw, r1
mov oT2.xy, r1
mov oT0.xy, v1
add oT1.xy, v1, c4
add oT2.zw, v1.xyyx, -c4.xyyx
mad oT3.xy, c5.w, r0, v1
mad oT4.zw, c5.z, r0.xywz, v1.xyyx
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Float 0 [_Sharpness]
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 19 ALU, 9 TEX
PARAM c[2] = { program.local[0],
		{ 0.21259999, 0.71520001, 0.0722 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEMP R4;
TEMP R5;
TEMP R6;
TEMP R7;
TEMP R8;
TEX R0, fragment.texcoord[0], texture[0], 2D;
TEX R1.xyz, fragment.texcoord[1], texture[0], 2D;
TEX R8.xyz, fragment.texcoord[4].wzzw, texture[0], 2D;
TEX R7.xyz, fragment.texcoord[4], texture[0], 2D;
TEX R6.xyz, fragment.texcoord[3].wzzw, texture[0], 2D;
TEX R5.xyz, fragment.texcoord[3], texture[0], 2D;
TEX R4.xyz, fragment.texcoord[2].wzzw, texture[0], 2D;
TEX R3.xyz, fragment.texcoord[2], texture[0], 2D;
TEX R2.xyz, fragment.texcoord[1].wzzw, texture[0], 2D;
MAD R0.xyz, R0, c[0].x, -R1;
ADD R0.xyz, R0, -R2;
ADD R0.xyz, R0, -R3;
ADD R0.xyz, R0, -R4;
ADD R0.xyz, R0, -R5;
ADD R0.xyz, R0, -R6;
ADD R0.xyz, R0, -R7;
ADD R0.xyz, R0, -R8;
DP3 result.color.xyz, R0, c[1];
MOV result.color.w, R0;
END
# 19 instructions, 9 R-regs
"
}
SubProgram "d3d9 " {
Float 0 [_Sharpness]
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 15 ALU, 9 TEX
dcl_2d s0
def c1, 0.21259999, 0.71520001, 0.07220000, 0
dcl t0.xy
dcl t1
dcl t2
dcl t3
dcl t4
texld r4, t4, s0
texld r6, t3, s0
texld r8, t2, s0
mov r1.xy, t2.wzyx
mov r2.xy, t3.wzyx
mov r0.xy, t1.wzyx
mov r3.xy, t4.wzyx
texld r5, r2, s0
texld r7, r1, s0
texld r0, r0, s0
texld r3, r3, s0
texld r1, t0, s0
texld r2, t1, s0
mad r1.xyz, r1, c0.x, -r2
add r0.xyz, r1, -r0
add r0.xyz, r0, -r8
add r0.xyz, r0, -r7
add r0.xyz, r0, -r6
add r0.xyz, r0, -r5
add r0.xyz, r0, -r4
add r0.xyz, r0, -r3
dp3 r0.xyz, r0, c1
mov r0.w, r1
mov oC0, r0
"
}
}
 }
}
Fallback Off
}