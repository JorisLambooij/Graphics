using System;
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

        public bool debugFrame;
        public int recursionCap = 5;

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
            scene.AddLight(new Vector3(0, -5, 10f), 3000, new Vector3(1, 1, 1));
            scene.AddLight(new Vector3(-5, 2, 1f), 2000, new Vector3(1, 1, 1));

            Plane p = new Plane(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1f, 1f, 0));
            scene.AddObject(p);

            Plane p2 = new Plane(new Vector3(40, 0, 0f), new Vector3(-1, 0, 0.2f), new Vector3(80, 80, 800));
            scene.AddObject(p2);

            Sphere s1 = new Sphere(new Vector3(2, 3, 1), 2, new Vector3(1.0f, 0.1f, 1.0f));
            scene.AddObject(s1);

            Sphere s2 = new Sphere(new Vector3(0, -2, 1), 2, new Vector3(0.0f, 1.0f, .0f));
            scene.AddObject(s2);
            
            Sphere s3 = new Sphere(new Vector3(-2.5f, 0, 1), 1, new Vector3(1.0f, 1.0f, 1.0f));
            s3.transparency = 0.8f;
            s3.refractionIndex = 2.15f;
            scene.AddObject(s3);

            Triangle t = new Triangle(new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 1), new Vector3(1f, 0, 0.5f));
            //scene.AddObject(t);
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
                        debugFrame = true;
                    
                    Ray tRay = TraceRay(ray, 0);
                    float inverseSquare = 1 / (tRay.distanceTraveled * tRay.distanceTraveled);
                    Vector3 color = tRay.color * inverseSquare;

                    screen.pixels[x + y * screen.width] = CreateColor(color);
                    
                    // keep for Debug (for now)
                    Intersection intersect = scene.intersectScene(ray);
                    if (debugFrame)
                    {
                        DebugRay(ray, intersect, CreateColor(color));
                        debugFrame = false;
                    }
                }

            DebugView();
        }

        public Ray TraceRay(Ray ray, int recursion)
        {
            ray.origin = ray.origin + Lambda * ray.direction;
            Intersection intersect = scene.intersectScene(ray);

            // did not hit anything, return black
            if (intersect.collider == null)
            {
                //ray.distanceTraveled = float.PositiveInfinity;
                ray.color = Vector3.Zero;
                return ray;
            }
            // TODO: check for materials
            // hitting something not transparent
            else if(intersect.collider.transparency == 0)
            {
                Vector3 color = DirectIllumination(intersect) * intersect.collider.color;
                ray.distanceTraveled += intersect.distance;
                ray.color = color;
                return ray;
            }
            // hitting transparent object, going into recursion
            else if(recursion < recursionCap)
            {
                //Vector3 newDirection = ray.direction;

                Vector3 newDirection;
                Vector3 newOrigin = intersect.intersectionPoint;
                Ray newRay;
                if (ray.refract == Ray.Refract.Outside)
                {
                    //newDirection = ray.direction;
                    newDirection = Refraction(ray.direction, intersect.normal, 1, intersect.collider.refractionIndex);

                    newRay = new Ray(newOrigin, newDirection);
                    newRay.refract = Ray.Refract.Inside;
                }
                else
                {
                    newDirection = Refraction(ray.direction, intersect.normal, intersect.collider.refractionIndex, 1);

                    newRay = new Ray(newOrigin, newDirection);
                    newRay.refract = Ray.Refract.Outside;
                }
                newRay.distanceTraveled = ray.distanceTraveled + intersect.distance;

                Ray returnRay = TraceRay(newRay, recursion + 1);
                returnRay.color *= intersect.collider.transparency;

                if (debugFrame)
                {
                    Intersection inter = scene.intersectScene(newRay);
                    DebugRay(newRay, inter, CreateColor(returnRay.color));
                }

                return returnRay;
            }
            else
                return null;
        }

        Vector3 Refraction(Vector3 d, Vector3 n, float n1, float n2)
        {
            Vector3 u1 = n * Vector3.Dot(n, d);
            Vector3 v1 = d - u1;

            float uMag = u1.Length;
            float vMag = v1.Length;

            float angle1 = (float) Math.Acos(Vector3.Dot(d, n));
            float angle2 = (float) Math.Asin(Math.Sin(angle1) * (n1 / n2));
            float cos2 = (float) Math.Cos(angle2);
            float sin2 = (float)Math.Sin(angle2);

            Vector3 v2 = v1 * (sin2 / vMag);
            Vector3 u2 = u1 * (cos2 / uMag);
            
            return v2 + u2;
        }

        public Vector3 DirectIllumination(Intersection intersect)
        {
            Vector3 totalIllumination = Vector3.Zero;

            foreach (Light lightSource in scene.lightSources)
            {
                Vector3 lightD = (lightSource.position - intersect.intersectionPoint).Normalized();
                float cos = Vector3.Dot(lightD, intersect.normal);

                if (cos > 0)
                {
                    Vector3 startPoint = intersect.intersectionPoint + Lambda * lightD;
                    Ray shadowRay = new Ray(startPoint, lightD);
                    
                    Vector3 d = lightSource.position - shadowRay.origin;
                    Vector3 color = scene.intersectSceneShadow(shadowRay, lightSource) * (lightSource.color * lightSource.intensity * cos / d.LengthSquared);

                    totalIllumination += color;

                    // debug stuff
                    if (debugFrame)
                    {
                        //DebugShadowRay(shadowRay, lightSource, CreateColor(totalIllumination));
                    }
                }
            }
            return totalIllumination;
        }
        

        #region Debug
        // screen scale relative to world scale
        float scale = -64;

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
                if (p is Triangle)
                {
                    screen.Print("TRIANGLE", DebugX(p.position.Y), DebugY(p.position.X) - 10, CreateColor(p.color));
                    
                }
            }
            foreach (Light l in scene.lightSources)
                screen.Print("LIGHT", (int)(-l.position.Y * scale + xOffset), (int)(l.position.X * scale + yOffset) - 10, 0x00ffff);
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
