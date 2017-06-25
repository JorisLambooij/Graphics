#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 position;

in mat4 transform_world;

in vec4 tangent;
in vec4 bitangent;

uniform vec4 camDir;
uniform sampler2D pixels;		// texture sampler

uniform vec4 lightPos1;
uniform vec4 lightPos2;
uniform vec4 lightPos3;
uniform vec4 lightPos4;

uniform vec4 lightData[13];

uniform vec4 ambient_Color;

uniform vec4 diffuse_Color_L1;
uniform vec4 speculr_Color_L1;

uniform vec4 diffuse_Color_L2;
uniform vec4 speculr_Color_L2;

uniform vec4 diffuse_Color_L3;
uniform vec4 speculr_Color_L3;

uniform vec4 diffuse_Color_L4;
uniform vec4 speculr_Color_L4;

int alpha = 40;

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
	float correctionFactor = 0.5;
	float lightIntensity = 40;

	// diffuse color of the mesh (texture)
    outputColor = texture( pixels, uv );// + 0.5f * vec4( normal.xyz, 1 );


	vec4 realNormal = normalize(normal);
	// times the diffuse illumination (and the diffuse light color)
	vec4 ld1 = lightPos1 - position;
	vec4 ld2 = lightPos2 - position;
	vec4 ld3 = lightPos3 - position;
	vec4 ld4 = lightPos4 - position;
	vec4 lightD1 = normalize(ld1);
	vec4 lightD2 = normalize(ld2);
	vec4 lightD3 = normalize(ld3);
	vec4 lightD4 = normalize(ld4);
	float nDotL1 = max(0, dot (realNormal, lightD1 ));
	float nDotL2 = max(0, dot (realNormal, lightD2 ));
	float nDotL3 = max(0, dot (realNormal, lightD3 ));
	float nDotL4 = max(0, dot (realNormal, lightD4 ));
	nDotL1 *= lightIntensity / ( ld1.x * ld1.x + ld1.y * ld1.y + ld1.z * ld1.z );
	nDotL1 *= lightIntensity / ( ld2.x * ld2.x + ld2.y * ld2.y + ld2.z * ld2.z );
	nDotL1 *= lightIntensity / ( ld3.x * ld3.x + ld3.y * ld3.y + ld3.z * ld3.z );
	nDotL1 *= lightIntensity / ( ld4.x * ld4.x + ld4.y * ld4.y + ld4.z * ld4.z );
	outputColor *= (nDotL1 * diffuse_Color_L1 + nDotL2 * diffuse_Color_L2 + nDotL3 * diffuse_Color_L3 + nDotL4 * diffuse_Color_L4);
	

	// plus the specular illumination per light source
	vec4 normalizedP = normalize(camDir - position);
	float nDotP = dot(realNormal, normalizedP);
	vec4 reflectedRay = (normalizedP - 2 * nDotP * realNormal); // ?
	//if(nDotP <= 0)
	//  {  reflectedRay = vec4(1, 0, 0, 0);  }

	float dotProduct1 = max(0, dot(reflectedRay, lightD1));
	float specularIntensity1 = pow(dotProduct1, alpha);
	vec4 specularColor1 = specularIntensity1 * speculr_Color_L1 * texture( pixels, uv ) * correctionFactor;
	outputColor += specularColor1;

	float dotProduct2 = min(1, max(0, -dot(reflectedRay, lightD2)));
	float specularIntensity2 = pow(dotProduct2, alpha);
	vec4 specularColor2 = specularIntensity2 * speculr_Color_L2 * texture( pixels, uv ) * correctionFactor;
	outputColor += specularColor2;

	float dotProduct3 = min(1, max(0, -dot(reflectedRay, lightD3)));
	float specularIntensity3 = pow(dotProduct3, alpha);
	vec4 specularColor3 = specularIntensity3 * speculr_Color_L3 * texture( pixels, uv ) * correctionFactor;
	outputColor += specularColor3;

	float dotProduct4 = min(1, max(0, -dot(reflectedRay, lightD4)));
	float specularIntensity4 = pow(dotProduct4, alpha);
	vec4 specularColor4 = specularIntensity4 * speculr_Color_L4 * texture( pixels, uv ) * correctionFactor;
	outputColor += specularColor4;


	// plus the ambient light color
	outputColor += ambient_Color * texture( pixels, uv );
	
	
	//outputColor.x = specularIntensity1;
	//outputColor = reflectedRay;
	//outputColor.z = nDotP;
}