Shader "Marvel/Base/SelectedObject" {
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Name "BASE"
  Tags { "LIGHTMODE"="Always" "QUEUE"="Transparent" }
  ZWrite Off
  Cull Off
  Fog {
   Color (0,0,0,0)
  }
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0.01
  ColorMask RGB
Program "vp" {
SubProgram "opengl " {
Bind "vertex" ATTR0
"!!ARBvp1.0
# 4 ALU
PARAM c[5] = { program.local[0],
		state.matrix.mvp };
DP4 result.position.w, vertex.attrib[0], c[4];
DP4 result.position.z, vertex.attrib[0], c[3];
DP4 result.position.y, vertex.attrib[0], c[2];
DP4 result.position.x, vertex.attrib[0], c[1];
END
# 4 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Bind "vertex" Vertex
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 4 ALU
dcl_position v0
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}
}
Program "fp" {
SubProgram "opengl " {
Vector 0 [_SelectedColor]
"!!ARBfp1.0
# 1 ALU, 0 TEX
PARAM c[1] = { program.local[0] };
MOV result.color, c[0];
END
# 1 instructions, 0 R-regs
"
}
SubProgram "d3d9 " {
Vector 0 [_SelectedColor]
"ps_2_0
; 1 ALU
mov_pp oC0, c0
"
}
}
 }
}
}