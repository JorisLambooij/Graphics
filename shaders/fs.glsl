#version 330
in vec4 color;
in vec3 N;
out vec4 outputColor;
void main()
{
 float angle = acos(dot(N, vec3(0, 0, 1)));
 outputColor = color * angle * 0.8;
}