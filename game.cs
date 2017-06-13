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
	    Mesh mesh, floor;						// a mesh to draw using OpenGL
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

        Vector4 lightPosition_1 = new Vector4(10, 10, 0, 1);
        Vector4 ambient_Color_1 = new Vector4(0.1f, 0.1f, 0.1f, 1);
        Vector4 diffuse_Color_1 = new Vector4(1, 1, 1, 1);
        Vector4 speculr_Color_1 = new Vector4(0.2f, 0.2f, 0.2f, 1);

        Vector4[] lightData;


        // initialize
        public void Init()
	    {
		    // load teapot
		    mesh = new Mesh( "../../assets/teapot.obj" );
            mesh.meshTransform = Matrix4.CreateTranslation(new Vector3(0, 0, 0));
            meshNode = new TreeNode(mesh);
            ////Als de mesh op deze treenode 2 kinderen heeft, dan zouden deze op deze manier worden toegevoegd:
            //meshNode.nodeChildren.Add(kind1);
            //meshNode.nodeChildren.Add(kind2);

            floor = new Mesh( "../../assets/floor.obj" );
            floorNode = new TreeNode(floor);
            //Als de mesh op deze treenode 0 kinderen heeft, dan worden er geen meshes toegevoegd:

            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();

            LightArray();

            // create shaders
            shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
            postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );

		    // load a texture
		    wood = new Texture( "../../assets/wood.jpg" );

		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad();
   	    }

	    // tick for background surface
	    public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffff00 );
	    }

        private void LightArray()
        {
            // set up light data array
            lightData = new Vector4[1 * 4];
            lightData[0] = lightPosition_1;
            lightData[1] = ambient_Color_1;
            lightData[2] = diffuse_Color_1;
            lightData[3] = speculr_Color_1;
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
            Matrix4 worldTransfrom = transform;
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
			    mesh.Render( shader, transform, worldTransfrom, wood, lightData);
			    floor.Render( shader, transform, worldTransfrom, wood, lightData);

			    // render quad
			    target.Unbind();
			    quad.Render( postproc, target.GetTextureID() );
		    }
		    else
		    {
			    // render scene directly to the screen
			    mesh.Render( shader, transform, worldTransfrom, wood, lightData);
			    floor.Render( shader, transform, worldTransfrom, wood, lightData);
		    }
	    }
    }

} // namespace Template_P3