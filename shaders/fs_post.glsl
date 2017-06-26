#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

uniform mat4 uniform_2wrld;
uniform vec2 invScreenRes;

uniform sampler2D color_cube;

// shader output
out vec3 outputColor;

float chromaticAberration = 1.006;

vec3 colorAt(vec2 uv);
vec3 colorAt(vec2 uv)
{
	float dx = uv.x - 0.5, dy = uv.y - 0.5;
	float distanceSQ = dx * dx + dy * dy;
	//between 0.25 + 0.25 = 0.5 and -0.5

	float inverse = 1 / chromaticAberration;
	float dx2 = dx * chromaticAberration;
	float dy2 = dy * chromaticAberration;
	vec2 ca_UV2 = vec2( dx2 + 0.5 , dy2 + 0.5);
	
	float dx3 = dx * inverse;
	float dy3 = dy * inverse;
	vec2 ca_UV3 = vec2( dx3 + 0.5 , dy3 + 0.5);
	

	vec3 ORcolor = vec3(0, 0, 0);
	ORcolor.x = texture( pixels, ca_UV2 ).x;
	ORcolor.y = texture( pixels, uv ).y;
	ORcolor.z = texture( pixels, ca_UV3 ).z;
	

	float a = 1 / 48.0;
	float b = 47 / 48.0;

	ORcolor.x = max(a, min(b, ORcolor.x));
	ORcolor.y = max(a, min(b, ORcolor.y));
	ORcolor.z = max(a, min(b, ORcolor.z));
	
	float cR = int(48 * ORcolor.x);
	float cG = ORcolor.y / 48.0;
	float cB = ORcolor.z;
	
	vec2 colorCubeUV = vec2 (cR / 48.0 + cG, cB);
	vec3 cubeColor = texture (color_cube, colorCubeUV).xyz;

	//cubeColor.xy = ca_UV;
	return cubeColor;
}


void main()
{
	// debug
	outputColor = vec3(0, 0, 0);

	// retrieve input pixel(s)
	outputColor = colorAt(uv);
	
	// anti-aliasing:
	// add up the colors of the pixel itself + the 8 surrounding pixels
	// average the result
	vec2 uv2 = uv;
	
	uv2 = uv + vec2(invScreenRes.x, 0);
	outputColor += colorAt(uv2);
	
	uv2 = uv + vec2(-invScreenRes.x, 0);
	outputColor += colorAt(uv2);

	uv2 = uv + vec2(0, invScreenRes.y);
	outputColor += colorAt(uv2);

	uv2 = uv + vec2(0, -invScreenRes.y);
	outputColor += colorAt(uv2);
	
	uv2 = uv + vec2(invScreenRes.x, invScreenRes.y);
	outputColor += colorAt(uv2);
	
	uv2 = uv + vec2(invScreenRes.x, -invScreenRes.y);
	outputColor += colorAt(uv2);

	uv2 = uv + vec2(-invScreenRes.x, invScreenRes.y);
	outputColor += colorAt(uv2);

	uv2 = uv + vec2(-invScreenRes.x, -invScreenRes.y);
	outputColor += colorAt(uv2);
	
	outputColor *= (1.0 / 9);
	

	float vigStrength = 4;
	float vigOffset = 0.9;

	// keep for now
	// use to implement vignetting?
	// apply dummy postprocessing effect
	float dx = P.x - 0.5, dy = P.y - 0.5;
	float distanceSQ = dx * dx + dy * dy;

	float vigFactor = pow(distanceSQ * vigStrength, vigStrength) + vigOffset;

	//outputColor *= (1 / vigFactor);
}
