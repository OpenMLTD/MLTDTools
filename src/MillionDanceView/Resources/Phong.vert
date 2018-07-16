// https://learnopengl.com/Lighting/Basic-Lighting
#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;

uniform mat4 uW;
uniform mat4 uWVP;
uniform mat4 uWti; // world, transposed, inverted

out vec3 iNormal;
out vec3 iFragPos;

void main() {
	gl_Position = uWVP * vec4(aPos, 1.0);

	iFragPos = vec3(uW * vec4(aPos, 1.0));

	iNormal = mat3(uWti) * aNormal;
}
