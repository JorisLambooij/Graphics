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

                if(currIntersection != null && currIntersection.distance < nearestDistance)
                    nearestIntersection = currIntersection;
                
            }
            return nearestIntersection;
        }

        public bool intersectSceneShadow(Ray ray)
        {
            ray.origin = ray.origin + RayTracer.Lambda * ray.direction;

            foreach (Primitive p in sceneObjects)
            {
                Intersection currIntersection = p.intersectPrimitive(ray);

                if (currIntersection != null && currIntersection.distance > RayTracer.Lambda)
                    return true;

            }
            return false;
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
