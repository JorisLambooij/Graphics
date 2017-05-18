﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace template
{
    class RayTracer
    {
        public static float Lambda = 0.02f;

        public Camera camera;
        public Scene scene;

        public bool shadowraydebug;

        Surface screen;

        // initialize
        public void Init(Surface screen)
        {
            this.screen = screen;
            // camera setup
            Vector3 camPos = new Vector3(-10, 0, 1);
            Vector3 camDir = new Vector3(1, 0, 0);
            float view = 1;
            camera = new Camera(camPos, camDir, view);


            // initialize the scene with a few object (only one plane atm)
            scene = new Scene();
            scene.AddLight(new Vector3(-5, 6, 1f), 5000, new Vector3(1, 1, 1));
            scene.AddLight(new Vector3(3, -6, 5f), 5000, new Vector3(1, 1, 1));
            //scene.AddLight(new Vector3(0, 0, 20), 5000, new Vector3(1, 1, 1));

            Plane p = new Plane(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0));
            scene.AddObject(p);

            Plane p2 = new Plane(new Vector3(40, 0, 0.1f), new Vector3(-1, 0, 1), new Vector3(100, 100, 1000));
            scene.AddObject(p2);

            Sphere s1 = new Sphere(new Vector3(2, 3, 1), 2, new Vector3(1.0f, 0.4f, 1.0f));
            scene.AddObject(s1);

            Sphere s2 = new Sphere(new Vector3(0, -2, 1), 2, new Vector3(0.0f, 0.9f, .0f));
            scene.AddObject(s2);
        }

        public void Render()
        {
            int width = screen.width / 2;

            float xSteps = 1f / width;
            float ySteps = 1f / screen.height;
            
            for (int x = 0; x < width; x++)
                for(int y = 0; y < screen.height; y++)
                {
                    // for each pixel, cast a ray
                    // for now, use static camera, so the 2nd vector is simply calculated with x and y
                    Vector3 pixelDirection = camera.direction + new Vector3(0, (x * xSteps) - 0.5f, -(y * ySteps) + 0.5f);
                    Ray ray = new Ray(camera.position, pixelDirection.Normalized());

                    if (x % 16 == 0 && ray.direction.Z == 0)
                    {
                        shadowraydebug = true;

                        //Console.WriteLine(x + " Intersection: " + intersect.Color);
                    }

                    Vector3 color = TraceRay(ray);

                    screen.pixels[x + y * screen.width] = CreateColor(color);


                    // keep for Debug (for now)
                    Intersection intersect = scene.intersectScene(ray);
                    if (shadowraydebug)
                    {
                        DebugRay(ray, intersect, CreateColor(intersect.Color));
                        shadowraydebug = false;
                        //Console.WriteLine(x + " Intersection: " + intersect.Color);
                    }
                }

            DebugView();
        }

        public Vector3 TraceRay(Ray ray)
        {
            ray.origin = ray.origin + Lambda * ray.direction;
            Intersection intersect = scene.intersectScene(ray);
            
            // did not hit anything, return black
            if (intersect.collider == null)
                return Vector3.Zero;
            // TODO: check for materials
            else
            {
                //return intersect.Color;
                return DirectIllumination(intersect) * intersect.Color;
            }
        }

        public Vector3 DirectIllumination(Intersection intersect)
        {
            Vector3 totalIllumination = Vector3.Zero;
            if (shadowraydebug)
            {
                int i = 0;
            }
            foreach (Light lightSource in scene.lightSources)
            {
                Vector3 lightD = (lightSource.position - intersect.intersectionPoint).Normalized();
                float cos = Vector3.Dot(lightD, intersect.normal);

                //cos = Math.Abs(cos);

                if (cos > 0)
                {
                    Vector3 startPoint = intersect.intersectionPoint + Lambda * lightD;
                    Ray shadowRay = new Ray(startPoint, lightD);
                    
                    // if we hit nothing
                    if ( !scene.intersectSceneShadow(shadowRay, lightSource) )
                    {
                        Vector3 d = lightSource.position - shadowRay.origin;
                        Vector3 color = (lightSource.color * lightSource.intensity * cos / d.LengthSquared);
                        
                        totalIllumination += color;

                        // debug stuff
                        if (shadowraydebug)
                            DebugShadowRay(shadowRay, lightSource, CreateColor(color));
                    }
                    else if(shadowraydebug)
                        DebugShadowRay(shadowRay, lightSource, 0xff0000);
                    
                }
            }
            return totalIllumination;
        }
        

        #region Debug
        // screen scale relative to world scale
        float scale = -32;

        // offsets in SCREEN COORDINATES
        // important because the axes are flipped
        // higher xOffset means image will be moved to the right
        int xOffset = 512 + 256;
        // higher yOffset means image will be moved down
        int yOffset = 200;

        public void DebugView()
        {
            screen.Print("Camera", (int)(-camera.position.Y * scale + xOffset), (int) (camera.position.X * scale + yOffset), 0xFF0000);

            screen.Line(767, 0, 767, 512, 0xffffff);
            screen.Print("+x", 760, 10, 0xffffff);
            screen.Print("-x", 760, 490, 0xffffff);
            screen.Line(512, 256, 1023, 256, 0xffffff);
            screen.Print("+y", 1000, 260, 0xffffff);
            screen.Print("-y", 516, 260, 0xffffff);

            foreach (Primitive p in scene.sceneObjects)
            {
                if (p is Sphere)
                {
                    screen.Print("SPHERE", DebugX(p.position.Y), DebugY(p.position.X) - 10, CreateColor(p.color));
                    Sphere sp = (Sphere)p;

                    float deltaA = (float) Math.PI / 6;

                    float h = Math.Abs(camera.position.Z - sp.position.Z);

                    if(h < sp.radius)
                    {
                        float r = sp.radius * (float) Math.Cos(h / sp.radius);

                        for (float a = 0; a < 2 * Math.PI; a += deltaA)
                        {
                            int x1 = DebugX((float)Math.Sin(a - deltaA) * r + sp.position.Y);
                            int y1 = DebugY((float)Math.Cos(a - deltaA) * r + sp.position.X);
                            int x2 = DebugX((float)Math.Sin(a) * r + sp.position.Y);
                            int y2 = DebugY((float)Math.Cos(a) * r + sp.position.X);

                            screen.Line(x1, y1, x2, y2, CreateColor(sp.color));
                        }
                    }
                }
            }
            foreach (Light l in scene.lightSources)
            {
                screen.Print("LIGHT", (int)(-l.position.Y * scale + xOffset), (int)(l.position.X * scale + yOffset) - 10, 0x00ffff);
            }
        }

        public int DebugX(float y)
        {
            return (int)(-y * scale) + xOffset;
        }
        public int DebugY(float x)
        {
            return (int)(x * scale) + yOffset;
        }


        // draw a debug line for the specified ray.
        public void DebugRay(Ray ray, Intersection i, int c)
        {
            //c = 0x888888;

            Vector3 startPoint = ray.origin;
            Vector3 endPoint;
            
            if (i.collider != null)
                endPoint = i.intersectionPoint;
            else
                endPoint = ray.origin + -scale * ray.direction;

            int x1 = DebugX(startPoint.Y);
            int y1 = DebugY(startPoint.X);

            int x2 = DebugX(endPoint.Y);
            int y2 = DebugY(endPoint.X);

            screen.Line(x1, y1, x2, y2, c);
        }

        public void DebugShadowRay(Ray ray, Light lightsource, int c)
        {
            Vector3 startPoint = ray.origin;
            Vector3 endPoint = lightsource.position;
         
            int x1 = DebugX(startPoint.Y);
            int y1 = DebugY(startPoint.X);

            int x2 = DebugX(endPoint.Y);
            int y2 = DebugY(endPoint.X);

            screen.Line(x1, y1, x2, y2, c);
        }
        #endregion

        int CreateColor(int red, int green, int blue)
        {
            return (red << 16) + (green << 8) + blue;
        }

        int CreateColor(Vector3 color)
        {
            int red = (int) (color.X * 255);
            int green = (int)(color.Y * 255);
            int blue = (int)(color.Z * 255);

            red = Math.Min(255, red);
            green = Math.Min(255, green);
            blue = Math.Min(255, blue);

            return CreateColor(red, green, blue);
        }
    }
}