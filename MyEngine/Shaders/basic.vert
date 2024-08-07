//#version 330 core
#version 460

layout (location = 0) in vec3 vaPos;
layout (location = 1) in vec2 vaUv;

uniform mat4 vuModelMat;
uniform mat4 vuViewMat;
uniform mat4 vuProjectMat;

out vec3 vfPos;
out vec2 vfUv;

void main()
{
	gl_Position = vuProjectMat * vuViewMat * vuModelMat * vec4(vaPos, 1.0f);
	vfPos = vec3(clamp(gl_Position, 0, 1));
	vfUv = vaUv;
}