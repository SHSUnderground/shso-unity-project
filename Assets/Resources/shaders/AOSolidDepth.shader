Shader "Hidden/AO Solid Depth" {
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Vector 5 [_AORadius]
"!!ARBvp1.0
# 7 ALU
PARAM c[6] = { { 1 },
		state.matrix.mvp,
		program.local[5] };
TEMP R0;
MOV R0.x, c[0];
ADD result.texcoord[0].xyz, R0.x, -c[5].y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
MOV result.texcoord[0].w, c[0].x;
END
# 7 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_AORadius]
"vs_2_0
; 7 ALU
def c5, 1.00000000, 0, 0, 0
dcl_position0 v0
mov r0.x, c4.y
add oT0.xyz, c5.x, -r0.x
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
mov oT0.w, c5.x
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
# 1 ALU, 0 TEX
MOV result.color, fragment.texcoord[0];
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 1 ALU
dcl t0
mov_pp oC0, t0
"
}
}
 }
}
Fallback Off
}