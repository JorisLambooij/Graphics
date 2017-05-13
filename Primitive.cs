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
        public Vector3 position;
        public Vector3 color;

        public Primitive(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }

        public abstract Intersection intersectPrimitive(Ray ray);
    }

    class Sphere : Primitive
    {
        public float radius;

        public Sphere(Vector3 position, float radius, Vector3 color) : base (position, color)
        {
            this.radius = radius;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            Vector3 c = position - ray.origin;
            float t = Vector3.Dot(c, ray.direction);
            Vector3 q = c - t * ray.direction;
            float p2 = Vector3.Dot(q, q);
            if (p2 > radius * radius || t < 0)
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
        public Vector3 normal;

        public Plane(Vector3 position, Vector3 normal, Vector3 color) : base (position, color)
        {
            this.normal = normal;
        }

        public override Intersection intersectPrimitive(Ray ray)
        {
            // the intersection point
            Vector3 iPoint;
            // the length of the ray from origin to intersection point
            float t = Vector3.Dot(normal, ray.origin) / Vector3.Dot(normal, ray.direction);
            iPoint = ray.origin + -t * ray.direction;

            // wrong direction, return null
            if (t > 0)
                return null;

            Intersection i = new Intersection();
            i.origin = ray.origin;
            i.direction = ray.direction;

            i.intersectionPoint = iPoint;
            i.normal = this.normal;

            i.collider = this;
            i.color = this.color;

            return i;
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

        public Light(Vector3 position, float intensity, Vector3 color)
        {
            this.position = position;
            this.intensity = intensity;
            this.color = color;
        }
    }

    class Ray
    {
        public Vector3 origin;
        public Vector3 direction;
        public float distance;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }
    }
}
