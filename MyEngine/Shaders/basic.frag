//#version 330 core
#version 460

in vec3 fPos;

out vec4 FragColor;

void main()
{
	FragColor = vec4(fPos, 1.0f);
}
