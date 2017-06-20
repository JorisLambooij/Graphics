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
	    Mesh lightbulb;		                 	// a mesh to draw using OpenGL
        //TreeNode meshNode, floorNode;           // the relation of the mesh with it's children
	    const float PI = 3.1415926535f;			// PI
	    float a = 0;							// teapot rotation angle
	    Stopwatch timer;						// timer for measuring frame duration
	    Shader shader;							// shader to use for rendering
	    Shader postproc;						// shader to use for post processing
	    Texture wood;							// texture to use for rendering
	    RenderTarget target;					// intermediate render target
	    ScreenQuad quad;						// screen filling quad for post processing
	    bool useRenderTarget = false;

        Vector4[] lightData;

        //TreeNode rootNode;
        TreeNode floorNode;


        // initialize
        public void Init()
	    {
            lightbulb = new Mesh("../../assets/lightbulb.obj");

            // load a texture
            wood = new Texture("../../assets/wood.jpg");

            Mesh floor = new Mesh("../../assets/floor.obj");
            floor.meshTransform = Matrix4.Identity;
            floorNode = new TreeNode(null, floor, wood);

            // load teapot
            Mesh mesh = new Mesh( "../../assets/teapot.obj" );
            mesh.meshTransform = Matrix4.CreateTranslation(new Vector3(0.5f, 0.2f, 0));
            TreeNode meshNode = new TreeNode(floorNode, mesh, wood);

            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();

            // create shaders
            shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
            postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );

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

			    //// render scene to render target
			    //mesh.Render( shader, transform, worldTransform, wood, lightData);
			    //floor.Render( shader, transform, worldTransform, wood, lightData);

                floorNode.TreeNodeRender(Matrix4.Identity, shader, worldTransform, lightData);

                //lightbulb.Render(shader, transform, worldTransform, wood, lightData);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
                // enable render target
                target.Bind();

                floorNode.treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData);
                floorNode.treeNodeChildren[0].treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData);

                //// render scene directly to the screen
                //mesh.Render( shader, transform, worldTransform, wood, lightData);
                //floor.Render( shader, transform, worldTransform, wood, lightData);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
	    }
    }

} // namespace Template_P3