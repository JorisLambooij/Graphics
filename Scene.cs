using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace template
{
    class Scene
    {
        public List<Primitive> sceneObjects;
        public List<Light> lightSources;

        public Bitmap skydome;

        public Scene()
        {
            sceneObjects = new List<Primitive>();
            lightSources = new List<Light>();
        }

        public Intersection intersectScene(Ray ray)
        {
            float nearestDistance = float.PositiveInfinity;
            Intersection nearestIntersection = new Intersection();

            foreach(Primitive p in sceneObjects)
            {
                Intersection currIntersection = p.intersectPrimitive(ray);
                if (currIntersection != null && currIntersection.distance < nearestDistance)
                {
                    nearestIntersection = currIntersection;
                    nearestDistance = currIntersection.distance;
                }
            }
            return nearestIntersection;
        }

        public Vector3 SkydomeColor(Vector3 direction)
        {
            if (skydome == null)
                return Vector3.Zero;

            // calculate circular angles theta(x/y) and phi(z)
            float phi = Vector3.CalculateAngle(Vector3.UnitZ, direction);
            float theta = (float)(Math.Atan(direction.Y / direction.X) + Math.PI);

            // use those angles to determine x and y on the texture
            int x = (int)((1 - theta * Sphere.RadiansToPercentage) * skydome.Width);
            int y = (int)(2 * phi * Sphere.RadiansToPercentage * skydome.Height);
            
            Color color = skydome.GetPixel(x, y);
            float colorRatio = 1f / 255f;
            return new Vector3(color.R * colorRatio, color.G * colorRatio, color.B * colorRatio);
        }

        public float intersectSceneShadow(Ray ray, Light lightSource)
        {
            float currTransparency = 1;
            foreach (Primitive p in sceneObjects)
            {
                Intersection currIntersection = p.intersectPrimitive(ray);

                if(currIntersection != null)
                {
                    float intersectdistanceSquared = (currIntersection.intersectionPoint - ray.origin).LengthSquared;
                    float lightDistanceSquared = (lightSource.position - ray.origin).LengthSquared;
                    
                    if (intersectdistanceSquared > 0 && intersectdistanceSquared < lightDistanceSquared)
                    {
                        currTransparency *= currIntersection.collider.transparency;
                        if (currTransparency <= 0.1f)
                            return 0;
                    }
                }
            }
            return currTransparency;
        }

        public void AddObject(Primitive p)
        {
            sceneObjects.Add(p);
        }

        public void AddLight(Light l)
        {
            lightSources.Add(l);
        }
        public void AddLight(Vector3 position, float intensity, Vector3 color)
        {
            Light l = new Light(position, intensity, color);
            lightSources.Add(l);
        }
    }
}
