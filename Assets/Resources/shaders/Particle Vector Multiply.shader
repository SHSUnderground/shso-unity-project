Shader "Particles/Vector Multiply" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" {}
 _Gradient ("Gradient", 2D) = "white" {}
 _FlipBookData ("Flip Book Data (Private)", Vector) = (0,0,0,0)
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZTest Less
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend DstColor Zero
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Vector 5 [_MainTex_ST]
Vector 6 [_FlipBookData]
"!!ARBvp1.0
# 8 ALU
PARAM c[7] = { program.local[0],
		state.matrix.mvp,
		program.local[5..6] };
TEMP R0;
MUL R0.xy, vertex.texcoord[0], c[5];
MOV result.texcoord[1], vertex.color;
ADD result.texcoord[0].zw, R0.xyxy, c[6].xyxy;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
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
Bind "color" Color
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_ST]
Vector 5 [_FlipBookData]
"vs_2_0
; 8 ALU
dcl_position0 v0
dcl_texcoord0 v1
dcl_color0 v2
mul r0.xy, v1, c4
mov oT1, v2
add oT0.zw, r0.xyxy, c5.xyxy
mad oT0.xy, v1, c4, c4.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_FlipBookData]
Vector 1 [_TintColor]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Gradient] 2D
"!!ARBfp1.0
# 9 ALU, 3 TEX
PARAM c[3] = { program.local[0..1],
		{ 0.5, 2 } };
TEMP R0;
TEMP R1;
TEX R0.w, fragment.texcoord[0], texture[0], 2D;
TEX R1.w, fragment.texcoord[0].zwzw, texture[0], 2D;
ADD R0.x, R1.w, -R0.w;
MOV R0.y, c[2].x;
MAD R0.x, R0, c[0].z, R0.w;
TEX R0, R0, texture[1], 2D;
MUL R0, R0, c[1];
MUL R0, R0, fragment.texcoord[1];
MUL result.color, R0, c[2].y;
END
# 9 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_FlipBookData]
Vector 1 [_TintColor]
SetTexture 0 [_MainTex] 2D
SetTexture 1 [_Gradient] 2D
"ps_2_0
; 10 ALU, 3 TEX
dcl_2d s0
dcl_2d s1
def c2, 0.50000000, 2.00000000, 0, 0
dcl t0
dcl t1
mov r0.y, t0.w
mov r0.x, t0.z
mov r1.xy, r0
texld r0, t0, s0
texld r1, r1, s0
add r0.x, r1.w, -r0.w
mov r0.y, c2.x
mad r0.x, r0, c0.z, r0.w
texld r0, r0, s1
mul r0, r0, c1
mul r0, r0, t1
mul r0, r0, c2.y
mov_pp oC0, r0
"
}
}
 }
}
}