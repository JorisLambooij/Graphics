using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Template {

    class Game
    {
	    // member variables
	    public Surface screen;
        float[] vertexData;

        int VBO;
        int programID;

        float a;
        double angle;

        Surface map;
        float[,] h;

        int vsID, fsID;
        int attribute_vpos, attribute_vcol;
        int uniform_mview;
        int vbo_pos;

	    // initialize
	    public void Init()
        {
            vertexData = new float[127 * 127 * 2 * 3 * 3];

            map = new Surface("../../assets/heightmap.png");
            h = new float[128, 128];
            for (int y = 0; y < 127; y++)
                for (int x = 0; x < 127; x++)
                {
                    int arrayPos = 18 * x + 127 * 18 * y;
                    float z;

                    // 1st triangle
                    z = ((float)(map.pixels[x + y * 128] & 255)) / 256;
                    z *= -10;

                    vertexData[arrayPos + 0] = x - 64;
                    vertexData[arrayPos + 1] = y - 64;
                    vertexData[arrayPos + 2] = z;

                    z = ((float)(map.pixels[(x+1) + y * 128] & 255)) / 256;
                    z *= -10;
                    vertexData[arrayPos + 3] = x + 1 - 64;
                    vertexData[arrayPos + 4] = y - 64;
                    vertexData[arrayPos + 5] = z;

                    z = ((float)(map.pixels[x + (y+1) * 128] & 255)) / 256;
                    z *= -10;

                    vertexData[arrayPos + 6] = x - 64;
                    vertexData[arrayPos + 7] = y + 1 - 64;
                    vertexData[arrayPos + 8] = z;

                    // 2nd triangle
                    z = ((float)(map.pixels[(x + 1) + y * 128] & 255)) / 256;
                    z *= -10;

                    vertexData[arrayPos + 09] = x + 1 - 64;
                    vertexData[arrayPos + 10] = y - 64;
                    vertexData[arrayPos + 11] = z;

                    z = ((float)(map.pixels[x + (y + 1) * 128] & 255)) / 256;
                    z *= -10;
                    vertexData[arrayPos + 12] = x - 64;
                    vertexData[arrayPos + 13] = y + 1 - 64;
                    vertexData[arrayPos + 14] = z;

                    z = ((float)(map.pixels[(x + 1) + (y + 1) * 128] & 255)) / 256;
                    z *= -10;

                    vertexData[arrayPos + 15] = x + 1 - 64;
                    vertexData[arrayPos + 16] = y + 1 - 64;
                    vertexData[arrayPos + 17] = z;
                }

            for (int i = 0; i < 127 * 127; i++)
            {

            }
            //h[x, y] = ((float)(map.pixels[x + y * 128] & 255)) / 256;
            
            //Create Shader program
            programID = GL.CreateProgram();
            LoadShader("../../shaders/vs.glsl", ShaderType.VertexShader, programID, out vsID);
            LoadShader("../../shaders/fs.glsl", ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);

            //Input variables vertex shader
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(programID, "vColor");
            uniform_mview = GL.GetUniformLocation(programID, "M" );

            //Link position data to Shader
            vbo_pos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_pos);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * 4), vertexData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.BufferData<float>(
            BufferTarget.ArrayBuffer,
            (IntPtr)(vertexData.Length * 4),
            vertexData,
            BufferUsageHint.StaticDraw
            );

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 12, 0);

            angle = 0;
            a = 0;
	    }
	    // tick: renders one frame
	    public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffffff );
            screen.Line(2, 20, 160, 20, 0xff0000);

            angle += 0.5f;

            //Creating Matrix
            Matrix4 M = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), a);
            M *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), 1.9f);
            M *= Matrix4.CreateTranslation(0, 0, -1);
            M *= Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);

            //Passing to the GPU
            GL.UseProgram(programID);
            GL.UniformMatrix4(uniform_mview, false, ref M);

            //Ready to render
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 127 * 127 * 2 *3);

            //Exercise_3();
        }

        public void RenderGL()
        {
            /*
            
            var M = Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);
            GL.LoadMatrix(ref M);
            GL.Translate(0, 0, -2);
            float scale = 0.03f;
            GL.Scale(new Vector3(scale, scale, scale));
            GL.Rotate(110, 1.5f, 0, 0);
            GL.Rotate(angle, 0, 0, 1);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 127 * 127 * 2 * 3);

            /*
            float maxHeight = -10;
            
            for(int x = 0; x < h.GetLength(0) - 1; x++)
                for (int y = 0; y < h.GetLength(1) - 1; y++)
                {
                    Vector3 v1 = new Vector3(x, y, h[x, y] * maxHeight);
                    Vector3 v2 = new Vector3(x + 1, y, h[x + 1, y] * maxHeight);
                    Vector3 v3 = new Vector3(x, y + 1, h[x, y + 1] * maxHeight);

                    v1.X -= h.GetLength(0) / 2;
                    v2.X -= h.GetLength(0) / 2;
                    v3.X -= h.GetLength(0) / 2;

                    v1.Y -= h.GetLength(1) / 2;
                    v2.Y -= h.GetLength(1) / 2;
                    v3.Y -= h.GetLength(1) / 2;

                    RenderPolygon(v1, v2, v3, .5f, 1f, .5f);

                    v1 = new Vector3(x + 1, y, h[x + 1, y] * maxHeight);
                    v2 = new Vector3(x, y + 1, h[x, y + 1] * maxHeight);
                    v3 = new Vector3(x + 1, y + 1, h[x + 1, y + 1] * maxHeight);

                    v1.X -= h.GetLength(0) / 2;
                    v2.X -= h.GetLength(0) / 2;
                    v3.X -= h.GetLength(0) / 2;

                    v1.Y -= h.GetLength(1) / 2;
                    v2.Y -= h.GetLength(1) / 2;
                    v3.Y -= h.GetLength(1) / 2;

                    RenderPolygon(v1, v2, v3, .2f, .5f, .2f);
                }
                */
        }

        int CreateColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }


        public void RenderPolygon(Vector3 v1, Vector3 v2, Vector3 v3, float r, float g, float b)
        {
            GL.Color3(r, g, b);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(v1);
            GL.Vertex3(v2);
            GL.Vertex3(v3);
            GL.End();
        }


        void Exercise_3()
        {
            // Velocity for 1 turn / second
            float angularVelocity = (float)(2 * Math.PI / 30);
            // Actual v
            a += angularVelocity / 16;

            // WATCH OUT: squareSize is only half the actual length
            float squareSize = 0.8f;

            // top-left corner
            float x1 = -squareSize, y1 = squareSize;
            float rx1 = (float)(x1 * Math.Cos(a) - y1 * Math.Sin(a));
            float ry1 = (float)(x1 * Math.Sin(a) + y1 * Math.Cos(a));
            // top-right corner
            float x2 = squareSize, y2 = squareSize;
            float rx2 = (float)(x2 * Math.Cos(a) - y2 * Math.Sin(a));
            float ry2 = (float)(x2 * Math.Sin(a) + y2 * Math.Cos(a));
            // bottom-right corner
            float x3 = squareSize, y3 = -squareSize;
            float rx3 = (float)(x3 * Math.Cos(a) - y3 * Math.Sin(a));
            float ry3 = (float)(x3 * Math.Sin(a) + y3 * Math.Cos(a));
            // bottom-left corner
            float x4 = -squareSize, y4 = -squareSize;
            float rx4 = (float)(x4 * Math.Cos(a) - y4 * Math.Sin(a));
            float ry4 = (float)(x4 * Math.Sin(a) + y4 * Math.Cos(a));

            screen.Line(TX(rx1), TY(ry1), TX(rx2), TY(ry2), 0xFFFF00);
            screen.Line(TX(rx2), TY(ry2), TX(rx3), TY(ry3), 0xFFFF00);
            screen.Line(TX(rx3), TY(ry3), TX(rx4), TY(ry4), 0xFFFF00);
            screen.Line(TX(rx4), TY(ry4), TX(rx1), TY(ry1), 0xFFFF00);

            screen.Line(TX(0), TY(0), TX(1), TY(-2), 0xFF0000);
        }

        #region TX and TY
        // the maximum range in both x-Directions
        float xCoordinateRange = 3f;
        // the translation. Positive values mean the origin of the coordinate system will be moved right/up respectively
        float offsetX = 0.2f;
        float offsetY = 0.2f;

        int TX(float x)
        {
            // translate the window size to coordinates within the specified range
            // CoordinateScreenWidth will be the x-screen-coordinate for 1 unit in simulation-coordinates
            float CoordinateScreenWidth = screen.width / (xCoordinateRange * 2);
            // translate "simulation-x" according to specified range and offset
            float adjustedX = x + xCoordinateRange + offsetX;

            return (int)(adjustedX * CoordinateScreenWidth);
        }

        int TY(float y)
        {
            // invert y
            y *= -1;
            // same as above. We need the reference to screen.width because we need to account for different aspect ratios
            float CoordinateScreenWidth = (float)screen.width / (xCoordinateRange * 2);

            // The height the screen has when referencing the specified x-coordinateRange
            float twoBasedHeight = screen.height / CoordinateScreenWidth;
            float adjustedY = y + twoBasedHeight / 2 - offsetY;

            return (int)(adjustedY * CoordinateScreenWidth);
        }
        #endregion


        void Exercise_1_And_2()
        {
            int centerX = screen.width / 2;
            int centerY = screen.height / 2;

            int offsetX = -128;
            int offsetY = -128;

            int squareSize = 256;

            for (int x = 0; x < squareSize; x++)
            {
                for (int y = 0; y < squareSize; y++)
                {
                    int location = (x + offsetX + centerX) + (y + offsetY + centerY) * screen.width;
                    screen.pixels[location] = CreateColor(x, y, 0);
                }
            }
        }

        //Load Shader code
        void LoadShader( String name, ShaderType type, int program, out int ID)
        {
            ID= GL.CreateShader( type );
            using (StreamReader sr = new StreamReader(name))
                GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program,ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }
    }

} // namespace Template