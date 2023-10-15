Shader "Hidden/AO Fast Projection" {
SubShader { 
 Tags { "QUEUE"="Geometry+900" "RenderType"="Opaque" }
 Pass {
  Tags { "QUEUE"="Geometry+900" "RenderType"="Opaque" }
  ZTest GEqual
  ZWrite Off
  Cull Front
  Fog { Mode Off }
  Blend DstColor Zero
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Vector 5 [_ProjectionParams]
Vector 6 [_WorldSpaceCameraPos]
Vector 7 [_ShadowTex_TexelSize]
Vector 8 [_AORadius]
Float 9 [_TileSize]
Float 10 [_BlurGutterSize]
"!!ARBvp1.0
# 29 ALU
PARAM c[12] = { { -2, 1, 0.5, 2 },
		state.matrix.mvp,
		program.local[5..10],
		{ 0.5, -0.5 } };
TEMP R0;
TEMP R1;
TEMP R2;
DP4 R1.w, vertex.position, c[4];
DP4 R2.x, vertex.position, c[3];
MOV R0.w, R1;
DP4 R0.x, vertex.position, c[1];
DP4 R0.y, vertex.position, c[2];
MUL R1.xyz, R0.xyww, c[0].z;
MUL R1.y, R1, c[5].x;
MOV R0.z, R2.x;
ADD result.texcoord[2].xy, R1, R1.z;
MOV R1.xy, c[7];
MOV result.position, R0;
MUL R0.xy, R1, c[9].x;
MAD R0.zw, vertex.texcoord[1].xyxy, R0.xyxy, c[7].xyxy;
MOV result.texcoord[1], R0;
RCP R1.x, c[9].x;
MUL R1.x, R1, c[10];
MUL R0.w, R1.x, c[0].x;
ADD R0.z, R0.w, c[0].y;
MUL R0.w, R0, c[0].z;
RCP R0.y, vertex.texcoord[0].y;
RCP R0.x, vertex.texcoord[0].x;
MUL R0.xy, R0.z, R0;
ADD result.texcoord[3].xyz, -vertex.position.xzyw, c[6].xzyw;
MUL result.texcoord[4].xy, R0, c[11];
MAD result.texcoord[0].xy, vertex.color.xzzw, R0.z, -R0.w;
MAD result.texcoord[0].z, vertex.color.y, c[0].w, -c[0].y;
ADD result.texcoord[2].z, R2.x, c[5].y;
MOV result.texcoord[2].w, R1;
MOV result.texcoord[4].z, c[8];
END
# 29 instructions, 3 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "color" Color
Bind "texcoord" TexCoord0
Bind "texcoord1" TexCoord1
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ProjectionParams]
Vector 5 [_ScreenParams]
Vector 6 [_WorldSpaceCameraPos]
Vector 7 [_ShadowTex_TexelSize]
Vector 8 [_AORadius]
Float 9 [_TileSize]
Float 10 [_BlurGutterSize]
"vs_2_0
; 29 ALU
def c11, -2.00000000, 1.00000000, 0.50000000, -0.50000000
def c12, 2.00000000, -1.00000000, 0, 0
dcl_position0 v0
dcl_texcoord0 v1
dcl_texcoord1 v2
dcl_color0 v3
dp4 r1.w, v0, c3
dp4 r2.x, v0, c2
mov r0.w, r1
dp4 r0.x, v0, c0
dp4 r0.y, v0, c1
mul r1.xyz, r0.xyww, c11.z
mov r0.z, r2.x
mul r1.y, r1, c4.x
mad oT2.xy, r1.z, c5.zwzw, r1
mov r1.x, c9
mov oPos, r0
mul r0.xy, c7, r1.x
mad r0.zw, v2.xyxy, r0.xyxy, c7.xyxy
mov oT1, r0
rcp r1.x, c9.x
mul r1.x, r1, c10
mul r0.w, r1.x, c11.x
add r0.z, r0.w, c11.y
mul r0.w, r0, c11.z
rcp r0.y, v1.y
rcp r0.x, v1.x
mul r0.xy, r0.z, r0
add oT3.xyz, -v0.xzyw, c6.xzyw
mul oT4.xy, r0, c11.zwzw
mad oT0.xy, v3.xzzw, r0.z, -r0.w
mad oT0.z, v3.y, c12.x, c12.y
add oT2.z, r2.x, c4.y
mov oT2.w, r1
mov oT4.z, c8
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_ZBufferParams]
SetTexture 0 [_GlobalDepthTexture] 2D
SetTexture 1 [_ShadowTex] 2D
"!!ARBfp1.0
# 30 ALU, 2 TEX
PARAM c[2] = { program.local[0],
		{ 1, 0 } };
TEMP R0;
TEMP R1;
RCP R0.x, fragment.texcoord[2].w;
MUL R0.xy, fragment.texcoord[2], R0.x;
TEX R0.x, R0, texture[0], 2D;
MAD R0.z, R0.x, c[0], c[0].w;
DP3 R0.y, fragment.texcoord[3], fragment.texcoord[3];
RCP R0.w, R0.z;
RSQ R0.x, R0.y;
MUL R0.xyz, R0.x, fragment.texcoord[3];
MUL R0.xyz, R0, fragment.texcoord[4];
ADD R0.w, fragment.texcoord[2].z, -R0;
MAD R1.xyz, R0, R0.w, fragment.texcoord[0];
ADD R0.y, -R1, c[1].x;
MOV R0.w, R0.y;
MOV R0.z, R1.x;
MAD R0.zw, R0, fragment.texcoord[1].xyxy, fragment.texcoord[1];
TEX R0.x, R0.zwzw, texture[1], 2D;
MOV_SAT R0.w, R0.y;
MOV_SAT R0.z, R1.x;
ADD R0.w, R0.y, -R0;
ADD R0.y, R1.x, -R0.z;
ABS R0.z, R0.w;
ABS R0.y, R0;
CMP R0.z, -R0, c[1].y, c[1].x;
CMP R0.y, -R0, c[1], c[1].x;
MUL R0.y, R0, R0.z;
CMP R0.z, -R0.y, c[1].y, c[1].x;
ABS R0.y, R1.z;
ADD R0.y, R0, R0.z;
ADD R0.x, -R0, c[1];
ADD result.color, R0.x, R0.y;
END
# 30 instructions, 2 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_ZBufferParams]
SetTexture 0 [_GlobalDepthTexture] 2D
SetTexture 1 [_ShadowTex] 2D
"ps_2_0
; 32 ALU, 2 TEX
dcl_2d s0
dcl_2d s1
def c1, 1.00000000, 0.00000000, 0, 0
dcl t0.xyz
dcl t1
dcl t2
dcl t3.xyz
dcl t4.xyz
rcp r0.x, t2.w
mul r0.xy, t2, r0.x
dp3 r1.x, t3, t3
rsq r1.x, r1.x
mul r1.xyz, r1.x, t3
mul r1.xyz, r1, t4
texld r0, r0, s0
mad r0.x, r0, c0.z, c0.w
rcp r0.x, r0.x
add r0.x, t2.z, -r0
mad r4.xyz, r1, r0.x, t0
add r0.x, -r4.y, c1
mov r2.y, r0.x
mov r2.x, r4
mov r1.y, t1.w
mov r1.x, t1.z
mad r1.xy, r2, t1, r1
mov_sat r2.x, r0
add r0.x, r0, -r2
abs r0.x, r0
cmp r0.x, -r0, c1, c1.y
texld r3, r1, s1
mov_sat r1.x, r4
add r1.x, r4, -r1
abs r1.x, r1
cmp r1.x, -r1, c1, c1.y
mul_pp r0.x, r1, r0
abs r1.x, r4.z
cmp r0.x, -r0, c1, c1.y
add r0.x, r1, r0
add r1.x, -r3, c1
add_pp r0.x, r1, r0
mov_pp r0, r0.x
mov_pp oC0, r0
"
}
}
 }
}
}