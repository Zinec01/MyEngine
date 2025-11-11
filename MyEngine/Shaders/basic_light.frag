//#version 330 core
#version 460

in vec3 fragPos;
in vec3 normal;
in vec2 uv;

out vec4 FragColor;

uniform sampler2D uTex;
uniform vec3 cameraPos;

void main()
{
	vec3 lightPos = vec3(0, 8, 0);
	vec3 lightColor = vec3(1, 1, 1);

	vec3 ambient = 0.1 * lightColor;

	vec3 _normal = normalize(normal);
	
	vec3 fragToLight = normalize(lightPos - fragPos);
	float diffuseIntensity = clamp(dot(fragToLight, _normal), 0, 1);

	vec3 diffuse = diffuseIntensity * lightColor;

	if (clamp(dot(normalize(cameraPos - fragPos), _normal), 0, 1) < 0.05)
	{
		FragColor = vec4(0.0, 1.0, 0.5, 1.0);
	}
	else
	{
		FragColor = texture(uTex, uv) * vec4(clamp(ambient + diffuse, 0, 1), 1.0);
	}
}
