#version 330
in vec3 vPosition;
in  vec3 vColor;
out vec4 color;
in vec3 vN;
out vec3 n;
uniform mat4 M;
void main()
{
 n = vN;
 gl_Position = M * vec4(vPosition, 1.0);
 float z = (vPosition.z / -10) * 0.90 + 0.025;
 color = vec4( 0, z, 0.5 - (2 * z), 1.0 );
}