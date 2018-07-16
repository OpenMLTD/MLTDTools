#version 330 core

layout(location = 0) in vec3 aPosition;

uniform mat4 uWVP;

void main() {
	gl_Position = uWVP * vec4(aPosition, 1.0);
}
