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
        float[] vertexNormals;

        int VBO;
        int programID;

        float a;

        Surface map;
        float[,] h;
        float zHeight = -16;

        int vsID, fsID;
        int attribute_vpos, attribute_vcol, attribute_vnormal;
        int uniform_mview;
        int vbo_pos;
        int vbo_col;
        int vbo_normal;

        // initialize
        public void Init()
        {
            vertexData = new float[127 * 127 * 2 * 3 * 3];
            vertexNormals = new float[127 * 127 * 2 * 3 * 3];

            map = new Surface("../../assets/heightmap.png");
            //h = new float[128, 128];

            for (int y = 0; y < 127; y++)
                for (int x = 0; x < 127; x++)
                {
                    float c = 64;

                    int arrayPos = 18 * x + 127 * 18 * y;
                    float z;

                    // 1st triangle
                    z = ((float)(map.pixels[x + y * 128] & 255)) / 256;
                    z *= zHeight;

                    vertexData[arrayPos + 0] = x - c;
                    vertexData[arrayPos + 1] = y - c;
                    vertexData[arrayPos + 2] = z;
                    
                    z = ((float)(map.pixels[(x+1) + y * 128] & 255)) / 256;
                    z *= zHeight;
                    vertexData[arrayPos + 3] = x + 1 - c;
                    vertexData[arrayPos + 4] = y - c;
                    vertexData[arrayPos + 5] = z;

                    z = ((float)(map.pixels[x + (y+1) * 128] & 255)) / 256;
                    z *= zHeight;

                    vertexData[arrayPos + 6] = x - c;
                    vertexData[arrayPos + 7] = y + 1 - c;
                    vertexData[arrayPos + 8] = z;

                    // 2nd triangle
                    z = ((float)(map.pixels[(x + 1) + y * 128] & 255)) / 256;
                    z *= zHeight;

                    vertexData[arrayPos + 09] = x + 1 - c;
                    vertexData[arrayPos + 10] = y - c;
                    vertexData[arrayPos + 11] = z;

                    z = ((float)(map.pixels[x + (y + 1) * 128] & 255)) / 256;
                    z *= zHeight;
                    vertexData[arrayPos + 12] = x - c;
                    vertexData[arrayPos + 13] = y + 1 - c;
                    vertexData[arrayPos + 14] = z;

                    z = ((float)(map.pixels[(x + 1) + (y + 1) * 128] & 255)) / 256;
                    z *= zHeight;

                    vertexData[arrayPos + 15] = x + 1 - c;
                    vertexData[arrayPos + 16] = y + 1 - c;
                    vertexData[arrayPos + 17] = z;
                }

            FillNormalArray();
            
            //Create Shader program
            programID = GL.CreateProgram();
            LoadShader("../../shaders/vs.glsl", ShaderType.VertexShader, programID, out vsID);
            LoadShader("../../shaders/fs.glsl", ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);

            //Input variables vertex shader
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(programID, "vColor");
            attribute_vnormal = GL.GetAttribLocation(programID, "vN");
            uniform_mview = GL.GetUniformLocation(programID, "M" );

            //Link position data to Shader
            vbo_pos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_pos);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * 4), vertexData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            //Link color data to Shader
            vbo_col = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_col);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * 4), vertexData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, false, 0, 0);

            //Link vertex normals to Shader
            vbo_normal = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_normal);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(vertexData.Length * 4), vertexNormals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vnormal, 3, VertexAttribPointerType.Float, false, 0, 0);

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
            
            a = -90;
	    }
	    // tick: renders one frame
	    public void Tick()
	    {
		    screen.Clear( 0 );
		    screen.Print( "hello world", 2, 2, 0xffffff );
            screen.Line(2, 20, 160, 20, 0xff0000);

            a += (float)(2 * Math.PI) / 360 / 2;
        }

        public void RenderGL()
        {
            //Creating Matrix
            Matrix4 M = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), a);
            M *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), 2.1f);
            M *= Matrix4.CreateTranslation(0, 0, -88);
            M *= Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);
            
            //Passing to the GPU
            GL.UseProgram(programID);
            GL.UniformMatrix4(uniform_mview, false, ref M);
            
            //Ready to render
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);
            GL.EnableVertexAttribArray(attribute_vnormal);
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, 127 * 127 * 2 * 3);
        }

        #region Old Method for drawing the landscape
        private void OldMethod()
        {
            float maxHeight = -10;

            for (int x = 0; x < h.GetLength(0) - 1; x++)
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
        #endregion

        #region vertex Normals and stuff

        void FillNormalArray()
        {
            for (int y = 0; y < 127; y++)
                for (int x = 0; x < 127; x++)
                {
                    int arrayPos = 18 * x + 127 * 18 * y;

                    Vector3 normal;

                    normal = getVertexNormal(x, y);

                    vertexNormals[arrayPos + 0] = normal.X;
                    vertexNormals[arrayPos + 1] = normal.Y;
                    vertexNormals[arrayPos + 2] = normal.Z;


                    normal = getVertexNormal(x + 1, y);

                    vertexNormals[arrayPos + 3] = normal.X;
                    vertexNormals[arrayPos + 4] = normal.Y;
                    vertexNormals[arrayPos + 5] = normal.Z;

                    vertexNormals[arrayPos + 09] = normal.X;
                    vertexNormals[arrayPos + 10] = normal.Y;
                    vertexNormals[arrayPos + 11] = normal.Z;


                    normal = getVertexNormal(x, y + 1);

                    vertexNormals[arrayPos + 6] = normal.X;
                    vertexNormals[arrayPos + 7] = normal.Y;
                    vertexNormals[arrayPos + 8] = normal.Z;

                    vertexNormals[arrayPos + 12] = normal.X;
                    vertexNormals[arrayPos + 13] = normal.Y;
                    vertexNormals[arrayPos + 14] = normal.Z;


                    normal = getVertexNormal(x + 1, y + 1);

                    vertexNormals[arrayPos + 15] = normal.X;
                    vertexNormals[arrayPos + 16] = normal.Y;
                    vertexNormals[arrayPos + 17] = normal.Z;
                }
        }
        

        Vector3 getFaceNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 u = p2 - p1;
            Vector3 v = p3 - p1;

            Vector3 normal = Vector3.Zero;

            normal.X = u.Y * v.Z - u.Z * v.Y;
            normal.Y = u.Z * v.X - u.X * v.Z;
            normal.Z = u.X * v.Y - u.Y * v.X;
            
            return normal;
        }

        Vector3 getVertexNormal(int x, int y)
        {
            Vector3[] faceNormals = new Vector3[6];

            Vector3 p1, p2, p3;

            p1 = new Vector3(x, y - 1, getZ(x, y - 1));
            p2 = new Vector3(x, y, getZ(x, y));
            p3 = new Vector3(x - 1, y, getZ(x - 1, y));

            faceNormals[0] = getFaceNormal(p1, p2, p3);


            p1 = new Vector3(x, y - 1, getZ(x, y - 1));
            p2 = new Vector3(x + 1, y - 1, getZ(x + 1, y - 1));
            p3 = new Vector3(x, y, getZ(x, y));

            faceNormals[1] = getFaceNormal(p1, p2, p3);


            p1 = new Vector3(x + 1, y, getZ(x + 1, y));
            p2 = new Vector3(x, y, getZ(x, y));
            p3 = new Vector3(x + 1, y - 1, getZ(x + 1, y - 1));

            faceNormals[2] = getFaceNormal(p1, p2, p3);


            p1 = new Vector3(x + 1, y, getZ(x + 1, y));
            p2 = new Vector3(x, y + 1, getZ(x, y + 1));
            p3 = new Vector3(x, y, getZ(x, y));

            faceNormals[3] = getFaceNormal(p1, p2, p3);


            p1 = new Vector3(x - 1, y + 1, getZ(x - 1, y + 1));
            p2 = new Vector3(x, y, getZ(x, y));
            p3 = new Vector3(x, y + 1, getZ(x, y + 1));

            faceNormals[4] = getFaceNormal(p1, p2, p3);


            p1 = new Vector3(x - 1, y + 1, getZ(x - 1, y + 1));
            p2 = new Vector3(x - 1, y, getZ(x - 1, y));
            p3 = new Vector3(x, y, getZ(x, y));

            faceNormals[5] = getFaceNormal(p1, p2, p3);

            Vector3 sum = Vector3.Zero;
            for(int i = 0; i < 6; i++)
                sum += faceNormals[i];
            
            return sum.Normalized();
        }

        float getZ(int x, int y)
        {
            if (x < 0 || x >= 128 || y < 0 || y >= 128)
                return 0;
            float z = ((float)(map.pixels[x + y * 128] & 255)) / 256;
            z *= zHeight;
            return z;
        }
        #endregion
        

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