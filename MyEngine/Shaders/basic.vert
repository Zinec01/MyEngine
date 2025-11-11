//#version 330 core
#version 460

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aUv;

uniform mat4 uModelMat;
uniform mat4 uViewMat;
uniform mat4 uProjectMat;

out vec2 uv;

void main()
{
	gl_Position = uProjectMat * uViewMat * uModelMat * vec4(aPos, 1.0f);
	uv = aUv;
}