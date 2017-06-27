using System.Diagnostics;
using System;
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
	    Shader postproc;                        // shader to use for post processing
        Texture wood, tiles, dwarfTexture, eyes_blue, marble;							// texture to use for rendering
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
        Texture colorCube_ID, colorCube_BW, colorCube_VNTG;
        enum Filter { None, BlackWhite, Vintage};
        Filter filter = Filter.None;


        // initialize
        public void Init()
	    {
            lightbulb = new Mesh("../../assets/lightbulb.obj", 1);

            // load a texture
            wood = new Texture("../../assets/wood.jpg");
            tiles = new Texture("../../assets/tiles.jpg");


            Mesh floor = new Mesh("../../assets/floor.obj", 4f);
            floor.meshTransform = Matrix4.CreateTranslation(new Vector3(0, 0f, 0)); ;
            floorNode = new SceneGraph(null, floor, tiles);

            // load teapots
            Mesh mesh = new Mesh( "../../assets/teapot.obj", 1);
            mesh.meshTransform = Matrix4.CreateTranslation(new Vector3(0f, 1f, 0));
            SceneGraph meshNode = new SceneGraph(floorNode, mesh, wood);

            mesh = new Mesh("../../assets/teapot.obj", 0.5f);
            mesh.meshTransform = Matrix4.CreateTranslation(new Vector3(6, 4f, 0)); ;
            meshNode = new SceneGraph(floorNode, mesh, wood);

            //Load Dwarf
            dwarfTexture = new Texture("../../assets/test.jpg");
            Mesh dwarf = new Mesh("../../assets/dwarf.obj", 1000);
            dwarf.meshTransform = Matrix4.CreateTranslation(new Vector3(0f, -50f, -40f));
            SceneGraph dwarfNode = new SceneGraph(floorNode, dwarf, dwarfTexture);

            //Load Eye
            //eyes_blue = new Texture("../../assets/normal_brick.png");
            eyes_blue = new Texture("../../assets/eyes_blue.jpg");
            Mesh eyeball = new Mesh("../../assets/eyeball.obj", 1);
            eyeball.meshTransform = Matrix4.CreateTranslation(new Vector3(0f, 4.25f, 7f));
            SceneGraph eyeballNode = new SceneGraph(floorNode, eyeball, eyes_blue);

            //Load Columns
            marble = new Texture("../../assets/marble2.jpg");
            Mesh column = new Mesh("../../assets/column.obj", 0.5f);
            column.meshTransform = Matrix4.CreateTranslation(new Vector3(-13f, -4f, -13f));
            SceneGraph columnNode = new SceneGraph(floorNode, column, marble);
            Mesh column2 = new Mesh("../../assets/column.obj", 0.5f);
            column2.meshTransform = Matrix4.CreateTranslation(new Vector3(13f, -4f, -13f));
            SceneGraph columnNode2 = new SceneGraph(floorNode, column2, marble);
            Mesh column3 = new Mesh("../../assets/column.obj", 0.5f);
            column3.meshTransform = Matrix4.CreateTranslation(new Vector3(13f, -4f, 13f));
            SceneGraph columnNode3 = new SceneGraph(floorNode, column3, marble);
            Mesh column4 = new Mesh("../../assets/column.obj", 0.5f);
            column4.meshTransform = Matrix4.CreateTranslation(new Vector3(-13f, -4f, 13f));
            SceneGraph columnNode4 = new SceneGraph(floorNode, column4, marble);


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

            colorCube_ID = new Texture("../../assets/color_cube.png");
            colorCube_BW = new Texture("../../assets/color_cube_bw.png");
            colorCube_VNTG = new Texture("../../assets/color_cube_vintage.png");


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

            HandleInput();
        }

        private void LightArray()
        {
            Vector4 ambient_Color = new Vector4(1, 1, 1, 1) * 0.1f;

            Vector4 color1 = new Vector4(3, 3, 3, 1);
            Vector4 lightPosition_1 = new Vector4(10, 10, -10, 1);
            Vector4 diffuse_Color_1 = color1;
            Vector4 speculr_Color_1 = color1;

            Vector4 color2 = new Vector4(0, 1, 1, 1);
            Vector4 lightPosition_2 = new Vector4(10, 10, 10, 1);
            Vector4 diffuse_Color_2 = color2;
            Vector4 speculr_Color_2 = color2;

            Vector4 color3 = new Vector4(1, 1, 1, 1);
            Vector4 lightPosition_3 = new Vector4(-10, 10, 10, 1);
            Vector4 diffuse_Color_3 = color3;
            Vector4 speculr_Color_3 = color3;

            Vector4 color4 = new Vector4(2, 0, 0, 1);
            Vector4 lightPosition_4 = new Vector4(-2, 8, 0, 1);
            Vector4 diffuse_Color_4 = color4;
            Vector4 speculr_Color_4 = color4;
            Vector4 spotLightDir_4 = new Vector4(-0.1f, -1, 0, 0);

            // set up light data array
            lightData = new Vector4[2 + 3 * 4];
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
            lightData[13] = spotLightDir_4;
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

            lightData[01] = new Vector4((float)(10 * Math.Sin(a)), 10, (float)(-10 * Math.Cos(a)), 1);

            Vector4 camDir = new Vector4(-camTransform.ExtractTranslation(), 0);

            if (useRenderTarget)
		    {
			    // enable render target
			    target.Bind();

                // render scene to render target
                floorNode.TreeNodeRender(shader, transform, worldTransform, lightData, camDir);

                // render quad
                target.Unbind();
                switch (filter)
                {
                    case (Filter.BlackWhite):
                        quad.Render(postproc, target.GetTextureID(), colorCube_BW);
                        break;
                    case (Filter.Vintage):
                        quad.Render(postproc, target.GetTextureID(), colorCube_VNTG);
                        break;
                    case (Filter.None):
                    default:
                        quad.Render(postproc, target.GetTextureID(), colorCube_ID);
                        break;
                }
		    }
		    else
		    {
                // enable render target
                target.Bind();

                // render scene directly to the screen
                floorNode.treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData, camDir);
                floorNode.treeNodeChildren[0].treeNodeMesh.Render(shader, transform, worldTransform, wood, lightData, camDir);
                
                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID(), colorCube_ID);
            }
	    }

        private void HandleInput()
        {
            Vector3 forward = camTransform.Column2.Xyz * 0.1f;
            Vector3 right = camTransform.Column0.Xyz * 0.1f;
            Vector3 up = camTransform.Column1.Xyz * 0.1f;
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
            if (keyboard[Key.Space])
                translation *= Matrix4.CreateTranslation(-up);
            else if (keyboard[Key.LControl])
                translation *= Matrix4.CreateTranslation(up);

            if (keyboard[Key.B])
                filter = Filter.BlackWhite;
            else if(keyboard[Key.N])
                filter = Filter.None;
            else if (keyboard[Key.V])
                filter = Filter.Vintage;

            currentMouseX = mousestate.X;
            currentMouseY = mousestate.Y;
            int deltaX = currentMouseX - previousMouseX;
            int deltaY = currentMouseY - previousMouseY;

            Vector3 up2 = Vector3.UnitY;
            
            Vector3 target = (-camTransform.Column2.Xyz + 0.0001f * mouseSensitivity * deltaX * camTransform.Column0.Xyz - 0.0001f * mouseSensitivity * deltaY * camTransform.Column1.Xyz).Normalized();
            Matrix4 rotation = Matrix4.LookAt(Vector3.Zero, target, up2);

            camTransform = rotation * translation;

            previousMouseX = currentMouseX;
            previousMouseY = currentMouseY;
        }

    }
} // namespace Template_P3