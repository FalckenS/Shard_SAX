#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;

uniform mat4 projection;
out vec3 vertexColor;

void main()
{
    gl_Position = vec4(aPos, 1.0) * projection ; // Apply projection matrix
    vertexColor = aColor; // Pass the color to the fragment shader
}