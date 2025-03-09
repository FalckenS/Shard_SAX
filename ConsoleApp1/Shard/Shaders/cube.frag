#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D textureDiff;
uniform sampler2D textureNormal;

void main()
{
    outputColor = texture(textureDiff, texCoord);
    //outputColor = vec4(1.0, 0.0, 0.0, 1.0);
}