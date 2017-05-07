#version 330
in vec4 color;
in vec3 n;
out vec4 outputColor;
void main()
{
 float angle = dot(n, vec3(0, 0, 1));
 outputColor = color * angle;
}