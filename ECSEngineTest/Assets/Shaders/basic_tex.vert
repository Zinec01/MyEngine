#version 420

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aUv;

layout (std140) uniform CameraMatrices {
	mat4 viewMat;
	mat4 projectMat;
};

uniform mat4 uModelMat;

out vec2 uv;

void main()
{
	gl_Position = projectMat * viewMat * uModelMat * vec4(aPos, 1.0f);

	uv = aUv;
}