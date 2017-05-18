using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace template
{
    class Scene
    {
        public List<Primitive> sceneObjects;
        public List<Light> lightSources;
        
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
