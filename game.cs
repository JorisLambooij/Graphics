using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

    class Game
    {
	    // member variables
	    public Surface screen;					// background surface for printing etc.
	    Mesh mesh, floor, lightbulb;			// a mesh to draw using OpenGL
        TreeNode meshNode, floorNode;           // the relation of the mesh with it's children
	    const float PI = 3.1415926535f;			// PI
	    float a = 0;							// teapot rotation angle
	    Stopwatch timer;						// timer for measuring frame duration
	    Shader shader;							// shader to use for rendering
	    Shader postproc;						// shader to use for post processing
	    Texture wood;							// texture to use for rendering
	    RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = true;

        Vector4[] lightData;


        // initialize
        public void Init()
	    {
            lightbulb = new Mesh("../../assets/lightbulb.obj", 1);

		    // load teapot
		    mesh = new Mesh( "../../assets/teapot.obj", 2);
            mesh.meshTransform *= Matrix4.CreateTranslation(new Vector3(0.5f, 0.2f, 0));
            mesh.meshTransform *= mesh.meshScale;
            meshNode = new TreeNode(mesh);
            ////Als de mesh op deze treenode 2 kinderen heeft, dan zouden deze op deze manier worden toegevoegd:
            //meshNode.nodeChildren.Add(kind1);
            //meshNode.nodeChildren.Add(kind2);

            floor = new Mesh( "../../assets/floor.obj", 0);
            floorNode = new TreeNode(floor);
            //Als de mesh op deze treenode 0 kinderen heeft, dan worden er geen meshes toegevoegd:

            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();

            // create shaders
            shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
            postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );

		    // load a texture
		    wood = new Texture( "../../assets/wood.jpg" );

		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();

            LightArray();

        }

        // tick for background surface
        public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffff00 );
	    }

        private void LightArray()
        {
            Vector4 color1 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_1 = new Vector4(0, 0, 0, 1);
            Vector4 ambient_Color_1 = color1 * 0.1f;
            Vector4 diffuse_Color_1 = color1;
            Vector4 speculr_Color_1 = color1;

            Vector4 color2 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_2 = new Vector4(1, 2, 0, 1);
            Vector4 ambient_Color_2 = color2 * 0.1f;
            Vector4 diffuse_Color_2 = color2;
            Vector4 speculr_Color_2 = color2;

            Vector4 color3 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_3 = new Vector4(0, 2, 0, 1);
            Vector4 ambient_Color_3 = color3 * 0.1f;
            Vector4 diffuse_Color_3 = color3;
            Vector4 speculr_Color_3 = color3;

            Vector4 color4 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_4 = new Vector4(0, 2, 10, 1);
            Vector4 ambient_Color_4 = color3 * 0.1f;
            Vector4 diffuse_Color_4 = color3;
            Vector4 speculr_Color_4 = color3;

            // set up light data array
            lightData = new Vector4[4 * 4];
            lightData[00] = lightPosition_1;
            lightData[01] = ambient_Color_1;
            lightData[02] = diffuse_Color_1;
            lightData[03] = speculr_Color_1;

            lightData[04] = lightPosition_2;
            lightData[05] = ambient_Color_2;
            lightData[06] = diffuse_Color_2;
            lightData[07] = speculr_Color_2;

            lightData[08] = lightPosition_3;
            lightData[09] = ambient_Color_3;
            lightData[10] = diffuse_Color_3;
            lightData[11] = speculr_Color_3;

            lightData[12] = lightPosition_4;
            lightData[13] = ambient_Color_4;
            lightData[14] = diffuse_Color_4;
            lightData[15] = speculr_Color_4;
        }

        // tick for OpenGL rendering code
        public void RenderGL()
	    {
		    // measure frame duration
		    float frameDuration = timer.ElapsedMilliseconds;
		    timer.Reset();
		    timer.Start();
	
		    // prepare matrix for vertex shader
		    Matrix4 transform = Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
            Matrix4 worldTransform = transform;
            transform *= Matrix4.CreateTranslation( 0, -4, -15 );
            transform *= Matrix4.CreatePerspectiveFieldOfView( 1.2f, 1.3f, .1f, 1000 );

		    // update rotation
		    a += 0.001f * frameDuration; 
		    if (a > 2 * PI) a -= 2 * PI;

		    if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

			    // render scene to render target
			    mesh.Render( shader, transform, worldTransform, wood, lightData);
			    floor.Render( shader, transform, worldTransform, wood, lightData);

                //lightbulb.Render(shader, transform, worldTransform, wood, lightData);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
			    // render scene directly to the screen
			    mesh.Render( shader, transform, worldTransform, wood, lightData);
			    floor.Render( shader, transform, worldTransform, wood, lightData);
		    }
	    }
    }

} // namespace Template_P3