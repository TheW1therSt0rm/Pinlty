// Fragment shader
#version 330 core
in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoord;

uniform sampler2D diffuseTexture;
uniform vec3 lightPos;
uniform vec3 viewPos;

out vec4 FragColor;

void main() {
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);

    vec3 diffuse = diff * texture(diffuseTexture, TexCoord).rgb;
    FragColor = vec4(diffuse, 1.0);
}