//#version 330 core
#version 460

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aUv;

uniform mat4 uModelMat;
uniform mat4 uViewMat;
uniform mat4 uProjectMat;

out vec3 fragPos;
out vec3 normal;
out vec2 uv;

void main()
{
	fragPos = vec3(uModelMat * vec4(aPos, 1.0f));

	gl_Position = uProjectMat * uViewMat * vec4(fragPos, 1.0f);

	normal = mat3(transpose(inverse(uModelMat))) * aNormal;
	uv = aUv;
}