#version 330 core
layout (location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 viewPos;

out vec2 texCoord;
out vec3 tangentViewPos;
out vec3 tangentFragPos;
out vec3 normal;
flat out mat3 TBN;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    vec3 fragPos = vec3(vec4(aPosition, 1.0) * model);
    texCoord = aTexCoord;
    mat3 normalMatrix =  mat3(transpose(inverse(model)));

    vec3 T = normalize(aTangent * normalMatrix);
    vec3 B = normalize(aBitangent * normalMatrix);
    vec3 N = normalize(aNormal * normalMatrix);
    
    TBN = mat3(T, B, N);  
    tangentViewPos  = viewPos * TBN;
    tangentFragPos  = fragPos * TBN;
    normal = normalize(normalize(aNormal * normalMatrix) * TBN);

}