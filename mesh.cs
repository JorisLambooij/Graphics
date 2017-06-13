using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template_P3 {

    // mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh
    {
	    // data members
	    public ObjVertex[] vertices;			// vertex positions, model space
	    public ObjTriangle[] triangles;			// triangles (3 vertex indices)
	    public ObjQuad[] quads;					// quads (4 vertex indices)
	    int vertexBufferId;						// vertex buffer
	    int triangleBufferId;					// triangle buffer
	    int quadBufferId;						// quad buffer

        public Matrix4 meshTransform;

	    // constructor
	    public Mesh( string fileName )
	    {
		    MeshLoader loader = new MeshLoader();
		    loader.Load( this, fileName );
            meshTransform = Matrix4.Identity;
	    }

	    // initialization; called during first render
	    public void Prepare( Shader shader )
	    {
            if (vertexBufferId != 0) return; // already taken care of

            // generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
		    GL.GenBuffers( 1, out vertexBufferId );
		    GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBufferId );
		    GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf( typeof( ObjVertex ) )), vertices, BufferUsageHint.StaticDraw );

		    // generate triangle index array
		    GL.GenBuffers( 1, out triangleBufferId );
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, triangleBufferId );
		    GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf( typeof( ObjTriangle ) )), triangles, BufferUsageHint.StaticDraw );

		    // generate quad index array
		    GL.GenBuffers( 1, out quadBufferId );
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, quadBufferId );
		    GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf( typeof( ObjQuad ) )), quads, BufferUsageHint.StaticDraw );
	    }

	    // render the mesh using the supplied shader and matrix
	    public void Render( Shader shader, Matrix4 transform, Matrix4 worldTransform, Texture texture , Vector4[] lightData)
	    {
		    // on first run, prepare buffers
		    Prepare( shader );

		    // enable texture
		    int texLoc = GL.GetUniformLocation( shader.programID, "pixels" );
		    GL.Uniform1( texLoc, 0 );
		    GL.ActiveTexture( TextureUnit.Texture0 );
		    GL.BindTexture( TextureTarget.Texture2D, texture.id );
            
            // enable shader
            GL.UseProgram( shader.programID );

            Vector4 lightPosition_1 = lightData[0];
            Vector4 ambient_Color_1 = lightData[1];
            Vector4 diffuse_Color_1 = lightData[2];
            Vector4 speculr_Color_1 = lightData[3];

            // pass lightPos
            int light = GL.GetUniformLocation(shader.programID, "lightPos1");
            GL.Uniform4(light, lightPosition_1);

            // pass light colors
            int ambient = GL.GetUniformLocation(shader.programID, "ambient_Color_L1");
            GL.Uniform4(ambient, ambient_Color_1);

            int diffuse = GL.GetUniformLocation(shader.programID, "diffuse_Color_L1");
            GL.Uniform4(diffuse, diffuse_Color_1);

            int specular = GL.GetUniformLocation(shader.programID, "speculr_Color_L1");
            GL.Uniform4(specular, speculr_Color_1);


            // pass view transform to vertex shader
            Matrix4 m = transform * meshTransform;
            GL.UniformMatrix4(shader.uniform_mview, false, ref m);

            // pass world transform to vertex shader
            m = worldTransform * meshTransform;
            GL.UniformMatrix4(shader.uniform_2wrld, false, ref m);
            
            // bind interleaved vertex data
            GL.EnableClientState( ArrayCap.VertexArray );
		    GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBufferId );
		    GL.InterleavedArrays( InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf( typeof( ObjVertex ) ), IntPtr.Zero );

		    // link vertex attributes to shader parameters 
		    GL.VertexAttribPointer( shader.attribute_vuvs, 2, VertexAttribPointerType.Float, false, 32, 0 );
		    GL.VertexAttribPointer( shader.attribute_vnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4 );
		    GL.VertexAttribPointer( shader.attribute_vpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4 );

            // enable position, normal and uv attributes
            GL.EnableVertexAttribArray( shader.attribute_vpos );
            GL.EnableVertexAttribArray( shader.attribute_vnrm );
            GL.EnableVertexAttribArray( shader.attribute_vuvs );

		    // bind triangle index data and render
		    GL.BindBuffer( BufferTarget.ElementArrayBuffer, triangleBufferId );
		    GL.DrawArrays( PrimitiveType.Triangles, 0, triangles.Length * 3 );

		    // bind quad index data and render
		    if (quads.Length > 0)
		    {
			    GL.BindBuffer( BufferTarget.ElementArrayBuffer, quadBufferId );
			    GL.DrawArrays( PrimitiveType.Quads, 0, quads.Length * 4 );
		    }

		    // restore previous OpenGL state
		    GL.UseProgram( 0 );
	    }

	    // layout of a single vertex
	    [StructLayout(LayoutKind.Sequential)] public struct ObjVertex
	    {
		    public Vector2 TexCoord;
		    public Vector3 Normal;
		    public Vector3 Vertex;
	    }

	    // layout of a single triangle
	    [StructLayout(LayoutKind.Sequential)] public struct ObjTriangle
	    {
		    public int Index0, Index1, Index2;
	    }

	    // layout of a single quad
	    [StructLayout(LayoutKind.Sequential)] public struct ObjQuad
	    {
		    public int Index0, Index1, Index2, Index3;
	    }
    }

} // namespace Template_P3