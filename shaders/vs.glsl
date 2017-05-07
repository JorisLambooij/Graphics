#version 330
in vec3 vPosition;
in  vec3 vColor;
out vec4 color;
uniform mat4 M;
void main()
{
 gl_Position = M * vec4(vPosition, 1.0);
 float z = (vPosition.z / -10) * 0.975 + 0.05;
 color = vec4( 0, z, 0, 1.0 );
}