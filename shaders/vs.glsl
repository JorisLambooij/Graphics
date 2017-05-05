#version 330
in vec3 vPosition;
<<<<<<< HEAD
in vec3 vColor;
=======
in  vec3 vColor;
>>>>>>> 7040abdeaf8a053b6cab8dc56803bf8f946043b7
out vec4 color;
uniform mat4 M;
void main()
{
<<<<<<< HEAD
 gl_Position = M * vec4( vPosition, 1.0 );
 color = vec4( vColor, 1.0);
=======
gl_Position = M * vec4(vPosition, 1.0);
color = vec4( vColor, 1.0);
>>>>>>> 7040abdeaf8a053b6cab8dc56803bf8f946043b7
}