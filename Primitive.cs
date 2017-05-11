using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template
{
    abstract class Primitive
    {
        protected Vector3 position;
        protected Vector3 color;

        public abstract Intersection intersectPrimitive(Ray ray);
    }

    class Sphere : Primitive
    {
        float radius;

        public override Intersection intersectPrimitive(Ray ray)
        {
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = Vector3.Dot(q, q);
            if (p2 > radius * radius)
            {
                return null;
            }

            Intersection i = new Intersection();
            i.origin = ray.origin;
            i.direction = ray.direction;
            i.intersectionPoint = ray.origin + t * ray.direction;
            i.normal = (i.intersectionPoint - this.position).Normalized();
            i.collider = this;
            i.color = this.color;
            i.distance = t;
            return i;

        }
    }

    class Plane : Primitive
    {
        Vector3 normal;

        public override Intersection intersectPrimitive(Ray ray)
        {

        }
    }

    class Intersection
    {
        public Vector3 origin;
        public Vector3 direction;

        public Vector3 intersectionPoint;
        public Vector3 normal;

        public Primitive collider;
        public Vector3 color;

        public float distance;
        /*
        public Intersection( Vector3 origin, Vector3 direction, Scene scene)
        {
            float distance = float.PositiveInfinity;
            this.origin = origin;
            this.direction = direction;
            foreach( Primitive p in scene.sceneObjects)
            {
                
            }
        }*/

    }

    class Light
    {
        Vector3 position;
        float intensity;
        Vector3 color;

    }

    class Ray
    {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;


    }
}
