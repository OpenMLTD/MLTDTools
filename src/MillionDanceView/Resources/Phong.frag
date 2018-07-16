#version 330 core

in vec3 iNormal;
in vec3 iFragPos;

uniform vec3 uLightPos;
uniform vec3 uLightColor;
uniform vec4 uObjectColor;
uniform vec3 uViewPos;

uniform float uAlpha;

out vec4 oColor;

void main() {
	float ambientStrength = 0.1;
	vec3 ambient = ambientStrength * uLightColor;

	vec3 norm = normalize(iNormal);
	vec3 lightDir = normalize(uLightPos - iFragPos);

	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = diff * uLightColor;

	float specularStrength = uObjectColor.w;

	vec3 viewDir = normalize(uViewPos - iFragPos);
	vec3 reflectDir = reflect(-lightDir, norm);

	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = specularStrength * spec * uLightColor;

	vec3 color = (ambient + diffuse + specular) * uObjectColor.xyz;

	oColor = vec4(color * uAlpha, uAlpha);

	//oColor = vec4(1, 1, 1, 1);
}
