#version 330
in vec3 vPosition;
in  vec3 vColor;
out vec4 color;
in vec3 vN;
out vec3 n;
uniform mat4 M;
void main()
{
 gl_Position = M * vec4(vPosition, 1.0);
 float z = (vPosition.z / -10) * 0.90 + 0.025;
 n = vN;
 color = vec4( 0, z, .5 - z, 1.0 );
}