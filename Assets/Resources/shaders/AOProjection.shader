Shader "Marvel/FX/AO Projector" {
Properties {
 _ShadowTex ("Cookie", 2D) = "gray" { TexGen ObjectLinear }
 _FalloffTex ("FallOff", 2D) = "white" { TexGen ObjectLinear }
 _ProjectionPlane ("Projection Plane (Private)", Vector) = (0,0,0,0)
 _AORadius ("AO Radius (Private)", Vector) = (0,0,0,0)
}
SubShader { 
 Tags { "RenderType"="Transparent" }
 Pass {
  Tags { "RenderType"="Transparent" }
  ZWrite Off
  Fog {
   Color (1,1,1,1)
  }
  Blend DstColor Zero
  AlphaTest Greater 0
  ColorMask RGB
  Offset -1, -1
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Matrix 5 [_Projector]
Matrix 9 [_ProjectorClip]
Vector 13 [_AORadius]
"!!ARBvp1.0
# 13 ALU
PARAM c[14] = { program.local[0],
		state.matrix.mvp,
		program.local[5..13] };
TEMP R0;
DP4 R0.x, vertex.position, c[12];
RCP R0.y, R0.x;
DP4 R0.x, vertex.position, c[9];
MUL R0.x, R0, R0.y;
MAD result.texcoord[1].x, R0, c[13].z, c[13].w;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
DP4 result.texcoord[0].w, vertex.position, c[8];
DP4 result.texcoord[0].z, vertex.position, c[7];
DP4 result.texcoord[0].y, vertex.position, c[6];
DP4 result.texcoord[0].x, vertex.position, c[5];
END
# 13 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Projector]
Matrix 8 [_ProjectorClip]
Vector 12 [_AORadius]
"vs_2_0
; 13 ALU
dcl_position0 v0
dp4 r0.x, v0, c11
rcp r0.y, r0.x
dp4 r0.x, v0, c8
mul r0.x, r0, r0.y
mad oT1.x, r0, c12.z, c12.w
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
dp4 oT0.w, v0, c7
dp4 oT0.z, v0, c6
dp4 oT0.y, v0, c5
dp4 oT0.x, v0, c4
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_ShadowTex] 2D
"!!ARBfp1.0
# 4 ALU, 1 TEX
OPTION ARB_fragment_program_shadow;
TEMP R0;
TEMP R1;
TXP R0, fragment.texcoord[0], texture[0], SHADOW2D;
ABS R1.x, fragment.texcoord[1];
MOV result.color.w, R0;
ADD result.color.xyz, R0, R1.x;
END
# 4 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_ShadowTex] 2D
"ps_2_0
; 3 ALU, 1 TEX
dcl_2d s0
dcl t0
dcl t1.x
texldp r0, t0, s0
abs r1.x, t1
add_pp r0.xyz, r0, r1.x
mov_pp oC0, r0
"
}
}
 }
}
}