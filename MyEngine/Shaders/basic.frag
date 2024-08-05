//#version 330 core
#version 460

in vec3 fPos;
in vec2 fUv;

out vec4 FragColor;

uniform sampler2D f_Tex;

void main()
{
	FragColor = texture(f_Tex, fUv);
}
