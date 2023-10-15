Shader "Hidden/Render Normal" {
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "opengl " {
Bind "vertex" Vertex
Bind "normal" Normal
"!!ARBvp1.0
# 7 ALU
PARAM c[9] = { program.local[0],
		state.matrix.mvp,
		state.matrix.modelview[0].invtrans };
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
DP3 result.texcoord[0].z, vertex.normal, c[7];
DP3 result.texcoord[0].y, vertex.normal, c[6];
DP3 result.texcoord[0].x, vertex.normal, c[5];
END
# 7 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_invtrans_modelview0]
"vs_2_0
; 7 ALU
dcl_position0 v0
dcl_normal0 v1
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
dp3 oT0.z, v1, c6
dp3 oT0.y, v1, c5
dp3 oT0.x, v1, c4
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!ARBfp1.0
# 5 ALU, 0 TEX
PARAM c[1] = { { 0, 0.5 } };
TEMP R0;
DP3 R0.x, fragment.texcoord[0], fragment.texcoord[0];
RSQ R0.x, R0.x;
MUL R0.xyz, R0.x, fragment.texcoord[0];
MAD result.color.xyz, R0, c[0].y, c[0].y;
MOV result.color.w, c[0].x;
END
# 5 instructions, 1 R-regs
"
}
SubProgram "d3d9 " {
"ps_2_0
; 6 ALU
def c0, 0.50000000, 0.00000000, 0, 0
dcl t0.xyz
dp3 r0.x, t0, t0
rsq r0.x, r0.x
mul r0.xyz, r0.x, t0
mad r0.xyz, r0, c0.x, c0.x
mov r0.w, c0.y
mov oC0, r0
"
}
}
 }
}
Fallback Off
}