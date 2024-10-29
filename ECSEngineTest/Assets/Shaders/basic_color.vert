#version 420

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

layout (std140, binding = 0) uniform CameraMatrices
{
	mat4 viewMat;
	mat4 projectMat;
};

uniform mat4 uModelMat;

void main()
{
	gl_Position = projectMat * viewMat * uModelMat * vec4(aPos, 1.0f);
}