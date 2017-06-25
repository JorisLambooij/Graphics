#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;			// transformed vertex normal	
out vec4 position;
out vec2 uv;
out mat4 transform_world;

out vec4 tangent;
out vec4 bitangent;

uniform mat4 transform;
uniform mat4 transform_2wrld;

// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = transform * vec4(vPosition, 1.0);

	// forward normal and uv coordinate; will be interpolated over triangle
	normal = transform_2wrld * vec4( vNormal, 0.0f );
	position = transform_2wrld * vec4(vPosition, 1.0);
	uv = vUV;
	transform_world = transform_2wrld;

	tangent = vec4(uv, vec2(0, 0));
}