Shader "Particles/Alpha Blended Soft" {
Properties {
 _MainTex ("Particle Texture", 2D) = "white" {}
 _FadeDistance ("Fade Distance", Float) = 0.5
}
Fallback "Particles/Alpha Blended"
}