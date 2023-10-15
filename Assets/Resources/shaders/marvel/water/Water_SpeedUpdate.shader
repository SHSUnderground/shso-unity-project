Shader "Marvel/FX/Water/Speed Update" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _HeightTex ("Height (Private)", 2D) = "white" {}
 _SmoothTex ("Smoothed (Private)", 2D) = "white" {}
 _ImpulseTex ("Impulse (Private)", 2D) = "white" {}
 _AverageTex ("Average (Private)", 2D) = "white" {}
 _Dampening ("Dampening (Private)", Float) = 0.99
 _DeltaTime ("Delta Time (Private)", Float) = 1
 _GravityPressure ("Gravity Pressure (Private)", Float) = 0.2
 _Friction ("Friction (Private)", Float) = 0.99
 _Persistence ("Persistence (Private)", Float) = 30
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
Float 0 [_Dampening]
Float 1 [_GravityPressure]
Float 2 [_Friction]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ImpulseTex] 2D
SetTexture 2 [_AverageTex] 2D
SetTexture 3 [_HeightTex] 2D
"!!ARBfp1.0
OPTION ARB_precision_hint_fastest;
# 15 ALU, 4 TEX
PARAM c[4] = { program.local[0..2],
		{ 2, 1, 0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
TEX R0.x, fragment.texcoord[0], texture[0], 2D;
TEX R3.x, fragment.texcoord[0], texture[1], 2D;
TEX R2.x, fragment.texcoord[0], texture[3], 2D;
TEX R1.x, fragment.texcoord[0], texture[2], 2D;
MAD R0.z, R2.x, c[3].x, -c[3].y;
MAD_SAT R0.y, R3.x, c[3].x, -c[3];
ADD R0.y, R0.z, R0;
MAD R0.z, R1.x, c[3].x, -c[3].y;
ADD R0.y, R0.z, -R0;
MAD R0.w, R0.x, c[3].x, -c[3].y;
MUL R0.z, R0, c[1].x;
ADD R0.x, R0.y, -R0.z;
MAD R0.x, R0, c[0], R0.w;
MUL R0.x, R0, c[2];
MAD result.color, R0.x, c[3].z, c[3].z;
END
# 15 instructions, 4 R-regs
"
}
SubProgram "d3d9 " {
Float 0 [_Dampening]
Float 1 [_GravityPressure]
Float 2 [_Friction]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_ImpulseTex] 2D
SetTexture 2 [_AverageTex] 2D
SetTexture 3 [_HeightTex] 2D
"ps_2_0
; 13 ALU, 4 TEX
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
def c3, 2.00000000, -1.00000000, 0.50000000, 0
dcl t0.xy
texld r0, t0, s0
texld r2, t0, s3
texld r3, t0, s2
texld r1, t0, s1
mad r2.x, r2, c3, c3.y
mad_sat r1.x, r1, c3, c3.y
add_pp r1.x, r2, r1
mad r2.x, r3, c3, c3.y
add_pp r1.x, r2, -r1
mul r2.x, r2, c1
mad r0.x, r0, c3, c3.y
add_pp r1.x, r1, -r2
mad_pp r0.x, r1, c0, r0
mul r0.x, r0, c2
mad_pp r0.x, r0, c3.z, c3.z
mov_pp r0, r0.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}