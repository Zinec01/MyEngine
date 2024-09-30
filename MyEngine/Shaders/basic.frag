//#version 330 core
#version 460

in vec2 uv;

out vec4 FragColor;

uniform sampler2D uTex;

void main()
{
	FragColor = texture(uTex, uv);
}
