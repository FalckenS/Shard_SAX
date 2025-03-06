#version 330 core

struct Material {
    sampler2D textureDiff;
    vec3 specular;
    float shininess; 
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;

uniform vec3 viewPos;

//uniform sampler2D textureDiff;
uniform sampler2D textureNormal;

out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 texCoord;



void main()
{
    //ambient
    vec3 ambient = light.ambient * vec3(texture(material.textureDiff, texCoord));

    //diffuse 
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * (diff * vec3(texture(material.textureDiff, texCoord))); 

    //specular
    vec3 viewDir = normalize(viewPos - FragPos);
    //vec3 reflectDir = reflect(-lightDir, norm);
    vec3 halfDir = normalize(viewDir + lightDir);
    // float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    float spec = pow(max(dot(halfDir, norm), 0.0), material.shininess);
    vec3 specular = light.specular * (spec * material.specular);


    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}