using System;
using System.Collections.Generic;
using System.Drawing;
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

        // adjustable stuff
        public int recursionCap = 5;
        public int FOV = 90;
        public Vector3 cameraPosition = new Vector3(-6, -6, 1);
        public Vector3 cameraDirection = new Vector3(1, 1, 0);

        Surface screen;

        // initialize
        public void Init(Surface screen)
        {
            this.screen = screen;
            
            float angle = FOV * (float)(Math.PI / 180);
            camera = new Camera(cameraPosition, cameraDirection, angle);
            

            // initialize the scene with a few object (only one plane atm)
            scene = new Scene();
            scene.AddLight(new Vector3(0, -5, 10f), 4000, new Vector3(1, 1, 1));
            scene.AddLight(new Vector3(-5, 2, 4f), 2000, new Vector3(0.2f, 1, 1));
            scene.skydome = new Bitmap("textures/sky2.jpg");

            Plane p = new Plane(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1.6f, 1.6f, 1.5f));
            p.reflection = 0;
            p.texture = new Bitmap("textures/floor_simple.jpg");
            p.textureScale = 0.25f;
            scene.AddObject(p);

            Plane p2 = new Plane(new Vector3(40, 0, 0f), new Vector3(-1, 0, 0f), new Vector3(1000, 1000, 1000));
            p2.texture = new Bitmap("textures/floor_simple.jpg");
            p2.textureScale = 0.05f;
            //scene.AddObject(p2);

            Sphere s1 = new Sphere(new Vector3(2, 3, 1), 2, new Vector3(1.0f, 0.1f, 1.0f));
            scene.AddObject(s1);

            Sphere s2 = new Sphere(new Vector3(2, -2, 3), 3, new Vector3(1.2f, 1.2f, 1.2f));
            s2.texture = new Bitmap("textures/earth.png");
            scene.AddObject(s2);
            
            Sphere s3 = new Sphere(new Vector3(-3.0f, 1, 1.2f), 1.5f, new Vector3(1.0f, 1.0f, 1.0f));
            s3.transparency = 0.8f;
            s3.refractionIndex = 1.5f;
            s3.reflection = 0.0f;
            scene.AddObject(s3);
            
            Triangle t = new Triangle(new Vector3(-3, -1, 3), new Vector3(-4, 1, 3.5f), new Vector3(-3, 0, 3), new Vector3(1f, 0, 0.5f));
            //scene.AddObject(t);
        }

        public void Render()
        {
            int width = screen.width / 2;

            float xSteps = 1f / width;
            float ySteps = 1f / screen.height;

            for (int x = 0; x < width; x++)
            {
                float screenRightPercent = 2.0f * (x - (width / 2)) / width;
                float screenHalfPercent = 1f / width;

                // before each y-column, we need to calculate the result for "y = -1"
                Vector3 previousVector1, previousVector2;
                previousVector1 = camera.Direction + (screenRightPercent - screenHalfPercent) * camera.screenRight + (1 - screenHalfPercent) * camera.screenUp;
                previousVector2 = camera.Direction + (screenRightPercent + screenHalfPercent) * camera.screenRight + (1 - screenHalfPercent) * camera.screenUp;

                Ray previousRay1, previousRay2;
                previousRay1 = TraceRay(new Ray(camera.position, previousVector1.Normalized()), 0);
                previousRay2 = TraceRay(new Ray(camera.position, previousVector2.Normalized()), 0);

                float previousInverseSquare1, previousInverseSquare2;
                previousInverseSquare1 = 1 / (previousRay1.distanceTraveled * previousRay1.distanceTraveled);
                previousInverseSquare2 = 1 / (previousRay2.distanceTraveled * previousRay2.distanceTraveled);

                Vector3 previousColor1, previousColor2;
                previousColor1 = previousRay1.color * previousInverseSquare1;
                previousColor2 = previousRay2.color * previousInverseSquare2;
                
                previousColor1 = ClampColor(previousColor1);
                previousColor2 = ClampColor(previousColor2);
                
                for (int y = 0; y < screen.height; y++)
                {
                    if (x % 16 == 0 && y == 256)
                        debugFrame = true;

                    float screenUpPercent = 2.0f * (y - (screen.height / 2)) / screen.height;

                    /*
                    // without anti-alias:
                    // for each pixel, cast a ray
                    // for now, use static camera, so the 2nd vector is simply calculated with x and y
                    Vector3 pixelDirection = camera.direction + new Vector3(0, (x * xSteps) - 0.5f, -(y * ySteps) + 0.5f);
                    Ray ray = new Ray(camera.position, pixelDirection.Normalized());

                    Ray tRay = TraceRay(ray, 0);
                    float inverseSquare = 1 / (tRay.distanceTraveled * tRay.distanceTraveled);
                    Vector3 color = tRay.color * inverseSquare;

                    screen.pixels[x + y * screen.width] = CreateColor(color);
                    */

                    // with anti-alias: calculate the two "new" rays, then add the "previous" rays, and take the average color

                    Vector3 currentVector1, currentVector2;
                    
                    currentVector1 = camera.Direction + (screenRightPercent - screenHalfPercent) * camera.screenRight - (screenUpPercent + screenHalfPercent) * camera.screenUp;
                    currentVector2 = camera.Direction + (screenRightPercent + screenHalfPercent) * camera.screenRight - (screenUpPercent + screenHalfPercent) * camera.screenUp;

                    Ray currentRay1, currentRay2;

                    currentRay1 = TraceRay(new Ray(camera.position, currentVector1.Normalized()), 0);
                    currentRay2 = TraceRay(new Ray(camera.position, currentVector2.Normalized()), 0);

                    float currentInverseSquare1 = Math.Min(1, 1 / (currentRay1.distanceTraveled * currentRay1.distanceTraveled));
                    float currentInverseSquare2 = Math.Min(1, 1 / (currentRay2.distanceTraveled * currentRay2.distanceTraveled));

                    Vector3 currentColor1, currentColor2;

                    currentColor1 = currentRay1.color * currentInverseSquare1;
                    currentColor2 = currentRay2.color * currentInverseSquare2;
                    
                    currentColor1 = ClampColor(currentColor1);
                    currentColor2 = ClampColor(currentColor2);
                    
                    Vector3 color = (previousColor1 + previousColor2 + currentColor1 + currentColor2) * 0.25f;

                    screen.pixels[x + y * screen.width] = CreateColor(color);

                    previousColor1 = currentColor1;
                    previousColor2 = currentColor2;


                    // keep for Debug (for now)
                    //Vector3 pixelDirection = camera.direction + new Vector3(0, (x * xSteps) - 0.5f, -(y * ySteps) + 0.5f);
                    //Ray debugRay = new Ray(camera.position, pixelDirection);
                    //Intersection intersect = scene.intersectScene(debugRay);
                    if (debugFrame)
                    {
                        //DebugRay(debugRay, intersect, CreateColor(color));
                        debugFrame = false;
                    }
                }
            }
            DebugView();
        }

        private Vector3 ClampColor(Vector3 color)
        {
            color.X = Math.Max(0, Math.Min(1, color.X));
            color.Y = Math.Max(0, Math.Min(1, color.Y));
            color.Z = Math.Max(0, Math.Min(1, color.Z));

            return color;
        }

        public Ray TraceRay(Ray ray, int recursion)
        {
            ray.origin = ray.origin + Lambda * ray.direction;
            Intersection intersect = scene.intersectScene(ray);
            
            // did not hit anything, return skydome
            if (intersect.collider == null)
            {
                ray.color = scene.SkydomeColor(ray.direction);
                ray.hitSkybox = true;
                // set distance to 1 to avoid distance attenuation (would be 1 / infinity = 0 = black otherwise
                // TODO: make skydome HDR instead of setting the distance to 1
                ray.distanceTraveled = 1f;
                return ray;
            }
            
            // hitting something not transparent
            if (intersect.collider.transparency == 0 && intersect.collider.reflection == 0)
            {
                Vector3 color = DirectIllumination(intersect) * intersect.collider.color;

                if (intersect.collider.texture != null)
                    color *= intersect.collider.colorFromTexture(intersect.intersectionPoint);
                
                ray.distanceTraveled += intersect.distance;
                ray.color = color;
                
                if (debugFrame)
                    DebugRay(ray, intersect, CreateColor(color, true));
                
                return ray;
            }
            // hitting reflective object, going into recursion
            if (intersect.collider.transparency > 0 && intersect.collider.reflection == 0)
            {
                float tvalue = intersect.collider.transparency;
                float nvalue = 1 - intersect.collider.transparency;

                if (recursion + 1 > recursionCap)
                {
                    ray.color = Vector3.Zero;
                    return ray;
                }

                Vector3 color = DirectIllumination(intersect) * intersect.collider.color;

                if (intersect.collider.texture != null)
                    color *= intersect.collider.colorFromTexture(intersect.intersectionPoint);

                ray.distanceTraveled += intersect.distance;
                Ray transparentRay = refraction(intersect, ray, recursion + 1);

                if (transparentRay.hitSkybox)
                    tvalue *= 36;

                ray.color = nvalue * intersect.collider.color + tvalue * transparentRay.color;

                if (debugFrame)
                    DebugRay(ray, intersect, CreateColor(ray.color, true));

                return ray;
            }

            if (intersect.collider.transparency == 0 && intersect.collider.reflection > 0)
            {
                float nvalue = 1 - intersect.collider.reflection;
                float rvalue = intersect.collider.reflection;

                Vector3 u;
                Vector3 v;

                u = intersect.normal * Vector3.Dot(intersect.normal, ray.direction);
                v = ray.direction - u;

                Vector3 newRayDirection = v - u;

                if (recursion + 1 > recursionCap)
                {
                    ray.color = Vector3.Zero;
                    return ray;
                }

                Ray reflectRay = TraceRay(new Ray(intersect.intersectionPoint, newRayDirection), recursion + 1);
                Vector3 color = DirectIllumination(intersect) * intersect.collider.color;

                if (intersect.collider.texture != null)
                    color *= intersect.collider.colorFromTexture(intersect.intersectionPoint);

                ray.distanceTraveled += intersect.distance;

                ray.color = rvalue * reflectRay.color + nvalue * intersect.collider.color;
                ray.distanceTraveled += reflectRay.distanceTraveled;

                if (debugFrame)
                    DebugRay(ray, intersect, CreateColor(ray.color, true));

                return ray;
            }

            if (intersect.collider.transparency > 0 && intersect.collider.reflection > 0)
            {
                float tvalue = 1 - intersect.collider.reflection;
                float rvalue = intersect.collider.reflection;

                Vector3 u;
                Vector3 v;

                u = intersect.normal * Vector3.Dot(intersect.normal, ray.direction);
                v = ray.direction - u;

                Vector3 newRayDirection = v - u;

                if (recursion + 1 > recursionCap)
                {
                    ray.color = Vector3.Zero;
                    return ray;
                }

                Ray reflectRay = TraceRay(new Ray(intersect.intersectionPoint, newRayDirection), recursion + 1);
                Ray transparentRay = refraction(intersect, ray, recursion + 1);

                if (transparentRay.hitSkybox)
                    tvalue *= 36;

                //reflectRay.distanceTraveled += ray.distanceTraveled;
                ray.color = rvalue * reflectRay.color + intersect.collider.transparency * tvalue * transparentRay.color;
                //ray.color *= 0.5f;
                ray.distanceTraveled = reflectRay.distanceTraveled;

                if (debugFrame)
                    DebugRay(ray, intersect, CreateColor(ray.color, true));

                return ray;
            }

            if (debugFrame)
                DebugRay(ray, intersect, CreateColor(ray.color, true));
            return null;
        }

        Ray refraction(Intersection intersect, Ray ray, int recursion)
        {
            Vector3 newDirection;
            Vector3 newOrigin = intersect.intersectionPoint;
            Ray newRay;
            if (ray.refract == Ray.Refract.Outside)
            {
                //newDirection = ray.direction;
                newDirection = RefractionDirection(ray.direction, intersect.normal, 1, intersect.collider.refractionIndex);

                newRay = new Ray(newOrigin, newDirection);
                newRay.refract = Ray.Refract.Inside;
            }
            else
            {
                newDirection = RefractionDirection(ray.direction, intersect.normal, intersect.collider.refractionIndex, 1);

                newRay = new Ray(newOrigin, newDirection);
                newRay.refract = Ray.Refract.Outside;
            }
            newRay.distanceTraveled = ray.distanceTraveled + intersect.distance;


            if (recursion + 1 > recursionCap)
            {
                ray.color = Vector3.Zero;
                return ray;
            }
            Ray returnRay = TraceRay(newRay, recursion + 1);
            returnRay.color *= intersect.collider.transparency;
            
            return returnRay;

        }

        Vector3 RefractionDirection(Vector3 d, Vector3 n, float n1, float n2)
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

                if (cos >= 0)
                {
                    Vector3 startPoint = intersect.intersectionPoint + Lambda * lightD;
                    Ray shadowRay = new Ray(startPoint, lightD);
                    
                    float d = 1 / (lightSource.position - shadowRay.origin).LengthSquared;
                    float illumination = scene.intersectSceneShadow(shadowRay, lightSource);
                    
                    Vector3 color = illumination * (lightSource.color * lightSource.intensity * cos * d);
                    
                    totalIllumination += color;

                    // debug stuff
                    if (debugFrame)
                    {
                        DebugShadowRay(shadowRay, lightSource, CreateColor(totalIllumination, true));
                    }
                }
            }
            //return intersect.collider.color * 100;
            return totalIllumination;
        }


        #region Debug
        // screen scale relative to world scale
        float scale = -48;

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
        private Vector3 ScaleColor(Vector3 color)
        {
            float biggestC = Math.Max(color.Z, Math.Max(color.X, color.Y));

            color *= (1 / biggestC);

            return color;
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

        int CreateColor(Vector3 color, bool scale = false)
        {
            if(scale)
                color = ScaleColor(color);

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
