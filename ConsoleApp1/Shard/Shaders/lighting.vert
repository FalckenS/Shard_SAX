#version 330 core
layout (location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aTangent;
layout (location = 3) in vec3 aBitangent;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 lightPos;
uniform vec3 viewPos;


out vec2 texCoord;
out vec3 tangentLightPos;
out vec3 tangentViewPos;
out vec3 tangentFragPos;
out vec3 normal;
//out vec3 fragPos;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    vec3 fragPos = vec3(vec4(aPosition, 1.0) * model);
    texCoord = aTexCoord;
    mat3 normalMatrix =  mat3(transpose(inverse(model)));
    vec3 aNormal = normalize(cross(aTangent, aBitangent));
    normal = normalize(aNormal * normalMatrix);

    vec3 T = normalize(aTangent * normalMatrix);
    vec3 B = normalize(aBitangent * normalMatrix);
    vec3 N = normalize(aNormal * normalMatrix);
    //T = normalize(T - dot(T, N) * N);
    // then retrieve perpendicular vector B with the cross product of T and N
    //B = cross(N,T);
    
    mat3 TBN = mat3(T, B, N);  
    tangentLightPos = lightPos * TBN;
    tangentViewPos  = viewPos * TBN;
    tangentFragPos  = fragPos * TBN;
    normal = normal * TBN;

}