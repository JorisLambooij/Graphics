using System.Diagnostics;
using OpenTK;
using OpenTK.Input;
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
	    bool useRenderTarget = true;
        int currentMouseX = 0;
        int currentMouseY = 0;
        int previousMouseX = 0;
        int previousMouseY = 0;

        float mouseSensitivity = 20;

        Matrix4 camTransform;
        Vector4[] lightData;

        //TreeNode rootNode;
        SceneGraph floorNode;
        Texture colorCube;

        // initialize
        public void Init()
	    {
            lightbulb = new Mesh("../../assets/lightbulb.obj", 1);
            
            // load a texture
            wood = new Texture("../../assets/wood.jpg");

            Mesh floor = new Mesh("../../assets/floor.obj", 2);
            floor.meshTransform = Matrix4.CreateTranslation(new Vector3(0, 0f, 0)); ;
            floorNode = new SceneGraph(null, floor, wood);

            floor = new Mesh("../../assets/floor.obj", 1);
            floor.meshTransform = Matrix4.CreateTranslation(new Vector3(0, 0.01f, 0)); ;
            SceneGraph meshNode = new SceneGraph(floorNode, floor, wood);

            // load teapot
            Mesh mesh = new Mesh( "../../assets/teapot.obj", 1);
            mesh.meshTransform = Matrix4.CreateTranslation(new Vector3(0f, 1f, 0));
            meshNode = new SceneGraph(floorNode, mesh, wood);

            camTransform = Matrix4.Identity;

            // initialize stopwatch
            timer = new Stopwatch();
		    timer.Reset();
		    timer.Start();

            // create shaders
            shader = new Shader( "../../shaders/vs.glsl", "../../shaders/fs.glsl" );
            postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );

		    // create the render target
		    target = new RenderTarget( screen.width, screen.height );
		    quad = new ScreenQuad(screen.width, screen.height);

            colorCube = new Texture("../../assets/color_cube.png");

            LightArray();

            currentMouseX = Mouse.GetState().X;
            currentMouseY = Mouse.GetState().Y;
            previousMouseX = currentMouseX;
            previousMouseY = currentMouseY;
        }

        // tick for background surface
        public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffff00 );

            //HandleInput();
        }

        private void LightArray()
        {
            Vector4 ambient_Color = new Vector4(1, 1, 1, 1) * 0.1f;

            Vector4 color1 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_1 = new Vector4(10, 10, -10, 1);
            Vector4 diffuse_Color_1 = color1;
            Vector4 speculr_Color_1 = color1;

            Vector4 color2 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_2 = new Vector4(10, 10, 10, 1);
            Vector4 diffuse_Color_2 = color2;
            Vector4 speculr_Color_2 = color2;

            Vector4 color3 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_3 = new Vector4(-10, 10, 10, 1);
            Vector4 diffuse_Color_3 = color3;
            Vector4 speculr_Color_3 = color3;

            Vector4 color4 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_4 = new Vector4(-10, 10, -10, 1);
            Vector4 diffuse_Color_4 = color3;
            Vector4 speculr_Color_4 = color3;

            // set up light data array
            lightData = new Vector4[1 + 3 * 4];
            lightData[00] = ambient_Color;

            lightData[01] = lightPosition_1;
            lightData[02] = diffuse_Color_1;
            lightData[03] = speculr_Color_1;

            lightData[04] = lightPosition_2;
            lightData[05] = diffuse_Color_2;
            lightData[06] = speculr_Color_2;

            lightData[07] = lightPosition_3;
            lightData[08] = diffuse_Color_3;
            lightData[09] = speculr_Color_3;

            lightData[10] = lightPosition_4;
            lightData[11] = diffuse_Color_4;
            lightData[12] = speculr_Color_4;
        }

        // tick for OpenGL rendering code
        public void RenderGL()
	    {
		    // measure frame duration
		    float frameDuration = timer.ElapsedMilliseconds;
		    timer.Reset();
		    timer.Start();

            // prepare matrix for vertex shader
            Matrix4 transform = Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), 0 );
            Matrix4 worldTransform = transform * Matrix4.CreateTranslation(camTransform.ExtractTranslation());
            transform *= Matrix4.CreateTranslation(0, -4, -20) * Matrix4.CreateTranslation(camTransform.ExtractTranslation());
            transform *= Matrix4.CreateFromQuaternion(camTransform.ExtractRotation());
            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            

		    // update rotation
		    a += 0.001f * frameDuration; 
		    if (a > 2 * PI) a -= 2 * PI;

            Vector4 camDir = new Vector4(-camTransform.ExtractTranslation(), 0);

            if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                //// render scene to render target
                //mesh.Render( shader, transform, worldTransform, wood, lightData);
                //floor.Render( shader, transform, worldTransform, wood, lightData);

                //floorNode.treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData);

                floorNode.TreeNodeRender(shader, transform, worldTransform, lightData, camDir);
                //System.Console.WriteLine(camDir);
                //lightbulb.Render(shader, transform, worldTransform, wood, lightData);

                // render quad
                target.Unbind();
			    quad.Render( postproc, target.GetTextureID(), colorCube);
		    }
		    else
		    {
                // enable render target
                target.Bind();
                
                floorNode.treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData, camDir);
                floorNode.treeNodeChildren[0].treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData, camDir);
                
                //// render scene directly to the screen
                //mesh.Render( shader, transform, worldTransform, wood, lightData);
                //floor.Render( shader, transform, worldTransform, wood, lightData);

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID(), colorCube);
            }
	    }

        private void HandleInput()
        {
            Vector3 forward = camTransform.Column2.Xyz * 0.1f;
            Vector3 right = camTransform.Column0.Xyz * 0.1f;
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mousestate = Mouse.GetState();

            Matrix4 translation = Matrix4.CreateTranslation(camTransform.ExtractTranslation());
            if (keyboard[Key.W])
                translation *= Matrix4.CreateTranslation(forward);
            else if (keyboard[Key.S])
                translation *= Matrix4.CreateTranslation(-forward);
            if (keyboard[Key.A])
                translation *= Matrix4.CreateTranslation(right);
            else if (keyboard[Key.D])
                translation *= Matrix4.CreateTranslation(-right);


            currentMouseX = mousestate.X;
            currentMouseY = mousestate.Y;
            int deltaX = currentMouseX - previousMouseX;
            int deltaY = currentMouseY - previousMouseY;

            Vector3 up = Vector3.UnitY;
            
            Vector3 target = (-camTransform.Column2.Xyz + 0.0001f * mouseSensitivity * deltaX * camTransform.Column0.Xyz - 0.0001f * mouseSensitivity * deltaY * camTransform.Column1.Xyz).Normalized();
            Matrix4 rotation = Matrix4.LookAt(Vector3.Zero, target, up);

            camTransform = rotation * translation;

            previousMouseX = currentMouseX;
            previousMouseY = currentMouseY;
        }

    }
} // namespace Template_P3