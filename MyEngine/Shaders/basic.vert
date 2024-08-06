//#version 330 core
#version 460

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aUv;

uniform mat4 v_uModelMat;

out vec3 fPos;
out vec2 fUv;

void main()
{
	gl_Position = v_uModelMat * vec4(aPos, 1.0f);
	fPos = vec3(clamp(gl_Position, 0, 1));
	fUv = aUv;
}