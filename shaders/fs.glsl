#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 position;				
uniform sampler2D pixels;		// texture sampler

vec4 lightP = vec4 (0, 10, -10, 1);

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    outputColor = texture( pixels, uv );// + 0.5f * vec4( normal.xyz, 1 );
	vec4 lightD = lightP - position;
	float nDotL = dot (normal, normalize(lightD) );
	outputColor *= nDotL;
}