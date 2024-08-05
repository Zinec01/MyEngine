//#version 330 core
#version 460

layout (location = 0) in vec3 vPos;

uniform mat4 modelMat;

out vec3 fPos;

void main()
{
	gl_Position = modelMat * vec4(vPos, 1.0f);
	fPos = abs(vec3(gl_Position));
}