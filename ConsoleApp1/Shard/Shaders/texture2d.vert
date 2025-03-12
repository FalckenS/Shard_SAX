#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;
uniform mat2 textureTransform;

void main(void)
{
    texCoord = aTexCoord * textureTransform;

    gl_Position = vec4(aPosition, 1.0) * transform;
}