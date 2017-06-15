#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 position;


uniform sampler2D pixels;		// texture sampler

uniform vec4 lightPos1;

uniform vec4 ambient_Color_L1;
uniform vec4 diffuse_Color_L1;
uniform vec4 speculr_Color_L1;

int alpha = 1;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	// diffuse color of the mesh (texture)
    outputColor = texture( pixels, uv );// + 0.5f * vec4( normal.xyz, 1 );

	// times the diffuse illumination (and the diffuse light color)
	vec4 lightD1 = lightPos1 - position;
	float nDotL = dot (normal, normalize(lightD1) );
	outputColor *= (nDotL * diffuse_Color_L1);

	// plus the specular illumination
	vec4 normalizedP = -normalize(position);
	float nDotP = dot(normal, normalizedP);
	vec4 reflectedRay = normalizedP - 2 * nDotP * normal; // ?
	float specularIntensity = pow(dot(reflectedRay, lightD1), alpha);
	specularIntensity = max(0, -specularIntensity);

	outputColor += specularIntensity * speculr_Color_L1;
	//outputColor.y = -specularIntensity / 10;// * speculr_Color_L1.y;

	// plus the ambient light color
	outputColor += ambient_Color_L1;

	//outputColor.x = -nDotP;
	//outputColor.y = -nDotP;
	//outputColor.z = nDotP;
}