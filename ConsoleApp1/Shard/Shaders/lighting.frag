#version 330 core

struct Material {
    sampler2D textureDiff;
    sampler2D textureNormal;
    vec3 specular;
    float shininess; 
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define MAX_NR_POINT_LIGHTS 10

uniform Light lights[MAX_NR_POINT_LIGHTS];
uniform Material material;
uniform int useNormalMap;
uniform int useDiffuseMap;
uniform vec3 pureColor;
uniform int nrLights;

out vec4 FragColor;


in vec2 texCoord;
in vec3 tangentViewPos;
in vec3 tangentFragPos;
in vec3 normal;
flat in mat3 TBN;




void main()
{
    vec3 norm;
    if (useNormalMap != 0){
        norm = texture(material.textureNormal, texCoord).rgb;
        norm = normalize(norm * 2.0 - 1.0);
    }
    else{
        norm = normalize(normal);
    }

    vec3 baseColor;
    if (useDiffuseMap != 0){
        baseColor = vec3(texture(material.textureDiff, texCoord));
    }
    else{
        baseColor = pureColor;
    }

    vec3 result = vec3(0);
    vec3 tangentLightPos[MAX_NR_POINT_LIGHTS];

    for (int i = 0; i < nrLights; i++){

        tangentLightPos[i] = lights[i].position * TBN;
        
        //ambient
        vec3 ambient = lights[i].ambient * baseColor;

        //diffuse 
        vec3 lightDir = normalize(tangentLightPos[i] - tangentFragPos);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = lights[i].diffuse * (diff * baseColor); 

        //specular
        vec3 viewDir = normalize(tangentViewPos - tangentFragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        vec3 halfDir = normalize(viewDir + lightDir);
        float spec = pow(max(dot(viewDir, reflectDir), 0.00001), material.shininess);
        vec3 specular = lights[i].specular * (spec * material.specular);


        result += ambient + diffuse + specular ;

    }

    
    FragColor = vec4(result, 1.0);
}