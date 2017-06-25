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

vec3 colorAt(vec2 uv);
vec3 colorAt(vec2 uv)
{
	vec3 ORcolor = texture( pixels, uv ).rgb;
	
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
	

	// keep for now
	// use to implement vignetting?
	// apply dummy postprocessing effect
	//float dx = P.x - 0.5, dy = P.y - 0.5;
	//float distanceSQ = dx * dx + dy * dy;
}
