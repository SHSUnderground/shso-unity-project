Shader "Chad/Chad Test" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _LightMap ("Lightmap (RGB) AO (A)", 2D) = "black" {}
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Bind "tangent" ATTR14
Vector 5 [_MainTex_ST]
Vector 6 [_LightMap_ST]
"!!ARBvp1.0
# 8 ALU
PARAM c[7] = { { 0.1 },
		state.matrix.mvp,
		program.local[5..6] };
MOV result.color, vertex.color;
MAD result.texcoord[0].xy, vertex.texcoord[0], c[5], c[5].zwzw;
MAD result.texcoord[1].xy, vertex.texcoord[1], c[6], c[6].zwzw;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
SGE result.texcoord[2].x, vertex.attrib[14], c[0];
END
# 8 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Bind "tangent" TexCoord2
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_ST]
Vector 5 [_LightMap_ST]
"vs_2_0
; 8 ALU
def c6, 0.10000000, 0, 0, 0
dcl_position0 v0
dcl_tangent0 v1
dcl_color0 v2
dcl_texcoord0 v3
dcl_texcoord1 v4
mov oD0, v2
mad oT0.xy, v3, c4, c4.zwzw
mad oT1.xy, v4, c5, c5.zwzw
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
sge oT2.x, v1, c6
"
}
}
Program "fp" {
SubProgram "opengl " {
SetTexture 0 [_MainTex] 2D
"!!ARBfp1.0
OPTION ARB_fog_exp2;
OPTION ARB_precision_hint_fastest;
# 3 ALU, 1 TEX
TEMP R0;
TEMP R1;
TEX R0, fragment.texcoord[0], texture[0], 2D;
ADD R1, fragment.color.primary, -R0;
MAD result.color, fragment.texcoord[2].x, R1, R0;
END
# 3 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
SetTexture 0 [_MainTex] 2D
"ps_2_0
; 3 ALU, 1 TEX
dcl_2d s0
dcl v0
dcl t0.xy
dcl t2.x
texld r0, t0, s0
add_pp r1, v0, -r0
mad_pp r0, t2.x, r1, r0
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
} 