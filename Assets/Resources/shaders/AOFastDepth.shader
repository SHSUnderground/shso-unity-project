Shader "Hidden/AO Fast Depth" {
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Cull Front
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Matrix 5 [_Object2World]
Vector 9 [_ProjectionPlane]
"!!ARBvp1.0
# 11 ALU
PARAM c[10] = { program.local[0],
		state.matrix.mvp,
		program.local[5..9] };
TEMP R0;
DP4 R0.x, vertex.position, c[5];
DP4 R0.y, vertex.position, c[6];
DP4 R0.w, vertex.position, c[8];
DP4 R0.z, vertex.position, c[7];
DP4 result.texcoord[0].x, R0, c[9];
DP4 R0.x, vertex.position, c[4];
DP4 R0.y, vertex.position, c[3];
ADD result.position.z, -R0.x, -R0.y;
MOV result.position.w, R0.x;
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 11 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [_Object2World]
Vector 8 [_ProjectionPlane]
"vs_2_0
; 11 ALU
dcl_position0 v0
dp4 r0.x, v0, c4
dp4 r0.y, v0, c5
dp4 r0.w, v0, c7
dp4 r0.z, v0, c6
dp4 oT0.x, r0, c8
dp4 r0.x, v0, c3
dp4 r0.y, v0, c2
add oPos.z, r0.x, -r0.y
mov oPos.w, r0.x
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_AORadius]
"!!ARBfp1.0
# 4 ALU, 0 TEX
PARAM c[2] = { program.local[0],
		{ 1 } };
TEMP R0;
ABS R0.x, fragment.texcoord[0];
MAD_SAT R0.x, R0, c[0].z, c[0].y;
ADD result.color.xyz, -R0.x, c[1].x;
MOV result.color.w, c[1].x;
END
# 4 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_AORadius]
"ps_2_0
; 5 ALU
def c1, 1.00000000, 0, 0, 0
dcl t0.x
abs r0.x, t0
mad_sat r0.x, r0, c0.z, c0.y
add r0.xyz, -r0.x, c1.x
mov_pp r0.w, c1.x
mov_pp oC0, r0
"
}
}
 }
}
Fallback Off
}