//#version 330 core
#version 460

in vec3 fPos;
in vec2 fUv;

out vec4 res_FragColor;

uniform sampler2D f_uTex;

void main()
{
	res_FragColor = texture(f_uTex, fUv);
}
