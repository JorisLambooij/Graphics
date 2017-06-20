#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

uniform mat4 uniform_2wrld;
uniform vec2 invScreenRes;

// shader output
out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture( pixels, uv ).rgb;
	
	// anti-aliasing:
	// add up the colors of the pixel itself + the 8 surrounding pixels
	// average the result
	vec2 uv2 = uv;
	
	uv2 = uv + vec2(invScreenRes.x, 0);
	outputColor += texture( pixels, uv2 ).rgb;
	
	uv2 = uv + vec2(-invScreenRes.x, 0);
	outputColor += texture( pixels, uv2 ).rgb;

	uv2 = uv + vec2(0, invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;

	uv2 = uv + vec2(0, -invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;
	
	uv2 = uv + vec2(invScreenRes.x, invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;
	
	uv2 = uv + vec2(invScreenRes.x, -invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;

	uv2 = uv + vec2(-invScreenRes.x, invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;

	uv2 = uv + vec2(-invScreenRes.x, -invScreenRes.y);
	outputColor += texture( pixels, uv2 ).rgb;
	
	outputColor *= (1.0 / 9.0);

	// keep for now
	// use to implement vignetting?
	// apply dummy postprocessing effect
	//float dx = P.x - 0.5, dy = P.y - 0.5;
	//float distanceSQ = dx * dx + dy * dy;
}