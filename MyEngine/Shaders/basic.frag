//#version 330 core
#version 460

in vec3 vfPos;
in vec2 vfUv;

out vec4 foFragColor;

uniform sampler2D fuTex;

void main()
{
	foFragColor = texture(fuTex, vfUv);
}
