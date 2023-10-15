Shader "Marvel/FX/Water/Difference" {
Properties {
 _MainTex ("Current intersection", 2D) = "white" {}
 _OldTex ("Last intersection", 2D) = "white" {}
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
# 10 ALU
PARAM c[6] = { { 0, 1, -2, -1 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
DP4 R0.y, vertex.position, c[2];
DP4 R0.x, vertex.position, c[1];
SLT R0.y, R0, c[0].x;
SLT R0.x, R0, c[0];
MOV R0.zw, c[0].xyyw;
MAD R0.y, R0, c[0].z, c[0];
MAD R0.x, R0, c[0].z, c[0].y;
MAD result.position.xy, R0.wzzw, c[5], R0;
MOV result.position.zw, c[0].xyxy;
MOV result.texcoord[0].xy, vertex.texcoord[0];
END
# 10 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
"vs_2_0
; 16 ALU
def c5, 0.00000000, 1.00000000, -1.00000000, 0
dcl_position0 v0
dcl_texcoord0 v1
dp4 r0.y, v0, c1
dp4 r0.x, v0, c0
slt r0.y, r0, c5.x
slt r0.x, r0, c5
max r0.y, -r0, r0
max r0.x, -r0, r0
slt r0.y, c5.x, r0
slt r0.x, c5, r0
add r0.y, -r0, -r0
add r0.x, -r0, -r0
mov r0.zw, c4.xyxy
add r0.y, r0, c5
add r0.x, r0, c5.y
mad oPos.xy, c5.zyzw, r0.zwzw, r0
mov oPos.zw, c5.xyxy
mov oT0.xy, v1
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_OldTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 4 ALU, 2 TEX
PARAM c[1] = { { 0.5 } };
TEMP R0;
TEMP R1;
TEX R1.x, fragment.texcoord[0], texture[1], 2D;
TEX R0.x, fragment.texcoord[0], texture[0], 2D;
ADD R0.x, R0, -R1;
MAD result.color, R0.x, c[0].x, c[0].x;
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_OldTex] 2D
"ps_2_0
; 4 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c0, 0.50000000, 0, 0, 0
dcl t0.xy
texld r0, t0, s1
texld r1, t0, s0
add_pp r0.x, r1, -r0
mad_pp r0.x, r0, c0, c0
mov_pp r0, r0.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}